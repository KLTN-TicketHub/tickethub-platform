using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetUserEventClicks
{
    public class GetUserEventClicksQueryHandler : IRequestHandler<GetUserEventClicksQuery, GetUserEventClicksResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserEventClicksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetUserEventClicksResult> Handle(GetUserEventClicksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await GetUserEventClicksAsync(request.From, request.To, cancellationToken);
            }
            catch (Exception ex)
            {
                return GetUserEventClicksResult.Fail($"Lỗi hệ thống khi lấy dữ liệu click: {ex.Message}");
            }
        }

        private async Task<GetUserEventClicksResult> GetUserEventClicksAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
        {
            DateTime fromDateTime = from.ToDateTime(TimeOnly.MinValue);
            DateTime toDateTime = to.ToDateTime(TimeOnly.MaxValue);

            IEnumerable<UserEventClickItemResult> clicks = await _unitOfWork.UserEventClickRepository.GetAllAsync<UserEventClickItemResult>(
                filter: c => c.ClickedAt >= fromDateTime && c.ClickedAt <= toDateTime,
                selector: c => new UserEventClickItemResult
                {
                    UserId = c.UserId,
                    EventId = c.EventId,
                    ClickType = c.ClickType.ToString(),
                    ClickedAt = c.ClickedAt
                },
                cancellation: cancellationToken);

            return new GetUserEventClicksResult
            {
                IsSuccess = true,
                Message = "Thành công",
                Clicks = clicks.ToList()
            };
        }
    }
}
