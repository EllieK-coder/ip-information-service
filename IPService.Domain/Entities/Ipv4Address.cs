using System;
using IPService.Domain.Enums;

namespace IPService.Domain.Entities
{
    public class Ipv4Address : IIpAddress
    {
        public long AddressId { get; private set; }
        public string? TwoLetterCode { get; set; }
        public DateTime LastUpdated { get; set; }
        public IpVersion Version { get; set; }

        public string DisplayString()
        {
            return $"{(AddressId >> 24) & 0xFF}." +
                   $"{(AddressId >> 16) & 0xFF}." +
                   $"{(AddressId >> 8) & 0xFF}." +
                   $"{AddressId & 0xFF}";
        }
    }
}