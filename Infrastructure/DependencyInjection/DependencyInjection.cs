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
            return services;
        }
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            //services.AddIdentityCore<ApplicationUser>(options =>
            //{
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequiredLength = 10;
            //    options.User.RequireUniqueEmail = true;
            //});
            var builder = services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
