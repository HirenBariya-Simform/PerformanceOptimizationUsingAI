using Microsoft.AspNetCore.Mvc;
using POC.DTOs.Extensions;
using POC.DTOs.Product;
using POC.Services;

namespace POC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Performance Issues:
    /// 1. Unnecessary Data Loading: Loads all products without pagination
    /// 2. Memory Inefficiencies: No result set size limits
    /// 3. Inefficient Query Patterns: No filtering or sorting at database level
    /// 4. Inefficient Query Patterns: No proper indexing strategy
    /// </summary>
    [HttpGet]
    public IActionResult AllProducts()
    {
        try
        {
            var products = productService.GetAllProducts();
            var response = products.ToListItems();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Inefficient Query Patterns: No proper indexing strategy
    /// 2. Unnecessary Data Loading: Loads full product details when only basic info is needed
    /// 3. Memory Inefficiencies: No caching strategy for frequently accessed products
    /// 4. Inefficient Query Patterns: N+1 query problem with category loading
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult ProductById(int id)
    {
        try
        {
            var product = productService.GetProductById(id);
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
    /// Performance Issues:
    /// 1. Inefficient Query Patterns: No proper indexing on categoryId
    /// 2. Unnecessary Data Loading: Loads full product details when only category products are needed
    /// 3. Memory Inefficiencies: No pagination for categories with many products
    /// 4. Inefficient Query Patterns: N+1 query problem with category details
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public IActionResult ProductsByCategory(int categoryId)
    {
        try
        {
            var products = productService.GetProductsByCategory(categoryId);
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
    /// Performance Issues:
    /// 1. Transaction and Concurrency Issues: No proper transaction management
    /// 2. Write Operation Inefficiencies: No bulk insert optimization
    /// 3. Memory Inefficiencies: No validation of input data size
    /// 4. Inefficient Query Patterns: Multiple database calls for product creation
    /// 5. Inefficient Query Patterns: N+1 query problem when reloading created products
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
            productService.AddProducts(products);

            // Inefficient: Reloading products with category information
            var productIds = products.Select(p => p.ProductId).ToList();
            var createdProducts = productService.GetProductsByIds(productIds);

            var response = createdProducts.ToResponseList();
            return CreatedAtAction(nameof(AllProducts), response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating products", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Transaction and Concurrency Issues: No optimistic concurrency control
    /// 2. Write Operation Inefficiencies: No bulk update optimization
    /// 3. Memory Inefficiencies: No validation of update data
    /// 4. Inefficient Query Patterns: Multiple database calls for product update
    /// 5. Inefficient Query Patterns: N+1 query problem when reloading updated product
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] ProductUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            existingProduct.UpdateFromRequest(request);
            productService.UpdateProduct(existingProduct);

            // Inefficient: Reloading product with category information
            var updatedProduct = productService.GetProductById(id);
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
    /// Performance Issues:
    /// 1. Transaction and Concurrency Issues: No proper transaction management
    /// 2. Write Operation Inefficiencies: No cascade delete optimization
    /// 3. Memory Inefficiencies: No cleanup of related data
    /// 4. Inefficient Query Patterns: Multiple database calls for product deletion
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            var existingProduct = productService.GetProductById(id);
            if (existingProduct == null!)
                return NotFound(new { message = $"Product with ID {id} not found" });

            productService.DeleteProduct(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the product", error = ex.Message });
        }
    }

    ///// <summary>
    /////     Update product stock quantity
    ///// </summary>
    //[HttpPut("{id}/stock")]
    //public IActionResult UpdateStock(int id, [FromBody] ProductStockUpdateRequest request)
    //{
    //    try
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(ModelState);

    //        var existingProduct = _productService.GetProductById(id);
    //        if (existingProduct == null)
    //            return NotFound(new { message = $"Product with ID {id} not found" });

    //        _productService.UpdateProductStock(id, request.StockQuantity);

    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500,
    //            new { message = "An error occurred while updating product stock", error = ex.Message });
    //    }
    //}

}