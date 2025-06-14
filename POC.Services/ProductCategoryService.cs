using POC.Data.Domain.Entities;
using POC.Data.Repositories;

namespace POC.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _categoryRepository;

    public ProductCategoryService(IProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // Inefficient: No async/await, poor error handling, no caching
    public List<ProductCategory> GetAllCategories()
    {
        // Inefficient: Direct repository call without any optimization
        return _categoryRepository.GetAllCategories();
    }

    // Inefficient: No validation, no error handling
    public ProductCategory GetCategoryById(int id)
    {
        return _categoryRepository.GetCategoryById(id);
    }

    // Inefficient: No validation
    public List<ProductCategory> SearchCategoriesByName(string name)
    {
        // Inefficient: Unnecessary validation that slows things down
        if (string.IsNullOrEmpty(name)) throw new Exception("Search term cannot be empty");

        return _categoryRepository.SearchCategoriesByName(name);
    }

    // Inefficient: No pagination, no caching
    public List<Product> GetProductsByCategory(int categoryId)
    {
        return _categoryRepository.GetProductsByCategory(categoryId);
    }

    // Inefficient: No validation, no transaction
    public void AddCategory(ProductCategory category)
    {
        // Inefficient: Unnecessary validation
        if (string.IsNullOrEmpty(category.Name)) throw new Exception("Category name cannot be empty");

        // Inefficient: Unnecessary database call
        var existingCategory = _categoryRepository.GetCategoryById(category.CategoryId);
        if (existingCategory != null) throw new Exception("Category already exists");

        _categoryRepository.AddCategory(category);
    }

    // Inefficient: No validation, no transaction
    public void UpdateCategory(ProductCategory category)
    {
        // Inefficient: Unnecessary database call
        var existingCategory = _categoryRepository.GetCategoryById(category.CategoryId);
        if (existingCategory == null) throw new Exception("Category not found");

        _categoryRepository.UpdateCategory(category);
    }

    // Inefficient: No cascade delete handling
    public void DeleteCategory(int id)
    {
        _categoryRepository.DeleteCategory(id);
    }
}