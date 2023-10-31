namespace CleanTemplate.Redirects
{
    public class RedirectUpdateService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
        private readonly IRedirectService _redirectService;

        public RedirectUpdateService(IRedirectService service, int updateIntervalSeconds)
        {
            _redirectService = service;
            _updateInterval = TimeSpan.FromSeconds(updateIntervalSeconds);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateCache, new object(), TimeSpan.Zero, _updateInterval);
            return Task.CompletedTask;
        }

        private void UpdateCache(object? _)
        {
            _redirectService.UpdateRedirects();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_timer != null)
                _timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}