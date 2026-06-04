using AutoMapper;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.UpdateEventCategory
{
    public class UpdateEventCategoryCommandHandler : IRequestHandler<UpdateEventCategoryCommand, EventCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEventCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EventCategoryDto> Handle(UpdateEventCategoryCommand command, CancellationToken cancellationToken)
        {
            return await UpdateEventCategoryAsync(command.Id, command.Request, cancellationToken);
        }

        private async Task<EventCategoryDto> UpdateEventCategoryAsync(Guid id, UpdateEventCategoryRequest request, CancellationToken cancellationToken = default)
        {
            EventCategory category = await _unitOfWork.EventCategoryRepository.GetOneUntrackedAsync<EventCategory>(
                filter: ec => ec.Id == id && !ec.IsDeleted,
                cancellation: cancellationToken) ?? throw new NotFoundException("Không tìm thấy danh mục sự kiện");

            _mapper.Map(request, category);

            EventCategory updatedCategory = await _unitOfWork.EventCategoryRepository.UpdateAsync(category, cancellationToken);

            return _mapper.Map<EventCategoryDto>(updatedCategory);
        }
    }
}
