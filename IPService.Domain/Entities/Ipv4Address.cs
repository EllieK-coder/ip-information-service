using System;
using IPService.Domain.Enums;

namespace IPService.Domain.Entities
{
    public class Ipv4Address : IIpAddress
    {
        public long AddressId { get; } //BIGINT representation
        public string Address { get; } //192.168.1.1 (from DB)
        public string TwoLetterCode { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public IpVersion Version => IpVersion.IPv4;

        public Ipv4Address(long addressId, string twoLetterCode, DateTime lastUpdated)
        {
            AddressId = addressId;
            Address = $"{(addressId >> 24) & 0xFF}." +
                      $"{(addressId >> 16) & 0xFF}." +
                      $"{(addressId >> 8) & 0xFF}." +
                      $"{addressId & 0xFF}";
            TwoLetterCode = twoLetterCode;
            LastUpdated = lastUpdated;
        }

        public Ipv4Address() { }

        public void SetTwoLetterCode(string twoLetterCode)
        {
            TwoLetterCode = twoLetterCode;
        }

        public void SetLastUpdated(DateTime lastUpdated)
        {
            LastUpdated = lastUpdated;
        }

    }
}