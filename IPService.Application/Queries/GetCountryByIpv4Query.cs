using MediatR;
using IPService.Application.DTOs;
using IPService.Application.Caching;

namespace IPService.Application.Queries
{
    public record GetCountryByIpv4Query(long IpAddress) : IRequest<CountryDto>, ICacheableQuery
    {
        public string CacheKey => IpCacheKeys.ForIp(IpAddress);
    }
}