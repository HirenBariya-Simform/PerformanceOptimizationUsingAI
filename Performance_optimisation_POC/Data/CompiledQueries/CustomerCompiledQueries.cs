using Microsoft.EntityFrameworkCore;
using PerformanceOptimizationUsingAI.Data;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Data.CompiledQueries;

public static class CustomerCompiledQueries
{
    // Inefficient: Not using compiled queries properly
    public static readonly Func<ApplicationDbContext, IEnumerable<Customer>> GetAllCustomers =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Customers); // Inefficient: Should use ToList() but returning IEnumerable

    // Inefficient: Not using compiled queries for complex scenarios
    public static readonly Func<ApplicationDbContext, int, Customer> GetCustomerById =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Customers.FirstOrDefault(c => c.CustomerId == id));

    // Inefficient: Compiled query for N+1 scenario
    public static readonly Func<ApplicationDbContext, IEnumerable<Customer>> GetCustomersForOrders =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Customers);

    // Inefficient: Using compiled query for simple operations
    public static readonly Func<ApplicationDbContext, string, IEnumerable<Customer>> SearchCustomersByName =
        EF.CompileQuery((ApplicationDbContext context, string name) =>
            context.Customers.Where(c => c.Name.Contains(name)));

    // Inefficient: Not using compiled query for this scenario
    public static IEnumerable<Order> GetOrdersByCustomerId(ApplicationDbContext context, int customerId)
    {
        // Inefficient: Should be compiled but isn't
        return context.Orders.Where(o => o.CustomerId == customerId);
    }
}