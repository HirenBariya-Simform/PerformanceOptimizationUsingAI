using POC.Data.Domain.Entities;

namespace POC.Services;

public interface IProductService : IDisposable
{
    Task<List<Product>> GetAllProducts();
    Task<Product?> GetProductById(int id);
    Task<List<Product>> GetProductsByIds(List<int> ids);
    Task<List<Product>> GetProductsByCategory(int categoryId);
    Task UpdateProduct(Product product);
    Task UpdateProductStock(int productId, int quantity);
    Task AddProduct(Product product);
    Task AddProducts(List<Product> products);
    Task DeleteProduct(int id);
}