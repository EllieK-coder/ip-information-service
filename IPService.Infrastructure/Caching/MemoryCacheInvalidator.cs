using Microsoft.Extensions.Caching.Memory;
using IPService.Application.Caching;


namespace IPService.Infrastructure.Caching
{
    public sealed class MemoryCacheInvalidator : ICacheInvalidator
    {
        private readonly IMemoryCache _cache;
        public MemoryCacheInvalidator(IMemoryCache cache) => _cache = cache;

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _cache.Remove(key);
            return Task.CompletedTask;   // IMemoryCache.Remove is synchronous
        }

        public Task RemoveAsync(object value, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}