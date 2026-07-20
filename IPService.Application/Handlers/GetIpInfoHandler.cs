// GetIpInfoHandler.cs
using IPService.Application.CQRS.Queries;
using IPService.Application.DTOs;
using IPService.Application.HttpClients;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using MediatR;

namespace IPService.Application.Handlers
{
    // Orchestration: db lookup → gateway on miss → ensure country exists → persist IP → return.
    // Under option (b), the handler guarantees the Country FK target exists before saving the IP,
    // since the gateway hands us all three country fields.
    public class GetIpInfoHandler : IRequestHandler<GetIpInfoQuery, CountryDto?>
    {
        private readonly IGetIpAddress _getIp;
        private readonly ISaveIpAddress _saveIp;
        private readonly IIp2cHttpClient _ip2c;
        private readonly IEnsureCountry _ensureCountry;   // NEW — get-or-create Country

        public GetIpInfoHandler(
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

        public async Task<CountryDto?> Handle(GetIpInfoQuery request, CancellationToken cancellationToken)
        {
            // 1. Database first.
            var (ip, country) = await _getIp.GetByAddressIdAsync(request.IpAddress);
            if (country is not null)
                return new CountryDto(country.TwoLetterCode, country.ThreeLetterCode, country.Name);

            // 2. Miss → external gateway. null = ip2c status 2 (unknown IP).
            var fromGateway = await _ip2c.GetCountryForIpv4Async(request.IpAddress, cancellationToken);
            if (fromGateway is null)
                return null;

            // 3. (b) Ensure the Country row exists before saving the IP, so the FK is valid.
            //    Idempotent: inserts if absent, otherwise leaves the existing row untouched.
            await _ensureCountry.EnsureExistsAsync(fromGateway, cancellationToken);

            // 4. Persist the IP (FK now guaranteed), then return.
            var entity = new Ipv4Address(request.IpAddress, fromGateway.TwoLetterCode, DateTime.UtcNow);
            await _saveIp.SaveAsync(entity);

            return fromGateway;
        }
    }
}