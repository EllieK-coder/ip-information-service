using MediatR;
using IPService.Domain.Entities;

namespace IPService.Application.Commands.IpInfo
{
    public sealed record UpdateIpInfoCommand(Ipv4Address Ip) : IRequest<bool>;
}