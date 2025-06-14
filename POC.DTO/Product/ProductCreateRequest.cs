using System.ComponentModel.DataAnnotations;

namespace POC.DTOs.Product;

public class ProductCreateRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product description is required")]
    [StringLength(1000, ErrorMessage = "Product description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "At least one category is required")]
    [MinLength(1, ErrorMessage = "Product must belong to at least one category")]
    public List<int> CategoryIds { get; set; } = new();
}