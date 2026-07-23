using IPService.Application.Interfaces;
using IPService.Application.Queries.Report;
using IPService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPService.Infrastructure.Persistence.Queries
{
    public class GetIpReportPersistence : IGetIpReportPersistence
    {
        private readonly IpDbContext _context;

        public GetIpReportPersistence(IpDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CountryReportItem>> GetReportAsync(
            IReadOnlyCollection<string>? twoLetterCodes,
            CancellationToken cancellationToken = default)
        {

            IQueryable<Ipv4Address> query = _context.IpAddresses.AsNoTracking();

            //conditional filter 
            if (twoLetterCodes is not null && twoLetterCodes.Count > 0)
            {
                query = query.Where(ip => twoLetterCodes.Contains(ip.TwoLetterCode));
            }

            var grouped = query
                .GroupBy(ip => ip.TwoLetterCode) //grouping by twolettercode
                .Select(g => new
                {
                    TwoLetterCode = g.Key,// US GR etc 
                    AddressesCount = g.Count(),
                    LastAddressUpdated = g.Max(ip => ip.LastUpdated)
                });

            //countries.name join
            var report = grouped
                .Join(
                    _context.Countries,
                    g => g.TwoLetterCode,
                    c => c.TwoLetterCode,
                    (g, c) => new CountryReportItem(
                        c.Name,
                        g.AddressesCount,
                        g.LastAddressUpdated
                    ));

            //sql
            return await report.ToListAsync(cancellationToken);
        }
    }
}