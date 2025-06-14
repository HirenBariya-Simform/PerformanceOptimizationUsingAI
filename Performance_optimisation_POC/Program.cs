using Microsoft.EntityFrameworkCore;
using POC.Data;
using POC.Data.QueryWrappers;
using POC.Data.Repositories;
using POC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Inefficient: No connection string validation
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Fixed: Changed from singleton to scoped lifetime for query wrappers
builder.Services.AddScoped<CustomerQueryWrapper>();
builder.Services.AddScoped<ProductQueryWrapper>();
builder.Services.AddScoped<OrderQueryWrapper>();
builder.Services.AddScoped<ProductCategoryQueryWrapper>();

// Fixed: Changed from singleton to scoped lifetime for repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();

// Fixed: Changed from singleton to scoped lifetime for services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();

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

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();