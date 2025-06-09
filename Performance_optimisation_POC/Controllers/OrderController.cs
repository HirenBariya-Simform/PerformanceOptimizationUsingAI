using Microsoft.AspNetCore.Mvc;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.DTOs.Order;
using PerformanceOptimizationUsingAI.Extensions;
using PerformanceOptimizationUsingAI.Services;

namespace PerformanceOptimizationUsingAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get all orders with minimal information for listing
    /// </summary>
    [HttpGet("List")]
    public IActionResult GetAllOrders()
    {
        try
        {
            var orders = _orderService.GetAllOrders();
            var response = orders.ToListItems();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
        }
    }

    /// <summary>
    /// Get orders with full details including order items
    /// </summary>
    [HttpGet("details")]
    public IActionResult GetOrdersWithDetails()
    {
        try
        {
            var orders = _orderService.GetOrdersWithDetails();
            var response = orders.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving orders with details", error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific order by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetOrderById(int id)
    {
        try
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound(new { message = $"Order with ID {id} not found" });
            
            var response = order.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
        }
    }

    /// <summary>
    /// Get orders by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public IActionResult GetOrdersByCustomerId(int customerId)
    {
        try
        {
            var orders = _orderService.GetOrdersByCustomerId(customerId);
            var response = orders.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving customer orders", error = ex.Message });
        }
    }

    /// <summary>
    /// Get orders by status
    /// </summary>
    [HttpGet("status/{status}")]
    public IActionResult GetOrdersByStatus(string status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "Status cannot be empty" });

            var orders = _orderService.GetOrdersByStatus(status);
            var response = orders.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving orders by status", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = request.ToEntity();
            _orderService.AddOrder(order);

            var response = order.ToResponse();
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing order (mainly status updates)
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult UpdateOrderStatus(int id, [FromBody] OrderUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingOrder = _orderService.GetOrderById(id);
            if (existingOrder == null)
                return NotFound(new { message = $"Order with ID {id} not found" });

            existingOrder.UpdateFromRequest(request);
            _orderService.UpdateOrder(existingOrder);

            var response = existingOrder.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete an order
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        try
        {
            var existingOrder = _orderService.GetOrderById(id);
            if (existingOrder == null)
                return NotFound(new { message = $"Order with ID {id} not found" });

            _orderService.DeleteOrder(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the order", error = ex.Message });
        }
    }
}