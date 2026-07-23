using IPService.Domain.Entities;
using IPService.Infrastructure.Persistence;
using IPService.Infrastructure.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace IPService.Tests
{
    public class GetIpBatchPersistenceTests
    {
        //fake in memory db
        private static IpDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<IpDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new IpDbContext(options);
        }

        [Fact]
        public async Task GetBatchAsync_FromStart_ReturnsOrderedBatch()//afterId 0 -> first N, ordered
        {
            //arrange
            var context = CreateInMemoryContext();
            //add ips out of order (10, 30, 20) to test ordering
            context.IpAddresses.Add(new Ipv4Address(10, "US", DateTime.UtcNow));
            context.IpAddresses.Add(new Ipv4Address(30, "GR", DateTime.UtcNow));
            context.IpAddresses.Add(new Ipv4Address(20, "DE", DateTime.UtcNow));
            await context.SaveChangesAsync();

            var persistence = new GetIpBatchPersistence(context);

            //act 
            var batch = await persistence.GetBatchAsync(0, 10);//from 0 to 10

            //assert
            Assert.Equal(3, batch.Count);//is 3?
            
            Assert.Equal(10, batch[0].AddressId);
            Assert.Equal(20, batch[1].AddressId);
            Assert.Equal(30, batch[2].AddressId);
        }

        [Fact]
        public async Task GetBatchAsync_WithCursor_ReturnsOnlyAfterId()//afterId=15 -> only 10 ips after 15
        {
            //arrange
            var context = CreateInMemoryContext();
            context.IpAddresses.Add(new Ipv4Address(10, "US", DateTime.UtcNow));
            context.IpAddresses.Add(new Ipv4Address(20, "GR", DateTime.UtcNow));
            context.IpAddresses.Add(new Ipv4Address(30, "DE", DateTime.UtcNow));
            await context.SaveChangesAsync();

            var persistence = new GetIpBatchPersistence(context);

            //act 
            var batch = await persistence.GetBatchAsync(15, 10);//from 15 take 10 (16,17,18...)

            //assert
            Assert.Equal(2, batch.Count);//is only the 20 and 30 ?
            Assert.Equal(20, batch[0].AddressId);
            Assert.Equal(30, batch[1].AddressId);
        }

        [Fact]
        public async Task GetBatchAsync_RespectsTakeLimit()//take=2 (only 2)
        {
            //arrange
            var context = CreateInMemoryContext();
            context.IpAddresses.Add(new Ipv4Address(10, "US", DateTime.UtcNow));
            context.IpAddresses.Add(new Ipv4Address(20, "GR", DateTime.UtcNow));
            context.IpAddresses.Add(new Ipv4Address(30, "DE", DateTime.UtcNow));
            await context.SaveChangesAsync();

            var persistence = new GetIpBatchPersistence(context);

            //act
            var batch = await persistence.GetBatchAsync(0, 2);//take 2 from 0 

            //assert
            Assert.Equal(2, batch.Count);//is 2?
            Assert.Equal(10, batch[0].AddressId);
            Assert.Equal(20, batch[1].AddressId);
        }
    }
}