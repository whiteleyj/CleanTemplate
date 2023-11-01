namespace CleanTemplate.Redirects
{
    public class RedirectUpdateService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
        private readonly IServiceProvider _serviceProvider;
        private int _updateCount = 0;

        public RedirectUpdateService(IServiceProvider provider, int updateIntervalSeconds)
        {
            _serviceProvider = provider;
            _updateInterval = TimeSpan.FromSeconds(updateIntervalSeconds);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateCache, new object(), TimeSpan.Zero, _updateInterval);
            return Task.CompletedTask;
        }

        private void UpdateCache(object? _)
        {
            // Redirect Service must be registered as a singleton to ensure that updates effect all threads.
            var redirectService = _serviceProvider.GetService<IRedirectService>();

            // Querying a datasource should be scoped (and disposed when the update is done).
            using (var scope = _serviceProvider.CreateScope())
            {
                var redirectRepository = scope.ServiceProvider.GetService<IRedirectRepository>();
                if (redirectRepository == null || redirectService == null)
                    return;

                var response = redirectRepository.QueryRedirects(_updateCount++);
                if (response == null)
                    return;
                
                redirectService.UpdateRedirects(response);
            }
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