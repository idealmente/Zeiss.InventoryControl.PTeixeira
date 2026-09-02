using Microsoft.AspNetCore.Mvc;
using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.DTOs;
using Zeiss.InventoryControl.PTeixeira.Interfaces;
using Zeiss.InventoryControl.PTeixeira.Mappers;
using Zeiss.InventoryControl.PTeixeira.Models;

namespace Zeiss.InventoryControl.PTeixeira.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(DbContextZeiss context, IProductService productService) : ControllerBase
    {
        private DbContextZeiss _db = context;
       
       // GET /api/products
        [HttpGet]
        public ActionResult<List<ProductResponseDto>> GetAll()
        {
            return Ok(ProductMapper.ProductListToResponseDto(productService.GetAll().Result!));
        }

        // GET /api/products/{id}
        [HttpGet("{id}")]
        public ActionResult<ProductResponseDto> GetById(int id)
        {
            var product =  productService.GetById((id)).Result;
            if (product == null) return NotFound($"Product with ID {id} not found.");
            return Ok(ProductMapper.ProductToResponseDto(product));
        }
        
        // POST /api/products
        [HttpPost]
        public ActionResult<ProductResponseDto> Create(ProductRequestDto dto)
        {
            var createdProduct = productService.Create(ProductMapper.RequestDtoToProduct(dto)).Result;
            if (createdProduct == null)
            {
                return BadRequest("Unable to create product.");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, ProductMapper.ProductToResponseDto(createdProduct));
        }
        
        // PUT /api/products/{id}
        [HttpPut("{id}")]
        public ActionResult Update(int id, ProductRequestDto dto)
        {
            var product = productService.GetAll().Result.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound($"Product with ID {id} not found.");
            product.Name        = dto.Name;
            product.Price       = dto.Price;
            product.Description = dto.Description;
            product = productService.Update(product).Result;
            
            if (product == null) return NotFound($"Product with ID {id} not found.");
            return Ok(ProductMapper.ProductToResponseDto(product));
        }
        
        // DELETE /api/products/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var product = productService.GetAll().Result.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound($"Product with ID {id} not found.");
            productService.Delete(product);
            
            return Ok();
        }
        
        // Decrement Stock
        // GET: /api/products/1/decrement-stock/143
        [HttpGet("{id}/decrement-stock/{quantity}")]
        public IActionResult DecrementStock(int id, int quantity)
        {
            var product = productService.DecrementStock(id, quantity).Result;
            if (product == null) return NotFound($"Product with ID {id} not found.");

            return Ok(ProductMapper.ProductToResponseDto(product));
        }
        
        // Increment Stock
        // GET: /api/products/1/add-to-stock/143
        [HttpGet("{id}/add-to-stock/{quantity}")]
        public IActionResult AddStock(int id, int quantity)
        {
            var product = productService.AddStock(id, quantity).Result;
            if (product == null) return NotFound($"Product with ID {id} not found.");

            return Ok(ProductMapper.ProductToResponseDto(product));
        }
        
        // Search by name
        // GET: /api/products/search?name=Lense
        [HttpGet("search")]
        public IActionResult Search(string name)
        {
            return Ok(productService.SearchByName(name).Result);
        }
        
        // Get by stcok interval
        // GET: /api/products/stock-level?min=0&max=100
        [HttpGet("stock-level")]
        public IActionResult Search(int min, int max)
        {
            return Ok(ProductMapper.ProductListToResponseDto(productService.GetByStockLevel(min, max).Result));
        }
        
        // Search by any propery of the product - Bonus Endpoint
        // GET: /api/products/search-all?stringToSearch=Lense
        [HttpGet("search-all")]
        public IActionResult SearchAllProperties(string stringToSearch)
        {
            return Ok(productService.SearchAllProperties(stringToSearch).Result);
        }
    }
