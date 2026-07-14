using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.EventCategory;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.CreateEventCategory
{
    public class CreateEventCategoryCommandHandler : IRequestHandler<CreateEventCategoryCommand, EventCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CreateEventCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<EventCategoryDto> Handle(CreateEventCategoryCommand command, CancellationToken cancellation = default)
        {
            return await CreateEventCategoryAsync(command.Request, cancellation);
        }

        private async Task<EventCategoryDto> CreateEventCategoryAsync(CreateEventCategoryRequest request, CancellationToken cancellation = default)
        {
            EventCategory? eventCategory = await _unitOfWork.EventCategoryRepository.GetOneUntrackedAsync<EventCategory>(x => x.CategoryName == request.CategoryName, cancellation: cancellation);

            if(eventCategory != null)   
                throw new BusinessRuleException($"Tên danh mục '{request.CategoryName}' đã tồn tại.");

            string categoryCode = await _unitOfWork
                .EventCategoryRepository
                .GenerateNextCategoryCodeAsync(request.CategoryName, cancellation);

            EventCategory category = _mapper.Map<EventCategory>(request);

            category.SetCategoryCode(categoryCode);

            EventCategory createdCategory = await _unitOfWork.EventCategoryRepository.CreateAsync(category, cancellation);

            await _eventPublisher.PublishAsync(new EventCategoryCommissionRateChangedEvent
            {
                CategoryId = createdCategory.Id,
                CategoryName = createdCategory.CategoryName,
                RecommendedCommissionRate = createdCategory.RecommendedCommissionRate
            }, cancellationToken: cancellation);

            await _unitOfWork.SaveChangesAsync(cancellation);

            return _mapper.Map<EventCategoryDto>(createdCategory);
        }
    }
}
