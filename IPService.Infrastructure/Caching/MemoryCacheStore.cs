using Microsoft.Extensions.Caching.Memory;
using IPService.Application.Caching;

namespace IPService.Infrastructure.Caching;

public class MemoryCacheStore : ICache
{
    private readonly IMemoryCache _cache;

    public MemoryCacheStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            value = (T?)cached;
            return true;
        }
        value = default;
        return false;
    }

    public void Set<T>(string key, T value, TimeSpan ttl)
    {
        _cache.Set(key, value, ttl);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}