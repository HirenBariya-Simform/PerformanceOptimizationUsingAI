using POC.Data.CompiledQueries;
using POC.Data.Domain.Entities;

namespace POC.Data.QueryWrappers;

public class OrderQueryWrapper
{
    private readonly ApplicationDbContext _context;

    public OrderQueryWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Order> GetAllOrders()
    {
        return OrderCompiledQueries.GetAllOrders(_context).ToList();
    }

    public List<Order> GetOrdersWithDetails()
    {
        return GetAllOrders(); // Already includes all details from compiled query
    }

    public Order GetOrderById(int id)
    {
        return OrderCompiledQueries.GetOrderByIdWithDetails(_context, id);
    }

    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        return OrderCompiledQueries.GetOrdersByCustomerIdWithDetails(_context, customerId).ToList();
    }

    public List<Order> GetOrdersByStatus(string status)
    {
        return OrderCompiledQueries.GetOrdersByStatusWithDetails(_context, status).ToList();
    }

    public void AddOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public void UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        _context.SaveChanges();
    }

    public void DeleteOrder(int id)
    {
        var order = OrderCompiledQueries.GetOrderByIdWithDetails(_context, id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }
}