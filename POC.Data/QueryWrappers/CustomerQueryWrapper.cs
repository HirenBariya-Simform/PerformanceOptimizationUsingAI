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
        var customers = CustomerCompiledQueries.GetAllCustomers(_context).ToList();

        // Inefficient: Unnecessary object creation and processing
        foreach (var customer in customers)
        {
            // Inefficient: Creating unnecessary temporary objects
            var tempCustomer = new Customer
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };
        }

        return customers;
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
                {
                    // Inefficient: Separate query for product
                    orderItem.Product = _context.Products
                        .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
                }

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
                {
                    // Inefficient: Separate query for product
                    orderItem.Product = _context.Products
                        .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
                }

                order.OrderItems = orderItems;
            }

            customer.Orders = orders;
        }

        return customer;
    }

    // Inefficient: Using non-compiled query and loading unnecessary data
    public List<Customer> SearchCustomers(string name)
    {
        // Inefficient: Not using compiled query and loading unnecessary data
        var customers = _context.Customers
            .Where(c => c.Name.Contains(name))
            .ToList();

        // Inefficient: N+1 query - loading orders for each customer
        foreach (var customer in customers)
        {
            customer.Orders = _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .ToList();
        }

        return customers;
    }

    // Inefficient: N+1 query problem and loading unnecessary data
    public List<Order> GetOrdersByCustomerId(int customerId)
    {
        // Inefficient: Not using compiled query for main query
        var orders = _context.Orders
            .Where(o => o.CustomerId == customerId)
            .ToList();

        // Inefficient: N+1 query - loading items for each order
        foreach (var order in orders)
        {
            order.OrderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToList();

            // Inefficient: N+1 query - loading product for each item
            foreach (var item in order.OrderItems)
            {
                item.Product = _context.Products
                    .FirstOrDefault(p => p.ProductId == item.ProductId);
            }
        }

        return orders;
    }

    // Inefficient: Not using transactions or validation
    public void AddCustomer(Customer customer)
    {
        // Inefficient: Multiple database calls
        _context.Customers.Add(customer);
        _context.SaveChanges();

        // Inefficient: Unnecessary validation query
        var savedCustomer = _context.Customers
            .FirstOrDefault(c => c.CustomerId == customer.CustomerId);
    }

    // Inefficient: Multiple database calls and no concurrency handling
    public void UpdateCustomer(Customer customer)
    {
        // Inefficient: Separate query to load customer
        var existingCustomer = _context.Customers
            .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

        if (existingCustomer != null)
        {
            // Inefficient: Manual property updates
            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Address = customer.Address;

            _context.SaveChanges();
        }
    }

    // Inefficient: Multiple database calls and no cascade handling
    public void DeleteCustomer(int id)
    {
        // Inefficient: Multiple database calls
        var customer = GetCustomerById(id);
        if (customer != null)
        {
            // Inefficient: Separate queries for related data
            var orders = _context.Orders
                .Where(o => o.CustomerId == id)
                .ToList();

            foreach (var order in orders)
            {
                var orderItems = _context.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();

                // Inefficient: Individual deletes
                foreach (var item in orderItems)
                {
                    _context.OrderItems.Remove(item);
                    _context.SaveChanges();
                }

                _context.Orders.Remove(order);
                _context.SaveChanges();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
    }
}