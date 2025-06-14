using Microsoft.AspNetCore.Mvc;
using POC.DTOs.Extensions;
using POC.DTOs.ProductCategory;
using POC.Services;

namespace POC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoryController : ControllerBase
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoryController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    ///     Get all categories with minimal information for listing
    /// </summary>
    [HttpGet]
    public IActionResult GetAllCategories()
    {
        try
        {
            var categories = _categoryService.GetAllCategories();
            var response = categories.ToListItems();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving categories", error = ex.Message });
        }
    }

    /// <summary>
    ///     Get a specific category by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetCategoryById(int id)
    {
        try
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound(new { message = $"Category with ID {id} not found" });

            var response = category.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving the category", error = ex.Message });
        }
    }

    /// <summary>
    ///     Search categories by name
    /// </summary>
    [HttpGet("search/{name}")]
    public IActionResult SearchCategoriesByName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Search name cannot be empty" });

            var categories = _categoryService.SearchCategoriesByName(name);
            var response = categories.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while searching categories", error = ex.Message });
        }
    }

    /// <summary>
    ///     Get products by category ID
    /// </summary>
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
            return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
        }
    }

    /// <summary>
    ///     Create a new category
    /// </summary>
    [HttpPost]
    public IActionResult CreateCategory([FromBody] ProductCategoryCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = request.ToEntity();
            _categoryService.AddCategory(category);

            var response = category.ToResponse();
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while creating the category", error = ex.Message });
        }
    }

    /// <summary>
    ///     Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult UpdateCategory(int id, [FromBody] ProductCategoryUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCategory = _categoryService.GetCategoryById(id);
            if (existingCategory == null)
                return NotFound(new { message = $"Category with ID {id} not found" });

            existingCategory.UpdateFromRequest(request);
            _categoryService.UpdateCategory(existingCategory);

            var response = existingCategory.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the category", error = ex.Message });
        }
    }

    /// <summary>
    ///     Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        try
        {
            var existingCategory = _categoryService.GetCategoryById(id);
            if (existingCategory == null)
                return NotFound(new { message = $"Category with ID {id} not found" });

            _categoryService.DeleteCategory(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the category", error = ex.Message });
        }
    }
}