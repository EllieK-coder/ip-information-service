using MediatR;
using IPService.Application.Caching;

namespace IPService.Application.Decorators
{
    public class CachingRequestHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableQuery
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly ICache _cache;

        public CachingRequestHandler(
            IRequestHandler<TRequest, TResponse> inner,
            ICache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var key = request.CacheKey;

            if (_cache.TryGet<TResponse>(key, out var cached))
                return cached!;

            var response = await _inner.Handle(request, cancellationToken);

            _cache.Set(key, response, TimeSpan.FromSeconds(60));

            return response;
        }
    }
}
