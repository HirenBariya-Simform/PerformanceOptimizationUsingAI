using POC.Data.Domain.Entities;
using POC.DTOs.Order;

namespace POC.DTOs.Extensions;

public static class OrderMappingExtensions
{
    // Convert OrderItem Entity to Response DTO
    public static OrderItemResponse ToResponse(this OrderItem orderItem)
    {
        return new OrderItemResponse
        {
            OrderItemId = orderItem.OrderItemId,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.Name ?? string.Empty,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            SubTotal = orderItem.Quantity * orderItem.UnitPrice
        };
    }

    // Convert OrderItemDto to Entity
    public static OrderItem ToEntity(this OrderItemDto dto, int orderId)
    {
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice
        };
    }

    // Convert Order Entity to Response DTO
    public static OrderResponse ToResponse(this Data.Domain.Entities.Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name ?? string.Empty,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            OrderItems = order.OrderItems?.Select(oi => oi.ToResponse()).ToList() ?? new List<OrderItemResponse>()
        };
    }

    // Convert Order Entity to List Item DTO
    public static OrderListItem ToListItem(this Data.Domain.Entities.Order order)
    {
        return new OrderListItem
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name ?? string.Empty,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status
        };
    }

    // Convert Create Request to Entity
    public static Data.Domain.Entities.Order ToEntity(this OrderCreateRequest request)
    {
        var order = new Data.Domain.Entities.Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = request.Status,
            TotalAmount = request.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
            OrderItems = new List<OrderItem>()
        };

        // Add order items
        foreach (var itemDto in request.OrderItems) order.OrderItems.Add(itemDto.ToEntity(order.OrderId));

        return order;
    }

    // Update Entity from Update Request
    public static void UpdateFromRequest(this Data.Domain.Entities.Order order, OrderUpdateRequest request)
    {
        order.Status = request.Status;
    }

    // Convert collection of entities to response DTOs
    public static List<OrderResponse> ToResponseList(this IEnumerable<Data.Domain.Entities.Order> orders)
    {
        return orders.Select(o => o.ToResponse()).ToList();
    }

    // Convert collection of entities to list item DTOs
    public static List<OrderListItem> ToListItems(this IEnumerable<Data.Domain.Entities.Order> orders)
    {
        return orders.Select(o => o.ToListItem()).ToList();
    }
}