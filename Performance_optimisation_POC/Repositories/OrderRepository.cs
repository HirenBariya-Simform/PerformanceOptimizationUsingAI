using PerformanceOptimizationUsingAI.Data;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Data.QueryWrappers;

namespace PerformanceOptimizationUsingAI.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderQueryWrapper _queryWrapper;

    public OrderRepository(ApplicationDbContext context)
    {
        _queryWrapper = new OrderQueryWrapper(context);
    }

    // Inefficient: Loading all orders without pagination, no caching
    public List<Order> GetAllOrders()
    {
        // Inefficient: Direct query wrapper call without any optimization
        return _queryWrapper.GetAllOrders();
    }

    // Inefficient: N+1 query problem
    public List<Order> GetOrdersWithDetails()
    {
        return _queryWrapper.GetOrdersWithDetails();
    }

    // Inefficient: No async/await
    public Order GetOrderById(int id)
    {
        return _queryWrapper.GetOrderById(id);
    }

    // Inefficient: No pagination or filtering
    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        return _queryWrapper.GetOrdersByCustomerId(customerId);
    }

    // Inefficient: No pagination or filtering
    public List<Order> GetOrdersByStatus(string status)
    {
        return _queryWrapper.GetOrdersByStatus(status);
    }

    // Inefficient: No validation, no transaction
    public void AddOrder(Order order)
    {
        _queryWrapper.AddOrder(order);
    }

    // Inefficient: No error handling, no transaction
    public void UpdateOrder(Order order)
    {
        _queryWrapper.UpdateOrder(order);
    }

    // Inefficient: No cascade delete handling
    public void DeleteOrder(int id)
    {
        _queryWrapper.DeleteOrder(id);
    }
}