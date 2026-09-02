using Microsoft.EntityFrameworkCore;
using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.Interfaces;
using Zeiss.InventoryControl.PTeixeira.Models;

namespace Zeiss.InventoryControl.PTeixeira.Helpers;

public class DbInitializationHelper(DbContextZeiss db)
{
    
    public void InitDb()
    {
        #region dataCreation

        int count = 234;
        
        db.Products.Add(new Product()
        {
            Name = "ClearMind Lenses",
            Description = "Premium progressive lenses designed to reduce cognitive load in visually busy environments.",
            Price = 20,
            ProductIdentifier = ++count,
            Stock = 10
        });
            
        db.Products.Add(new Product()
        {
            Name = "SmartLife Lenses",
            Description = "Lenses optimized for dynamic, on-the-move, and digitally connected lifestyles.",
            Price = 25,
            ProductIdentifier = ++count,
            Stock = 100,
        });
        
        db.Products.Add(new Product()
        {
            Name = "ClearView FSV Lenses",
            Description = "Lenses optimized for dynamic, on-the-move, and digitally connected lifestyles.",
            Price = 10,
            ProductIdentifier = ++count,
            Stock = 10,
        });
        
        db.Products.Add(new Product()
        {
            Name = "DriveSafe Lenses",
            Description = "Specialized lenses tailored for driving safety, reducing glare and easing eye fatigue at night or in low light.",
            Price = 15,
            ProductIdentifier = ++count,
            Stock = 4,
        });
        
        db.Products.Add(new Product()
        {
            Name = "MyoCare Lenses",
            Description = "Specialized lenses designed to help slow the progression of myopia in children.",
            Price = 35,
            ProductIdentifier = ++count,
            Stock = 3,
        });
        
        
        // ADD MORE PRODUCTS ON THE TOP
        
        db.ProductIdentifierSeeds.Add(new ProductIdentifierSeed()
        {
            CurrentIdentifier = count
        });
        
         db.SaveChanges();
       
        
        // create SearchString prop
        foreach (var product in db.Products.ToList())
        {
            string propSeperator = "|";
            product.SearchString =  product.ProductIdentifier + propSeperator +
                                         product.Name + propSeperator +
                                         product.Description + propSeperator +
                                         product.Price + propSeperator +
                                         product.Stock + propSeperator;
        }
        #endregion
        
        db.SaveChanges();
       
    }
}