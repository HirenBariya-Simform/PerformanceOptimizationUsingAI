namespace POC.DTOs.ProductCategory;

public class ProductCategoryResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}