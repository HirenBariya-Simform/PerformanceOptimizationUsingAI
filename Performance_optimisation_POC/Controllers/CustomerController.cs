using Microsoft.AspNetCore.Mvc;
using POC.DTOs.Customer;
using POC.DTOs.Extensions;
using POC.Services;

namespace POC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    /// <summary>
    ///     Get all customers with minimal information for listing
    /// </summary>
    [HttpGet("List")]
    public IActionResult AllCustomers()
    {
        try
        {
            var customers = customerService.GetAllCustomers();
            var response = customers.ToListItems();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving customers", error = ex.Message });
        }
    }

    /// <summary>
    ///     Get customers with their orders
    /// </summary>
    [HttpGet("With-Orders")]
    public IActionResult CustomersWithOrders()
    {
        try
        {
            var customers = customerService.GetCustomersWithOrders();
            var response = customers.ToResponseList();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving customers with orders", error = ex.Message });
        }
    }

    /// <summary>
    ///     Get a specific customer by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult CustomerById(int id)
    {
        try
        {
            var customer = customerService.GetCustomerById(id);
            if (customer == default!)
                return NotFound(new { message = $"Customer with ID {id} not found" });

            var response = customer.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while retrieving the customer", error = ex.Message });
        }
    }

    /// <summary>
    ///     Create a new customer
    /// </summary>
    [HttpPost]
    public IActionResult CreateCustomer([FromBody] CustomerCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = request.ToEntity();
            customerService.AddCustomer(customer);

            var response = customer.ToResponse();
            return CreatedAtAction(nameof(CustomerById), new { id = customer.CustomerId }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while creating the customer", error = ex.Message });
        }
    }

    /// <summary>
    ///     Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult UpdateCustomer(int id, [FromBody] CustomerUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCustomer = customerService.GetCustomerById(id);
            if (existingCustomer == null!)
                return NotFound(new { message = $"Customer with ID {id} not found" });

            existingCustomer.UpdateFromRequest(request);
            customerService.UpdateCustomer(existingCustomer);

            var response = existingCustomer.ToResponse();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the customer", error = ex.Message });
        }
    }

    /// <summary>
    ///     Delete a customer
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(int id)
    {
        try
        {
            var existingCustomer = customerService.GetCustomerById(id);
            if (existingCustomer == null!)
                return NotFound(new { message = $"Customer with ID {id} not found" });

            customerService.DeleteCustomer(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the customer", error = ex.Message });
        }
    }
}