namespace Finance.Infrastructure.Interfaces.IServices
{
    public interface IReleaseFundsJobService
    {
        Task ProcessReleaseFundsAsync(CancellationToken cancellationToken);
    }
}
