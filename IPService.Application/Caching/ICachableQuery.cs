namespace IPService.Application.Caching

{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
    }
}
