/// Application/Commands/IpInfo/UpdateIpInfoHandler.cs
using MediatR;
using IPService.Application.Interfaces;
using IPService.Application.HttpClients;
using IPService.Application.Caching;
using IPService.Application.Infrastructure;

namespace IPService.Application.Commands.IpInfo
{
    public sealed class UpdateIpInfoHandler : IRequestHandler<UpdateIpInfoCommand, bool>
    {
        private readonly IIp2cHttpClient _ip2c;
        private readonly IUpdateIpInfoPersistence _update;
        private readonly IEnsureCountry _ensureCountry;
        private readonly ICache _cache;

        public UpdateIpInfoHandler(
            IIp2cHttpClient ip2c,
            IUpdateIpInfoPersistence update,
            IEnsureCountry ensureCountry,
            ICache cache)
        {
            _ip2c = ip2c;
            _update = update;
            _ensureCountry = ensureCountry;
            _cache = cache;
        }

        public async Task<bool> Handle(UpdateIpInfoCommand request, CancellationToken ct)
        {
            var entity = request.Ip;

            var latest = await _ip2c.GetCountryForIpv4Async(entity.AddressId, ct);
            if (latest is null) return false;

            var changed = latest.TwoLetterCode != entity.TwoLetterCode;
            if (!changed) return false;

            // ensure the country row exists in DB
            await _ensureCountry.EnsureExistsAsync(latest, ct);

            entity.SetTwoLetterCode(latest.TwoLetterCode);
            entity.SetLastUpdated(DateTime.UtcNow);

            await _update.UpdateAsync(entity, ct);

            // information changed → evict the stale read-cache entry
            _cache.Remove(IpCacheKeys.ForIp(entity.AddressId));
            return true;
        }
    }
}