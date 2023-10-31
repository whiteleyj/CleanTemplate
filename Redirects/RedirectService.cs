using System.Collections.Immutable;
using System.Runtime;

namespace CleanTemplate.Redirects
{
    public interface IRedirectService
    {
        Redirect? GetRedirect(string redirectUrl);

        void UpdateRedirects();
    }

    public class RedirectUrlService : IRedirectService
    {
        private ImmutableDictionary<string, Redirect> _absoluteRedirects;
        private ImmutableList<Redirect> _relativeRedirects;
        protected ILogger _logger;

        public RedirectUrlService(ILogger<RedirectMockService> logger)
        {
            _absoluteRedirects = ImmutableDictionary<string, Redirect>.Empty;
            _relativeRedirects = ImmutableList<Redirect>.Empty;
            _logger = logger;
            UpdateRedirects();
        }

        public Redirect? GetRedirect(string redirectUrl)
        {
            // Absolute redirects are much faster than the relative ones so get these out of the way first.
            if (_absoluteRedirects.ContainsKey(redirectUrl))
                return _absoluteRedirects[redirectUrl];

            // The relative redirects are stored in order of longest length to retrieve the best match first.
            var relativeRedirect = _relativeRedirects.FirstOrDefault(r => r.MatchesRelative(redirectUrl));
            if (relativeRedirect != null)
                return relativeRedirect;

            return null;
        }

        public void UpdateRedirects()
        {
            var response = QueryRedirects();
            if (response == null)
                return;
            
            _logger.LogInformation($"Query Redirects returns successfully with {response.Count} results.");

            _absoluteRedirects = ImmutableDictionary<string, Redirect>.Empty.AddRange(
                response.Where(r => !r.UseRelative)
                        .Select(r => new KeyValuePair<string, Redirect>(r.RedirectUrl, r)));
                
            _relativeRedirects = ImmutableList<Redirect>.Empty.AddRange(
                response.Where(r => r.UseRelative)
                        .OrderBy(r => r.RedirectUrl.Length)); // Return longest url that matches first.
        }

        protected virtual List<Redirect>? QueryRedirects()
        {
            throw new NotImplementedException();
        }
    }
}