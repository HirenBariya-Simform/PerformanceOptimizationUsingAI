using PerformanceOptimizationUsingAI.Data;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Data.QueryWrappers;

namespace PerformanceOptimizationUsingAI.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly ProductCategoryQueryWrapper _queryWrapper;

    public ProductCategoryRepository(ApplicationDbContext context)
    {
        _queryWrapper = new ProductCategoryQueryWrapper(context);
    }

    // Inefficient: Loading all categories without pagination, no caching
    public List<ProductCategory> GetAllCategories()
    {
        // Inefficient: Direct query wrapper call without any optimization
        return _queryWrapper.GetAllCategories();
    }

    // Inefficient: No async/await
    public ProductCategory GetCategoryById(int id)
    {
        return _queryWrapper.GetCategoryById(id);
    }

    // Inefficient: No pagination or filtering
    public List<ProductCategory> SearchCategoriesByName(string name)
    {
        return _queryWrapper.SearchCategoriesByName(name);
    }

    // Inefficient: N+1 query potential
    public List<Product> GetProductsByCategory(int categoryId)
    {
        return _queryWrapper.GetProductsByCategory(categoryId);
    }

    // Inefficient: No validation, no transaction
    public void AddCategory(ProductCategory category)
    {
        _queryWrapper.AddCategory(category);
    }

    // Inefficient: No error handling, no transaction
    public void UpdateCategory(ProductCategory category)
    {
        _queryWrapper.UpdateCategory(category);
    }

    // Inefficient: No cascade delete handling
    public void DeleteCategory(int id)
    {
        _queryWrapper.DeleteCategory(id);
    }
}