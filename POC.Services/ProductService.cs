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

    public async Task<List<Product>> GetAllProducts()
    {
        return await _productRepository.GetAllProductsAsync();
    }

    public async Task<List<Product>> GetProductsByCategory(int categoryId)
    {
        return await _productRepository.GetProductsByCategoryAsync(categoryId);
    }

    public async Task<Product?> GetProductById(int id)
    {
        return await _productRepository.GetProductByIdAsync(id);
    }

    public async Task<List<Product>> GetProductsByIds(List<int> ids)
    {
        return await _productRepository.GetProductsByIdsAsync(ids);
    }

    public async Task UpdateProduct(Product product)
    {
        await _productRepository.UpdateProductAsync(product);
    }

    public async Task UpdateProductStock(int productId, int quantity)
    {
        await _productRepository.UpdateProductStockAsync(productId, quantity);
    }

    public async Task AddProduct(Product product)
    {
        await _productRepository.AddProductAsync(product);
    }

    public async Task AddProducts(List<Product> products)
    {
        await _productRepository.AddProductsAsync(products);
    }

    public async Task DeleteProduct(int id)
    {
        await _productRepository.DeleteProductAsync(id);
    }

    public void Dispose()
    {
        _productRepository.Dispose();
    }
}