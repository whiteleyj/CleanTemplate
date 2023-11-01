using CleanTemplate.Redirects;

public static class RedirectMiddlewareExtensions
{
    public static void RegisterRedirectService(this WebApplicationBuilder builder, AppSettings settings)
    {
        builder.Services.AddScoped<IRedirectRepository, RedirectRepository>();
        builder.Services.AddSingleton<IRedirectService, RedirectUrlService>();
        builder.Services.AddHostedService(s => 
            new RedirectUpdateService(s, settings.RedirectUpdateIntervalSeconds));
    }

    public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RedirectMiddleware>();
    }
}
