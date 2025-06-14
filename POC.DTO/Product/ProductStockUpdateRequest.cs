using System.ComponentModel.DataAnnotations;

namespace POC.DTOs.Product;

public class ProductStockUpdateRequest
{
    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }
}