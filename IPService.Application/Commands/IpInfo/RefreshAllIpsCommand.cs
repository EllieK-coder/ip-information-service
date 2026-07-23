using MediatR;

namespace IPService.Application.Commands.IpInfo
{
    // carries batch size so Step 6 can pass the configured value; default keeps it testable
    public sealed record RefreshAllIpsCommand(int BatchSize = 100)
        : IRequest<RefreshAllIpsResult>;

    public sealed record RefreshAllIpsResult(int Processed, int Changed, int Failed);
}