using Catalog.Application.Common.DTOs.Reports;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetAdminEventsByCategory
{
    public class GetAdminEventsByCategoryQueryHandler : IRequestHandler<GetAdminEventsByCategoryQuery, List<AdminEventByCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminEventsByCategoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminEventByCategoryDto>> Handle(GetAdminEventsByCategoryQuery query, CancellationToken cancellation = default)
        {
            return await GetByCategoryAsync(query.From, query.To, cancellation);
        }

        private async Task<List<AdminEventByCategoryDto>> GetByCategoryAsync(DateTime from, DateTime to, CancellationToken cancellation)
        {
            List<(Guid CategoryId, string CategoryName, int EventCount)> counts =
                await _unitOfWork.EventRepository.GetCountByCategoryAsync(from, to, cancellation);

            return counts
                .Select(c => new AdminEventByCategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    EventCount = c.EventCount
                })
                .ToList();
        }
    }
}
