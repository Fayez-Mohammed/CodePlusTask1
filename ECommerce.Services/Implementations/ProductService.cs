using ECommerce.Shared.DTOs;
using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using ECommerce.Services.Interfaces;
using LearnSphere.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Product>>> GetAllAsync()
        {
            var products = await _context.Products.ToListAsync();
            return Result<List<Product>>.Success(products);
        }

        public async Task<Result<Product>> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return Result<Product>.Failure($"Product with ID {id} not found.", 404);

            return Result<Product>.Success(product);
        }

        public async Task<Result<Product>> CreateAsync(CreateProductDto dto)
        {
            if (dto.Price <= 0)
                return Result<Product>.Failure("Product price must be greater than zero.", 400);

            if (dto.StockQuantity < 0)
                return Result<Product>.Failure("Stock quantity cannot be negative.", 400);

            var skuExists = await _context.Products.AnyAsync(p => p.SKU.ToLower() == dto.SKU.ToLower());
            if (skuExists)
                return Result<Product>.Failure($"Product with SKU '{dto.SKU}' already exists.", 400);

            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU.ToUpper(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Result<Product>.Success(product);
        }

        public async Task<Result<bool>> UpdateAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return Result<bool>.Failure($"Product with ID {id} not found.", 404);

            if (product.Price <= 0)
                return Result<bool>.Failure("Price must be positive.", 400);

            existing.Name = product.Name;
            existing.SKU = product.SKU;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;

            _context.Products.Update(existing);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return Result<bool>.Failure($"Product with ID {id} not found.", 404);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}