namespace Identity.Application.Common.Interfaces.IDataSeedingServices
{
    public interface IDataSeedingService
    {
        Task SeedDataAsync(CancellationToken cancellationToken = default);
    }
}
