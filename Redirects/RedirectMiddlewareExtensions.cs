using CleanTemplate.Redirects;

public static class RedirectMiddlewareExtensions
{
    public static void RegisterRedirectService(this WebApplicationBuilder builder, AppSettings settings)
    {
        builder.Services.AddSingleton<IRedirectService, RedirectMockService>();
        builder.Services.AddHostedService(s => 
            new RedirectUpdateService(s.GetService<IRedirectService>(), settings.RedirectUpdateIntervalSeconds));
    }

    public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RedirectMiddleware>();
    }
}
