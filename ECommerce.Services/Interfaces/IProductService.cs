using ECommerce.Shared.DTOs;
using ECommerce.DAL.Entities;
using LearnSphere.Shared.DTOs;

namespace ECommerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<Result<List<Product>>> GetAllAsync();
        Task<Result<Product>> GetByIdAsync(int id);
        Task<Result<Product>> CreateAsync(CreateProductDto dto);
        Task<Result<bool>> UpdateAsync(int id, Product product);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
