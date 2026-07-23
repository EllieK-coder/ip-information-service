using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPService.Infrastructure.Persistence.Queries
{
    public class GetIpAddressPersistence : IGetIpAddress
    {
        private readonly IpDbContext _context;

        public GetIpAddressPersistence(IpDbContext context)
        {
            _context = context;
        }

        public async Task<(Ipv4Address? IpAddress, Country? Country)> GetByAddressIdAsync(long addressId)
        {
            //Read-only lookup, no tracking needed
            var ipAddress = await _context.IpAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(ip => ip.AddressId == addressId);

            if (ipAddress is null)
            {
                return (null, null);
            }

            //No navigation property on Ipv4Address -> separate lookup
            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.TwoLetterCode == ipAddress.TwoLetterCode);

            return (ipAddress, country);
        }
    }
}