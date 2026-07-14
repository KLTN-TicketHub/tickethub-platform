using BuildingBlocks.Contracts.Models.Pagination;
using Finance.Common.Dtos.Wallets;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IServices;

namespace Finance.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WalletDto> GetWalletAsync(Guid organizerId, CancellationToken cancellationToken = default)
        {
            WalletDto? wallet = await _unitOfWork.WalletRepository.GetOneUntrackedAsync<WalletDto>(
                filter: w => w.OrganizerId == organizerId,
                selector: w => new WalletDto
                {
                    Id = w.Id,
                    OrganizerId = w.OrganizerId,
                    Balance = w.Balance
                },
                cancellation: cancellationToken);

            return wallet ?? new WalletDto { OrganizerId = organizerId, Balance = 0 };
        }

        public async Task<PaginatedResult<WalletTransactionDto>> GetWalletTransactionsAsync(
            Guid organizerId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            (IEnumerable<WalletTransactionDto> items, int totalCount) = await _unitOfWork.WalletTransactionRepository.GetPagedAsync(
                selector: t => new WalletTransactionDto
                {
                    Id = t.Id,
                    OrderId = t.OrderId,
                    EventId = t.EventId,
                    EventTitle = t.EventTitle,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                },
                filter: t => t.Wallet.OrganizerId == organizerId,
                orderBy: q => q.OrderByDescending(t => t.CreatedAt),
                pageNumber: pageNumber,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            return new PaginatedResult<WalletTransactionDto>(items, totalCount, pageNumber, pageSize);
        }
    }
}
