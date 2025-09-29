using Application.Common.Interfaces;
using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Infrastructure.Data;
using Infrastructure.DependencyInjection;
using Infrastructure.Localization;
using Infrastructure.Repo;
using Inventory_Management.DependencyInjection;
using Inventory_Management.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           //.LogTo(Console.WriteLine, LogLevel.Information) // log SQL queries to know the query time
           //.EnableDetailedErrors()
           );

// adding Dependency Injection

builder.Services.AddAppDI();

// localization setup
#region localization setup
var env = builder.Environment;
var localizationFilePath = Path.Combine(env.ContentRootPath, "Resources", "localization.json");

builder.Services.AddSingleton<IStringLocalizerFactory>(new Infrastructure.Localization.JsonStringLocalizerFactory(localizationFilePath));
builder.Services.AddSingleton<IStringLocalizer>(provider =>
{
    var factory = provider.GetRequiredService<IStringLocalizerFactory>();
    return factory.Create(null!);
});
builder.Services.AddLocalization();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var supportedCultures = new[] { "en", "ar" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
});
#endregion localization setup
//end

// Set up Serilog for file logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)  // Only log warnings+ from framework
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Information() // Your app logs info and higher
    .WriteTo.File("Logs/myapp.txt", rollingInterval: RollingInterval.Day)  // Log file path and roll over every day
    .CreateLogger();
// Use Serilog for logging
builder.Host.UseSerilog();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>(); // Global exception handler

try
{

    // Seed data when the application starts
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        DbInitializer.SeedData(dbContext);
        //ApplicationDbContext.SeedData(dbContext);

        await IdentitySeeder.SeedAdminUserAsync(scope.ServiceProvider);
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
    app.UseExceptionHandler("/Error"); // For exceptions
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Register the rate limiting middleware with a max limit of 5 requests per 10 seconds
app.UseMiddleware<RateLimitingMiddleware>();
//app.UseWhen(
//    ctx => ctx.Request.Path.StartsWithSegments("/cart"),
//    appBuilder => appBuilder.UseMiddleware<RateLimitingMiddleware>(30, TimeSpan.FromMinutes(1))
//);
//app.UseWhen(
//    ctx => ctx.Request.Path.Value?.Contains("Cart", StringComparison.OrdinalIgnoreCase) == true,
//    appBuilder => appBuilder.UseMiddleware<RateLimitingMiddleware>(10, TimeSpan.FromMinutes(1))
//);      // ~1 request every 2 sec
//app.UseWhen(
//    ctx => ctx.Request.Path.Value?.Contains("CheckoutDetails", StringComparison.OrdinalIgnoreCase) == true,
//    appBuilder => appBuilder.UseMiddleware<RateLimitingMiddleware>(10, TimeSpan.FromMinutes(1))
//);
//// 10 per minute per IP
//app.UseWhen(
//    ctx => ctx.Request.Path.StartsWithSegments("/admin"),
//    appBuilder => appBuilder.UseMiddleware<RateLimitingMiddleware>(300, TimeSpan.FromMinutes(1))
//);



var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseHttpsRedirection();

app.UseStaticFiles();

//app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
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
