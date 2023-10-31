namespace CleanTemplate.Redirects
{
    public class RedirectMockService : RedirectUrlService, IRedirectService
    {
        private int _updateCount = 0;
        private List<Redirect> _fakeServiceResponseData = new() {
            new(){ RedirectUrl = "/moving-target", TargetUrl = "/unmoving", RedirectType=302, UseRelative = true},
            new(){ RedirectUrl = "/campaignA", TargetUrl = "/campaigns/targetcampaign" },
            new(){ RedirectUrl = "/campaignB", TargetUrl = "/campaigns/targetcampaign/channelB" },
            new(){ RedirectUrl = "/product-directory", TargetUrl = "/products", RedirectType=301, UseRelative = true }
        };

        public RedirectMockService(ILogger<RedirectMockService> logger) : base(logger)
        {
        }

        protected override List<Redirect>? QueryRedirects()
        {
            try
            {
                var response = _fakeServiceResponseData;
                
                // To help visualize that the response changes after each update 
                // I've added a temp redirect to a url that changes every update.
                var movingTarget = string.Format($"/target/{_updateCount++}"); 
                var targetRedirect = _fakeServiceResponseData.FirstOrDefault(r => r.RedirectUrl.Equals("/moving-target"));
                if (targetRedirect != null)
                    targetRedirect.TargetUrl = movingTarget;

                if (_updateCount % 11 == 0)
                    throw new BadHttpRequestException("Fake Error Simulation");

                return response;
            }
            catch (BadHttpRequestException ex)
            {
                this._logger.LogError("Error retrieving redirects", ex);
                return null;
            }
        }
    }
}