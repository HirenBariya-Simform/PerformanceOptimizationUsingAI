using POC.Data.Domain.Entities;

namespace POC.Services;

public interface ICustomerService
{
    List<Customer> GetAllCustomers();
    List<Customer> GetCustomersWithOrders();
    Customer GetCustomerById(int id);
    void AddCustomer(Customer customer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(int id);
}