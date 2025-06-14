using POC.Data.Domain.Entities;

namespace POC.Services;

public interface IOrderService
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