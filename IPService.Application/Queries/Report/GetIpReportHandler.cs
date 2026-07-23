using System;
using System.Collections.Generic;
using System.Text;
using IPService.Application.Interfaces;
using IPService.Application.Validation;
using MediatR;

namespace IPService.Application.Queries.Report
{
    public class GetIpReportHandler : IRequestHandler<GetIpReportQuery, IReadOnlyList<CountryReportItem>>
    {
        private readonly IGetIpReportPersistence _reportPersistence;

        public GetIpReportHandler(IGetIpReportPersistence reportPersistence)
        {
            _reportPersistence = reportPersistence;
        }

        public Task<IReadOnlyList<CountryReportItem>> Handle(
            GetIpReportQuery request, CancellationToken cancellationToken)
        {
            // FR-4: validate + normalize case before it ever reaches Step 2's Contains filter.
            var codes = CountryCodeValidator.NormalizeAndValidate(request.TwoLetterCodes);

            // No cache→db→gateway chain here (FR-5) — straight pass-through to persistence.
            return _reportPersistence.GetReportAsync(codes, cancellationToken);
        }
    }
}

