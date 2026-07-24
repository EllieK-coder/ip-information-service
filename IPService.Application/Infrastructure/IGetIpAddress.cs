using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface IGetIpAddress
    {
        Task<(Ipv4Address? IpAddress, Country? Country)> GetByAddressIdAsync(long addressId);
    }
}
