using BAL;
using BAL.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Sinks.MariaDB.Extensions;
using System.Globalization;
using Utils;
using Utils.Interface;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using TIMEAPI.Infrastructure;
using Model.ViewModel;
using BAL.BackgroundWorkerService;
using TIMEAPI.Middleware;
using DateTimeConverter = Utils.DateTimeConverter;

var builder = WebApplication.CreateBuilder(args);


// Environment
string env = builder.Configuration.GetSection("Environment").Value.ToString();
if (string.IsNullOrEmpty(env))
{
    env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "DEV";
}
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true);
// Environment


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

List<string> Orgins = new List<string>();
builder.Configuration.GetSection("Cors:Domains").Bind(Orgins);

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                      policy =>
//                      {
//                          policy.WithOrigins(Orgins.ToArray()).AllowAnyMethod().AllowAnyHeader();
//                      });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

EncryptDecrypt.IV = builder.Configuration["AESJWT:IV"];
EncryptDecrypt.Key = builder.Configuration["AESJWT:Key"];

EncryptDecrypt.publickey = builder.Configuration["EnDecodeKey:publickey"];
EncryptDecrypt.secretkey = builder.Configuration["EnDecodeKey:secretkey"];

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.WriteTo.Console();
    configuration.WriteTo.MariaDB(connectionString, default, 50, 10000, default, default, "Logs", true, default, Serilog.Events.LogEventLevel.Warning);
});

builder.Services.Configure<SMSConfiguration>(builder.Configuration.GetSection("SMS"));
builder.Services.Configure<GeneralDetail>(builder.Configuration.GetSection("GeneralDetail"));
builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("EmailConfig"));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton(provider => builder.Configuration);
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

builder.Services.AddHostedService<BackgroundServiceTenderGet>();

builder.Services.AddScoped<IMySqlDapperHelper, MySqlDapperHelper>();
builder.Services.AddScoped<IMySqlHelper, MySqlHelper>();
builder.Services.AddScoped<IFTPHelpers, FTPHelpers>();
builder.Services.AddScoped<ISMSHelper, SMSHelper>();
builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddScoped<IGeneralBAL, GeneralBAL>();
builder.Services.AddScoped<IAccountBAL, AccountBAL>();
builder.Services.AddScoped<ISettingBAL, SettingBAL>();
builder.Services.AddScoped<IWorkBAL, WorkBAL>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddControllers(options => { options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true; })
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()); });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TAHDCO_TIME_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var jwtTokenConfig = builder.Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
builder.Services.AddSingleton(jwtTokenConfig);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
        ValidAudience = jwtTokenConfig.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
builder.Services.AddHostedService<JwtRefreshTokenCache>();

builder.Services.AddMemoryCache();
builder.Services.AddMvc();
builder.Services.AddSingleton<RazorView>();

var app = builder.Build();

// Globalization

var culture = CultureInfo.CreateSpecificCulture("en-IN");
var dateformat = new DateTimeFormatInfo
{
    ShortDatePattern = "dd/MM/yyyy",
    LongDatePattern = "dd/MM/yyyy hh:mm:ss tt"
};
culture.DateTimeFormat = dateformat;

var supportedCultures = new[]
{
    culture
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Globalization

app.UseSession();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TAHDCO_TIME_API");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

//app.UseCors(MyAllowSpecificOrigins);
app.UseCors("AllowAll");

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();





app.UseImageMiddlwware();

app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        //if (ctx.Context.User.Identity != null && !ctx.Context.User.Identity.IsAuthenticated && ctx.Context.Request.Path.ToString().Contains("assets/imageassets"))
        //{
        //    ctx.Context.Response.Redirect("/index.html");
        //}

        ctx.Context.Response.Redirect("/index.html");
    }
});

app.MapControllers();

app.Run();
