using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace IPService.Application.Queries.Report
{
    // FR-2: null means "all countries", a non-null list means "filter to these codes".
    // Deliberately does NOT implement ICacheableQuery — this read is never cached (FR-5).
    public record GetIpReportQuery(IReadOnlyCollection<string>? TwoLetterCodes)
        : IRequest<IReadOnlyList<CountryReportItem>>;
}

