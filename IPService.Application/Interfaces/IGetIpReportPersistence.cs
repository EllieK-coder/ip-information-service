using System;
using System.Collections.Generic;
using System.Text;
using IPService.Application.Queries.Report;

namespace IPService.Application.Interfaces
{
    public interface IGetIpReportPersistence
    {
        Task<IReadOnlyList<CountryReportItem>> GetReportAsync(
            IReadOnlyCollection<string>? twoLetterCodes,
            CancellationToken cancellationToken = default);
    }
}
