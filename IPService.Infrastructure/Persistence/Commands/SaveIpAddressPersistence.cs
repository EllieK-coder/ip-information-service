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
            var batch = ipAddresses.ToList();//took the ips into a list 

            var ids = batch.Select(ip => ip.AddressId).ToList();//all AddressIds we want to save

            //took all the ips that existing
            var existingDict = await _context.IpAddresses
                .Where(ip => ids.Contains(ip.AddressId))
                .ToDictionaryAsync(ip => ip.AddressId);

            //one transaction for the whole batch
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var ipAddress in batch)
                {
                    if (existingDict.TryGetValue(ipAddress.AddressId, out var existing))//look in memory
                    {
                        existing.SetTwoLetterCode(ipAddress.TwoLetterCode);
                        existing.SetLastUpdated(ipAddress.LastUpdated);
                    }
                    else
                    {
                        _context.IpAddresses.Add(ipAddress);//new record
                    }
                }

                //single write for all changes 
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