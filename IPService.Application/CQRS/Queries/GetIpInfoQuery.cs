// GetIpInfoQuery.cs
// Project: IPService.Application  →  Queries/IpInfo/
using MediatR;
using IPService.Application.DTOs;

namespace IPService.Application.CQRS.Queries
{
    // The query is just a message: the IP as a packed 32-bit value (long),
    // matching IGetIpAddress.GetByAddressIdAsync(long) and the ip2c gateway.
    // Returns CountryDto? — null means the IP is unknown.
    public record GetIpInfoQuery(long IpAddress) : IRequest<CountryDto?>;
}