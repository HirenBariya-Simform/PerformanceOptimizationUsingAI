using POC.DTOs.Customer;
using POC.DTOs.Order;

namespace POC.DTOs.Extensions;

public static class CustomerMappingExtensions
{
    // Convert Entity to Response DTO
    public static CustomerResponse ToResponse(this Data.Domain.Entities.Customer customer)
    {
        return new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Orders = customer.Orders?.Select(o => new CustomerOrderResponse
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderItems = o.OrderItems?.Select(oi => oi.ToResponse()).ToList() ?? new List<OrderItemResponse>()
            }).ToList() ?? new List<CustomerOrderResponse>()
        };
    }

    // Convert Entity to List Item DTO
    public static CustomerListItem ToListItem(this Data.Domain.Entities.Customer customer)
    {
        return new CustomerListItem
        {
            CustomerId = customer.CustomerId,
            Name = customer.Name,
            Email = customer.Email,
            Address = customer.Address
        };
    }

    // Convert Create Request to Entity
    public static Data.Domain.Entities.Customer ToEntity(this CustomerCreateRequest request)
    {
        return new Data.Domain.Entities.Customer
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };
    }

    // Update Entity from Update Request
    public static void UpdateFromRequest(this Data.Domain.Entities.Customer customer, CustomerUpdateRequest request)
    {
        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
    }

    // Convert collection of entities to response DTOs
    public static List<CustomerResponse> ToResponseList(this IEnumerable<Data.Domain.Entities.Customer> customers)
    {
        return customers.Select(c => c.ToResponse()).ToList();
    }

    // Convert collection of entities to list item DTOs
    public static List<CustomerListItem> ToListItems(this IEnumerable<Data.Domain.Entities.Customer> customers)
    {
        return customers.Select(c => c.ToListItem()).ToList();
    }
}