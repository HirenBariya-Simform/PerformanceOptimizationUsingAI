using Microsoft.EntityFrameworkCore;
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
        return _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsNoTracking()
            .AsSplitQuery()
            .ToList();
    }

    public Order? GetOrderById(int id)
    {
        return OrderCompiledQueries.GetOrderById(_context, id);
    }

    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        return OrderCompiledQueries.GetOrdersByCustomerId(_context, customerId).ToList();
    }

    public List<Order> GetOrdersByStatus(string status)
    {
        return OrderCompiledQueries.GetOrdersByStatus(_context, status).ToList();
    }

    public void AddOrder(Order order)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var productIds = order.OrderItems.Select(oi => oi.ProductId).Distinct().ToList();
                var products = _context.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToDictionary(p => p.ProductId);

                // Update stock in bulk
                foreach (var item in order.OrderItems)
                {
                    if (products.TryGetValue(item.ProductId, out var product))
                    {
                        if (product.StockQuantity < item.Quantity)
                            throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
                        
                        product.StockQuantity -= item.Quantity;
                    }
                }

                _context.Orders.Add(order);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
    }

    public void UpdateOrder(Order order)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingOrder = _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.OrderId == order.OrderId);

                if (existingOrder == null)
                    throw new InvalidOperationException($"Order {order.OrderId} not found");

                // Calculate stock adjustments
                var stockAdjustments = new Dictionary<int, int>();
                
                // Return existing order items stock
                foreach (var item in existingOrder.OrderItems)
                {
                    if (!stockAdjustments.ContainsKey(item.ProductId))
                        stockAdjustments[item.ProductId] = 0;
                    stockAdjustments[item.ProductId] += item.Quantity;
                }

                // Calculate new order items stock requirements
                foreach (var item in order.OrderItems)
                {
                    if (!stockAdjustments.ContainsKey(item.ProductId))
                        stockAdjustments[item.ProductId] = 0;
                    stockAdjustments[item.ProductId] -= item.Quantity;
                }

                // Update stock in bulk
                var products = _context.Products
                    .Where(p => stockAdjustments.Keys.Contains(p.ProductId))
                    .ToDictionary(p => p.ProductId);

                foreach (var adjustment in stockAdjustments)
                {
                    if (products.TryGetValue(adjustment.Key, out var product))
                    {
                        if (product.StockQuantity + adjustment.Value < 0)
                            throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
                        
                        product.StockQuantity += adjustment.Value;
                    }
                }

                _context.Entry(existingOrder).CurrentValues.SetValues(order);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
    }

    public void DeleteOrder(int id)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.OrderId == id);

                if (order != null)
                {
                    // Return stock in bulk
                    var stockAdjustments = order.OrderItems
                        .GroupBy(oi => oi.ProductId)
                        .ToDictionary(g => g.Key, g => g.Sum(oi => oi.Quantity));

                    var products = _context.Products
                        .Where(p => stockAdjustments.Keys.Contains(p.ProductId))
                        .ToDictionary(p => p.ProductId);

                    foreach (var adjustment in stockAdjustments)
                    {
                        if (products.TryGetValue(adjustment.Key, out var product))
                        {
                            product.StockQuantity += adjustment.Value;
                        }
                    }

                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
    }
}