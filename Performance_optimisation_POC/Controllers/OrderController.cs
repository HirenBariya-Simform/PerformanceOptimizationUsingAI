using Microsoft.AspNetCore.Mvc;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
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

    // Inefficient: No pagination, no filtering, no caching
    [HttpGet]
    public IActionResult GetAllOrders()
    {
        try
        {
            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: N+1 queries, no caching
    [HttpGet("with-details")]
    public IActionResult GetOrdersWithDetails()
    {
        try
        {
            // Inefficient: Direct service call without any optimization
            var orders = _orderService.GetOrdersWithDetails();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No error handling, no async/await
    [HttpGet("{id}")]
    public IActionResult GetOrderById(int id)
    {
        var order = _orderService.GetOrderById(id);
        if (order == null!) return NotFound();
        return Ok(order);
    }

    // Inefficient: No pagination
    [HttpGet("customer/{customerId}")]
    public IActionResult GetOrdersByCustomerId(int customerId)
    {
        try
        {
            var orders = _orderService.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No pagination
    [HttpGet("status/{status}")]
    public IActionResult GetOrdersByStatus(string status)
    {
        try
        {
            var orders = _orderService.GetOrdersByStatus(status);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper validation
    [HttpPost]
    public IActionResult AddOrder([FromBody] Order order)
    {
        try
        {
            // Inefficient: Unnecessary validation
            if (order == null!) return BadRequest("No order provided");

            _orderService.AddOrder(order);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper validation
    [HttpPut("{id}")]
    public IActionResult UpdateOrder(int id, [FromBody] Order order)
    {
        if (id != order.OrderId) return BadRequest();

        try
        {
            _orderService.UpdateOrder(order);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }

    // Inefficient: No proper cleanup
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(int id)
    {
        try
        {
            _orderService.DeleteOrder(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Inefficient: Poor error handling
            return StatusCode(500, ex.Message);
        }
    }
}