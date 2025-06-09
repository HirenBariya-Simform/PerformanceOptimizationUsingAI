using Microsoft.AspNetCore.Mvc;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Services;

namespace PerformanceOptimizationUsingAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoryController : ControllerBase
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoryController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Inefficient: No pagination, no filtering, no caching
    [HttpGet]
    public IActionResult GetAllCategories()
    {
        try
        {
            var categories = _categoryService.GetAllCategories();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No error handling, no async/await
    [HttpGet("{id}")]
    public IActionResult GetCategoryById(int id)
    {
        var category = _categoryService.GetCategoryById(id);
        if (category == null!) return NotFound();
        return Ok(category);
    }

    // Inefficient: No pagination
    [HttpGet("search/{name}")]
    public IActionResult SearchCategoriesByName(string name)
    {
        try
        {
            var categories = _categoryService.SearchCategoriesByName(name);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No pagination
    [HttpGet("{id}/products")]
    public IActionResult GetProductsByCategory(int id)
    {
        try
        {
            var products = _categoryService.GetProductsByCategory(id);
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
    public IActionResult AddCategory([FromBody] ProductCategory category)
    {
        try
        {
            // Inefficient: Unnecessary validation
            if (category == null!) return BadRequest("No category provided");

            _categoryService.AddCategory(category);

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper validation
    [HttpPut("{id}")]
    public IActionResult UpdateCategory(int id, [FromBody] ProductCategory category)
    {
        if (id != category.CategoryId) return BadRequest();

        try
        {
            _categoryService.UpdateCategory(category);
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
    public IActionResult DeleteCategory(int id)
    {
        try
        {
            _categoryService.DeleteCategory(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }
}