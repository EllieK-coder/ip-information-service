using IPService.Application.Interfaces;
using IPService.Domain.Entities;

namespace IPService.Infrastructure.Persistence.Commands
{
    public class SaveJobHistoryPersistence : ISaveJobHistory
    {
        private readonly IpDbContext _context;

        public SaveJobHistoryPersistence(IpDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(JobHistory jobHistory, CancellationToken cancellationToken = default)
        {
            _context.JobHistories.Add(jobHistory);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}