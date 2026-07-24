using IPService.Domain.Entities;

namespace IPService.Application.Infrastructure
{
    public interface IUpdateIpInfoPersistence
    {
        Task UpdateAsync(Ipv4Address ipAddress, CancellationToken cancellationToken = default);
    }
}