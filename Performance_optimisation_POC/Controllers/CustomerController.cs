using Microsoft.AspNetCore.Mvc;
using PerformanceOptimizationUsingAI.Data.Domain.Entities;
using PerformanceOptimizationUsingAI.Services;

namespace PerformanceOptimizationUsingAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // Inefficient: No async/await, no pagination, no caching
    [HttpGet]
    public IActionResult GetAllCustomers()
    {
        // Inefficient: Direct service call without any optimization
        var customers = _customerService.GetAllCustomers();
        return Ok(customers);
    }

    // Inefficient: N+1 queries, no async/await
    [HttpGet("with-orders")]
    public IActionResult GetCustomersWithOrders()
    {
        var customers = _customerService.GetCustomersWithOrders();
        return Ok(customers);
    }

    // Inefficient: No error handling, no async/await
    [HttpGet("{id}")]
    public IActionResult GetCustomerById(int id)
    {
        var customer = _customerService.GetCustomerById(id);
        if (customer == null!) return NotFound();
        return Ok(customer);
    }

    // Inefficient: No validation, no async/await
    [HttpPost]
    public IActionResult AddCustomer([FromBody] Customer customer)
    {
        try
        {
            _customerService.AddCustomer(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Inefficient: No validation, no async/await
    [HttpPut("{id}")]
    public IActionResult UpdateCustomer(int id, [FromBody] Customer customer)
    {
        if (id != customer.CustomerId) return BadRequest();

        try
        {
            _customerService.UpdateCustomer(customer);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Inefficient: No cascade delete handling, no async/await
    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(int id)
    {
        try
        {
            _customerService.DeleteCustomer(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}