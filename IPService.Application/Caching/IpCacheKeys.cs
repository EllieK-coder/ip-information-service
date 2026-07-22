namespace IPService.Application.Caching
{
    public static class IpCacheKeys
    {
        public static string ForIp(long ipAddress) => $"GetCountryByIp{ipAddress}";


    }
}