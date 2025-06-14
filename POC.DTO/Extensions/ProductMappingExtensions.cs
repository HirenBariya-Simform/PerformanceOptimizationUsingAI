using POC.Data.Domain.Entities;
using POC.DTOs.Product;

namespace POC.DTOs.Extensions;

public static class ProductMappingExtensions
{
    // Convert ProductCategory entity to ProductCategoryDto
    public static ProductCategoryDto ToCategoryDto(this Data.Domain.Entities.ProductCategory category)
    {
        return new ProductCategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name
            //Description = category.Description
        };
    }

    // Convert Entity to Response DTO
    public static ProductResponse ToResponse(this Data.Domain.Entities.Product product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Categories = product.CategoryMappings?.Select(cm => cm.Category.ToCategoryDto()).ToList() ??
                         new List<ProductCategoryDto>()
        };
    }

    // Convert Entity to List Item DTO
    public static ProductListItem ToListItem(this Data.Domain.Entities.Product product)
    {
        return new ProductListItem
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryNames = product.CategoryMappings?.Select(cm => cm.Category.Name).ToList() ?? new List<string>()
        };
    }

    // Convert Create Request to Entity
    public static Data.Domain.Entities.Product ToEntity(this ProductCreateRequest request)
    {
        var product = new Data.Domain.Entities.Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryMappings = new List<ProductCategoryMapping>()
        };

        // Create category mappings
        foreach (var categoryId in request.CategoryIds)
            product.CategoryMappings.Add(new ProductCategoryMapping
            {
                ProductId = product.ProductId, CategoryId // Will be set after product is saved
                    = categoryId
            });

        return product;
    }

    // Update Entity from Update Request
    public static void UpdateFromRequest(this Data.Domain.Entities.Product product, ProductUpdateRequest request)
    {
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        // Clear existing category mappings
        product.CategoryMappings?.Clear();

        // Add new category mappings
        if (product.CategoryMappings == null)
            product.CategoryMappings = new List<ProductCategoryMapping>();

        foreach (var categoryId in request.CategoryIds)
            product.CategoryMappings.Add(new ProductCategoryMapping
            {
                ProductId = product.ProductId,
                CategoryId = categoryId
            });
    }

    // Update stock from Stock Update Request
    public static void UpdateStockFromRequest(this Data.Domain.Entities.Product product,
        ProductStockUpdateRequest request)
    {
        product.StockQuantity = request.StockQuantity;
    }

    // Convert collection of entities to response DTOs
    public static List<ProductResponse> ToResponseList(this IEnumerable<Data.Domain.Entities.Product> products)
    {
        return products.Select(p => p.ToResponse()).ToList();
    }

    // Convert collection of entities to list item DTOs
    public static List<ProductListItem> ToListItems(this IEnumerable<Data.Domain.Entities.Product> products)
    {
        return products.Select(p => p.ToListItem()).ToList();
    }
}