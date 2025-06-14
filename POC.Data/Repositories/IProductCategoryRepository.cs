using POC.Data.Domain.Entities;

namespace POC.Data.Repositories;

public interface IProductCategoryRepository
{
    List<ProductCategory> GetAllCategories();
    ProductCategory GetCategoryById(int id);
    List<ProductCategory> SearchCategoriesByName(string name);
    List<Product> GetProductsByCategory(int categoryId);
    void AddCategory(ProductCategory category);
    void UpdateCategory(ProductCategory category);
    void DeleteCategory(int id);
}