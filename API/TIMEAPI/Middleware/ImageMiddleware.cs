namespace TIMEAPI.Middleware
{
    public static class ImageMiddleware
    {
        public static IApplicationBuilder UseImageMiddlwware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageProvider>();
        }
    }
}
