using ECommerce.Shared.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> Create([FromBody] CreateCustomerDto dto)
        {
            var result = await _customerService.CreateAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPost("{id}/upgrade-vip")]
        public async Task<IActionResult> UpgradeToVip(int id)
        {
            var result = await _customerService.UpgradeToVipAsync(id);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return Ok(new { message = result.Value });
        }
    }
}