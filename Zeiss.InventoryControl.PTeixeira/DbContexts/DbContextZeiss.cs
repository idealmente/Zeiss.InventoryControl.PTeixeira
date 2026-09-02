using Microsoft.EntityFrameworkCore;
using Zeiss.InventoryControl.PTeixeira.Models;

namespace Zeiss.InventoryControl.PTeixeira.DbContexts
{
    public class DbContextZeiss(DbContextOptions<DbContextZeiss> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIdentifierSeed> ProductIdentifierSeeds { get; set; }
    }
}

