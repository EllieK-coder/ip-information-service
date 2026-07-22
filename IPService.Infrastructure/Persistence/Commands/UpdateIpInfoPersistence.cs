// Persistence/Commands/UpdateIpInfoPersistence.cs
using Microsoft.EntityFrameworkCore;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;

namespace IPService.Infrastructure.Persistence.Commands
{
    public class UpdateIpInfoPersistence : IUpdateIpInfoPersistence
    {
        private readonly IpDbContext _db;

        public UpdateIpInfoPersistence(IpDbContext db) => _db = db;

        public async Task UpdateAsync(Ipv4Address ipAddress, CancellationToken cancellationToken = default)
        {
            _db.IpAddresses.Update(ipAddress);   // UPDATE on existing key — never an INSERT
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
