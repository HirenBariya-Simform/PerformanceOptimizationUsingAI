using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;
using POC.Data.QueryWrappers;

namespace POC.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    // Inefficient: Static list that holds DbContext instances, causing memory leaks
    private static readonly List<ApplicationDbContext> _contextPool = new();
    private readonly string _connectionString;

    public CustomerRepository(ApplicationDbContext context)
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
    public List<Customer> GetAllCustomers()
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: Loading all customers without pagination, no caching
        var customers = context.Customers
            .Include(c => c.Orders)
            .ThenInclude(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

        // Inefficient: Context is not disposed, causing memory leak
        return customers;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// 5. N+1 Query Problem: Separate queries for each customer's orders
    /// </summary>
    public List<Customer> GetCustomersWithOrders()
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: First query to get all customers
        var customers = context.Customers.ToList();

        // Inefficient: N+1 query problem
        foreach (var customer in customers)
        {
            // Inefficient: Creating new context for customer's orders
            var orderContext = CreateContext();
            
            customer.Orders = orderContext.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .ToList();

            // Inefficient: N+1 query problem for order items
            foreach (var order in customer.Orders)
            {
                // Inefficient: Creating new context for each order's items
                var itemContext = CreateContext();
                
                order.OrderItems = itemContext.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();

                // Inefficient: N+1 query problem for products
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product = itemContext.Products
                        .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
                }
                // Inefficient: itemContext is not disposed, causing memory leak
            }
            // Inefficient: orderContext is not disposed, causing memory leak
        }

        // Inefficient: context is not disposed, causing memory leak
        return customers;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public Customer GetCustomerById(int id)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: First query to get basic customer info
        var customer = context.Customers
            .FirstOrDefault(c => c.CustomerId == id);

        if (customer != null)
        {
            // Inefficient: Creating new context for customer's orders
            var orderContext = CreateContext();
            
            customer.Orders = orderContext.Orders
                .Where(o => o.CustomerId == id)
                .ToList();

            // Inefficient: N+1 query problem for order items
            foreach (var order in customer.Orders)
            {
                // Inefficient: Creating new context for each order's items
                var itemContext = CreateContext();
                
                order.OrderItems = itemContext.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();

                // Inefficient: N+1 query problem for products
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product = itemContext.Products
                        .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
                }
                // Inefficient: itemContext is not disposed, causing memory leak
            }
            // Inefficient: orderContext is not disposed, causing memory leak
        }

        // Inefficient: context is not disposed, causing memory leak
        return customer;
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public void AddCustomer(Customer customer)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: No validation, no transaction
        context.Customers.Add(customer);
        context.SaveChanges();

        // Inefficient: Unnecessary validation query
        var savedCustomer = context.Customers
            .Include(c => c.Orders)
            .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

        // Inefficient: context is not disposed, causing memory leak
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Object Disposal: Creates new DbContext without proper disposal
    /// 2. Memory Leaks: DbContext instances stored in static collection
    /// 3. Resource Management: No using statement for DbContext
    /// 4. Connection Pool Issues: New connection for each request
    /// </summary>
    public void UpdateCustomer(Customer customer)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: No error handling, no transaction
        var existingCustomer = context.Customers.Find(customer.CustomerId);
        if (existingCustomer != null)
        {
            context.Entry(existingCustomer).CurrentValues.SetValues(customer);
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
    public void DeleteCustomer(int id)
    {
        // Inefficient: Creating new context without using statement
        var context = CreateContext();
        
        // Inefficient: No cascade delete handling
        var customer = context.Customers.Find(id);
        if (customer != null)
        {
            // Inefficient: Creating new context for orders
            var orderContext = CreateContext();
            
            // Inefficient: Manual cascade delete without proper handling
            var orders = orderContext.Orders
                .Where(o => o.CustomerId == id)
                .ToList();

            foreach (var order in orders)
            {
                // Inefficient: Creating new context for order items
                var itemContext = CreateContext();
                
                var orderItems = itemContext.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();

                foreach (var orderItem in orderItems)
                {
                    itemContext.OrderItems.Remove(orderItem);
                    itemContext.SaveChanges(); // Inefficient: Save after each item
                }

                orderContext.Orders.Remove(order);
                orderContext.SaveChanges(); // Inefficient: Save after each order
                // Inefficient: itemContext is not disposed, causing memory leak
            }

            context.Customers.Remove(customer);
            context.SaveChanges();
            // Inefficient: orderContext is not disposed, causing memory leak
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