using PerformanceOptimizationUsingAI.Data.CompiledQueries;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Data.QueryWrappers;

public class ProductQueryWrapper
{
    private readonly ApplicationDbContext _context;

    public ProductQueryWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    // Inefficient: Loading all products with categories in a single query, no caching
    public List<Product> GetAllProducts()
    {
        // Inefficient: Loading all related data at once without any optimization
        var products = ProductCompiledQueries.GetAllProducts(_context).ToList();
        return products;
    }

    // Inefficient: Multiple database calls in a loop
    public List<Product> GetProductsByCategory(int categoryId)
    {
        // Inefficient: Using compiled query but still inefficient approach
        var products = ProductCompiledQueries.GetProductsByCategory(_context, categoryId).ToList();

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

    // Inefficient: No proper indexing consideration
    public List<Product> SearchProducts(string searchTerm)
    {
        // Inefficient: Using compiled query for dynamic search
        return ProductCompiledQueries.SearchProducts(_context, searchTerm).ToList();
    }

    // Inefficient: Not using compiled query consistently
    private Product GetProductById(int id)
    {
        return ProductCompiledQueries.GetProductById(_context, id);
    }

    // Inefficient: No transaction, no validation
    public void UpdateProductStock(int productId, int quantity)
    {
        var product = GetProductById(productId);
        if (product != null!)
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
        var product = GetProductById(id);
        if (product != null!)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}