namespace IPService.Application.Caching
{
    public interface ICacheInvalidator
    {
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveAsync(object value, CancellationToken ct);
    }
}
