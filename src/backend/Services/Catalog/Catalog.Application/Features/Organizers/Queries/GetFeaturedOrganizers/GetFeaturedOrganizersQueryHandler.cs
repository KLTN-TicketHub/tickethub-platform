using BuildingBlocks.Application.Interfaces;
using Catalog.Application.Common.DTOs.Profiles;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Organizers.Queries.GetFeaturedOrganizers
{
    public class GetFeaturedOrganizersQueryHandler : IRequestHandler<GetFeaturedOrganizersQuery, List<FeaturedOrganizerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetFeaturedOrganizersQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<List<FeaturedOrganizerDto>> Handle(GetFeaturedOrganizersQuery query, CancellationToken cancellationToken)
        {
            return await GetFeaturedOrganizersAsync(query.Count, cancellationToken);
        }

        private async Task<List<FeaturedOrganizerDto>> GetFeaturedOrganizersAsync(int count, CancellationToken cancellation = default)
        {
            var organizers = await _unitOfWork.OrganizerSnapshotRepository.GetFeaturedOrganizersAsync(count, cancellation);

            return organizers
                .Select(x => new FeaturedOrganizerDto
                {
                    Id = x.Id,
                    OrganizerName = x.OrganizerName,
                    ImageUrl = x.ImageUrl != null ? _fileService.GetAbsoluteUrl(x.ImageUrl) : null,
                    PublishedEventCount = x.PublishedEventCount
                })
                .ToList();
        }
    }
}
