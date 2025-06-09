using Microsoft.AspNetCore.Mvc;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Services;

namespace PerformanceOptimizationUsingAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    // Inefficient: No pagination, no filtering, no caching
    [HttpGet]
    public IActionResult GetAllProducts()
    {
        try
        {
            var products = _productService.GetAllProducts();
            return Ok(products);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No caching strategy
    [HttpGet("category/{categoryId}")]
    public IActionResult GetProductsByCategory(int categoryId)
    {
        try
        {
            // Inefficient: Direct service call without any optimization
            var products = _productService.GetProductsByCategory(categoryId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper validation
    [HttpPost]
    public IActionResult AddProducts([FromBody] List<Product> products)
    {
        try
        {
            // Inefficient: Unnecessary validation
            if (products == null! || !products.Any()) return BadRequest("No products provided");

            _productService.AddProducts(products);

            return CreatedAtAction(nameof(GetAllProducts), products);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper validation
    [HttpPut("{id}/stock")]
    public IActionResult UpdateStock(int id, [FromBody] int quantity)
    {
        try
        {
            // Inefficient: Unnecessary validation
            if (quantity == 0) return BadRequest("Quantity cannot be zero");

            _productService.UpdateProductStock(id, quantity);

            return NoContent();
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper cleanup
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            _productService.DeleteProduct(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }
}