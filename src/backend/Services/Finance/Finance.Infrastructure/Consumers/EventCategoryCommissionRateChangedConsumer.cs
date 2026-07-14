using BuildingBlocks.Contracts.Events.EventCategory;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Consumers
{
    public class EventCategoryCommissionRateChangedConsumer : IConsumer<EventCategoryCommissionRateChangedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventCategoryCommissionRateChangedConsumer> _logger;

        public EventCategoryCommissionRateChangedConsumer(IUnitOfWork unitOfWork, ILogger<EventCategoryCommissionRateChangedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EventCategoryCommissionRateChangedEvent> context)
        {
            var message = context.Message;

            CommissionSetting? setting = await _unitOfWork.CommissionSettingRepository.GetOneAsync<CommissionSetting>(
                filter: cs => cs.CategoryId == message.CategoryId);

            if (setting == null)
            {
                setting = new CommissionSetting(message.CategoryId, message.CategoryName, message.RecommendedCommissionRate);
                await _unitOfWork.CommissionSettingRepository.CreateAsync(setting);
            }
            else
            {
                setting.SyncFromCatalog(message.CategoryName, message.RecommendedCommissionRate);
                _unitOfWork.CommissionSettingRepository.UpdateEntity(setting);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Đã đồng bộ % hoa hồng tham khảo cho danh mục {CategoryId}: {Rate}%",
                message.CategoryId, message.RecommendedCommissionRate);
        }
    }
}
