using IPService.Domain.Enums;

namespace IPService.Domain.Entities
{
    public interface IIpAddress
    {
        IpVersion Version { get; }
        string DisplayString();
    }
}