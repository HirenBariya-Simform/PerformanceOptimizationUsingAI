using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Repositories;

public interface ICustomerRepository
{
    List<Customer> GetAllCustomers();
    List<Customer> GetCustomersWithOrders();
    Customer GetCustomerById(int id);
    void AddCustomer(Customer customer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(int id);
}