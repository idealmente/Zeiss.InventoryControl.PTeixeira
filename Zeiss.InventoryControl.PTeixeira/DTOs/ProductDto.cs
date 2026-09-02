using System.ComponentModel.DataAnnotations;

namespace Zeiss.InventoryControl.PTeixeira.DTOs
{

    public class ProductRequestDto
    {
        [Required] [StringLength(100)] 
        public string Name { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required][StringLength(500)] 
        public string Description { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue, ErrorMessage = "Stock must be greater than 0")]
        public int Stock { get; set; }
    }
    
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(500)] 
        public string Description { get; set; } = string.Empty;
        
        public int ProductIdentifier { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Stock must be greater than 0")]
        public int Stock { get; set; }
    }
}