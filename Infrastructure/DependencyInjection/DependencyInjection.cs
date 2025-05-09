using Application.Common.Interfaces;
using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Infrastructure.Repo;
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
            return services;
        }

    }
}
