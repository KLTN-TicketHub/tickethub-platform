using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.DeleteEventCategory
{
    public class DeleteEventCategoryCommandHandler : IRequestHandler<DeleteEventCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteEventCategoryCommand command, CancellationToken cancellationToken)
        {
            return await DeleteEventCategoryAsync(command.Id, cancellationToken);
        }

        private async Task<Unit> DeleteEventCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            EventCategory category = await _unitOfWork.EventCategoryRepository.GetOneUntrackedAsync<EventCategory>(
                filter: ec => ec.Id == id && !ec.IsDeleted,
                cancellation: cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy danh mục sự kiện");

            await _unitOfWork.EventCategoryRepository.DeleteAsync(category, cancellationToken);

            return Unit.Value;
        }
    }
}
