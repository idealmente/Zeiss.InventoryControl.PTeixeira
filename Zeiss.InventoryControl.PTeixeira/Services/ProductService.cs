using Microsoft.EntityFrameworkCore;
using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.Interfaces;
using Zeiss.InventoryControl.PTeixeira.Models;

namespace Zeiss.InventoryControl.PTeixeira.Services;

public class ProductService(DbContextZeiss context):IProductService
{
    public async Task<Product?> Create(Product? product)
    {
        if (product == null) return null;
            product.ProductIdentifier = await GetProductUniqueIdenfier();
            
            // Generate extra field that concatenates all the class properties values for a fast search
            // TODO to improve, - Move this generation to the base class dbcontext for the method Create
            //                  - Add prop SearchString to all entities, creating a base class entity that have this prop
            //                  - Using reflexion is possible to create the concatenation dynamically iterating the properties of each class in runtime
            string propSeparator = "|";
            product.SearchString = product.ProductIdentifier + propSeparator +
                                   product.Name + propSeparator +
                                   product.Description + propSeparator +
                                   product.Price + propSeparator +
                                   product.Stock + propSeparator;
            
            var entityEntry = context.Products.Add(product);
            var addedEntity = entityEntry.Entity;

            await context.SaveChangesAsync();

            return addedEntity;
    }

    public async Task<List<Product>> GetAll()
    {
        return (await context.Products.ToListAsync());
    }

    public async Task<Product?> GetById(int productId)
    {
        return (await GetAll()).FirstOrDefault(x => x.Id.Equals(productId));
    }

    public async Task<Product?> Update(Product? product)
    {
        if (product == null) return null;
            var entityEntry = context.Products.Update(product);
            var updatedEntity = entityEntry.Entity;

            await context.SaveChangesAsync();

            return updatedEntity;
    }

    public async Task Delete(Product product)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }

    public async Task<List<Product>> SearchByName(string searchedName)
    {
        var result = (await GetAll()).FindAll(x => x.Name.Contains(searchedName, StringComparison.OrdinalIgnoreCase));
        if (result.Count>0) return  result.ToList();
        else return new List<Product>();
    }

    public async Task<List<Product>> SearchAllProperties(string searchedString)
    {
        var result =(await GetAll()).FindAll(x => x.SearchString.Contains(searchedString, StringComparison.OrdinalIgnoreCase));
        if (result.Count>0) return  result.ToList();
        else return new List<Product>();
    }

    public async Task<List<Product>> GetByStockLevel(int minStockLevel, int maxStockLevel)
    {
        var result =(await GetAll()).FindAll(x => x.Stock >= minStockLevel && x.Stock<= maxStockLevel);
        if (result.Count>0) return  result.ToList();
        else return new List<Product>();
    }

    public async Task<Product?> AddStock(int productId, int quantity)
    {
        var productToChange = (await GetAll()).FirstOrDefault(x => x.Id.Equals(productId));
        if (productToChange != null)
        {
            productToChange.Stock += quantity;
            await Update(productToChange);
        }
       
        return productToChange;  
    }

    public async Task<Product?> DecrementStock(int productId, int quantity)
    {   
        var productToChange = (await GetAll()).FirstOrDefault(x =>x.Id.Equals(productId));
        if (productToChange == null) return productToChange;
        if (productToChange.Stock <= quantity)
        {
            productToChange.Stock = 0;
        }
        else
        {
            productToChange.Stock -= quantity;
        }
        
        await Update(productToChange);
        await context.SaveChangesAsync();

        return productToChange;  
    }

    public async Task<int> GetProductUniqueIdenfier()
    {
        var currentProductUniqueIdentifier = context.ProductIdentifierSeeds.FirstOrDefault();
        if (currentProductUniqueIdentifier is { CurrentIdentifier: < 999999 })
        {
            var nextPreproductionIdentifier = currentProductUniqueIdentifier.CurrentIdentifier + 1;
            currentProductUniqueIdentifier.CurrentIdentifier = nextPreproductionIdentifier;

            context.ProductIdentifierSeeds.Update(currentProductUniqueIdentifier);

            await context.SaveChangesAsync();

            return currentProductUniqueIdentifier.CurrentIdentifier;
        }

        return 0;
    }
}