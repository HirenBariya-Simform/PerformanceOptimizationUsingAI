using Microsoft.EntityFrameworkCore;
using PerformanceOptimizationUsingAI.Data;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Inefficient: Loading all products with categories in a single query, no caching
    public List<Product> GetAllProducts()
    {
        // Inefficient: Loading all related data at once without any optimization
        var products = _context.Products
            .Include(p => p.CategoryMappings)
            .ThenInclude(cm => cm.Category)
            .Include(p => p.OrderItems)
            .ToList();

        return products;
    }

    // Inefficient: Multiple database calls in a loop
    public List<Product> GetProductsByCategory(int categoryId)
    {
        var products = new List<Product>();
        var mappings = _context.ProductCategoryMappings
            .Where(pcm => pcm.CategoryId == categoryId)
            .ToList();

        foreach (var mapping in mappings)
        {
            // Inefficient: N+1 query problem
            var product = _context.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .FirstOrDefault(p => p.ProductId == mapping.ProductId);

            if (product != null) products.Add(product);
        }

        return products;
    }

    // Inefficient: No proper indexing consideration
    public List<Product> SearchProducts(string searchTerm)
    {
        // Inefficient: Using Contains which can't use indexes effectively
        return _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToList();
    }

    // Inefficient: No transaction, no validation
    public void UpdateProductStock(int productId, int quantity)
    {
        var product = _context.Products.Find(productId);
        if (product != null)
        {
            // Inefficient: No concurrency handling
            product.StockQuantity += quantity;
            _context.SaveChanges();
        }
    }

    // Inefficient: No bulk insert
    public void AddProducts(List<Product> products)
    {
        foreach (var product in products) _context.Products.Add(product);
        _context.SaveChanges();
    }

    // Inefficient: No soft delete
    public void DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}