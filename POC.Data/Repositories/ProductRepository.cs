using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Product> GetQueryable()
    {
        return _context.Products
            .Include(p => p.CategoryMappings)
            .ThenInclude(cm => cm.Category)
            .AsNoTracking();
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await GetQueryable().ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await GetQueryable()
            .Where(p => p.ProductId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
    {
        if (ids == null || !ids.Any())
            return new List<Product>();

        return await GetQueryable()
            .Where(p => ids.Contains(p.ProductId))
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await GetQueryable()
            .Where(p => p.CategoryMappings.Any(cm => cm.CategoryId == categoryId))
            .ToListAsync();
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Product>();

        return await GetQueryable()
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

        if (existingProduct == null)
            throw new KeyNotFoundException($"Product with ID {product.ProductId} not found");

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        await SaveChangesAsync();
    }

    public async Task UpdateProductStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        product.StockQuantity += quantity;
        await SaveChangesAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        await AddAsync(product);
    }

    public async Task AddProductsAsync(List<Product> products)
    {
        await AddRangeAsync(products);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            await DeleteAsync(product);
        }
    }
}