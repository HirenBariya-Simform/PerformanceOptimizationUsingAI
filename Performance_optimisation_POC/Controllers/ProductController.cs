using Microsoft.AspNetCore.Mvc;
using POC.DTOs.Extensions;
using POC.DTOs.Product;
using POC.Services;

namespace POC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    ///     Get all products with minimal information for listing
    /// </summary>
    [HttpGet]
    public IActionResult GetAllProducts()
    {
        try
        {
            var products = _productService.GetAllProducts();
            var response = products.ToListItems();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
        }
    }

    /// <summary>
    ///     Get a specific product by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        try
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            var response = product.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving the product", error = ex.Message });
        }
    }

    /// <summary>
    ///     Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public IActionResult GetProductsByCategory(int categoryId)
    {
        try
        {
            var products = _productService.GetProductsByCategory(categoryId);
            var response = products.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving products by category", error = ex.Message });
        }
    }

    /// <summary>
    ///     Create new products
    /// </summary>
    [HttpPost]
    public IActionResult CreateProducts([FromBody] List<ProductCreateRequest> requests)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (requests == null || !requests.Any())
                return BadRequest(new { message = "No products provided" });

            var products = requests.Select(r => r.ToEntity()).ToList();
            _productService.AddProducts(products);

            // Reload products with category information to avoid null reference errors
            var productIds = products.Select(p => p.ProductId).ToList();
            var createdProducts = _productService.GetProductsByIds(productIds);

            var response = createdProducts.ToResponseList();
            return CreatedAtAction(nameof(GetAllProducts), response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating products", error = ex.Message });
        }
    }

    /// <summary>
    ///     Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            existingProduct.UpdateFromRequest(request);
            _productService.UpdateProduct(existingProduct);

            // Reload product with category information to avoid null reference errors
            var updatedProduct = _productService.GetProductById(id);
            var response = updatedProduct.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the product", error = ex.Message });
        }
    }

    /// <summary>
    ///     Update product stock quantity
    /// </summary>
    [HttpPut("{id}/stock")]
    public IActionResult UpdateStock(int id, [FromBody] ProductStockUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            _productService.UpdateProductStock(id, request.StockQuantity);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating product stock", error = ex.Message });
        }
    }

    /// <summary>
    ///     Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            _productService.DeleteProduct(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the product", error = ex.Message });
        }
    }
}