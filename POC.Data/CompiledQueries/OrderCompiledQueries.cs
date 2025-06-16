using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data.CompiledQueries;

public static class OrderCompiledQueries
{
    // Optimized basic order list query with minimal includes
    public static readonly Func<ApplicationDbContext, IEnumerable<Order>> GetAllOrders =
        EF.CompileQuery((ApplicationDbContext context) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .AsNoTracking());

    // Optimized: Single database query with filtered loading
    public static readonly Func<ApplicationDbContext, int, Order?> GetOrderById =
        EF.CompileQuery((ApplicationDbContext context, int id) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems.Where(oi => oi.Quantity > 0))
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefault(o => o.OrderId == id));

    // Optimized: Efficient query with filtered loading
    public static readonly Func<ApplicationDbContext, int, IEnumerable<Order>> GetOrdersByCustomerId =
        EF.CompileQuery((ApplicationDbContext context, int customerId) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems.Where(oi => oi.Quantity > 0))
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId == customerId)
                .AsNoTracking());

    // Optimized: Status-based filtering with selective loading
    public static readonly Func<ApplicationDbContext, string, IEnumerable<Order>> GetOrdersByStatus =
        EF.CompileQuery((ApplicationDbContext context, string status) =>
            context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems.Where(oi => oi.Quantity > 0))
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == status)
                .AsNoTracking());

    // Optimized: Specific items query with minimal loading
    public static readonly Func<ApplicationDbContext, int, IEnumerable<OrderItem>> GetOrderItems =
        EF.CompileQuery((ApplicationDbContext context, int orderId) =>
            context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId && oi.Quantity > 0)
                .AsNoTracking());

    // Optimized: Product stock query with efficient projection
    public static readonly Func<ApplicationDbContext, int, int> GetProductStock =
        EF.CompileQuery((ApplicationDbContext context, int productId) =>
            context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => p.StockQuantity)
                .FirstOrDefault());
}