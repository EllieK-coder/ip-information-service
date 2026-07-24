using IPService.Domain.Entities;
using IPService.Infrastructure.Persistence;
using IPService.Infrastructure.Persistence.Commands;
using IPService.Infrastructure.Persistence.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IPService.Tests.Infrastructure.Persistence
{
    public class PersistenceTests
    {
        private static IpDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<IpDbContext>()//fake new db in memory
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new IpDbContext(options);
        }

        [Fact]
        public async Task SaveAsync_ThenGet_ReturnsSavedIp()//check if the base save an ip and return it back (new insert)
        {
            //arrange
            var context = CreateInMemoryContext();//set the base

            //first add a country for FK 
            context.Countries.Add(new Country("United States", "US", "USA"));
            await context.SaveChangesAsync();

            var saver = new SaveIpAddressPersistence(context);//the class which writes
            var getter = new GetIpAddressPersistence(context);//the class which reads

            var ip = new Ipv4Address(3232235777, "US", DateTime.UtcNow);

            //act
            await saver.SaveAsync(ip);
            var (savedIp, country) = await getter.GetByAddressIdAsync(3232235777);

            //assert
            Assert.NotNull(savedIp);//is there the ip?
            Assert.Equal(3232235777, savedIp.AddressId);//is correct ?
            Assert.Equal("US", savedIp.TwoLetterCode);//is the right country?
        }

        [Fact]
        public async Task GetByAddressIdAsync_NotFound_ReturnsNull()//ip that doesnt exist
        {
            var context = CreateInMemoryContext();
            var getter = new GetIpAddressPersistence(context);

            var (savedIp, country) = await getter.GetByAddressIdAsync(999999);//check for ip 999999

            Assert.Null(savedIp);
        }

        [Fact]
        public async Task SaveAsync_ExistingIp_UpdatesInsteadOfDuplicate()
        {
            var context = CreateInMemoryContext();
            context.Countries.Add(new Country("United States", "US", "USA"));
            context.Countries.Add(new Country("Greece", "GR", "GRC"));
            await context.SaveChangesAsync();

            var saver = new SaveIpAddressPersistence(context);

            await saver.SaveAsync(new Ipv4Address(3232235777, "US", DateTime.UtcNow));
            //save the same with other twoletter
            await saver.SaveAsync(new Ipv4Address(3232235777, "GR", DateTime.UtcNow));

            var count = await context.IpAddresses.CountAsync();//is one row ?
            Assert.Equal(1, count);
        }
    }
}
