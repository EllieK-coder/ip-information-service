using IPService.Domain.Exceptions;
using IPService.Gateway;
using Moq;
using Moq.Protected;
using System.Net;

namespace IPService.Tests
{
    public class Ip2cHttpClientTests
    {
        private static HttpClient CreateFakeHttpClient(string responseBody)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseBody)
                });

            return new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://ip2c.org")
            };
        }

        [Fact]
        public async Task GetCountryForIpv4Async_Status1_ReturnsCountry()//known IP:status(case), twolettercode, threelettercode, countryname
        {
            //arrange
            var httpClient = CreateFakeHttpClient("1;US;USA;United States");
            var gateway = new Ip2cHttpClient(httpClient);

            //act
            var result = await gateway.GetCountryForIpv4Async(3232235777);

            //assert
            Assert.NotNull(result);
            Assert.Equal("US", result.TwoLetterCode);
            Assert.Equal("USA", result.ThreeLetterCode);
            Assert.Equal("United States", result.CountryName);
        }

        [Fact]
        public async Task GetCountryForIpv4Async_Status2_ReturnsNull()//unknown IP: null
        {
            //arrange
            var httpClient = CreateFakeHttpClient("2;;;");
            var gateway = new Ip2cHttpClient(httpClient);

            //act
            var result = await gateway.GetCountryForIpv4Async(3232235777);

            //assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCountryForIpv4Async_Status0_ThrowsException()//invalid: exception
        {
            //arrange
            var httpClient = CreateFakeHttpClient("0;;;");
            var gateway = new Ip2cHttpClient(httpClient);

            //act & Assert
            await Assert.ThrowsAsync<Ip2cException>(
                () => gateway.GetCountryForIpv4Async(3232235777));
        }

        [Fact]
        public async Task GetCountryForIpv4Async_MalformedSuccess_ThrowsException() //status 1 missing parts
        {
            var httpClient = CreateFakeHttpClient("1;US");
            var gateway = new Ip2cHttpClient(httpClient);
            await Assert.ThrowsAsync<Ip2cException>(
                () => gateway.GetCountryForIpv4Async(3232235777));
        }
    }
}