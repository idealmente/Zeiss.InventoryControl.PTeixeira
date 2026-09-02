using Microsoft.EntityFrameworkCore;
using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.Models;
using Zeiss.InventoryControl.PTeixeira.Services;

namespace Zeiss.InventoryControl.PTeixeira.Tests;

public class ProductServiceTests
{
    private static DbContextZeiss CreateContext()
    {
        var options = new DbContextOptionsBuilder<DbContextZeiss>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DbContextZeiss(options);
    }

    [Fact]
    public async Task Create_WhenProductIsValid_ShouldPersistProductAndGenerateIdentifier()
    {
        using var context = CreateContext();
        context.ProductIdentifierSeeds.Add(new ProductIdentifierSeed { CurrentIdentifier = 100 });
        await context.SaveChangesAsync();

        var service = new ProductService(context);
        var product = new Product
        {
            Name = "SunGuard Pro",
            Description = "Premium photochromic lenses.",
            Price = 149.99m,
            Stock = 12
        };

        var result = await service.Create(product);

        Assert.NotNull(result);
        Assert.Equal(101, result!.ProductIdentifier);
        Assert.Equal(101, context.ProductIdentifierSeeds.Single().CurrentIdentifier);
        Assert.Contains("SunGuard Pro", result.SearchString);
        Assert.Equal(1, context.Products.Count());
    }

    [Fact]
    public async Task SearchByName_WhenNameMatches_ShouldReturnMatchingProductsIgnoringCase()
    {
        await using var context = CreateContext();
        context.Products.AddRange(
            new Product { Id = 1, Name = "SmartLife Lenses", Description = "A", Price = 20m, Stock = 10, SearchString = "smartlife" },
            new Product { Id = 2, Name = "ClearMind Lenses", Description = "B", Price = 25m, Stock = 15, SearchString = "clearing" }
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.SearchByName("smartlife");

        var item = Assert.Single(result);
        Assert.Equal("SmartLife Lenses", item.Name);
    }

    [Fact]
    public async Task AddStock_WhenProductExists_ShouldIncreaseExistingStock()
    {
        await using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "DriveSafe Lenses", Description = "A", Price = 35m, Stock = 8, SearchString = "drivesafe" });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.AddStock(1, 5);

        Assert.NotNull(result);
        Assert.Equal(13, result!.Stock);
        Assert.Equal(13, context.Products.Single(x => x.Id == 1).Stock);
    }

    [Fact]
    public async Task DecrementStock_WhenRequestedQuantityExceedsStock_ShouldClampToZero()
    {
        using var context = CreateContext();
        context.Products.Add(new Product { Id = 1, Name = "MyoCare Lenses", Description = "A", Price = 55m, Stock = 3, SearchString = "myocare" });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.DecrementStock(1, 10);

        Assert.NotNull(result);
        Assert.Equal(0, result!.Stock);
        Assert.Equal(0, context.Products.Single(x => x.Id == 1).Stock);
    }

    [Fact]
    public async Task GetProductUniqueIdenfier_WhenSeedIsAtMaximumValue_ShouldReturnZeroAndNeverExceedLimit()
    {
        using var context = CreateContext();
        context.ProductIdentifierSeeds.Add(new ProductIdentifierSeed { CurrentIdentifier = 999999 });
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        var result = await service.GetProductUniqueIdenfier();

        Assert.Equal(0, result);
        Assert.InRange(result, 0, 999999);
    }
}
