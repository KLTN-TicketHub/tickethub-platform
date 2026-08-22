using BuildingBlocks.Application.Interfaces;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetTrendingEvents
{
    public class GetTrendingEventsQueryHandler : IRequestHandler<GetTrendingEventsQuery, List<EventListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetTrendingEventsQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<List<EventListItemDto>> Handle(GetTrendingEventsQuery query, CancellationToken cancellationToken)
        {
            return await GetTrendingEventsAsync(query.Count, cancellationToken);
        }

        private async Task<List<EventListItemDto>> GetTrendingEventsAsync(int count, CancellationToken cancellation = default)
        {
            var trending = await _unitOfWork.EventRepository.GetTrendingEventsAsync(count, cancellation);

            return trending
                .Select(x => new EventListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt,
                    CoverImageUrl = _fileService.GetAbsoluteUrl(x.CoverImageUrl),
                    CategoryName = x.CategoryName,
                    MinPrice = x.MinPrice,
                    ProvinceCity = x.ProvinceCity
                })
                .ToList();
        }
    }
}
