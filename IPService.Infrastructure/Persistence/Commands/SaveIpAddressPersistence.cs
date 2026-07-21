using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPService.Infrastructure.Persistence.Commands
{
    public class SaveIpAddressPersistence : ISaveIpAddress
    {
        private readonly IpDbContext _context;

        public SaveIpAddressPersistence(IpDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(Ipv4Address ipAddress)
        {
            // Upsert: update if it exists, insert otherwise
            var existing = await _context.IpAddresses
                .FirstOrDefaultAsync(ip => ip.AddressId == ipAddress.AddressId);

            if (existing is null)
            {
                _context.IpAddresses.Add(ipAddress);
            }
            else
            {
                existing.SetTwoLetterCode(ipAddress.TwoLetterCode);
                existing.SetLastUpdated(ipAddress.LastUpdated);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveBatchAsync(IEnumerable<Ipv4Address> ipAddresses)
        {
            // Single transaction for the whole batch (all-or-nothing)
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var ipAddress in ipAddresses)
                {
                    var existing = await _context.IpAddresses
                        .FirstOrDefaultAsync(ip => ip.AddressId == ipAddress.AddressId);

                    if (existing is null)
                    {
                        _context.IpAddresses.Add(ipAddress);
                    }
                    else
                    {
                        existing.SetTwoLetterCode(ipAddress.TwoLetterCode);
                        existing.SetLastUpdated(ipAddress.LastUpdated);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}