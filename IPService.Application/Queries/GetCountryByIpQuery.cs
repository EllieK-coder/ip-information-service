using MediatR;
using IPService.Application.DTOs;
using IPService.Application.Caching;

namespace IPService.Application.Queries
{
    public record GetCountryByIpQuery(long IpAddress) : IRequest<CountryDto?>, ICacheableQuery
    {
        public string CacheKey => $"GetCountryByIp_{IpAddress}";
    }
}