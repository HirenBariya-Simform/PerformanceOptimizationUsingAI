using POC.Data.Domain.Entities;
using POC.Data.QueryWrappers;

namespace POC.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerQueryWrapper _queryWrapper;

    public CustomerRepository(ApplicationDbContext context)
    {
        _queryWrapper = new CustomerQueryWrapper(context);
    }

    // Inefficient: Loading all customers without pagination, no caching
    public List<Customer> GetAllCustomers()
    {
        // Inefficient: Direct query wrapper call without any optimization
        return _queryWrapper.GetAllCustomers();
    }

    // Inefficient: N+1 query problem
    public List<Customer> GetCustomersWithOrders()
    {
        return _queryWrapper.GetCustomersWithOrders();
    }

    // Inefficient: No async/await
    public Customer GetCustomerById(int id)
    {
        return _queryWrapper.GetCustomerById(id);
    }

    // Inefficient: No validation, no transaction
    public void AddCustomer(Customer customer)
    {
        _queryWrapper.AddCustomer(customer);
    }

    // Inefficient: No error handling, no transaction
    public void UpdateCustomer(Customer customer)
    {
        _queryWrapper.UpdateCustomer(customer);
    }

    // Inefficient: No cascade delete handling
    public void DeleteCustomer(int id)
    {
        _queryWrapper.DeleteCustomer(id);
    }
}