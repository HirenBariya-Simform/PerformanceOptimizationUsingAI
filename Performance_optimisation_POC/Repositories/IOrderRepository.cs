using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Repositories;

public interface IOrderRepository
{
    List<Order> GetAllOrders();
    List<Order> GetOrdersWithDetails();
    Order GetOrderById(int id);
    List<Order> GetOrdersByCustomerId(int customerId);
    List<Order> GetOrdersByStatus(string status);
    void AddOrder(Order order);
    void UpdateOrder(Order order);
    void DeleteOrder(int id);
}