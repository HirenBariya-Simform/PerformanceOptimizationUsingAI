using POC.Data.Domain.Entities;

namespace POC.Data.Repositories;

public interface IProductRepository : IDisposable
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<List<Product>> GetProductsByIdsAsync(List<int> ids);
    Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<List<Product>> SearchProductsAsync(string searchTerm);
    Task UpdateProductAsync(Product product);
    Task UpdateProductStockAsync(int productId, int quantity);
    Task AddProductAsync(Product product);
    Task AddProductsAsync(List<Product> products);
    Task DeleteProductAsync(int id);
}