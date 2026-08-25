using ECommerce.Shared.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return Ok(result.Value);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<Order>>> GetCustomerOrders(int customerId)
        {
            var result = await _orderService.GetCustomerOrdersAsync(customerId);
            return Ok(result.Value);
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return Ok(new { message = "Order cancelled successfully" });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request)
        {
            var result = await _orderService.CheckoutAsync(request);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var order = result.Value;
            return Ok(new
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                Subtotal = order.Subtotal,
                Discount = order.DiscountAmount,
                Tax = order.TaxAmount,
                Shipping = order.ShippingFee,
                Total = order.TotalAmount,
                TransactionReference = order.Payment?.TransactionReference
            });
        }
    }
}