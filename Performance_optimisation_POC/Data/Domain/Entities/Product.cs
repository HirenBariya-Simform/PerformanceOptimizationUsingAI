namespace PerformanceOptimizationUsingAI.Data.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<ProductCategoryMapping> CategoryMappings { get; set; }
}