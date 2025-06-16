using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.CompiledQueries;

public static class CustomerCompiledQueries
{
    // Inefficient: Not using compiled queries properly, loading unnecessary data
    public static readonly Func<ApplicationDbContext, IEnumerable<Customer>> GetAllCustomers =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Customers
                .Include(c => c.Orders) // Inefficient: Loading unnecessary order data
                    .ThenInclude(o => o.OrderItems) // Inefficient: Loading unnecessary order items
                        .ThenInclude(oi => oi.Product)); // Inefficient: Loading unnecessary product data

    // Inefficient: Loading all related data eagerly without filtering
    public static readonly Func<ApplicationDbContext, IEnumerable<Customer>> GetAllCustomersWithOrders =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.CategoryMappings) // Inefficient: Loading unnecessary category data
                                .ThenInclude(cm => cm.Category)); // Inefficient: Loading more unnecessary data

    // Inefficient: Loading too much related data for a single customer
    public static readonly Func<ApplicationDbContext, int, Customer> GetCustomerByIdWithOrders =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.CategoryMappings) // Inefficient: Loading unnecessary mappings
                                .ThenInclude(cm => cm.Category) // Inefficient: Loading unnecessary categories
                .FirstOrDefault(c => c.CustomerId == id)); // Inefficient: Not using AsNoTracking

    // Inefficient: Not using AsNoTracking for read-only operation
    public static readonly Func<ApplicationDbContext, int, Customer> GetCustomerById =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Customers
                .Include(c => c.Orders) // Inefficient: Loading unnecessary orders
                .FirstOrDefault(c => c.CustomerId == id));

    // Inefficient: Not using compiled query for search operations
    public static IEnumerable<Customer> SearchCustomersByName(ApplicationDbContext context, string name)
    {
        // Inefficient: Should be compiled but isn't
        return context.Customers
            .Include(c => c.Orders) // Inefficient: Loading unnecessary orders for search
                .ThenInclude(o => o.OrderItems) // Inefficient: Loading unnecessary items
            .Where(c => c.Name.Contains(name));
    }

    // Inefficient: Loading too much related data and not using AsNoTracking
    public static readonly Func<ApplicationDbContext, int, IEnumerable<Order>> GetOrdersByCustomerIdWithDetails =
        EF.CompileQuery((ApplicationDbContext context, int customerId) =>
            context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.CategoryMappings) // Inefficient: Loading unnecessary category mappings
                            .ThenInclude(cm => cm.Category)); // Inefficient: Loading unnecessary categories
}