using BuildingBlocks.Contracts.Models.Pagination;
using Finance.Common.Dtos.Wallets;

namespace Finance.Infrastructure.Interfaces.IServices
{
    public interface IWalletService
    {
        Task<WalletDto> GetWalletAsync(Guid organizerId, CancellationToken cancellationToken = default);

        Task<PaginatedResult<WalletTransactionDto>> GetWalletTransactionsAsync(
            Guid organizerId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
