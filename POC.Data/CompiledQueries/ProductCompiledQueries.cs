using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.CompiledQueries;

public static class ProductCompiledQueries
{
    // Inefficient: Not using compiled queries properly
    public static readonly Func<ApplicationDbContext, IEnumerable<Product>> GetAllProducts =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .Include(p => p.OrderItems)
                .AsNoTracking()); // Inefficient: Loading all related data at once

    // Inefficient: Compiled query with complex joins
    public static readonly Func<ApplicationDbContext, int, IEnumerable<Product>> GetProductsByCategory =
        EF.CompileQuery((ApplicationDbContext context, int categoryId) =>
            context.Products
                .Where(p => p.CategoryMappings.Any(cm => cm.CategoryId == categoryId))
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .AsNoTracking());

    // Inefficient: Using compiled query for search operations
    public static readonly Func<ApplicationDbContext, string, IEnumerable<Product>> SearchProducts =
        EF.CompileQuery((ApplicationDbContext context, string searchTerm) =>
            context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .AsNoTracking());

    // Inefficient: Using compiled query for stock operations
    public static readonly Func<ApplicationDbContext, int, int> GetProductStock =
        EF.CompileQuery((ApplicationDbContext context, int productId) =>
            context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => p.StockQuantity)
                .FirstOrDefault());

    // Inefficient: Not using compiled query for this operation
    public static Product GetProductById(ApplicationDbContext context, int id)
    {
        // Inefficient: Should be compiled but isn't
        return context.Products
            .Include(p => p.CategoryMappings)
            .ThenInclude(cm => cm.Category)
            .FirstOrDefault(p => p.ProductId == id);
    }
}