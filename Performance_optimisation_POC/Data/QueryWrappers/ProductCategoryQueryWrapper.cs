using PerformanceOptimizationUsingAI.Data.CompiledQueries;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Data.QueryWrappers;

public class ProductCategoryQueryWrapper
{
    private readonly ApplicationDbContext _context;

    public ProductCategoryQueryWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    // Inefficient: Converting IEnumerable to List without caching
    public List<ProductCategory> GetAllCategories()
    {
        // Inefficient: Direct compiled query call without any optimization
        return ProductCategoryCompiledQueries.GetAllCategories(_context).ToList();
    }

    // Inefficient: Direct compiled query call without caching
    public ProductCategory GetCategoryById(int id)
    {
        // Inefficient: Direct compiled query call without any optimization
        return ProductCategoryCompiledQueries.GetCategoryById(_context, id);
    }

    // Inefficient: Using compiled query for search operations
    public List<ProductCategory> SearchCategoriesByName(string name)
    {
        // Inefficient: Using compiled query for dynamic search
        return ProductCategoryCompiledQueries.SearchCategoriesByName(_context, name).ToList();
    }

    // Inefficient: N+1 query problem
    public List<Product> GetProductsByCategory(int categoryId)
    {
        return ProductCategoryCompiledQueries.GetProductsByCategoryId(_context, categoryId).ToList();
    }

    // Inefficient: Not using compiled queries for write operations
    public void AddCategory(ProductCategory category)
    {
        _context.ProductCategories.Add(category);
        _context.SaveChanges();
    }

    // Inefficient: Not using compiled queries for write operations
    public void UpdateCategory(ProductCategory category)
    {
        _context.ProductCategories.Update(category);
        _context.SaveChanges();
    }

    // Inefficient: Not using compiled queries for write operations
    public void DeleteCategory(int id)
    {
        var category = GetCategoryById(id);
        if (category != null!)
        {
            _context.ProductCategories.Remove(category);
            _context.SaveChanges();
        }
    }
}