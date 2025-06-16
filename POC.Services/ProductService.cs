using POC.Data.Domain.Entities;
using POC.Data.Repositories;

namespace POC.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // Inefficient: No pagination, no caching
    public List<Product> GetAllProducts()
    {
        // Inefficient: Direct repository call without any optimization
        return _productRepository.GetAllProducts();
    }

    // Inefficient: Multiple database calls, no caching
    public List<Product> GetProductsByCategory(int categoryId)
    {
        var products = _productRepository.GetProductsByCategory(categoryId);
        return products;
    }

    // Get product by ID with categories included
    public Product GetProductById(int id)
    {
        return _productRepository.GetProductById(id);
    }

    // Get multiple products by IDs with categories included
    public List<Product> GetProductsByIds(List<int> ids)
    {
        return _productRepository.GetProductsByIds(ids);
    }

    // Update product with category mappings
    public void UpdateProduct(Product product)
    {
        _productRepository.UpdateProduct(product);
    }

    // Inefficient: No validation, no transaction
    public void UpdateProductStock(int productId, int quantity)
    {
        // Inefficient: Unnecessary database call
        var product = GetProductById(productId);
        if (product == null) throw new Exception("Product not found");

        // Inefficient: No stock validation
        _productRepository.UpdateProductStock(productId, quantity);
    }

    // Inefficient: No bulk operation optimization
    public void AddProducts(List<Product> products)
    {
        // Inefficient: Unnecessary validation
        foreach (var product in products)
            if (string.IsNullOrEmpty(product.Name))
                throw new Exception($"Product name cannot be empty for product {product.ProductId}");

        _productRepository.AddProducts(products);
    }

    // Inefficient: No proper cleanup
    public void DeleteProduct(int id)
    {
        _productRepository.DeleteProduct(id);
    }
}