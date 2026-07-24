using IPService.Application.DTOs;
using IPService.Application.HttpClients;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using MediatR;

namespace IPService.Application.Queries
{
    public class GetCountryByIpv4Handler : IRequestHandler<GetCountryByIpv4Query, CountryDto?>
    {
        private readonly IGetIpAddress _getIp;
        private readonly ISaveIpAddress _saveIp;
        private readonly IIp2cHttpClient _ip2c;
        private readonly IEnsureCountry _ensureCountry; 

        public GetCountryByIpv4Handler(
            IGetIpAddress getIp,
            ISaveIpAddress saveIp,
            IIp2cHttpClient ip2c,
            IEnsureCountry ensureCountry)
        {
            _getIp = getIp;
            _saveIp = saveIp;
            _ip2c = ip2c;
            _ensureCountry = ensureCountry;
        }

        public async Task<CountryDto> Handle(GetCountryByIpv4Query request, CancellationToken cancellationToken)
        {
            // Our system knows this this ip's country
            var (ip, country) = await _getIp.GetByAddressIdAsync(request.IpAddress);
            if (country is not null)
            {
       
                return new CountryDto(country.TwoLetterCode, country.ThreeLetterCode, country.Name);
            }

            // our system doesnt know thsi ip's counytry -> calls ip2c
            var fromGateway = await _ip2c.GetCountryForIpv4Async(request.IpAddress, cancellationToken);
            if (fromGateway is null)
                fromGateway = new CountryDto("XX", "XXX", "Unknown");


            await _ensureCountry.EnsureExistsAsync(fromGateway, cancellationToken);

        
            var entity = new Ipv4Address(request.IpAddress, fromGateway.TwoLetterCode, DateTime.UtcNow);
            await _saveIp.SaveAsync(entity);

            return fromGateway;
        }
    }
}