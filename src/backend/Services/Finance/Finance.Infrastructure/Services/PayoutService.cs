using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Domain.Exceptions;
using Finance.Common.Dtos.Payouts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IRepositories;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PayoutService> _logger;

        public PayoutService(IUnitOfWork unitOfWork, IEventPublisher eventPublisher, ILogger<PayoutService> logger)
        {
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task RequestPayoutAsync(
            Guid eventId,
            Guid organizerId,
            CancellationToken cancellationToken = default)
        {
            bool hasUnreleasedRevenue = await _unitOfWork.WalletTransactionRepository.HasUnreleasedRevenueAsync(eventId, cancellationToken);

            if (hasUnreleasedRevenue)
                throw new BusinessRuleException("Sự kiện chưa kết thúc hoặc vẫn còn trong thời gian chờ đối soát. Chỉ có thể yêu cầu giải ngân sau khi toàn bộ sự kiện đã kết thúc.");

            PendingPayoutSummary? summary = await _unitOfWork.WalletTransactionRepository.GetEventPendingSummaryAsync(eventId, cancellationToken);

            if (summary == null)
                throw new BusinessRuleException("Không có doanh thu nào sẵn sàng để yêu cầu giải ngân cho sự kiện này.");

            if (summary.OrganizerId != organizerId)
                throw new ForbiddenAccessException("Bạn không có quyền yêu cầu giải ngân cho sự kiện này.");

            PayoutRequest? existingRequest = await _unitOfWork.PayoutRequestRepository.GetOneUntrackedAsync<PayoutRequest>(
                filter: pr => pr.EventId == eventId && pr.Status == PayoutRequestStatus.Pending,
                cancellation: cancellationToken);

            if (existingRequest != null)
                throw new BusinessRuleException("Đã có yêu cầu giải ngân đang chờ xử lý cho sự kiện này.");

            PayoutRequest request = new PayoutRequest(eventId, summary.EventTitle, summary.CategoryId, organizerId, summary.WalletId);
            await _unitOfWork.PayoutRequestRepository.CreateAsync(request, cancellationToken);

            await _eventPublisher.PublishAsync(new NotificationRequestedEvent
            {
                TargetRole = Roles.Moderator,
                Type = "PayoutRequested",
                Title = "Có yêu cầu giải ngân mới",
                Message = $"Sự kiện \"{summary.EventTitle}\" vừa gửi yêu cầu giải ngân doanh thu.",
                LinkUrl = "/moderator/payouts",
                ReferenceId = request.Id
            }, cancellationToken: cancellationToken);
        }

        public async Task<PaginatedResult<PayoutRequestDto>> GetPayoutRequestsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            (IEnumerable<PayoutRequestSummary> items, int totalCount) = await _unitOfWork.PayoutRequestRepository
                .GetPendingRequestsWithSummaryAsync(pageNumber, pageSize, cancellationToken);

            List<PayoutRequestDto> data = items.Select(s => new PayoutRequestDto
            {
                PayoutRequestId = s.PayoutRequestId,
                EventId = s.EventId,
                EventTitle = s.EventTitle,
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName,
                OrganizerId = s.OrganizerId,
                OrganizerName = s.OrganizerName,
                GrossAmount = s.GrossAmount,
                RecommendedRate = s.RecommendedRate,
                OrderCount = s.OrderCount,
                RequestedAt = s.RequestedAt,
                IsResubmitted = s.IsResubmitted,
                LastRejectionReason = s.LastRejectionReason,
                LastRejectedAt = s.LastRejectedAt
            }).ToList();

            return new PaginatedResult<PayoutRequestDto>(data, totalCount, pageNumber, pageSize);
        }

        public async Task<EventPayoutResultDto> ProposePayoutAsync(
            Guid payoutRequestId,
            decimal appliedRate,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellationToken = default)
        {
            if (appliedRate < 0 || appliedRate > 100)
                throw new ValidatorException("appliedRate", "Phần trăm hoa hồng áp dụng phải nằm trong khoảng từ 0 đến 100.");

            PayoutRequest payoutRequest = await _unitOfWork.PayoutRequestRepository.GetOneAsync<PayoutRequest>(
                filter: pr => pr.Id == payoutRequestId, cancellation: cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy yêu cầu giải ngân với ID {payoutRequestId}.");

            if (payoutRequest.Status != PayoutRequestStatus.Pending)
                throw new BusinessRuleException("Yêu cầu giải ngân này đã được xử lý.");

            List<WalletTransaction> pendingTransactions = (await _unitOfWork.WalletTransactionRepository.GetAllAsync<WalletTransaction>(
                filter: t => t.EventId == payoutRequest.EventId
                          && t.Status == WalletTransactionStatus.Pending
                          && t.Type == WalletTransactionType.Revenue
                          && t.EventPayoutId == null
                          && t.ReleaseAt <= DateTime.UtcNow,
                cancellation: cancellationToken)).ToList();

            if (!pendingTransactions.Any())
                throw new BusinessRuleException($"Không tìm thấy giao dịch doanh thu nào đang chờ giải ngân cho sự kiện {payoutRequest.EventId}.");

            decimal grossAmount = pendingTransactions.Sum(t => t.Amount);

            Wallet wallet = await _unitOfWork.WalletRepository.GetByIdAsync(payoutRequest.WalletId, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy ví với ID {payoutRequest.WalletId}.");

            CommissionSetting? setting = await _unitOfWork.CommissionSettingRepository.GetOneUntrackedAsync<CommissionSetting>(
                filter: cs => cs.CategoryId == payoutRequest.CategoryId,
                cancellation: cancellationToken);

            EventPayout payout = new EventPayout(
                eventId: payoutRequest.EventId,
                eventTitle: payoutRequest.EventTitle,
                categoryId: payoutRequest.CategoryId,
                organizerId: payoutRequest.OrganizerId,
                walletId: wallet.Id,
                grossAmount: grossAmount,
                recommendedRate: setting?.Rate ?? 0,
                appliedRate: appliedRate,
                reviewedByUserId: reviewerUserId,
                reviewedByName: reviewerName);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _unitOfWork.EventPayoutRepository.AddEntity(payout);

                foreach (WalletTransaction tx in pendingTransactions)
                {
                    tx.AttachToPayout(payout.Id);
                    _unitOfWork.WalletTransactionRepository.UpdateEntity(tx);
                }

                payoutRequest.MarkAsProcessed(payout.Id);
                _unitOfWork.PayoutRequestRepository.UpdateEntity(payoutRequest);

                OrganizerSnapshot? organizer = await _unitOfWork.OrganizerSnapshotRepository.GetByIdAsync(payoutRequest.OrganizerId, cancellationToken);

                if (organizer != null)
                {
                    await _eventPublisher.PublishAsync(new PayoutProposedEmailEvent
                    {
                        PayoutId = payout.Id,
                        EventId = payout.EventId,
                        EventTitle = payout.EventTitle,
                        OrganizerEmail = organizer.Email,
                        OrganizerName = organizer.OrganizerName,
                        GrossAmount = payout.GrossAmount,
                        AppliedRate = payout.AppliedRate,
                        FeeAmount = payout.FeeAmount,
                        NetAmount = payout.NetAmount
                    }, cancellationToken: cancellationToken);
                }
                else
                {
                    _logger.LogWarning("OrganizerSnapshot not found for OrganizerId {OrganizerId}, skipping payout notification email.", payoutRequest.OrganizerId);
                }

                await _eventPublisher.PublishAsync(new NotificationRequestedEvent
                {
                    RecipientUserId = payout.OrganizerId,
                    Type = "PayoutProposed",
                    Title = "Có đề xuất giải ngân",
                    Message = $"Sự kiện \"{payout.EventTitle}\" có đề xuất giải ngân {payout.NetAmount:N0} VNĐ đang chờ bạn xác nhận.",
                    LinkUrl = "/organizer/wallet",
                    ReferenceId = payout.Id
                }, cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while proposing payout for event {EventId}", payoutRequest.EventId);
                throw;
            }

            return MapToResultDto(payout);
        }

        public async Task<PaginatedResult<ProposedPayoutDto>> GetProposedPayoutsAsync(
            Guid organizerId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            (IEnumerable<ProposedPayoutDto> items, int totalCount) = await _unitOfWork.EventPayoutRepository.GetPagedAsync(
                selector: p => new ProposedPayoutDto
                {
                    Id = p.Id,
                    EventId = p.EventId,
                    EventTitle = p.EventTitle,
                    GrossAmount = p.GrossAmount,
                    AppliedRate = p.AppliedRate,
                    FeeAmount = p.FeeAmount,
                    NetAmount = p.NetAmount,
                    ReviewedByName = p.ReviewedByName,
                    ReviewedAt = p.ReviewedAt
                },
                filter: p => p.OrganizerId == organizerId && p.Status == EventPayoutStatus.Proposed,
                orderBy: q => q.OrderByDescending(p => p.ReviewedAt),
                pageNumber: pageNumber,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            return new PaginatedResult<ProposedPayoutDto>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<EventPayoutResultDto> AcceptPayoutAsync(
            Guid payoutId,
            Guid organizerId,
            CancellationToken cancellationToken = default)
        {
            EventPayout payout = await _unitOfWork.EventPayoutRepository.GetOneAsync<EventPayout>(
                filter: p => p.Id == payoutId, cancellation: cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy đề xuất giải ngân với ID {payoutId}.");

            if (payout.OrganizerId != organizerId)
                throw new ForbiddenAccessException("Bạn không có quyền chấp nhận đề xuất giải ngân này.");

            if (payout.Status != EventPayoutStatus.Proposed)
                throw new BusinessRuleException("Đề xuất giải ngân này đã được xử lý.");

            List<WalletTransaction> attachedTransactions = (await _unitOfWork.WalletTransactionRepository.GetAllAsync<WalletTransaction>(
                filter: t => t.EventPayoutId == payout.Id && t.Status == WalletTransactionStatus.Pending,
                cancellation: cancellationToken)).ToList();

            Wallet wallet = await _unitOfWork.WalletRepository.GetByIdAsync(payout.WalletId, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy ví với ID {payout.WalletId}.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (WalletTransaction tx in attachedTransactions)
                {
                    tx.MarkAsSuccess();
                    _unitOfWork.WalletTransactionRepository.UpdateEntity(tx);
                }

                if (payout.FeeAmount > 0)
                {
                    WalletTransaction feeTransaction = new WalletTransaction(
                        walletId: wallet.Id,
                        orderId: null,
                        eventId: payout.EventId,
                        eventTitle: payout.EventTitle,
                        categoryId: payout.CategoryId,
                        amount: payout.FeeAmount,
                        type: WalletTransactionType.Fee,
                        description: $"Phí hoa hồng sàn ({payout.AppliedRate}%) cho sự kiện {payout.EventTitle}",
                        releaseAt: DateTime.UtcNow);
                    feeTransaction.AssignPayout(payout.Id);

                    _unitOfWork.WalletTransactionRepository.AddEntity(feeTransaction);
                }

                wallet.Credit(payout.NetAmount);
                _unitOfWork.WalletRepository.UpdateEntity(wallet);

                payout.Accept();
                _unitOfWork.EventPayoutRepository.UpdateEntity(payout);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while approving payout {PayoutId}", payoutId);
                throw;
            }

            return MapToResultDto(payout);
        }

        public async Task RejectPayoutAsync(
            Guid payoutId,
            Guid organizerId,
            string? reason,
            CancellationToken cancellationToken = default)
        {
            EventPayout payout = await _unitOfWork.EventPayoutRepository.GetOneAsync<EventPayout>(
                filter: p => p.Id == payoutId, cancellation: cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy đề xuất giải ngân với ID {payoutId}.");

            if (payout.OrganizerId != organizerId)
                throw new ForbiddenAccessException("Bạn không có quyền từ chối đề xuất giải ngân này.");

            if (payout.Status != EventPayoutStatus.Proposed)
                throw new BusinessRuleException("Đề xuất giải ngân này đã được xử lý.");

            List<WalletTransaction> attachedTransactions = (await _unitOfWork.WalletTransactionRepository.GetAllAsync<WalletTransaction>(
                filter: t => t.EventPayoutId == payout.Id && t.Status == WalletTransactionStatus.Pending,
                cancellation: cancellationToken)).ToList();

            PayoutRequest? payoutRequest = await _unitOfWork.PayoutRequestRepository.GetOneAsync<PayoutRequest>(
                filter: pr => pr.EventPayoutId == payout.Id, cancellation: cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (WalletTransaction tx in attachedTransactions)
                {
                    tx.DetachFromPayout();
                    _unitOfWork.WalletTransactionRepository.UpdateEntity(tx);
                }

                payout.Reject(reason);
                _unitOfWork.EventPayoutRepository.UpdateEntity(payout);

                if (payoutRequest != null)
                {
                    payoutRequest.ResetToPending();
                    _unitOfWork.PayoutRequestRepository.UpdateEntity(payoutRequest);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while rejecting payout {PayoutId}", payoutId);
                throw;
            }

        }

        public async Task<EventPayoutStatusDto> GetEventPayoutStatusAsync(
            Guid eventId,
            Guid organizerId,
            CancellationToken cancellationToken = default)
        {
            EventPayout? acceptedPayout = await _unitOfWork.EventPayoutRepository.GetOneUntrackedAsync<EventPayout>(
                filter: p => p.EventId == eventId && p.OrganizerId == organizerId && p.Status == EventPayoutStatus.Accepted,
                cancellation: cancellationToken);

            if (acceptedPayout != null)
                return new EventPayoutStatusDto { HasAcceptedPayout = true };

            EventPayout? proposedPayout = await _unitOfWork.EventPayoutRepository.GetOneUntrackedAsync<EventPayout>(
                filter: p => p.EventId == eventId && p.OrganizerId == organizerId && p.Status == EventPayoutStatus.Proposed,
                cancellation: cancellationToken);

            if (proposedPayout != null)
                return new EventPayoutStatusDto { HasProposedPayout = true, ProposedPayoutId = proposedPayout.Id };

            PayoutRequest? pendingRequest = await _unitOfWork.PayoutRequestRepository.GetOneUntrackedAsync<PayoutRequest>(
                filter: pr => pr.EventId == eventId && pr.OrganizerId == organizerId && pr.Status == PayoutRequestStatus.Pending,
                cancellation: cancellationToken);

            return new EventPayoutStatusDto { HasPendingRequest = pendingRequest != null };
        }

        private static EventPayoutResultDto MapToResultDto(EventPayout payout)
        {
            return new EventPayoutResultDto
            {
                Id = payout.Id,
                EventId = payout.EventId,
                EventTitle = payout.EventTitle,
                OrganizerId = payout.OrganizerId,
                GrossAmount = payout.GrossAmount,
                AppliedRate = payout.AppliedRate,
                FeeAmount = payout.FeeAmount,
                NetAmount = payout.NetAmount,
                Status = payout.Status.ToString(),
                ReviewedAt = payout.ReviewedAt,
                AcceptedAt = payout.AcceptedAt
            };
        }
    }
}
