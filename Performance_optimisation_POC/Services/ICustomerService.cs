using PerformanceOptimizationUsingAI.Data.Domain.Entities;

namespace PerformanceOptimizationUsingAI.Services;

public interface ICustomerService
{
    List<Customer> GetAllCustomers();
    List<Customer> GetCustomersWithOrders();
    Customer GetCustomerById(int id);
    void AddCustomer(Customer customer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(int id);
}