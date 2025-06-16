using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.CompiledQueries;

public static class OrderCompiledQueries
{
    public static readonly Func<ApplicationDbContext, IEnumerable<Order>> GetAllOrders =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking());

    public static readonly Func<ApplicationDbContext, int, Order> GetOrderByIdWithDetails =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefault(o => o.OrderId == id));

    public static readonly Func<ApplicationDbContext, int, IEnumerable<Order>> GetOrdersByCustomerIdWithDetails =
        EF.CompileQuery((ApplicationDbContext context, int customerId) =>
            context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking());

    public static readonly Func<ApplicationDbContext, string, IEnumerable<Order>> GetOrdersByStatusWithDetails =
        EF.CompileQuery((ApplicationDbContext context, string status) =>
            context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking());

    public static readonly Func<ApplicationDbContext, int, IEnumerable<OrderItem>> GetOrderItemsByOrderId =
        EF.CompileQuery((ApplicationDbContext context, int orderId) =>
            context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .AsNoTracking());
}