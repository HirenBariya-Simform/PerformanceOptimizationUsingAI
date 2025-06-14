using POC.Data.Domain.Entities;
using POC.Data.Repositories;

namespace POC.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    // Inefficient: No async/await, poor error handling, no caching
    public List<Order> GetAllOrders()
    {
        // Inefficient: Direct repository call without any optimization
        return _orderRepository.GetAllOrders();
    }

    // Inefficient: N+1 queries, no async/await
    public List<Order> GetOrdersWithDetails()
    {
        return _orderRepository.GetOrdersWithDetails();
    }

    // Inefficient: No validation, no error handling
    public Order GetOrderById(int id)
    {
        return _orderRepository.GetOrderById(id);
    }

    // Inefficient: No pagination, no caching
    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        // Inefficient: Unnecessary database call to validate customer
        var orders = _orderRepository.GetOrdersByCustomerId(customerId);

        // Inefficient: Unnecessary processing
        foreach (var order in orders)
            // Inefficient: Unnecessary validation on each order
            if (order.TotalAmount <= 0)
                throw new Exception($"Invalid order total for order {order.OrderId}");

        return orders;
    }

    // Inefficient: No validation
    public List<Order> GetOrdersByStatus(string status)
    {
        return _orderRepository.GetOrdersByStatus(status);
    }

    // Inefficient: No validation, no transaction
    public void AddOrder(Order order)
    {
        // Inefficient: Unnecessary validation
        if (order.TotalAmount <= 0) throw new Exception("Order total must be greater than zero");

        // Inefficient: Not setting order date
        order.OrderDate = DateTime.Now;

        _orderRepository.AddOrder(order);
    }

    // Inefficient: No validation, no transaction
    public void UpdateOrder(Order order)
    {
        // Inefficient: Unnecessary database call
        var existingOrder = _orderRepository.GetOrderById(order.OrderId);
        if (existingOrder == null) throw new Exception("Order not found");

        _orderRepository.UpdateOrder(order);
    }

    // Inefficient: No cascade delete handling
    public void DeleteOrder(int id)
    {
        _orderRepository.DeleteOrder(id);
    }
}