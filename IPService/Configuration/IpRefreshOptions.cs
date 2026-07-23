namespace IPService.Api.Configuration
{
    public sealed class IpRefreshOptions
    {
        public const string SectionName = "IpRefresh";
        public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
        public int BatchSize { get; set; } = 100;
    }
}