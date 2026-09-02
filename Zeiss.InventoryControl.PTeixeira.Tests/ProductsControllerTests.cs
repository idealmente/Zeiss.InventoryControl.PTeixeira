using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeiss.InventoryControl.PTeixeira.Controllers;
using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.DTOs;
using Zeiss.InventoryControl.PTeixeira.Models;
using Zeiss.InventoryControl.PTeixeira.Services;

namespace Zeiss.InventoryControl.PTeixeira.Tests;

public class ProductsControllerTests
{
    private static ProductsController CreateControllerWithSeed(params Product[] products)
    {
        var options = new DbContextOptionsBuilder<DbContextZeiss>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new DbContextZeiss(options);

        if (products.Length > 0)
        {
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        var service = new ProductService(context);
        return new ProductsController(context, service);
    }

    [Fact]
    public void GetById_WhenProductExists_ReturnsOkWithProductDto()
    {
        var controller = CreateControllerWithSeed(
            new Product { Id = 1, Name = "SmartLife Lenses", Description = "Premium", Price = 25m, ProductIdentifier = 100, Stock = 10, SearchString = "smartlife" }
        );

        var result = controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<ProductResponseDto>(okResult.Value);
        Assert.Equal(1, dto.Id);
        Assert.Equal("SmartLife Lenses", dto.Name);
    }

    [Fact]
    public void GetById_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var controller = CreateControllerWithSeed();

        var result = controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public void Create_WhenRequestIsValid_ReturnsCreatedAtActionWithPersistedProduct()
    {
        var context = new DbContextZeiss(new DbContextOptionsBuilder<DbContextZeiss>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        context.ProductIdentifierSeeds.Add(new ProductIdentifierSeed { CurrentIdentifier = 100 });
        context.SaveChanges();

        var controller = new ProductsController(context, new ProductService(context));
        var request = new ProductRequestDto
        {
            Name = "DriveSafe Lenses",
            Description = "Night driving lenses",
            Price = 99.99m,
            Stock = 12
        };

        var result = controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues!["id"]);

        var dto = Assert.IsType<ProductResponseDto>(createdResult.Value);
        Assert.Equal("DriveSafe Lenses", dto.Name);
        Assert.Equal(101, dto.ProductIdentifier);
    }

    [Fact]
    public void Update_WhenProductExists_UpdatesPriceAndDescriptionAndReturnsOk()
    {
        var controller = CreateControllerWithSeed(
            new Product { Id = 1, Name = "ClearMind Lenses", Description = "Old description", Price = 12m, ProductIdentifier = 500, Stock = 15, SearchString = "clearmind" }
        );

        var request = new ProductRequestDto
        {
            Name = "ClearMind Lenses",
            Description = "Updated description",
            Price = 18m,
            Stock = 20
        };

        var result = controller.Update(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ProductResponseDto>(okResult.Value);
        Assert.Equal("Updated description", dto.Description);
        Assert.Equal(18m, dto.Price);
    }

    [Fact]
    public void AddStock_WhenProductExists_IncrementsStockAndReturnsOk()
    {
        var controller = CreateControllerWithSeed(
            new Product { Id = 1, Name = "MyoCare Lenses", Description = "Child eye care", Price = 45m, ProductIdentifier = 750, Stock = 5, SearchString = "myocare" }
        );

        var result = controller.AddStock(1, 3);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ProductResponseDto>(okResult.Value);
        Assert.Equal(8, dto.Stock);
    }
}
