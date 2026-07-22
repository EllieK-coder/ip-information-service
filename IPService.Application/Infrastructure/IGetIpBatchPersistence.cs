using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface IGetIpBatchPersistence
    {
        Task<IReadOnlyList<Ipv4Address>> GetBatchAsync(
                long afterId, int take, CancellationToken cancellationToken = default);
    }
}