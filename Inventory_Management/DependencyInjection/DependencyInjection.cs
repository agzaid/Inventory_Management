using Application.DependencyInjection;
using Infrastructure.DependencyInjection;

namespace Inventory_Management.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services.AddInfrastructureDI().AddApplicationDI();
            return services;
        }

    }
}
