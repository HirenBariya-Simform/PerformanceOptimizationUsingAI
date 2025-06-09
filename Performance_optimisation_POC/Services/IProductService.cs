using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Services;

public interface IProductService
{
    List<Product> GetAllProducts();
    List<Product> GetProductsByCategory(int categoryId);
    Product GetProductById(int id);
    void UpdateProductStock(int productId, int quantity);
    void AddProducts(List<Product> products);
    void DeleteProduct(int id);
}