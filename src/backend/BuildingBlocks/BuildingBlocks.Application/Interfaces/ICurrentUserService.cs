namespace BuildingBlocks.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }

        string? UserName { get; }

        bool IsAuthenticated { get; }

        IEnumerable<string> Roles { get; }

        string? IpAddress { get; }

        string? DeviceInfo { get; }
    }
}
