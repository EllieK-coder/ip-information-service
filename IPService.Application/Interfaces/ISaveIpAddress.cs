using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface ISaveIpAddress
    {
        /// <summary>
        ///save a single IP address to database(insert or update).
        /// </summary>
        Task SaveAsync(Ipv4Address ipAddress);

        /// <summary>
        ///save multiple IPs in a batch transaction(for Task 2 updates).
        /// </summary>
        Task SaveBatchAsync(IEnumerable<Ipv4Address> ipAddresses);
    }
}