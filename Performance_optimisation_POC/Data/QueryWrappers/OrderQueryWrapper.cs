using Microsoft.EntityFrameworkCore;
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
        // Inefficient: First query to get all orders
        var orders = _context.Orders.ToList();

        // Inefficient: N+1 query problem - separate query for each order's items
        foreach (var order in orders)
        {
            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToList();

            // Inefficient: N+1 query problem - separate query for each order item's product
            foreach (var orderItem in orderItems)
            {
                // Inefficient: Separate query for product
                orderItem.Product = _context.Products
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            order.OrderItems = orderItems;
        }

        return orders;
    }

    // Inefficient: Direct compiled query call without caching
    public Order GetOrderById(int id)
    {
        // Inefficient: First query to get order
        var order = _context.Orders
            .FirstOrDefault(o => o.OrderId == id);

        if (order != null)
        {
            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToList();

            // Inefficient: N+1 query problem - separate query for each order item's product
            foreach (var orderItem in orderItems)
            {
                // Inefficient: Separate query for product
                orderItem.Product = _context.Products
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            order.OrderItems = orderItems;
        }

        return order;
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