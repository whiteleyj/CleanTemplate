using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CleanTemplate.Redirects
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedirectService _redirectService;

        public RedirectMiddleware(RequestDelegate next, IRedirectService redirectService)
        {
            _next = next;
            _redirectService = redirectService;
        }

        public async Task Invoke(HttpContext context)
        {
            Redirect redirect = _redirectService.GetRedirect(context.Request.Path);
            if (redirect != null)
            {
                context.Response.Redirect(redirect.Location(context.Request.Path), redirect.IsPermanant);
                context.Response.StatusCode = redirect.RedirectType;
                return;
            }

            // The request should not be redirected, continue processing
            await _next(context);
        }
    }
}