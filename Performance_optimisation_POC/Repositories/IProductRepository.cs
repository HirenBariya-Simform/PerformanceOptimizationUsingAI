using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Repositories;

public interface IProductRepository
{
    List<Product> GetAllProducts();
    List<Product> GetProductsByCategory(int categoryId);
    List<Product> SearchProducts(string searchTerm);
    void UpdateProductStock(int productId, int quantity);
    void AddProducts(List<Product> products);
    void DeleteProduct(int id);
}