using Microsoft.EntityFrameworkCore;
using POC.Data.Domain.Entities;

namespace POC.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductCategoryMapping> ProductCategoryMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerId);

        // Configure Product
        modelBuilder.Entity<Product>()
            .HasKey(p => p.ProductId);

        // Configure Order
        modelBuilder.Entity<Order>()
            .HasKey(o => o.OrderId);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure OrderItem
        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => oi.OrderItemId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure ProductCategory
        modelBuilder.Entity<ProductCategory>()
            .HasKey(pc => pc.CategoryId);

        // Configure ProductCategoryMapping
        modelBuilder.Entity<ProductCategoryMapping>()
            .HasKey(pcm => new { pcm.ProductId, pcm.CategoryId });

        modelBuilder.Entity<ProductCategoryMapping>()
            .HasOne(pcm => pcm.Product)
            .WithMany(p => p.CategoryMappings)
            .HasForeignKey(pcm => pcm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductCategoryMapping>()
            .HasOne(pcm => pcm.Category)
            .WithMany(pc => pc.ProductMappings)
            .HasForeignKey(pcm => pcm.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}