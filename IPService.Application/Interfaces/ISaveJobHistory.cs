using IPService.Domain.Entities;

namespace IPService.Application.Interfaces
{
    public interface ISaveJobHistory
    {
        Task SaveAsync(JobHistory jobHistory, CancellationToken cancellationToken = default);
    }
}