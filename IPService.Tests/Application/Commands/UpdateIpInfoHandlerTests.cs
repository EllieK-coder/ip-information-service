using IPService.Application.Caching;
using IPService.Application.Commands.IpInfo;
using IPService.Application.DTOs;
using IPService.Application.HttpClients;
using IPService.Application.Infrastructure;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using Moq;

namespace IPService.Tests.Application.Commands
{
    public class UpdateIpInfoHandlerTests
    {
        [Fact]
        public async Task Handle_CountryChanged_UpdatesAndInvalidatesCache()//ip country changed in ip2c -> update db ,delete cache
        {
            //arrange
            var ip = new Ipv4Address(3232235777, "US", new DateTime(2026, 1, 1));

            //ip2c now says GR (changed from US)
            var fakeIp2c = new Mock<IIp2cHttpClient>();
            fakeIp2c
                .Setup(x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CountryDto("GR", "GRC", "Greece"));

            var fakeUpdate = new Mock<IUpdateIpInfoPersistence>();
            var fakeEnsure = new Mock<IEnsureCountry>();
            var fakeCache = new Mock<ICache>();

            var handler = new UpdateIpInfoHandler(
                fakeIp2c.Object, fakeUpdate.Object, fakeEnsure.Object, fakeCache.Object);

            //act
            var result = await handler.Handle(new UpdateIpInfoCommand(ip), default);

            //assert
            Assert.True(result);//changed

            //entity updated to GR
            Assert.Equal("GR", ip.TwoLetterCode);

            //update happened in db
            fakeUpdate.Verify(
                x => x.UpdateAsync(It.IsAny<Ipv4Address>(), It.IsAny<CancellationToken>()),
                Times.Once);

            //cache invalidated with the corect key
            fakeCache.Verify(
                x => x.Remove(IpCacheKeys.ForIp(3232235777)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_GatewayReturnsNull_DoesNothing()//ip2c null ->handler does nothing, no update, no cache just false
        {
            //arrange
            var ip = new Ipv4Address(3232235777, "US", new DateTime(2026, 1, 1));

            //ip2c returns null (unknown)
            var fakeIp2c = new Mock<IIp2cHttpClient>();
            fakeIp2c
                .Setup(x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CountryDto?)null);

            var fakeUpdate = new Mock<IUpdateIpInfoPersistence>();
            var fakeEnsure = new Mock<IEnsureCountry>();
            var fakeCache = new Mock<ICache>();

            var handler = new UpdateIpInfoHandler(
                fakeIp2c.Object, fakeUpdate.Object, fakeEnsure.Object, fakeCache.Object);

            //act
            var result = await handler.Handle(new UpdateIpInfoCommand(ip), default);

            //assert
            Assert.False(result);//nothing changed

            //no update, no cache evict
            fakeUpdate.Verify(
                x => x.UpdateAsync(It.IsAny<Ipv4Address>(), It.IsAny<CancellationToken>()),
                Times.Never);
            fakeCache.Verify(
                x => x.Remove(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_SameCountry_DoesNothing()//same country -> false, no update, no delete cache
        {
            //arrange
            var ip = new Ipv4Address(3232235777, "US", new DateTime(2026, 1, 1));

            //ip2c returns the SAME country (US)
            var fakeIp2c = new Mock<IIp2cHttpClient>();
            fakeIp2c
                .Setup(x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CountryDto("US", "USA", "United States"));//same

            var fakeUpdate = new Mock<IUpdateIpInfoPersistence>();
            var fakeEnsure = new Mock<IEnsureCountry>();
            var fakeCache = new Mock<ICache>();

            var handler = new UpdateIpInfoHandler(
                fakeIp2c.Object, fakeUpdate.Object, fakeEnsure.Object, fakeCache.Object);

            //act
            var result = await handler.Handle(new UpdateIpInfoCommand(ip), default);

            //assert
            Assert.False(result);//no change

            //noupdate, no cache
            fakeUpdate.Verify(
                x => x.UpdateAsync(It.IsAny<Ipv4Address>(), It.IsAny<CancellationToken>()),
                Times.Never);
            fakeCache.Verify(
                x => x.Remove(It.IsAny<string>()),
                Times.Never);
        }
    }
}