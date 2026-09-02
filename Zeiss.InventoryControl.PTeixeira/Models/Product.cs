namespace Zeiss.InventoryControl.PTeixeira.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public int ProductIdentifier { get; set; }
    public int Stock { get; set; }
    
    // Extra property that allows to search in all properties
    public string SearchString { get; set; } = string.Empty;
}