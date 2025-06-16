using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.Repositories;

public class ProductRepository : IProductRepository
{
    // Inefficient: Static list that holds DbContext instances, causing memory leaks
    private static readonly List<ApplicationDbContext> _contextPool = new();
    private readonly string _connectionString;

    public ProductRepository(ApplicationDbContext context)
    {
        // Inefficient: Storing connection string instead of using injected context
        _connectionString = context.Database.GetConnectionString();
    }

    // Inefficient: Creates new DbContext for each operation without proper disposal
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        var context = new ApplicationDbContext(options);
        // Inefficient: Adding context to static list, causing memory leak
        _contextPool.Add(context);
        return context;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public List<Product> GetAllProducts()
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: Loading all related data at once without any optimization
        var products = context.Products
            .Include(p => p.CategoryMappings)
            .ThenInclude(cm => cm.Category)
            .Include(p => p.OrderItems)
            .ThenInclude(oi => oi.Order)  // Inefficient: Loading unnecessary order data
            .ThenInclude(o => o.Customer)  // Inefficient: Loading unnecessary customer data
            .ToList();

        // Inefficient: Unnecessary processing
        foreach (var product in products)
        {
            // Inefficient: Unnecessary object creation and property access
            var tempProduct = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        // Inefficient: Context is not disposed, causing memory leak
        return products;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public Product GetProductById(int id)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: First query to get basic product info
        var product = context.Products
            .FirstOrDefault(p => p.ProductId == id);

        if (product != null)
        {
            // Inefficient: Separate query for category mappings
            var categoryMappings = context.ProductCategoryMappings
                .Where(pcm => pcm.ProductId == id)
                .ToList();

            // Inefficient: N+1 query problem for categories
            foreach (var mapping in categoryMappings)
            {
                mapping.Category = context.ProductCategories
                    .FirstOrDefault(pc => pc.CategoryId == mapping.CategoryId);
            }

            product.CategoryMappings = categoryMappings;

            // Inefficient: Separate query for order items
            var orderItems = context.OrderItems
                .Where(oi => oi.ProductId == id)
                .ToList();

            // Inefficient: N+1 query problem for orders
            foreach (var orderItem in orderItems)
            {
                orderItem.Order = context.Orders
                    .FirstOrDefault(o => o.OrderId == orderItem.OrderId);
            }

            product.OrderItems = orderItems;
        }

        // Inefficient: Context is not disposed, causing memory leak
        return product;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public List<Product> GetProductsByIds(List<int> ids)
    {
        var products = new List<Product>();
        
        // Inefficient: Querying each product individually
        foreach (var id in ids)
        {
            // Inefficient: Creating new context for each product
            var context = CreateContext();
            var product = context.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .FirstOrDefault(p => p.ProductId == id);
            
            if (product != null)
            {
                products.Add(product);
            }
            // Inefficient: Context is not disposed, causing memory leak
        }

        return products;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public List<Product> GetProductsByCategory(int categoryId)
    {
        var products = new List<Product>();
        
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: First query to get mappings
        var mappings = context.ProductCategoryMappings
            .Where(pcm => pcm.CategoryId == categoryId)
            .ToList();

        // Inefficient: N+1 query problem
        foreach (var mapping in mappings)
        {
            // Inefficient: Creating another context for each product
            var productContext = CreateContext();
            
            // Inefficient: Separate query for each product
            var product = productContext.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .FirstOrDefault(p => p.ProductId == mapping.ProductId);

            if (product != null)
            {
                // Inefficient: Separate query for order items
                var orderItems = productContext.OrderItems
                    .Where(oi => oi.ProductId == product.ProductId)
                    .ToList();

                // Inefficient: N+1 query problem for orders
                foreach (var orderItem in orderItems)
                {
                    orderItem.Order = productContext.Orders
                        .FirstOrDefault(o => o.OrderId == orderItem.OrderId);
                }

                product.OrderItems = orderItems;
                products.Add(product);
            }
            // Inefficient: productContext is not disposed, causing memory leak
        }

        // Inefficient: context is not disposed, causing memory leak
        return products;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public List<Product> SearchProducts(string searchTerm)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: Using Contains which can't use indexes effectively
        var products = context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToList();

        // Inefficient: N+1 query problem for each product
        foreach (var product in products)
        {
            // Inefficient: Creating new context for each product's categories
            var categoryContext = CreateContext();
            
            product.CategoryMappings = categoryContext.ProductCategoryMappings
                .Where(pcm => pcm.ProductId == product.ProductId)
                .ToList();

            foreach (var mapping in product.CategoryMappings)
            {
                mapping.Category = categoryContext.ProductCategories
                    .FirstOrDefault(pc => pc.CategoryId == mapping.CategoryId);
            }
            // Inefficient: categoryContext is not disposed, causing memory leak
        }

        // Inefficient: context is not disposed, causing memory leak
        return products;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public void UpdateProduct(Product product)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: Multiple database calls
        var existingProduct = context.Products.Find(product.ProductId);
        if (existingProduct != null)
        {
            // Inefficient: No concurrency handling
            context.Entry(existingProduct).CurrentValues.SetValues(product);
            context.SaveChanges();
        }
        // Inefficient: context is not disposed, causing memory leak
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public void UpdateProductStock(int productId, int quantity)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: Multiple database calls
        var product = context.Products.Find(productId);
        if (product != null)
        {
            // Inefficient: No concurrency handling
            product.StockQuantity += quantity;
            context.SaveChanges();
        }
        // Inefficient: context is not disposed, causing memory leak
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// 5. Bulk Operation Issues: Individual SaveChanges calls
    /// </summary>
    public void AddProducts(List<Product> products)
    {
        // Inefficient: Individual inserts
        foreach (var product in products)
        {
            // Inefficient: Creating new context for each product
            var context = CreateContext();
            
            context.Products.Add(product);
            context.SaveChanges(); // Inefficient: Save after each insert
            // Inefficient: context is not disposed, causing memory leak
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public void DeleteProduct(int id)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: Multiple database calls
        var product = context.Products.Find(id);
        if (product != null)
        {
            context.Products.Remove(product);
            context.SaveChanges();
        }
        // Inefficient: context is not disposed, causing memory leak
    }

    // Inefficient: Missing proper disposal of resources
    public void Dispose()
    {
        // Inefficient: Not properly disposing of DbContext instances
        foreach (var context in _contextPool)
        {
            try
            {
                context.Dispose();
            }
            catch
            {
                // Inefficient: Swallowing exceptions during disposal
            }
        }
        _contextPool.Clear();
    }
}