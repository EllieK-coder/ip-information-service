using MediatR;
using Microsoft.Extensions.Options;
using IPService.Api.Configuration;
using IPService.Application.Commands.IpInfo;

namespace IPService.BackgroundServices
{
    public sealed class IpRefreshBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IpRefreshOptions _options;
        private readonly ILogger<IpRefreshBackgroundService> _logger;

        public IpRefreshBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<IpRefreshOptions> options,
            ILogger<IpRefreshBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_options.Interval);

            do
            {
                try
                {
                    // fresh scope per run → fresh scoped DbContext, disposed after
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var result = await mediator.Send(
                        new RefreshAllIpsCommand(_options.BatchSize), stoppingToken);

                    _logger.LogInformation(
                        "IP refresh run finished. Processed={Processed} Changed={Changed} Failed={Failed}",
                        result.Processed, result.Changed, result.Failed);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;   // app shutting down — expected, exit quietly
                }
                catch (Exception ex)
                {
                    // one bad RUN must not kill the service — log and wait for the next tick
                    _logger.LogError(ex, "IP refresh run failed");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}