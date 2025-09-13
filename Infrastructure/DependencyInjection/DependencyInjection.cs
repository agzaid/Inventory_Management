using Application.Common.Interfaces;
using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IShippingFreightService, ShippingFreightService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOnlineOrderService, OnlineOrderService>();
            services.AddScoped<IDeliverySlotService, DeliverySlotService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<Application.Common.Interfaces.IEmailSender, Infrastructure.EmailSender.SmtpEmailSender>();

            return services;
        }
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            var builder = services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Redirect here if not logged in
                options.AccessDeniedPath = "/Account/AccessDenied";
            });


            return services;
        }
    }
}
