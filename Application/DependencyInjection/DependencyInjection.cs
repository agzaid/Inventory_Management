using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services) 
        {
            // Application services are registered in Infrastructure DependencyInjection
            // to follow the project's architecture pattern
            return services;
        }
    }
}
