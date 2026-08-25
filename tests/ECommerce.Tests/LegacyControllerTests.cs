using ECommerce.API.Controllers;
using ECommerce.Services.Interfaces;
using ECommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ECommerce.Tests;

public class LegacyControllerTests
{
    private readonly Mock<IProductService> _productServiceMock = new();

    [Fact]
    public async Task CreateProduct_WithZeroPrice_ReturnsBadRequest()
    {
        var controller = new ProductsController(_productServiceMock.Object);

        var dto = new CreateProductDto
        {
            Name = "Invalid",
            SKU = "INV-01",
            Price = 0,
            StockQuantity = 5
        };

        // If validation triggers via ModelState:
        controller.ModelState.AddModelError("Price", "Price must be greater than zero.");

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}