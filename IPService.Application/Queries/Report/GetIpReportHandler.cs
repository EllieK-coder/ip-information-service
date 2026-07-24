using IPService.Application.Infrastructure;
using IPService.Application.Validation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IPService.Application.Queries.Report
{
    public class GetIpReportHandler : IRequestHandler<GetIpReportQuery, IReadOnlyList<CountryReportItem>>
    {
        private readonly IGetIpReportPersistence _reportPersistence;
        private readonly ILogger<GetIpReportHandler> _logger;

        public GetIpReportHandler(IGetIpReportPersistence reportPersistence, ILogger<GetIpReportHandler> logger)
        {
            _reportPersistence = reportPersistence;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CountryReportItem>> Handle(
            GetIpReportQuery request, CancellationToken cancellationToken)
        {
            // FR-4: validate + normalize case before it ever reaches Step 2's Contains filter.
            var codes = CountryCodeValidator.NormalizeAndValidate(request.TwoLetterCodes);

            // No cache→db→gateway chain here (FR-5) — straight pass-through to persistence.
            var report = await _reportPersistence.GetReportAsync(codes, cancellationToken);

            // Log any requested code that wasn't found (valid format, but no data for it).
            if (codes is not null && codes.Count > 0)
            {
                var returned = report.Select(r => r.TwoLetterCode).ToHashSet();
                foreach (var code in codes)
                {
                    if (!returned.Contains(code))
                        _logger.LogInformation(
                            "Country code {Code} not found in system / no IPs stored", code);
                }
            }

            return report;
        }
    }
}

