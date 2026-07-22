namespace IPService.Application.Caching
{
    public static class IpCacheKeys
    {
        public static string ForIp(long ipAddress) => $"GetCountryByIp_{ipAddress}";

        internal static string ForIp(string address)
        {
            throw new NotImplementedException();
        }
    }
}