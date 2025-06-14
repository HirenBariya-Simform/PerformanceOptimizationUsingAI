using POC.Data.CompiledQueries;
using POC.Data.Domain.Entities;

namespace POC.Data.QueryWrappers;

public class CustomerQueryWrapper
{
    private readonly ApplicationDbContext _context;

    public CustomerQueryWrapper(ApplicationDbContext context)
    {
        _context = context;
    }

    // Inefficient: Converting IEnumerable to List without caching
    public List<Customer> GetAllCustomers()
    {
        // Inefficient: Direct compiled query call without any optimization
        return CustomerCompiledQueries.GetAllCustomers(_context).ToList();
    }

    // Inefficient: Multiple N+1 queries, unnecessary database calls
    public List<Customer> GetCustomersWithOrders()
    {
        // Inefficient: First query to get all customers
        var customers = _context.Customers.ToList();

        // Inefficient: N+1 query problem - separate query for each customer's orders
        foreach (var customer in customers)
        {
            // Inefficient: Separate query for orders
            var orders = _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .ToList();

            // Inefficient: N+1 query problem - separate query for each order's items
            foreach (var order in orders)
            {
                // Inefficient: Separate query for order items
                var orderItems = _context.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();

                // Inefficient: N+1 query problem - separate query for each order item's product
                foreach (var orderItem in orderItems)
                    // Inefficient: Separate query for product
                    orderItem.Product = _context.Products
                        .FirstOrDefault(p => p.ProductId == orderItem.ProductId);

                order.OrderItems = orderItems;
            }

            customer.Orders = orders;
        }

        return customers;
    }

    // Inefficient: Multiple separate queries, no caching
    public Customer GetCustomerById(int id)
    {
        // Inefficient: First query to get customer
        var customer = _context.Customers
            .FirstOrDefault(c => c.CustomerId == id);

        if (customer != null)
        {
            // Inefficient: Separate query for orders
            var orders = _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .ToList();

            // Inefficient: N+1 query problem - separate query for each order's items
            foreach (var order in orders)
            {
                // Inefficient: Separate query for order items
                var orderItems = _context.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();

                // Inefficient: N+1 query problem - separate query for each order item's product
                foreach (var orderItem in orderItems)
                    // Inefficient: Separate query for product
                    orderItem.Product = _context.Products
                        .FirstOrDefault(p => p.ProductId == orderItem.ProductId);

                order.OrderItems = orderItems;
            }

            customer.Orders = orders;
        }

        return customer;
    }

    // Inefficient: Using compiled query for search operations
    public List<Customer> SearchCustomers(string searchTerm)
    {
        // Inefficient: Using compiled query for dynamic search
        var customers = CustomerCompiledQueries.SearchCustomersByName(_context, searchTerm).ToList();
        return customers;
    }

    // Inefficient: Not using compiled queries for write operations
    public void AddCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

    // Inefficient: Not using compiled queries for write operations
    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();
    }

    // Inefficient: Not using compiled queries for write operations
    public void DeleteCustomer(int id)
    {
        var customer = GetCustomerById(id);
        if (customer != null!)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
    }
}