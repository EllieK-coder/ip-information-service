using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface ISaveIpAddress
    {
        Task SaveAsync(Ipv4Address ipAddress);


        Task SaveBatchAsync(IEnumerable<Ipv4Address> ipAddresses);
    }
}