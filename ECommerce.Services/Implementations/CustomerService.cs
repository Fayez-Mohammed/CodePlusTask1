using ECommerce.Shared.DTOs;
using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using ECommerce.Services.Interfaces;

using LearnSphere.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Customer>> GetByIdAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return Result<Customer>.Failure($"Customer with ID {id} not found.", 404);

            return Result<Customer>.Success(customer);
        }

        public async Task<Result<Customer>> CreateAsync(CreateCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return Result<Customer>.Failure("Full name is required.", 400);

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
                return Result<Customer>.Failure("A valid email address is required.", 400);

            var emailExists = await _context.Customers.AnyAsync(c => c.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
                return Result<Customer>.Failure("Email is already registered.", 400);

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                IsVip = dto.IsVip
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return Result<Customer>.Success(customer);
        }

        public async Task<Result<string>> UpgradeToVipAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return Result<string>.Failure($"Customer with ID {id} not found.", 404);

            var totalSpent = customer.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .Sum(o => o.TotalAmount);

            if (totalSpent < 500m)
                return Result<string>.Failure($"Customer does not qualify for VIP. Total spend {totalSpent:C} is less than required $500.00", 400);

            customer.IsVip = true;
            await _context.SaveChangesAsync();

            return Result<string>.Success("Customer upgraded to VIP successfully.");
        }
    }
}