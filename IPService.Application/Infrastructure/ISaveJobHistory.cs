using IPService.Domain.Entities;

namespace IPService.Application.Infrastructure
{
    public interface ISaveJobHistory
    {
        Task SaveAsync(JobHistory jobHistory, CancellationToken cancellationToken = default);
    }
}