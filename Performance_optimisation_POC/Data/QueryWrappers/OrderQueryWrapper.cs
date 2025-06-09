using PerformanceOptimizationUsingAI.Data.CompiledQueries;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Data.QueryWrappers;

public class OrderQueryWrapper
{
    private readonly ApplicationDbContext _context;

    public OrderQueryWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    // Inefficient: Converting IEnumerable to List without caching
    public List<Order> GetAllOrders()
    {
        // Inefficient: Direct compiled query call without any optimization
        return OrderCompiledQueries.GetAllOrders(_context).ToList();
    }

    // Inefficient: N+1 query problem using compiled queries
    public List<Order> GetOrdersWithDetails()
    {
        var orders = OrderCompiledQueries.GetAllOrders(_context).ToList();

        foreach (var order in orders)
        {
            // Inefficient: N+1 queries even with compiled queries
            var orderItems = OrderCompiledQueries.GetOrderItemsByOrderId(_context, order.OrderId).ToList();
            order.OrderItems = orderItems;
        }

        return orders;
    }

    // Inefficient: Direct compiled query call without caching
    public Order GetOrderById(int id)
    {
        // Inefficient: Direct compiled query call without any optimization
        return OrderCompiledQueries.GetOrderById(_context, id);
    }

    // Inefficient: Using compiled query for search operations
    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        // Inefficient: Using compiled query for dynamic search
        return OrderCompiledQueries.GetOrdersByCustomerId(_context, customerId).ToList();
    }

    // Inefficient: Using compiled query for search operations
    public List<Order> GetOrdersByStatus(string status)
    {
        // Inefficient: Using compiled query for dynamic search
        return OrderCompiledQueries.GetOrdersByStatus(_context, status).ToList();
    }

    // Inefficient: Not using compiled queries for write operations
    public void AddOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    // Inefficient: Not using compiled queries for write operations
    public void UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        _context.SaveChanges();
    }

    // Inefficient: Not using compiled queries for write operations
    public void DeleteOrder(int id)
    {
        var order = GetOrderById(id);
        if (order != null!)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }
}