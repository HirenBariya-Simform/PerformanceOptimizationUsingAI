using System.ComponentModel.DataAnnotations;

namespace PerformanceOptimizationUsingAI.DTOs.ProductCategory;

public class ProductCategoryUpdateRequest
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category description is required")]
    [StringLength(500, ErrorMessage = "Category description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;
} 