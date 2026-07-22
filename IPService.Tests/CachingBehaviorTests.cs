using IPService.Application.Behaviors;
using IPService.Application.Caching;
using IPService.Application.DTOs;
using IPService.Application.Queries;
using MediatR;
using Moq;

namespace IPService.Tests
{
    public class CachingBehaviorTests
    {
        [Fact]
        public async Task Handle_CacheHit_ReturnsCachedWithoutCallingNext()
        {
            //arrange
            var query = new GetCountryByIpQuery(3232235777);
            var cachedDto = new CountryDto("US", "USA", "United States");//cache

            //fake cache that it has this ip 
            var fakeCache = new Mock<ICache>();
            fakeCache
                .Setup(c => c.TryGet<CountryDto>(It.IsAny<string>(), out It.Ref<CountryDto?>.IsAny))
                .Returns((string k, out CountryDto? v) => { v = cachedDto; return true; });

            var behavior = new CachingBehavior<GetCountryByIpQuery, CountryDto?>(fakeCache.Object);

            //fake next to see if it called 
            var nextCalled = false;
            RequestHandlerDelegate<CountryDto?> next = (ct) =>
            {
                nextCalled = true;
                return Task.FromResult<CountryDto?>(new CountryDto("XX", "XXX", "ShouldNotHappen"));
            };

            //act
            var result = await behavior.Handle(query, next, default);

            //assert
            Assert.NotNull(result);
            Assert.Equal("US", result.TwoLetterCode);//came from cache
            Assert.False(nextCalled);//handlers not called 
        }

        [Fact]
        public async Task Handle_CacheMiss_CallsNextAndStores()
        {
            //arrange
            var query = new GetCountryByIpQuery(3232235777);
            var freshDto = new CountryDto("GR", "GRC", "Greece");

            // fake cache that dont have it 
            var fakeCache = new Mock<ICache>();
            fakeCache
                .Setup(c => c.TryGet<CountryDto?>(It.IsAny<string>(), out It.Ref<CountryDto?>.IsAny))
                .Returns((string k, out CountryDto? v) => { v = null; return false; });//miss

            var behavior = new CachingBehavior<GetCountryByIpQuery, CountryDto?>(fakeCache.Object);

            // fake next to call it
            var nextCalled = false;
            RequestHandlerDelegate<CountryDto?> next = (ct) =>
            {
                nextCalled = true;
                return Task.FromResult<CountryDto?>(freshDto);
            };

            //act
            var result = await behavior.Handle(query, next, default);

            //assert
            Assert.NotNull(result);
            Assert.Equal("GR", result.TwoLetterCode);//came from handler (next)
            Assert.True(nextCalled);//handler called

            //store in cache
            fakeCache.Verify(
                c => c.Set(It.IsAny<string>(), It.IsAny<CountryDto?>(), It.IsAny<TimeSpan>()),
                Times.Once);
        }
    }
}