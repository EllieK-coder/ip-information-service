using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using IPService.Api.Configuration;
using IPService.Application.Commands.IpInfo;

namespace IPService.Jobs
{
    // Quartz job that drives the refresh pipeline: keyset batch read → per-IP update via mediator.
    // [DisallowConcurrentExecution] stops a second run from starting if one is still going —
    // exactly the overlap protection PeriodicTimer couldn't give cleanly.
    [DisallowConcurrentExecution]
    public class IpRefreshJob : IJob
    {
        private readonly IMediator _mediator;
        private readonly IpRefreshOptions _options;
        private readonly ILogger<IpRefreshJob> _logger;

        public IpRefreshJob(
            IMediator mediator,
            IOptions<IpRefreshOptions> options,
            ILogger<IpRefreshJob> logger)
        {
            _mediator = mediator;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("IpRefreshJob started");

            try
            {
                var result = await _mediator.Send(
                    new RefreshAllIpsCommand(_options.BatchSize), context.CancellationToken);

                stopwatch.Stop();
                _logger.LogInformation(
                    "IpRefreshJob completed: Processed={Processed} Changed={Changed} Failed={Failed} in {ElapsedMs} ms",
                    result.Processed, result.Changed, result.Failed, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "IpRefreshJob failed after {ElapsedMs} ms", stopwatch.ElapsedMilliseconds);
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }
    }
}