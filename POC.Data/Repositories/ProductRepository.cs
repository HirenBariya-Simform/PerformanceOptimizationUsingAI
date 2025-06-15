using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Inefficient: Loading all products with categories in a single query, no caching
    public List<Product> GetAllProducts()
    {
        // Inefficient: Loading all related data at once without any optimization
        var products = _context.Products
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

        return products;
    }

    // Inefficient: Multiple database calls for a single product
    public Product GetProductById(int id)
    {
        // Inefficient: First query to get basic product info
        var product = _context.Products
            .FirstOrDefault(p => p.ProductId == id);

        if (product != null)
        {
            // Inefficient: Separate query for category mappings
            var categoryMappings = _context.ProductCategoryMappings
                .Where(pcm => pcm.ProductId == id)
                .ToList();

            // Inefficient: N+1 query problem for categories
            foreach (var mapping in categoryMappings)
            {
                mapping.Category = _context.ProductCategories
                    .FirstOrDefault(pc => pc.CategoryId == mapping.CategoryId);
            }

            product.CategoryMappings = categoryMappings;

            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.ProductId == id)
                .ToList();

            // Inefficient: N+1 query problem for orders
            foreach (var orderItem in orderItems)
            {
                orderItem.Order = _context.Orders
                    .FirstOrDefault(o => o.OrderId == orderItem.OrderId);
            }

            product.OrderItems = orderItems;
        }

        return product;
    }

    // Inefficient: Multiple database calls for multiple products
    public List<Product> GetProductsByIds(List<int> ids)
    {
        var products = new List<Product>();
        
        // Inefficient: Querying each product individually
        foreach (var id in ids)
        {
            var product = GetProductById(id);
            if (product != null)
            {
                products.Add(product);
            }
        }

        return products;
    }

    // Inefficient: Multiple database calls in a loop
    public List<Product> GetProductsByCategory(int categoryId)
    {
        var products = new List<Product>();
        
        // Inefficient: First query to get mappings
        var mappings = _context.ProductCategoryMappings
            .Where(pcm => pcm.CategoryId == categoryId)
            .ToList();

        // Inefficient: N+1 query problem
        foreach (var mapping in mappings)
        {
            // Inefficient: Separate query for each product
            var product = _context.Products
                .Include(p => p.CategoryMappings)
                .ThenInclude(cm => cm.Category)
                .FirstOrDefault(p => p.ProductId == mapping.ProductId);

            if (product != null)
            {
                // Inefficient: Separate query for order items
                var orderItems = _context.OrderItems
                    .Where(oi => oi.ProductId == product.ProductId)
                    .ToList();

                // Inefficient: N+1 query problem for orders
                foreach (var orderItem in orderItems)
                {
                    orderItem.Order = _context.Orders
                        .FirstOrDefault(o => o.OrderId == orderItem.OrderId);
                }

                product.OrderItems = orderItems;
                products.Add(product);
            }
        }

        return products;
    }

    // Inefficient: No proper indexing consideration
    public List<Product> SearchProducts(string searchTerm)
    {
        // Inefficient: Using Contains which can't use indexes effectively
        var products = _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToList();

        // Inefficient: N+1 query problem for each product
        foreach (var product in products)
        {
            product.CategoryMappings = _context.ProductCategoryMappings
                .Where(pcm => pcm.ProductId == product.ProductId)
                .ToList();

            foreach (var mapping in product.CategoryMappings)
            {
                mapping.Category = _context.ProductCategories
                    .FirstOrDefault(pc => pc.CategoryId == mapping.CategoryId);
            }
        }

        return products;
    }

    // Inefficient: No transaction, no validation
    public void UpdateProduct(Product product)
    {
        // Inefficient: Multiple database calls
        var existingProduct = _context.Products.Find(product.ProductId);
        if (existingProduct != null)
        {
            // Inefficient: No concurrency handling
            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            _context.SaveChanges();
        }
    }

    // Inefficient: No transaction, no validation
    public void UpdateProductStock(int productId, int quantity)
    {
        // Inefficient: Multiple database calls
        var product = _context.Products.Find(productId);
        if (product != null)
        {
            // Inefficient: No concurrency handling
            product.StockQuantity += quantity;
            _context.SaveChanges();
        }
    }

    // Inefficient: No bulk insert
    public void AddProducts(List<Product> products)
    {
        // Inefficient: Individual inserts
        foreach (var product in products)
        {
            _context.Products.Add(product);
            _context.SaveChanges(); // Inefficient: Save after each insert
        }
    }

    // Inefficient: No soft delete
    public void DeleteProduct(int id)
    {
        // Inefficient: Multiple database calls
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}