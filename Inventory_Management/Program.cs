using Application.Common.Interfaces;
using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Infrastructure.Data;
using Infrastructure.Repo;
using Inventory_Management.DependencyInjection;
using Inventory_Management.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// adding Dependency Injection
builder.Services.AddAppDI();


// Set up Serilog for file logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/myapp.txt", rollingInterval: RollingInterval.Day)  // Log file path and roll over every day
    .CreateLogger();
// Use Serilog for logging
builder.Host.UseSerilog();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>(); // Global exception handler

try
{

    // Seed data when the application starts
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ApplicationDbContext.SeedData(dbContext);
    }

}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while seeding the database.");
    throw;
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Register the rate limiting middleware with a max limit of 5 requests per 10 seconds
// app.UseMiddleware<RateLimitingMiddleware>(10, TimeSpan.FromSeconds(10));

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
