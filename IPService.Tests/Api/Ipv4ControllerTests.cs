using IPService.Application.DTOs;
using IPService.Application.Queries;
using IPService.Controllers;
using IPService.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IPService.Tests.Infrastructure.Persistence
{
    public class Ipv4ControllerTests
    {
        [Fact]
        public async Task GetCountryByIpv4_ValidIpWithResult_Returns200()
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetCountryByIpv4Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CountryDto("US", "USA", "United States"));

            var controller = new Ipv4Controller(fakeMediator.Object);

            //act
            var result = await controller.GetCountryByIpv4("192.168.1.1");

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);//200 ok
            var dto = Assert.IsType<CountryDto>(okResult.Value);
            Assert.Equal("US", dto.TwoLetterCode);
        }

        [Fact]
        public async Task GetCountryByIpv4_InvalidIp_ThrowsInvalidIpv4InputException()
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            var controller = new Ipv4Controller(fakeMediator.Object);

            //act + assert
            // Ο controller πλέον ΔΕΝ πιάνει το exception — το αφήνει να ανέβει
            // στο κεντρικό exception middleware (S1). Το 400 το εγγυάται εκείνο.
            await Assert.ThrowsAsync<InvalidIpv4InputException>(
                () => controller.GetCountryByIpv4("999.999.999.999"));

            //mediator δεν κλήθηκε — σταμάτησε στο ParseToLong (domain)
            fakeMediator.Verify(
                m => m.Send(It.IsAny<GetCountryByIpv4Query>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GetCountryByIpv4_ValidIpButUnknown_Returns200WithUnknown()
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetCountryByIpv4Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CountryDto("XX", "XXX", "Unknown"));//unknown → Unknown DTO, όχι null

            var controller = new Ipv4Controller(fakeMediator.Object);

            //act
            var result = await controller.GetCountryByIpv4("192.168.1.1");

            //assert
            // Unknown IP → 200 με "Unknown" (όχι 404). Σκόπιμη απόφαση:
            // cache-άρει τα unknowns, συνεπές με IP2C status 2.
            var okResult = Assert.IsType<OkObjectResult>(result);//200
            var dto = Assert.IsType<CountryDto>(okResult.Value);
            Assert.Equal("Unknown", dto.CountryName);
        }
    }
}