using IPService.Application.DTOs;
using IPService.Application.Queries;
using IPService.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IPService.Tests
{
    public class IpControllerTests
    {
        [Fact]
        public async Task GetByIp_ValidIpWithResult_Returns200()//valid ip, get counrty , get 200 ok 
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetCountryByIpQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CountryDto("US", "USA", "United States"));

            var controller = new IpController(fakeMediator.Object);

            //act
            var result = await controller.GetByIp("192.168.1.1");

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);//200 ok
            var dto = Assert.IsType<CountryDto>(okResult.Value);
            Assert.Equal("US", dto.TwoLetterCode);
        }

        [Fact]
        public async Task GetByIp_InvalidIp_Returns400()
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            var controller = new IpController(fakeMediator.Object);

            //act
            var result = await controller.GetByIp("999.999.999.999");

            //assert
            Assert.IsType<BadRequestObjectResult>(result);

            //mediator shouldnt called because proccess stop in parsetolong (domain)
            fakeMediator.Verify(
                m => m.Send(It.IsAny<GetCountryByIpQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GetByIp_ValidIpButNotFound_Returns404()
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetCountryByIpQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CountryDto?)null);//mediator returns null

            var controller = new IpController(fakeMediator.Object);

            //act
            var result = await controller.GetByIp("192.168.1.1");

            //assert
            Assert.IsType<NotFoundResult>(result);//404
        }


    }
}