using MediatR;
using IPService.Application.DTOs;

namespace IPService.Application.Queries
{ 
    public record GetCountryByIpQuery(long IpAddress) : IRequest<CountryDto?>;
}