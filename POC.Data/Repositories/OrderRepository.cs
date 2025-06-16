using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;
using POC.Data.QueryWrappers;

namespace POC.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderQueryWrapper _queryWrapper;
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _queryWrapper = new OrderQueryWrapper(context);
        _context = context;
    }

    // Inefficient: Loading all orders without pagination, no caching
    public List<Order> GetAllOrders()
    {
        // Inefficient: Loading all related data at once
        var orders = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.CategoryMappings)  // Inefficient: Loading unnecessary category data
            .ThenInclude(cm => cm.Category)        // Inefficient: Loading unnecessary category data
            .ToList();

        // Inefficient: Unnecessary processing
        foreach (var order in orders)
        {
            // Inefficient: Unnecessary object creation
            var tempOrder = new Order
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };
        }

        return orders;
    }

    // Inefficient: N+1 query problem
    public List<Order> GetOrdersWithDetails()
    {
        // Inefficient: First query to get all orders
        var orders = _context.Orders.ToList();

        // Inefficient: N+1 query problem - separate query for each order's customer
        foreach (var order in orders)
        {
            // Inefficient: Separate query for customer
            order.Customer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == order.CustomerId);

            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToList();

            // Inefficient: N+1 query problem - separate query for each order item's product
            foreach (var orderItem in orderItems)
            {
                // Inefficient: Separate query for product
                orderItem.Product = _context.Products
                    .Include(p => p.CategoryMappings)  // Inefficient: Loading unnecessary category data
                    .ThenInclude(cm => cm.Category)    // Inefficient: Loading unnecessary category data
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            order.OrderItems = orderItems;
        }

        return orders;
    }

    // Inefficient: Multiple database calls for a single order
    public Order GetOrderById(int id)
    {
        // Inefficient: First query to get basic order info
        var order = _context.Orders
            .FirstOrDefault(o => o.OrderId == id);

        if (order != null)
        {
            // Inefficient: Separate query for customer
            order.Customer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == order.CustomerId);

            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .ToList();

            // Inefficient: N+1 query problem for products
            foreach (var orderItem in orderItems)
            {
                // Inefficient: Separate query for product with unnecessary includes
                orderItem.Product = _context.Products
                    .Include(p => p.CategoryMappings)  // Inefficient: Loading unnecessary category data
                    .ThenInclude(cm => cm.Category)    // Inefficient: Loading unnecessary category data
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            order.OrderItems = orderItems;
        }

        return order;
    }

    // Inefficient: Multiple database calls for customer orders
    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        // Inefficient: First query to get orders
        var orders = _context.Orders
            .Where(o => o.CustomerId == customerId)
            .ToList();

        // Inefficient: N+1 query problem for each order
        foreach (var order in orders)
        {
            // Inefficient: Separate query for customer
            order.Customer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == order.CustomerId);

            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToList();

            // Inefficient: N+1 query problem for products
            foreach (var orderItem in orderItems)
            {
                // Inefficient: Separate query for product with unnecessary includes
                orderItem.Product = _context.Products
                    .Include(p => p.CategoryMappings)  // Inefficient: Loading unnecessary category data
                    .ThenInclude(cm => cm.Category)    // Inefficient: Loading unnecessary category data
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            order.OrderItems = orderItems;
        }

        return orders;
    }

    // Inefficient: Multiple database calls for status-based orders
    public List<Order> GetOrdersByStatus(string status)
    {
        // Inefficient: First query to get orders
        var orders = _context.Orders
            .Where(o => o.Status == status)
            .ToList();

        // Inefficient: N+1 query problem for each order
        foreach (var order in orders)
        {
            // Inefficient: Separate query for customer
            order.Customer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == order.CustomerId);

            // Inefficient: Separate query for order items
            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToList();

            // Inefficient: N+1 query problem for products
            foreach (var orderItem in orderItems)
            {
                // Inefficient: Separate query for product with unnecessary includes
                orderItem.Product = _context.Products
                    .Include(p => p.CategoryMappings)  // Inefficient: Loading unnecessary category data
                    .ThenInclude(cm => cm.Category)    // Inefficient: Loading unnecessary category data
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            order.OrderItems = orderItems;
        }

        return orders;
    }

    // Inefficient: No transaction, no validation, individual processing
    public void AddOrder(Order order)
    {
        // Inefficient: Process order items one by one
        foreach (var orderItem in order.OrderItems)
        {
            // Inefficient: Individual database call for each product
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == orderItem.ProductId);

            if (product != null)
            {
                // Inefficient: Individual update for each product
                product.StockQuantity -= orderItem.Quantity;
                _context.SaveChanges(); // Inefficient: Save after each item
            }
        }

        // Inefficient: Individual save for order
        _context.Orders.Add(order);
        _context.SaveChanges();

        // Inefficient: Unnecessary validation query
        var savedOrder = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == order.OrderId);
    }

    // Inefficient: No transaction, no validation, individual processing
    public void UpdateOrder(Order order)
    {
        // Inefficient: Process order items one by one
        foreach (var orderItem in order.OrderItems)
        {
            // Inefficient: Individual database call for each product
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == orderItem.ProductId);

            if (product != null)
            {
                // Inefficient: Individual update for each product
                product.StockQuantity -= orderItem.Quantity;
                _context.SaveChanges(); // Inefficient: Save after each item
            }
        }

        // Inefficient: Individual save for order
        var existingOrder = _context.Orders.Find(order.OrderId);
        if (existingOrder != null)
        {
            // Inefficient: No concurrency handling
            _context.Entry(existingOrder).CurrentValues.SetValues(order);
            _context.SaveChanges();
        }
    }

    // Inefficient: No cascade delete handling, individual processing
    public void DeleteOrder(int id)
    {
        // Inefficient: Multiple database calls
        var order = GetOrderById(id);
        if (order != null)
        {
            // Inefficient: Process order items one by one
            foreach (var orderItem in order.OrderItems)
            {
                // Inefficient: Individual database call for each product
                var product = _context.Products
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);

                if (product != null)
                {
                    // Inefficient: Individual update for each product
                    product.StockQuantity += orderItem.Quantity;
                    _context.SaveChanges(); // Inefficient: Save after each item
                }

                // Inefficient: Individual delete for each order item
                _context.OrderItems.Remove(orderItem);
                _context.SaveChanges(); // Inefficient: Save after each item
            }

            // Inefficient: Individual delete for order
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }
    }
}