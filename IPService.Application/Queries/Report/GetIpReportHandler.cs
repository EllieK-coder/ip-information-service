using System;
using System.Collections.Generic;
using System.Text;
using IPService.Application.Interfaces;
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
            // No cache→db→gateway chain here (FR-5) — straight pass-through to persistence.
            return _reportPersistence.GetReportAsync(request.TwoLetterCodes, cancellationToken);
        }
    }
}

