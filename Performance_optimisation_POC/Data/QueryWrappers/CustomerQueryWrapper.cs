using PerformanceOptimizationUsingAI.Data.CompiledQueries;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Data.QueryWrappers;

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

    // Inefficient: N+1 query problem using compiled queries
    public List<Customer> GetCustomersWithOrders()
    {
        var customers = CustomerCompiledQueries.GetCustomersForOrders(_context).ToList();

        foreach (var customer in customers)
        {
            // Inefficient: N+1 queries even with compiled queries
            var orders = CustomerCompiledQueries.GetOrdersByCustomerId(_context, customer.CustomerId).ToList();
            customer.Orders = orders;
        }

        return customers;
    }

    // Inefficient: Direct compiled query call without caching
    public Customer GetCustomerById(int id)
    {
        // Inefficient: Direct compiled query call without any optimization
        return CustomerCompiledQueries.GetCustomerById(_context, id);
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