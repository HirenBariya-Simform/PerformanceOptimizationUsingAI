using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.CompiledQueries;

public static class ProductCompiledQueries
{
    public static readonly Func<ApplicationDbContext, IEnumerable<Product>> GetAllProductsWithDetails =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .AsNoTracking());

    public static readonly Func<ApplicationDbContext, int, Product> GetProductByIdWithDetails =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .AsNoTracking()
                .FirstOrDefault(p => p.ProductId == id));

    public static readonly Func<ApplicationDbContext, int, IEnumerable<Product>> GetProductsByCategoryWithDetails =
        EF.CompileQuery((ApplicationDbContext context, int categoryId) =>
            context.Products
                .Where(p => p.CategoryMappings.Any(cm => cm.CategoryId == categoryId))
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .AsNoTracking());

    public static readonly Func<ApplicationDbContext, string, IEnumerable<Product>> SearchProducts =
        EF.CompileQuery((ApplicationDbContext context, string searchTerm) =>
            context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .AsNoTracking());

    public static readonly Func<ApplicationDbContext, int, Product> GetProductByIdForStock =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Products
                .AsNoTracking()
                .FirstOrDefault(p => p.ProductId == id));
}