using Microsoft.AspNetCore.Mvc;
using POC.DTOs.Extensions;
using POC.DTOs.Order;
using POC.Services;

namespace POC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Performance Issues:
    /// 1. Unnecessary Data Loading: Loads all orders without pagination
    /// 2. Memory Inefficiencies: No result set size limits
    /// 3. Inefficient Query Patterns: No filtering or sorting at database level
    /// </summary>
    [HttpGet("List")]
    public IActionResult AllOrders()
    {
        try
        {
            var orders = orderService.GetAllOrders();
            var response = orders.ToListItems();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. N+1 Query Problem: Loads orders first, then makes additional queries for each order's details
    /// 2. Unnecessary Data Loading: Loads all related data even if not needed
    /// 3. Memory Inefficiencies: No result set size limits, loads entire order history
    /// 4. Inefficient Query Patterns: No eager loading of related entities
    /// </summary>
    [HttpGet("details")]
    public IActionResult OrdersWithDetails()
    {
        try
        {
            var orders = orderService.GetOrdersWithDetails();
            var response = orders.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving orders with details", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Inefficient Query Patterns: No proper indexing strategy
    /// 2. Unnecessary Data Loading: Loads full order details when only basic info is needed
    /// 3. Memory Inefficiencies: No caching strategy for frequently accessed orders
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult OrderById(int id)
    {
        try
        {
            var order = orderService.GetOrderById(id);
            if (order == null!)
                return NotFound(new { message = $"Order with ID {id} not found" });

            var response = order.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving the order", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Inefficient Query Patterns: No proper indexing on customerId
    /// 2. Unnecessary Data Loading: Loads full order details when only customer orders are needed
    /// 3. Memory Inefficiencies: No pagination for customers with many orders
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public IActionResult OrdersByCustomerId(int customerId)
    {
        try
        {
            var orders = orderService.GetOrdersByCustomerId(customerId);
            var response = orders.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving customer orders", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Inefficient Query Patterns: No proper indexing on status field
    /// 2. Unnecessary Data Loading: Loads full order details when only status is needed
    /// 3. Memory Inefficiencies: No pagination for statuses with many orders
    /// </summary>
    [HttpGet("status/{status}")]
    public IActionResult OrdersByStatus(string status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "Status cannot be empty" });

            var orders = orderService.GetOrdersByStatus(status);
            var response = orders.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving orders by status", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Transaction and Concurrency Issues: No proper transaction management
    /// 2. Write Operation Inefficiencies: No bulk insert optimization
    /// 3. Memory Inefficiencies: No validation of input data size
    /// 4. Inefficient Query Patterns: Multiple database calls for order creation
    /// </summary>
    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = request.ToEntity();
            orderService.AddOrder(order);

            var response = order.ToResponse();
            return CreatedAtAction(nameof(OrderById), new { id = order.OrderId }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Transaction and Concurrency Issues: No optimistic concurrency control
    /// 2. Write Operation Inefficiencies: No bulk update optimization
    /// 3. Memory Inefficiencies: No validation of update data
    /// 4. Inefficient Query Patterns: Multiple database calls for order update
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult UpdateOrderStatus(int id, [FromBody] OrderUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingOrder = orderService.GetOrderById(id);
            if (existingOrder == null!)
                return NotFound(new { message = $"Order with ID {id} not found" });

            existingOrder.UpdateFromRequest(request);
            orderService.UpdateOrder(existingOrder);

            var response = existingOrder.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
        }
    }

    /// <summary>
    /// Performance Issues:
    /// 1. Transaction and Concurrency Issues: No proper transaction management
    /// 2. Write Operation Inefficiencies: No cascade delete optimization
    /// 3. Memory Inefficiencies: No cleanup of related data
    /// 4. Inefficient Query Patterns: Multiple database calls for order deletion
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        try
        {
            var existingOrder = orderService.GetOrderById(id);
            if (existingOrder == null!)
                return NotFound(new { message = $"Order with ID {id} not found" });

            orderService.DeleteOrder(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the order", error = ex.Message });
        }
    }
}