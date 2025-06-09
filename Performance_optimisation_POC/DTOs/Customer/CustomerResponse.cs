using PerformanceOptimizationUsingAI.DTOs.Order;

namespace PerformanceOptimizationUsingAI.DTOs.Customer;

public class CustomerResponse
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<CustomerOrderResponse> Orders { get; set; } = new();
} 