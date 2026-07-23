using IPService.Domain.Entities;
using IPService.Infrastructure.Persistence;
using IPService.Infrastructure.Persistence.Commands;
using Microsoft.EntityFrameworkCore;

namespace IPService.Tests
{
    public class SaveJobHistoryPersistenceTests
    {
        private static IpDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<IpDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new IpDbContext(options);
        }

        [Fact]
        public async Task SaveAsync_PersistsJobHistory()//save then read back
        {
            //arrange
            var context = CreateInMemoryContext();
            var persistence = new SaveJobHistoryPersistence(context);

            var job = new JobHistory(
                new DateTime(2026, 1, 1, 10, 0, 0), //started
                new DateTime(2026, 1, 1, 10, 5, 0), //finished
                "Success",
                100, // processed
                5,   // updated
                2);  // failed

            //act
            await persistence.SaveAsync(job);

            //assert, read it back
            var saved = await context.JobHistories.FirstOrDefaultAsync();
            Assert.NotNull(saved);//finded
            Assert.Equal("Success", saved.Status);
            Assert.Equal(100, saved.ProcessedRecords);
            Assert.Equal(5, saved.UpdatedRecords);
            Assert.Equal(2, saved.FailedRecords);
        }
    }
}