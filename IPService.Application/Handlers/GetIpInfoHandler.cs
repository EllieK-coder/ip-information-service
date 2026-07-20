// GetIpInfoHandler.cs
// Project: IPService.Application  →  Handlers/  (or Queries/IpInfo/)
using IPService.Application.CQRS.Queries;
using IPService.Application.DTOs;
using IPService.Application.HttpClients;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using MediatR;

namespace IPService.Application.Handlers
{
    // Orchestration only. Depends on three interfaces (promises), never concretes:
    //   db lookup → gateway on miss → persist → return.
    // Caching is added later (Step 6) as a decorator around this handler; this stays cache-ignorant.
    public class GetIpInfoHandler : IRequestHandler<GetIpInfoQuery, CountryDto?>
    {
        private readonly IGetIpAddress _getIp;
        private readonly ISaveIpAddress _saveIp;
        private readonly IIp2cHttpClient _ip2c;

        public GetIpInfoHandler(
            IGetIpAddress getIp,
            ISaveIpAddress saveIp,
            IIp2cHttpClient ip2c)
        {
            _getIp = getIp;
            _saveIp = saveIp;
            _ip2c = ip2c;
        }

        public async Task<CountryDto?> Handle(GetIpInfoQuery request, CancellationToken cancellationToken)
        {
            // 1. Database first — cheap local lookup.
            var (ip, country) = await _getIp.GetByAddressIdAsync(request.IpAddress);
            if (country is not null)
            {
                // Populated value lives in Country.Name (not the dead CountryName field).
                return new CountryDto(country.TwoLetterCode, country.ThreeLetterCode, country.Name);
            }

            // 2. Miss → external gateway. null = ip2c status 2 (unknown IP).
            var fromGateway = await _ip2c.GetCountryForIpv4Async(request.IpAddress, cancellationToken);
            if (fromGateway is null)
                return null;

            // 3. Persist so the next lookup hits the DB instead of the network,
            //    then return. Ipv4Address derives its dotted Address from the long.
            var entity = new Ipv4Address(request.IpAddress, fromGateway.TwoLetterCode, DateTime.UtcNow);
            await _saveIp.SaveAsync(entity);

            return fromGateway;
        }
    }
}