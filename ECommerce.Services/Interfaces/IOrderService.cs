using ECommerce.Shared.DTOs;
using ECommerce.DAL.Entities;
using LearnSphere.Shared.DTOs;

namespace ECommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Result<Order>> GetOrderByIdAsync(int id);
        Task<Result<List<Order>>> GetCustomerOrdersAsync(int customerId);
        Task<Result<bool>> CancelOrderAsync(int id);
        Task<Result<Order>> CheckoutAsync(CreateOrderDto request);
    }
}
