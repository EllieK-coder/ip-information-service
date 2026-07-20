using IPService.Application.DTOs;

namespace IPService.Application.HttpClients
{
    public interface IIp2cHttpClient
    {
        Task<CountryDto?> GetCountryForIpv4Async(long addressIp, CancellationToken cancellationToken = default);
    }
}
