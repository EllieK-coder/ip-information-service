namespace IPService.Domain.Exceptions;

public class InvalidCountryCodeException : IpServiceException
{
    public string ProvidedCode { get; }

    public InvalidCountryCodeException(string providedCode)
        : base($"The value '{providedCode}' is not a valid two-letter country code.")
    {
        ProvidedCode = providedCode;
    }
}
