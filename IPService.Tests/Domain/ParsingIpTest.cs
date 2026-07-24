using IPService.Domain.Entities;
using IPService.Domain.Exceptions;

namespace IPService.Tests.Domain
{
    public class ParsingIpTest
    {
        
        [Theory]
        [InlineData("0.0.0.0", 0)]
        [InlineData("192.168.1.1", 3232235777)]
        [InlineData("255.255.255.255", 4294967295)]
        [InlineData("8.8.8.8", 134744072)]
        public void ParseToLong_ValidIp_ReturnsCorrectNumber(string ip, long expected)//valid
        {
            long result = Ipv4Address.ParseToLong(ip);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("999.999.999.999")]
        [InlineData("192.168.1")]
        [InlineData("abc")]
        [InlineData("192.168.1.1.5")]
        public void ParseToLong_InvalidIp_ThrowsException(string ip)//invalid
        {
            Assert.Throws<InvalidIpv4InputException>(() => Ipv4Address.ParseToLong(ip));
        }
    }
}