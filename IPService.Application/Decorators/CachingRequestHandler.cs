using IPService.Application.Caching;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IPService.Application.Decorators;

public class CachingRequestHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableQuery
{
    private readonly IRequestHandler<TRequest, TResponse> _inner;
    private readonly ICache _cache;
    private readonly ILogger<CachingRequestHandler<TRequest, TResponse>> _logger;

    public CachingRequestHandler(
        IRequestHandler<TRequest, TResponse> inner,
        ICache cache,
        ILogger<CachingRequestHandler<TRequest, TResponse>> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var key = request.CacheKey;

        if (_cache.TryGet<TResponse>(key, out var cached))
        {
            _logger.LogInformation("Cache HIT for {Key}", key);
            return cached!;
        }

        _logger.LogInformation("Cache MISS for {Key}", key);

        var response = await _inner.Handle(request, cancellationToken);

        _cache.Set(key, response, TimeSpan.FromSeconds(60));
        return response;
    }
}