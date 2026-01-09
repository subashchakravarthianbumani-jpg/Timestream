using BAL;
using BAL.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MariaDB.Extensions;
using System.Globalization;
using System.Text;
using Utils;
using Utils.Interface;
using TIMEAPI.Infrastructure;
using Model.ViewModel;
using BAL.BackgroundWorkerService;
using TIMEAPI.Middleware;
using DateTimeConverter = Utils.DateTimeConverter;

var builder = WebApplication.CreateBuilder(args);

#region Environment
string env = builder.Configuration.GetSection("Environment").Value;
if (string.IsNullOrEmpty(env))
{
    env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "DEV";
}

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true);
#endregion

#region ✅ CORS (FIXED – JWT SAFE)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://13.201.13.151:5000"
                // add https later if needed
                // ,"https://13.201.13.151:5000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endregion

#region Encryption Keys
EncryptDecrypt.IV = builder.Configuration["AESJWT:IV"];
EncryptDecrypt.Key = builder.Configuration["AESJWT:Key"];
EncryptDecrypt.publickey = builder.Configuration["EnDecodeKey:publickey"];
EncryptDecrypt.secretkey = builder.Configuration["EnDecodeKey:secretkey"];
#endregion

#region Serilog
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MariaDB(
            connectionString,
            tableName: "Logs",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
        );
});
#endregion

#region Services
builder.Services.AddDistributedMemoryCache();
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
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMemoryCache();

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});

builder.Services.AddEndpointsApiExplorer();
#endregion

#region Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TAHDCO_TIME_API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region JWT
var jwtTokenConfig = builder.Configuration
    .GetSection("jwtTokenConfig")
    .Get<JwtTokenConfig>();

builder.Services.AddSingleton(jwtTokenConfig);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtTokenConfig.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)
        ),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
builder.Services.AddHostedService<JwtRefreshTokenCache>();
#endregion

var app = builder.Build();

#region Localization
var culture = CultureInfo.CreateSpecificCulture("en-IN");
culture.DateTimeFormat = new DateTimeFormatInfo
{
    ShortDatePattern = "dd/MM/yyyy",
    LongDatePattern = "dd/MM/yyyy hh:mm:ss tt"
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});
#endregion

app.UseSession();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

#region ✅ APPLY CORS (ORDER MATTERS)
app.UseCors("AllowFrontend");
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.UseImageMiddlwware();

app.MapControllers();
app.Run();
