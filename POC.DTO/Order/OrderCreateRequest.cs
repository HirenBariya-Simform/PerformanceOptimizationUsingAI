using System.ComponentModel.DataAnnotations;

namespace POC.DTOs.Order;

public class OrderCreateRequest
{
    [Required(ErrorMessage = "Customer ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be greater than 0")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Order items are required")]
    [MinLength(1, ErrorMessage = "At least one order item is required")]
    public List<OrderItemDto> OrderItems { get; set; } = new();

    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = "Pending";
}