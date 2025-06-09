using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.DTOs.Product;

namespace PerformanceOptimizationUsingAI.Extensions;

public static class ProductMappingExtensions
{
    // Convert ProductCategory entity to ProductCategoryDto
    public static ProductCategoryDto ToCategoryDto(this ProductCategory category)
    {
        return new ProductCategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            //Description = category.Description
        };
    }

    // Convert Entity to Response DTO
    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Categories = product.CategoryMappings?.Select(cm => cm.Category.ToCategoryDto()).ToList() ?? new List<ProductCategoryDto>()
        };
    }

    // Convert Entity to List Item DTO
    public static ProductListItem ToListItem(this Product product)
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
    public static Product ToEntity(this ProductCreateRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryMappings = new List<ProductCategoryMapping>()
        };

        // Create category mappings
        foreach (var categoryId in request.CategoryIds)
        {
            product.CategoryMappings.Add(new ProductCategoryMapping
            {
                ProductId = product.ProductId, // Will be set after product is saved
                CategoryId = categoryId
            });
        }

        return product;
    }

    // Update Entity from Update Request
    public static void UpdateFromRequest(this Product product, ProductUpdateRequest request)
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
        {
            product.CategoryMappings.Add(new ProductCategoryMapping
            {
                ProductId = product.ProductId,
                CategoryId = categoryId
            });
        }
    }

    // Update stock from Stock Update Request
    public static void UpdateStockFromRequest(this Product product, ProductStockUpdateRequest request)
    {
        product.StockQuantity = request.StockQuantity;
    }

    // Convert collection of entities to response DTOs
    public static List<ProductResponse> ToResponseList(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToResponse()).ToList();
    }

    // Convert collection of entities to list item DTOs
    public static List<ProductListItem> ToListItems(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToListItem()).ToList();
    }
} 