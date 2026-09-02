using Zeiss.InventoryControl.PTeixeira.DTOs;
using Zeiss.InventoryControl.PTeixeira.Models;

namespace Zeiss.InventoryControl.PTeixeira.Mappers;

public static class ProductMapper
{

    public static ProductResponseDto ProductToResponseDto(this Product product)
    {
        return new ProductResponseDto()
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Id = product.Id,
            ProductIdentifier = product.ProductIdentifier,
        };
    }
    
    public static List<ProductResponseDto> ProductListToResponseDto(this List<Product> list)
    {
        var products = new List<ProductResponseDto>();
        foreach (var product in list)
        {
            products.Add(ProductToResponseDto(product));
        }
      
        return products;
    }
    public static Product RequestDtoToProduct(this ProductRequestDto product)
    {
        return new Product()
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
        };
    }
    
    
}