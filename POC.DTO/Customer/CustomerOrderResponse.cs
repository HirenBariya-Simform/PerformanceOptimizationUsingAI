using POC.DTOs.Order;

namespace POC.DTOs.Customer;

public class CustomerOrderResponse
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponse> OrderItems { get; set; } = new();
}