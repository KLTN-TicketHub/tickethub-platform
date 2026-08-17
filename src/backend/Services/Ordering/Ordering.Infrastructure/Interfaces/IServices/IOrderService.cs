using Ordering.Common.Dtos;

namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<Guid> CheckoutAsync(CheckoutRequestDto request, Guid userId);
    }
}
