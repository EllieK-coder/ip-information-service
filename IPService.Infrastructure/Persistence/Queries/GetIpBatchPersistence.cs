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
            int skip, int take, CancellationToken cancellationToken = default)
        {
            return await _db.IpAddresses
                .OrderBy(ip => ip.AddressId)   // stable order → pages never overlap or skip
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}