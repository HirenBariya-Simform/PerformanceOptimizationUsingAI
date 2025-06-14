using POC.DTOs.ProductCategory;

namespace POC.DTOs.Extensions;

public static class ProductCategoryMappingExtensions
{
    // Convert Entity to Response DTO
    public static ProductCategoryResponse ToResponse(this Data.Domain.Entities.ProductCategory category)
    {
        return new ProductCategoryResponse
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description
        };
    }

    // Convert Entity to List Item DTO
    public static ProductCategoryListItem ToListItem(this Data.Domain.Entities.ProductCategory category)
    {
        return new ProductCategoryListItem
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description
        };
    }

    // Convert Create Request to Entity
    public static Data.Domain.Entities.ProductCategory ToEntity(this ProductCategoryCreateRequest request)
    {
        return new Data.Domain.Entities.ProductCategory
        {
            Name = request.Name,
            Description = request.Description
        };
    }

    // Update Entity from Update Request
    public static void UpdateFromRequest(this Data.Domain.Entities.ProductCategory category,
        ProductCategoryUpdateRequest request)
    {
        category.Name = request.Name;
        category.Description = request.Description;
    }

    // Convert collection of entities to response DTOs
    public static List<ProductCategoryResponse> ToResponseList(
        this IEnumerable<Data.Domain.Entities.ProductCategory> categories)
    {
        return categories.Select(c => c.ToResponse()).ToList();
    }

    // Convert collection of entities to list item DTOs
    public static List<ProductCategoryListItem> ToListItems(
        this IEnumerable<Data.Domain.Entities.ProductCategory> categories)
    {
        return categories.Select(c => c.ToListItem()).ToList();
    }
}