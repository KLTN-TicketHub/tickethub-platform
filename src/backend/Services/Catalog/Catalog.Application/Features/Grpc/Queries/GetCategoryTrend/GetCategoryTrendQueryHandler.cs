using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetCategoryTrend
{
    public class GetCategoryTrendQueryHandler : IRequestHandler<GetCategoryTrendQuery, GetCategoryTrendResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryTrendQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCategoryTrendResult> Handle(GetCategoryTrendQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await GetCategoryTrendAsync(request.From, request.To, cancellationToken);
            }
            catch (Exception ex)
            {
                return GetCategoryTrendResult.Fail($"Lỗi hệ thống khi lấy xu hướng theo category: {ex.Message}");
            }
        }

        private async Task<GetCategoryTrendResult> GetCategoryTrendAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
        {
            int periodDays = to.DayNumber - from.DayNumber + 1;
            DateOnly prevTo = from.AddDays(-1);
            DateOnly prevFrom = prevTo.AddDays(-(periodDays - 1));

            List<(Guid CategoryId, string CategoryName, long ViewCount, long PurchaseIntentCount, int ActiveEventCount)> currentRows =
                await _unitOfWork.EventClickStatRepository.GetCategoryTrendAsync(from, to, cancellationToken);

            List<(Guid CategoryId, string CategoryName, long ViewCount, long PurchaseIntentCount, int ActiveEventCount)> prevRows =
                await _unitOfWork.EventClickStatRepository.GetCategoryTrendAsync(prevFrom, prevTo, cancellationToken);

            List<(Guid CategoryId, double AvgOverallRating, int SampleSize)> ratingRows =
                await _unitOfWork.EventRatingRepository.GetCategoryRatingAverageAsync(from, to, cancellationToken);

            Dictionary<Guid, long> prevViewByCategory = prevRows.ToDictionary(r => r.CategoryId, r => r.ViewCount);
            Dictionary<Guid, (double AvgOverallRating, int SampleSize)> ratingByCategory =
                ratingRows.ToDictionary(r => r.CategoryId, r => (r.AvgOverallRating, r.SampleSize));

            List<CategoryTrendItemResult> categories = currentRows.Select(r =>
            {
                long prevView = prevViewByCategory.TryGetValue(r.CategoryId, out long pv) ? pv : 0;
                double growth = prevView > 0 ? Math.Round((r.ViewCount - prevView) * 100.0 / prevView, 1) : 0;
                (double AvgOverallRating, int SampleSize) rating = ratingByCategory.TryGetValue(r.CategoryId, out var rt) ? rt : (0, 0);

                return new CategoryTrendItemResult
                {
                    CategoryId = r.CategoryId,
                    CategoryName = r.CategoryName,
                    ViewCount = r.ViewCount,
                    PurchaseIntentCount = r.PurchaseIntentCount,
                    ActiveEventCount = r.ActiveEventCount,
                    ViewGrowthPercent = growth,
                    AvgOverallRating = rating.AvgOverallRating,
                    RatingSampleSize = rating.SampleSize
                };
            }).ToList();

            return new GetCategoryTrendResult
            {
                IsSuccess = true,
                Message = "Thành công",
                Categories = categories
            };
        }
    }
}
