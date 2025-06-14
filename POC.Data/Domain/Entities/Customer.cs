namespace POC.Data.Domain.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }

    // Navigation property
    public virtual ICollection<Order> Orders { get; set; }
}