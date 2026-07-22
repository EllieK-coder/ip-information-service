using IPService.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace IPService.Tests
{
    public class MemoryCacheStoreTests
    {
        
        private static MemoryCacheStore CreateCache()//helper: in-memory cache small, no need to mock
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            return new MemoryCacheStore(memoryCache);
        }

        [Fact]
        public void SetThenTryGet_ReturnsValue()//save then read gives back the value
        {
            var cache = CreateCache();

            cache.Set("key1", "hello", TimeSpan.FromMinutes(1));
            var found = cache.TryGet<string>("key1", out var value);

            Assert.True(found);
            Assert.Equal("hello", value);
        }

        [Fact]
        public void TryGet_MissingKey_ReturnsFalse()//key that doesnt exist gives false
        {
            var cache = CreateCache();

            var found = cache.TryGet<string>("doesnotexist", out var value);

            Assert.False(found);
            Assert.Null(value);
        }
    }
}