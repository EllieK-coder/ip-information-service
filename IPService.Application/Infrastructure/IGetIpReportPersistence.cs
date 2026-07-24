using IPService.Application.Queries.Report;

namespace IPService.Application.Infrastructure
{
    public interface IGetIpReportPersistence
    {
        Task<IReadOnlyList<CountryReportItem>> GetReportAsync(
            IReadOnlyCollection<string>? twoLetterCodes,
            CancellationToken cancellationToken = default);
    }
}
