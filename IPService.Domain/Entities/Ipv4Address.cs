using IPService.Domain.Enums;
using IPService.Domain.Exceptions;
using System;

namespace IPService.Domain.Entities
{
    public class Ipv4Address : IIpAddress
    {
        public long AddressId { get; } //BIGINT representation
        public string Address { get; } = string.Empty; //192.168.1.1 (from DB)
        public string TwoLetterCode { get; private set; } = string.Empty;
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

        public static long ParseToLong(string ipString)
        {
            var parts = ipString.Split('.');//took the ip without . ex 19216811

            if (parts.Length != 4 )//we need 4 parts for a valid ip
            {
                throw new InvalidIpv4InputException(ipString);
            }

            long result = 0;//start with a zero

            foreach (var part in parts)
            {
                if (!byte.TryParse(part, out byte octet))//parse from string to byte
                    throw new InvalidIpv4InputException(ipString);

                result = result * 256 + octet;

            }
            return result;
        }

    }
}