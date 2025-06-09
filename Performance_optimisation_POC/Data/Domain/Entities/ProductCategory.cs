namespace PerformanceOptimizationUsingAI.Data.Domain.Entities;

public class ProductCategory
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation property
    public virtual ICollection<ProductCategoryMapping> ProductMappings { get; set; }
}