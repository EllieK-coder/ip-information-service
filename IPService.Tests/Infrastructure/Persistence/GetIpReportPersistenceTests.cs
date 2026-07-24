using IPService.Domain.Entities;
using IPService.Infrastructure.Persistence;
using IPService.Infrastructure.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace IPService.Tests.Infrastructure.Persistence
{
    public class GetIpReportPersistenceTests
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
        public async Task GetReportAsync_AllCountries_GroupsAndCounts()//null codes: all countries, count + max
        {
            //arrange
            var context = CreateInMemoryContext();

            //2 countries
            context.Countries.Add(new Country("United States", "US", "USA"));
            context.Countries.Add(new Country("Greece", "GR", "GRC"));

            //3 US ips, 2 GR ips
            context.IpAddresses.Add(new Ipv4Address(1, "US", new DateTime(2026, 1, 1)));
            context.IpAddresses.Add(new Ipv4Address(2, "US", new DateTime(2026, 1, 5))); //latest US
            context.IpAddresses.Add(new Ipv4Address(3, "US", new DateTime(2026, 1, 3)));
            context.IpAddresses.Add(new Ipv4Address(4, "GR", new DateTime(2026, 1, 2)));
            context.IpAddresses.Add(new Ipv4Address(5, "GR", new DateTime(2026, 1, 8))); //latest GR
            await context.SaveChangesAsync();

            var persistence = new GetIpReportPersistence(context);

            //act null, all countris
            var report = await persistence.GetReportAsync(null);

            //assert
            Assert.Equal(2, report.Count);

            var us = report.First(r => r.CountryName == "United States");
            Assert.Equal(3, us.AddressesCount); //3 US ips
            Assert.Equal(new DateTime(2026, 1, 5), us.LastAddressUpdated); //latest

            var gr = report.First(r => r.CountryName == "Greece");
            Assert.Equal(2, gr.AddressesCount); //2 GR ips
            Assert.Equal(new DateTime(2026, 1, 8), gr.LastAddressUpdated); //latest
        }

        [Fact]
        public async Task GetReportAsync_WithCodes_ReturnsOnlyThoseCountries()//codes, only those countries
        {
            //arrange
            var context = CreateInMemoryContext();

            context.Countries.Add(new Country("United States", "US", "USA"));
            context.Countries.Add(new Country("Greece", "GR", "GRC"));
            context.Countries.Add(new Country("Germany", "DE", "DEU"));

            context.IpAddresses.Add(new Ipv4Address(1, "US", new DateTime(2026, 1, 1)));
            context.IpAddresses.Add(new Ipv4Address(2, "GR", new DateTime(2026, 1, 2)));
            context.IpAddresses.Add(new Ipv4Address(3, "DE", new DateTime(2026, 1, 3)));
            await context.SaveChangesAsync();

            var persistence = new GetIpReportPersistence(context);

            //act only US & GR
            var report = await persistence.GetReportAsync(new[] { "US", "GR" });

            //assert
            Assert.Equal(2, report.Count);//only US & GR
            Assert.Contains(report, r => r.CountryName == "United States");
            Assert.Contains(report, r => r.CountryName == "Greece");
            Assert.DoesNotContain(report, r => r.CountryName == "Germany");
        }

        [Fact]
        public async Task GetReportAsync_EmptyDatabase_ReturnsEmptyReport()//empty db
        {
            //arrange
            var context = CreateInMemoryContext();//empty
            var persistence = new GetIpReportPersistence(context);

            //act
            var report = await persistence.GetReportAsync(null);

            //assert
            Assert.Empty(report);
        }
    }
}