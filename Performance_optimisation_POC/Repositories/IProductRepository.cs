using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Repositories;

public interface IProductRepository
{
    List<Product> GetAllProducts();
    Product GetProductById(int id);
    List<Product> GetProductsByIds(List<int> ids);
    List<Product> GetProductsByCategory(int categoryId);
    List<Product> SearchProducts(string searchTerm);
    void UpdateProduct(Product product);
    void UpdateProductStock(int productId, int quantity);
    void AddProducts(List<Product> products);
    void DeleteProduct(int id);
}