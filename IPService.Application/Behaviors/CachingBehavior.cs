using IPService.Application.Caching;
using MediatR;

namespace IPService.Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheableQuery
    {
        private readonly ICache _cache;

        public CachingBehavior(ICache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var key = request.CacheKey;

            if (_cache.TryGet<TResponse>(key, out var cached))
                return cached!;

            var response = await next();   // ← τρέχει τον handler

            _cache.Set(key, response, TimeSpan.FromSeconds(60));
            return response;
        }
    }
}