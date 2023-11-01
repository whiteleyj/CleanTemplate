namespace CleanTemplate.Redirects
{
    public interface IRedirectRepository
    {
        List<Redirect>? QueryRedirects(int updateCount);
    }

    public class RedirectRepository : IRedirectRepository
    {
        private ILogger _logger;

        private List<Redirect> _fakeServiceResponseData = new() {
            new(){ RedirectUrl = "/moving-target", TargetUrl = "/unmoving", RedirectType=302, UseRelative = true},
            new(){ RedirectUrl = "/campaignA", TargetUrl = "/campaigns/targetcampaign" },
            new(){ RedirectUrl = "/campaignB", TargetUrl = "/campaigns/targetcampaign/channelB" },
            new(){ RedirectUrl = "/product-directory", TargetUrl = "/products", RedirectType=301, UseRelative = true }
        };

        public RedirectRepository(ILogger<RedirectRepository> logger) 
        {
            _logger = logger;
        }

        public List<Redirect>? QueryRedirects(int updateCount)
        {
            try
            {
                var response = _fakeServiceResponseData;
                
                // To help visualize that the response changes after each update 
                // I've added a temp redirect to a url that changes every update.
                var movingTarget = string.Format($"/target/{updateCount++}"); 
                var targetRedirect = _fakeServiceResponseData.FirstOrDefault(r => r.RedirectUrl.Equals("/moving-target"));
                if (targetRedirect != null)
                    targetRedirect.TargetUrl = movingTarget;

                if (updateCount % 12 == 0)
                    throw new BadHttpRequestException("Fake Error Simulation");

                return response;
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("Error retrieving redirects", ex);
                return null;
            }
        }
    }
}