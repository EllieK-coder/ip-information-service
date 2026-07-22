using IPService.Application.DTOs;
using IPService.Application.HttpClients;
using IPService.Application.Interfaces;
using IPService.Application.Queries;
using IPService.Domain.Entities;
using Moq;

namespace IPService.Tests
{
    public class GetCountryByIpHandlerTests
    {
        [Fact]
        public async Task Handle_IpInDb_ReturnsFromDbWithoutCallingGateway()//if IP is in db we dont call ip2c
        {
            //arrange
            var country = new Country("United States", "US", "USA");
            var ip = new Ipv4Address(3232235777, "US", DateTime.UtcNow);

            var fakeGetIp = new Mock<IGetIpAddress>();
            fakeGetIp
                .Setup(x => x.GetByAddressIdAsync(It.IsAny<long>()))
                .ReturnsAsync((ip, country));//san na vrethikan 
            var fakeSaveIp = new Mock<ISaveIpAddress>();
            var fakeIp2c = new Mock<IIp2cHttpClient>();
            var fakeEnsure = new Mock<IEnsureCountry>();

            var handler = new GetCountryByIpHandler(
                fakeGetIp.Object,//fake
                fakeSaveIp.Object,
                fakeIp2c.Object,
                fakeEnsure.Object);

            // Act
            var result = await handler.Handle(new GetCountryByIpQuery(3232235777), default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("US", result.TwoLetterCode);

            fakeIp2c.Verify(
            x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()),
            Times.Never);//ip2c not called 
        }

        [Fact]
        public async Task Handle_IpNotInDb_CallsGatewayAndSaves()//call ip2c if there isnt in db
        {
            // Arrange
            var fakeGetIp = new Mock<IGetIpAddress>();
            fakeGetIp
                .Setup(x => x.GetByAddressIdAsync(It.IsAny<long>()))
                .ReturnsAsync(((Ipv4Address?)null, (Country?)null));//not in db

            var fakeIp2c = new Mock<IIp2cHttpClient>();
            fakeIp2c
                .Setup(x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CountryDto("US", "USA", "United States"));//ip2c find ip

            var fakeSaveIp = new Mock<ISaveIpAddress>();
            var fakeEnsure = new Mock<IEnsureCountry>();

            var handler = new GetCountryByIpHandler(
                fakeGetIp.Object, fakeSaveIp.Object, fakeIp2c.Object, fakeEnsure.Object);

            // Act
            var result = await handler.Handle(new GetCountryByIpQuery(3232235777), default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("US", result.TwoLetterCode);

            
            //ip2c called once
            fakeIp2c.Verify(
                x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()),
                Times.Once);

            //ip saved in base
            fakeSaveIp.Verify(
                x => x.SaveAsync(It.IsAny<Ipv4Address>()),
                Times.Once);
        }

        [Fact]//TODO : check again what will do if ip2c return unknown
        public async Task Handle_GatewayReturnsNull_ReturnsUnknown()//ip2c return null
        {
            // Arrange
            var fakeGetIp = new Mock<IGetIpAddress>();
            fakeGetIp
                .Setup(x => x.GetByAddressIdAsync(It.IsAny<long>()))
                .ReturnsAsync(((Ipv4Address?)null, (Country?)null));//not in basse 

            var fakeIp2c = new Mock<IIp2cHttpClient>();
            fakeIp2c
                .Setup(x => x.GetCountryForIpv4Async(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CountryDto?)null);//ip2c: unknown IP -> null

            var fakeSaveIp = new Mock<ISaveIpAddress>();
            var fakeEnsure = new Mock<IEnsureCountry>();

            var handler = new GetCountryByIpHandler(
                fakeGetIp.Object, fakeSaveIp.Object, fakeIp2c.Object, fakeEnsure.Object);

            // Act
            var result = await handler.Handle(new GetCountryByIpQuery(3232235777), default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("XX", result.TwoLetterCode);
            Assert.Equal("Unknown", result.CountryName);
        }
    }

}