using Microsoft.EntityFrameworkCore;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Data.CompiledQueries;

public static class ProductCategoryCompiledQueries
{
    // Inefficient: Not using compiled queries properly
    public static readonly Func<ApplicationDbContext, IEnumerable<ProductCategory>> GetAllCategories =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.ProductCategories
                .Include(pc => pc.ProductMappings)
                .ThenInclude(pm => pm.Product)
                .AsNoTracking()); // Inefficient: Loading all related data at once

    // Inefficient: Not using compiled query for simple operations
    public static readonly Func<ApplicationDbContext, int, ProductCategory> GetCategoryById =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.ProductCategories.FirstOrDefault(pc => pc.CategoryId == id));

    // Inefficient: Using compiled query for search operations
    public static readonly Func<ApplicationDbContext, string, IEnumerable<ProductCategory>> SearchCategoriesByName =
        EF.CompileQuery((ApplicationDbContext context, string name) =>
            context.ProductCategories.Where(pc => pc.Name.Contains(name)).AsNoTracking());

    // Inefficient: Not using compiled query for this operation
    public static IEnumerable<Product> GetProductsByCategoryId(ApplicationDbContext context, int categoryId)
    {
        // Inefficient: Should be compiled but isn't
        return context.ProductCategoryMappings
            .Where(pcm => pcm.CategoryId == categoryId)
            .Select(pcm => pcm.Product)
            .AsNoTracking();
    }
}