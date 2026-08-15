using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.ReorderEventCategories
{
    public class ReorderEventCategoriesCommandHandler : IRequestHandler<ReorderEventCategoriesCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReorderEventCategoriesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ReorderEventCategoriesCommand request, CancellationToken cancellationToken)
        {
            return await ReorderEventCategoriesAsync(request.Categories, cancellationToken);
        }

        private async Task<bool> ReorderEventCategoriesAsync(List<CategoryOrderDto> categories, CancellationToken cancellationToken)
        {
            List<Guid> categoryIds = categories.Select(c => c.CategoryId).ToList();

            List<EventCategory> categoryList = (await _unitOfWork.EventCategoryRepository.GetAllAsync<EventCategory>(
                filter: ec => categoryIds.Contains(ec.Id) && !ec.IsDeleted,
                selector: ec => ec,
                cancellation: cancellationToken)).ToList();

            List<Guid> missingIds = categoryIds.Except(categoryList.Select(c => c.Id)).ToList();
            if (missingIds.Count > 0)
                throw new NotFoundException($"Không tìm thấy danh mục sự kiện với ID: {string.Join(", ", missingIds)}.");

            foreach (EventCategory category in categoryList)
            {
                int newOrder = categories.First(c => c.CategoryId == category.Id).DisplayOrder;
                category.UpdateDisplayOrder(newOrder);
                await _unitOfWork.EventCategoryRepository.UpdateAsync(category, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
