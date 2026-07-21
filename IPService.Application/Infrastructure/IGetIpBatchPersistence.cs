using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface IGetIpBatchPersistence
    {
        Task<IReadOnlyList<Ipv4Address>> GetBatchAsync(int skip, int take, CancellationToken cancellationToken = default);
    }
}