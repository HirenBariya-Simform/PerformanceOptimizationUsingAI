using Microsoft.EntityFrameworkCore;
using PerformanceOptimizationUsingAI.Data;
using PerformanceOptimizationUsingAI.Data.QueryWrappers;
using PerformanceOptimizationUsingAI.Repositories;
using PerformanceOptimizationUsingAI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Inefficient: No connection string validation
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inefficient: No scoped lifetime for query wrappers
builder.Services.AddSingleton<CustomerQueryWrapper>();
builder.Services.AddSingleton<ProductQueryWrapper>();
builder.Services.AddSingleton<OrderQueryWrapper>();
builder.Services.AddSingleton<ProductCategoryQueryWrapper>();

// Inefficient: No scoped lifetime for repositories
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IProductCategoryRepository, ProductCategoryRepository>();

// Inefficient: No scoped lifetime for services
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IProductCategoryService, ProductCategoryService>();

// Inefficient: No CORS policy
builder.Services.AddCors();

// Inefficient: No API versioning
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Inefficient: No proper CORS configuration
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Inefficient: No proper error handling middleware
app.Run();