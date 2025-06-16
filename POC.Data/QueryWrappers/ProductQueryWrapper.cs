using POC.Data.CompiledQueries;
using POC.Data.Domain.Entities;

namespace POC.Data.QueryWrappers;

public class ProductQueryWrapper
{
    private readonly ApplicationDbContext _context;

    public ProductQueryWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Product> GetAllProducts()
    {
        return ProductCompiledQueries.GetAllProductsWithDetails(_context).ToList();
    }

    public List<Product> GetProductsByCategory(int categoryId)
    {
        return ProductCompiledQueries.GetProductsByCategoryWithDetails(_context, categoryId).ToList();
    }

    public List<Product> SearchProducts(string searchTerm)
    {
        return ProductCompiledQueries.SearchProducts(_context, searchTerm).ToList();
    }

    private Product GetProductById(int id)
    {
        return ProductCompiledQueries.GetProductByIdWithDetails(_context, id);
    }

    public void UpdateProductStock(int productId, int quantity)
    {
        var product = ProductCompiledQueries.GetProductByIdForStock(_context, productId);
        if (product != null)
        {
            product.StockQuantity += quantity;
            _context.SaveChanges();
        }
    }

    public void AddProducts(List<Product> products)
    {
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    public void DeleteProduct(int id)
    {
        var product = ProductCompiledQueries.GetProductByIdForStock(_context, id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}