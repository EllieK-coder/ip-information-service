using IPService.Application.DTOs;
using IPService.Application.HttpClients;
using IPService.Domain.Exceptions;

namespace IPService.Gateway
{
    public class Ip2cHttpClient : IIp2cHttpClient
    {
        private readonly HttpClient _httpClient;

        public Ip2cHttpClient(HttpClient httpClient)   // ← το factory το περνάει
        {
            _httpClient = httpClient;
        }

        public async Task<CountryDto?> GetCountryForIpv4Async(long addressId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"?dec={addressId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var parts = body.Split(';');
            var status = parts[0];

            switch (status)
            {
                case "1":  // success
                    if (parts.Length < 4)
                        throw new Ip2cException($"IP2C success response was malformed: '{body}'");

                    return new CountryDto(
                        TwoLetterCode: parts[1],
                        ThreeLetterCode: parts[2],
                        CountryName: parts[3]
                        );
            

                case "2":  // unknown IP — expected outcome
                    return null;   

                case "0":  // invalid input — shouldn't happen if validated upstream
                    throw new Ip2cException($"IP2C reported invalid input for dec={addressId}. This should have been caught by upstream validation.");

                default:
                    throw new Ip2cException($"Unexpected IP2C status: {status}");
            }


        }

        
    }
}
