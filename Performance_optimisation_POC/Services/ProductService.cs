using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Repositories;

namespace PerformanceOptimizationUsingAI.Services;

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

        // Inefficient: Unnecessary processing
        foreach (var product in products)
        {
            // Inefficient: Unnecessary object creation
            var tempProduct = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        return products;
    }

    // Inefficient: No proper error handling, no validation, no caching
    public Product GetProductById(int id)
    {
        // Inefficient: Always hitting repository/database
        var product = _productRepository.GetAllProducts()
            .FirstOrDefault(p => p.ProductId == id);

        return product;
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