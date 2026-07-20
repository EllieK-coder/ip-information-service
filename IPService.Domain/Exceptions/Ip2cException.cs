namespace IPService.Domain.Exceptions;

public class Ip2cException : IpServiceException
{
    public Ip2cException(string message) : base(message) { }
    public Ip2cException(string message, Exception inner) : base(message, inner) { }
}