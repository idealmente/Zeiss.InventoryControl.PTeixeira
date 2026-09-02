using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.Models;

namespace Zeiss.InventoryControl.PTeixeira.Interfaces;

public interface IProductService
{
    public  Task<Product?> Create(Product? product);
    
    public  Task<List<Product>> GetAll();
    
    public  Task<Product?> GetById(int productId);
    
    public  Task<Product?> Update(Product? product);
    
    public  Task Delete(Product product);
    
    public Task<List<Product>> SearchByName(string name);
    
    public  Task<List<Product>> SearchAllProperties(string name);
    
    public  Task<List<Product>> GetByStockLevel(int minStockLevel, int maxStockLevel);
    
    public  Task<Product?> AddStock(int productId, int quantity);
    
    public  Task<Product?> DecrementStock(int productId, int quantity);

    public Task<int> GetProductUniqueIdenfier();
   
}