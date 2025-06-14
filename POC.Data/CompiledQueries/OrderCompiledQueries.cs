using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.CompiledQueries;

public static class OrderCompiledQueries
{
    // Inefficient: Not using compiled queries properly
    public static readonly Func<ApplicationDbContext, IEnumerable<Order>> GetAllOrders =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()); // Inefficient: Loading all related data at once

    // Inefficient: Not using compiled query for simple operations
    public static readonly Func<ApplicationDbContext, int, Order> GetOrderById =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Orders.FirstOrDefault(o => o.OrderId == id));

    // Inefficient: Compiled query for N+1 scenario
    public static readonly Func<ApplicationDbContext, int, IEnumerable<Order>> GetOrdersByCustomerId =
        EF.CompileQuery((ApplicationDbContext context, int customerId) =>
            context.Orders.Where(o => o.CustomerId == customerId).AsNoTracking());

    // Inefficient: Using compiled query for search operations
    public static readonly Func<ApplicationDbContext, string, IEnumerable<Order>> GetOrdersByStatus =
        EF.CompileQuery((ApplicationDbContext context, string status) =>
            context.Orders.Where(o => o.Status == status).AsNoTracking());

    // Inefficient: Not using compiled query for this operation
    public static IEnumerable<OrderItem> GetOrderItemsByOrderId(ApplicationDbContext context, int orderId)
    {
        // Inefficient: Should be compiled but isn't
        return context.OrderItems.Where(oi => oi.OrderId == orderId).AsNoTracking();
    }
}