using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Repositories;

namespace PerformanceOptimizationUsingAI.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    // Inefficient: No async/await, poor error handling, no caching
    public List<Customer> GetAllCustomers()
    {
        // Inefficient: Direct repository call without any optimization
        return _customerRepository.GetAllCustomers();
    }

    // Inefficient: N+1 queries, no async/await
    public List<Customer> GetCustomersWithOrders()
    {
        return _customerRepository.GetCustomersWithOrders();
    }

    // Inefficient: No validation, no error handling
    public Customer GetCustomerById(int id)
    {
        return _customerRepository.GetCustomerById(id);
    }

    // Inefficient: No validation, no transaction
    public void AddCustomer(Customer customer)
    {
        // Inefficient: Unnecessary database call
        var existingCustomer = _customerRepository.GetCustomerById(customer.CustomerId);
        if (existingCustomer != null) throw new Exception("Customer already exists");

        _customerRepository.AddCustomer(customer);
    }

    // Inefficient: No validation, no transaction
    public void UpdateCustomer(Customer customer)
    {
        _customerRepository.UpdateCustomer(customer);
    }

    // Inefficient: No cascade delete handling
    public void DeleteCustomer(int id)
    {
        _customerRepository.DeleteCustomer(id);
    }
}