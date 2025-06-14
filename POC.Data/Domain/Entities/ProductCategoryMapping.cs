namespace POC.Data.Domain.Entities;

public class ProductCategoryMapping
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual ProductCategory Category { get; set; }
}