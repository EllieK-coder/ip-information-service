// Persistence/Queries/GetIpBatchPersistence.cs
using Microsoft.EntityFrameworkCore;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;

namespace IPService.Infrastructure.Persistence.Queries
{
    public class GetIpBatchPersistence : IGetIpBatchPersistence
    {
        private readonly IpDbContext _db;
        public GetIpBatchPersistence(IpDbContext db) => _db = db;

        public async Task<IReadOnlyList<Ipv4Address>> GetBatchAsync(
            long afterId, int take, CancellationToken cancellationToken = default)
        {
            return await _db.IpAddresses
                .Where(ip => ip.AddressId > afterId)
                .OrderBy(ip => ip.AddressId)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}