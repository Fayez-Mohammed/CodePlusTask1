using ECommerce.Shared.DTOs;
using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using ECommerce.Services.Interfaces;
using LearnSphere.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Order>> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return Result<Order>.Failure("Order not found.", 404);

            return Result<Order>.Success(order);
        }

        public async Task<Result<List<Order>>> GetCustomerOrdersAsync(int customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            return Result<List<Order>>.Success(orders);
        }

        public async Task<Result<bool>> CancelOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return Result<bool>.Failure("Order not found.", 404);

            if (order.Status == OrderStatus.Cancelled)
                return Result<bool>.Failure("Order is already cancelled.", 400);

            if (order.Status == OrderStatus.Paid)
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<Order>> CheckoutAsync(CreateOrderDto request)
        {
            if (request.Items == null || !request.Items.Any())
                return Result<Order>.Failure("Cannot checkout an empty order.", 400);

            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
                return Result<Order>.Failure($"Customer with ID {request.CustomerId} not found.", 404);

            decimal subtotal = 0m;
            var orderItemsToSave = new List<OrderItem>();
            var productsToUpdate = new List<Product>();

            foreach (var itemDto in request.Items)
            {
                if (itemDto.Quantity <= 0)
                    return Result<Order>.Failure("Product quantity must be at least 1.", 400);

                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    return Result<Order>.Failure($"Product with ID {itemDto.ProductId} not found.", 404);

                if (product.StockQuantity < itemDto.Quantity)
                    return Result<Order>.Failure($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}", 400);

                subtotal += product.Price * itemDto.Quantity;

                orderItemsToSave.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });

                product.StockQuantity -= itemDto.Quantity;
                productsToUpdate.Add(product);
            }

            decimal discount = 0m;
            if (customer.IsVip)
            {
                discount += Math.Round(subtotal * 0.15m, 2);
            }

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.ToUpper() == request.CouponCode.ToUpper() && c.IsActive);

                if (coupon == null)
                    return Result<Order>.Failure($"Invalid or inactive coupon code '{request.CouponCode}'.", 400);

                discount += Math.Round(subtotal * (coupon.DiscountPercentage / 100m), 2);
            }

            if (discount > subtotal)
            {
                discount = subtotal;
            }

            var netAmount = subtotal - discount;
            var tax = Math.Round(netAmount * 0.14m, 2);
            var shipping = netAmount >= 1000m ? 0m : 75m;
            var finalTotal = netAmount + tax + shipping;

            if (finalTotal > 50000m)
                return Result<Order>.Failure("Payment processing failed. Amount exceeds limit.", 400);

            var txRef = $"TX-LEGACY-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            var order = new Order
            {
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Paid,
                Subtotal = subtotal,
                DiscountAmount = discount,
                TaxAmount = tax,
                ShippingFee = shipping,
                TotalAmount = finalTotal,
                Items = orderItemsToSave
            };

            var payment = new Payment
            {
                Order = order,
                Amount = finalTotal,
                PaymentDate = DateTime.UtcNow,
                TransactionReference = txRef,
                IsSuccess = true
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Result<Order>.Failure("An error occurred while saving the order.", 500);
            }

            return Result<Order>.Success(order);
        }
    }
}