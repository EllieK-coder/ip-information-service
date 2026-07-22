using IPService.Application.Caching;
using IPService.Application.Decorators;
using IPService.Application.DTOs;
using IPService.Application.Queries;
using MediatR;
using Moq;

namespace IPService.Tests
{
    public class CachingRequestHandlerTests
    {
        [Fact]
        public async Task Handle_CacheHit_ReturnsCachedWithoutCallingInner()//cache hit -> return cached, dont call inner
        {
            //arrange
            var query = new GetCountryByIpQuery(3232235777);
            var cachedDto = new CountryDto("US", "USA", "United States");

            var fakeCache = new Mock<ICache>();
            fakeCache
                .Setup(c => c.TryGet<CountryDto?>(It.IsAny<string>(), out It.Ref<CountryDto?>.IsAny))
                .Returns((string k, out CountryDto? v) => { v = cachedDto; return true; });

            //the inner handler that should NOT be called
            var fakeInner = new Mock<IRequestHandler<GetCountryByIpQuery, CountryDto?>>();

            var handler = new CachingRequestHandler<GetCountryByIpQuery, CountryDto?>(
                fakeInner.Object, fakeCache.Object);

            //act
            var result = await handler.Handle(query, default);

            //assert
            Assert.NotNull(result);
            Assert.Equal("US", result.TwoLetterCode);

            //inner NOT called (cache hit)
            fakeInner.Verify(
                x => x.Handle(It.IsAny<GetCountryByIpQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_CacheMiss_CallsInnerAndStores()//cache miss -> call inner, then store
        {
            //arrange
            var query = new GetCountryByIpQuery(3232235777);
            var freshDto = new CountryDto("GR", "GRC", "Greece");

            //cache says "miss"
            var fakeCache = new Mock<ICache>();
            fakeCache
                .Setup(c => c.TryGet<CountryDto?>(It.IsAny<string>(), out It.Ref<CountryDto?>.IsAny))
                .Returns((string k, out CountryDto? v) => { v = null; return false; });

            //inner returns fresh data
            var fakeInner = new Mock<IRequestHandler<GetCountryByIpQuery, CountryDto?>>();
            fakeInner
                .Setup(x => x.Handle(It.IsAny<GetCountryByIpQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(freshDto);

            var handler = new CachingRequestHandler<GetCountryByIpQuery, CountryDto?>(
                fakeInner.Object, fakeCache.Object);

            //act
            var result = await handler.Handle(query, default);

            //assert
            Assert.NotNull(result);
            Assert.Equal("GR", result.TwoLetterCode);//came from inner

            //inner WAS called
            fakeInner.Verify(
                x => x.Handle(It.IsAny<GetCountryByIpQuery>(), It.IsAny<CancellationToken>()),
                Times.Once);

            //and stored in cache
            fakeCache.Verify(
                c => c.Set(It.IsAny<string>(), It.IsAny<CountryDto?>(), It.IsAny<TimeSpan>()),
                Times.Once);
        }
    }
}