using POC.Data.Domain.Entities;

namespace POC.Services;

public interface IProductService
{
    List<Product> GetAllProducts();
    List<Product> GetProductsByCategory(int categoryId);
    Product GetProductById(int id);
    List<Product> GetProductsByIds(List<int> ids);
    void UpdateProduct(Product product);
    void UpdateProductStock(int productId, int quantity);
    void AddProducts(List<Product> products);
    void DeleteProduct(int id);
}