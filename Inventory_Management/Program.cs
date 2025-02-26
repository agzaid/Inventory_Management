using Application.Common.Interfaces;
using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Infrastructure.Data;
using Infrastructure.Repo;
using Inventory_Management.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IShippingFreightService, ShippingFreightService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Set up Serilog for file logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/myapp.txt", rollingInterval: RollingInterval.Day)  // Log file path and roll over every day
    .CreateLogger();
// Use Serilog for logging
builder.Host.UseSerilog();

var app = builder.Build();

// Seed data when the application starts
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ApplicationDbContext.SeedData(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Register the rate limiting middleware with a max limit of 5 requests per 10 seconds
app.UseMiddleware<RateLimitingMiddleware>(10, TimeSpan.FromSeconds(10));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
      name: "admin",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "secondRoute",
    pattern: "{controller=Home}/{action=Index}/{id?}/{date?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
