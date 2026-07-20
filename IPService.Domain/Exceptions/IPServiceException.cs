namespace IPService.Domain.Exceptions;

public class IpServiceException : Exception
{
    public IpServiceException(string message) : base(message) { }
    public IpServiceException(string message, Exception inner) : base(message, inner) { }
}