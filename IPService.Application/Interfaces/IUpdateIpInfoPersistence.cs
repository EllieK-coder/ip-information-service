using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface IUpdateIpInfoPersistence
    {
        Task UpdateAsync(Ipv4Address ipAddress, CancellationToken cancellationToken = default);
    }
}