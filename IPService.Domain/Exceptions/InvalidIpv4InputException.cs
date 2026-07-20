namespace IPService.Domain.Exceptions;

public class InvalidIpv4InputException : IpServiceException
{
    public string ProvidedInput { get; }

    public InvalidIpv4InputException(string providedInput)
        : base($"The value '{providedInput}' is not a valid IPv4 address.")
    {
        ProvidedInput = providedInput;
    }
}
