using AutoMapper;
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

        public CreateEventCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EventCategoryDto> Handle(CreateEventCategoryCommand command, CancellationToken cancellation = default)
        {
            return await CreateEventCategoryAsync(command.Request, cancellation);
        }

        private async Task<EventCategoryDto> CreateEventCategoryAsync(CreateEventCategoryRequest request, CancellationToken cancellation = default)
        {
            if (await _unitOfWork
                .EventCategoryRepository
                .IsExistsAsync(
                    nameof(EventCategory.CategoryName),
                    request.CategoryName, cancellation))
                throw new ValidatorException($"Tên danh mục sự kiện '{request.CategoryName}' đã tồn tại.");

            string categoryCode = await _unitOfWork
                .EventCategoryRepository
                .GenerateNextCategoryCodeAsync(request.CategoryName, cancellation);

            EventCategory category = _mapper.Map<EventCategory>(request);

            category.SetCategoryCode(categoryCode);

            return _mapper.Map<EventCategoryDto>(
                await _unitOfWork.EventCategoryRepository.CreateAsync(category, cancellation));
        }
    }
}
