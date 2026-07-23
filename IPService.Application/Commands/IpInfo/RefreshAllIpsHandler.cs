using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IPService.Application.Commands.IpInfo
{
    public sealed class RefreshAllIpsHandler
        : IRequestHandler<RefreshAllIpsCommand, RefreshAllIpsResult>
    {
        private readonly IGetIpBatchPersistence _batch;
        private readonly IMediator _mediator;
        private readonly ILogger<RefreshAllIpsHandler> _logger;
        private readonly ISaveJobHistory _saveJobHistory;

        public RefreshAllIpsHandler(
            IGetIpBatchPersistence batch,
            IMediator mediator,
            ILogger<RefreshAllIpsHandler> logger,
            ISaveJobHistory saveJobHistory)
        {
            _batch = batch;
            _mediator = mediator;
            _logger = logger;
            _saveJobHistory = saveJobHistory;
        }

        public async Task<RefreshAllIpsResult> Handle(
            RefreshAllIpsCommand request, CancellationToken ct)
        {
            var startedAt = DateTime.UtcNow;
            var batchSize = request.BatchSize;
            long afterId = 0;                     // keyset cursor: start before the first key
            int processed = 0, changed = 0, failed = 0;

            while (!ct.IsCancellationRequested)
            {
                var batch = await _batch.GetBatchAsync(afterId, batchSize, ct);
                if (batch.Count == 0) break;      // no more rows

                foreach (var ip in batch)
                {
                    ct.ThrowIfCancellationRequested();
                    processed++;
                    try
                    {
                        var didChange = await _mediator.Send(new UpdateIpInfoCommand(ip), ct);
                        if (didChange) changed++;
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        // one bad IP must NOT abort the whole sweep
                        _logger.LogWarning(ex,
                            "Failed to refresh IP {AddressId}", ip.AddressId);
                    }
                }

                afterId = batch[^1].AddressId;    // advance cursor to last row's key
                if (batch.Count < batchSize) break; // short page = last page

            }

            var finishedAt = DateTime.UtcNow;

            var status = failed > 0 ? "Failed" : "Success";
            await _saveJobHistory.SaveAsync(
                new JobHistory(startedAt, finishedAt, status, processed, changed, failed),
                ct);

            _logger.LogInformation(
                "IP refresh complete. Processed={Processed} Changed={Changed} Failed={Failed}",
                processed, changed, failed);

            return new RefreshAllIpsResult(processed, changed, failed);
        }
    }
}