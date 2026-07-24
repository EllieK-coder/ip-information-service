using IPService.Application.Queries.Report;
using IPService.Controllers;
using IPService.Domain.Exceptions;
using IPService.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IPService.Tests
{
    public class ReportControllerTests
    {
        [Fact]
        public async Task GetReport_NoCodes_SendsQueryWithNullCodesAndReturns200()//no codes -> query.TwoLetterCodes null, 200 with mapped dtos
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetIpReportQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<CountryReportItem>)new List<CountryReportItem>
                {
                    new CountryReportItem("GR", "Greece", 2, new DateTime(2026, 1, 8)),
                    new CountryReportItem("US", "United States", 3, new DateTime(2026, 1, 5)),
                });

            var controller = new ReportController(fakeMediator.Object);

            //act
            var result = await controller.GetReport(null);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<CountryReportResponse>>(okResult.Value).ToList();

            Assert.Equal(2, dtos.Count);
            Assert.Contains(dtos, d => d.CountryName == "Greece" && d.AddressesCount == 2);
            Assert.Contains(dtos, d => d.CountryName == "United States" && d.AddressesCount == 3);

            //null codes reached the query untouched (all countries, FR-2)
            fakeMediator.Verify(
                m => m.Send(It.Is<GetIpReportQuery>(q => q.TwoLetterCodes == null), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetReport_WithCommaSeparatedCodes_SplitsTrimsAndSendsList()//"US, gr" -> ["US","gr"] passed through to the query as-is
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetIpReportQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<CountryReportItem>)new List<CountryReportItem>());

            var controller = new ReportController(fakeMediator.Object);

            //act
            var result = await controller.GetReport("US, gr");

            //assert
            Assert.IsType<OkObjectResult>(result);

            //controller only splits/trims the raw query string -- normalization/validation
            //is Step 4's job (CountryCodeValidator), not the controller's
            fakeMediator.Verify(
                m => m.Send(
                    It.Is<GetIpReportQuery>(q =>
                        q.TwoLetterCodes != null &&
                        q.TwoLetterCodes.SequenceEqual(new[] { "US", "gr" })),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetReport_EmptyReport_Returns200WithEmptyList()//no matches -> still 200, not 404
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetIpReportQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<CountryReportItem>)new List<CountryReportItem>());

            var controller = new ReportController(fakeMediator.Object);

            //act
            var result = await controller.GetReport("ZZ");

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<CountryReportResponse>>(okResult.Value);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetReport_MalformedCode_Returns400()//FR-4: handler/validator throws -> controller maps to 400
        {
            //arrange
            var fakeMediator = new Mock<IMediator>();
            fakeMediator
                .Setup(m => m.Send(It.IsAny<GetIpReportQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidCountryCodeException("XYZ"));

            var controller = new ReportController(fakeMediator.Object);

            //act
            var result = await controller.GetReport("XYZ");

            //assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("XYZ", badRequest.Value!.ToString());
        }
    }
}
