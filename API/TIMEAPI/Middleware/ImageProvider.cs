using BAL.Interface;

namespace TIMEAPI.Middleware
{
    public class ImageProvider
    {
        private readonly RequestDelegate _next;
        private string pathCondition = "/images/";
        private readonly IServiceProvider _serviceProvider;

        public ImageProvider(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        private static readonly string[] suffixes = new string[] { ".png", ".jpg", ".gif", ".jpeg" };

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
            {
                return false;
            }
            else
            {
                bool res = suffixes.Any(x => path.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
                return res;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var path = context.Request.Path;
                if (IsImagePath(path) && path.Value != null && path.Value.Contains(pathCondition, StringComparison.OrdinalIgnoreCase))
                {
                    byte[]? buffer = null;
                    var Id = Path.GetFileName(path);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _bal = scope.ServiceProvider.GetRequiredService<IGeneralBAL>();
                        buffer = await _bal.GetImage(Id);
                    }
                    if (buffer != default && buffer != null && buffer.Length > 1)
                    {
                        context.Response.ContentType = "image/"+Path.GetExtension(path).Replace(".", "");
                        context.Response.ContentLength = buffer.Length;
                        await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }
            }
            finally
            {
                if (!context.Response.HasStarted)
                {
                    await _next(context);
                }
            }
        }
    }
}
