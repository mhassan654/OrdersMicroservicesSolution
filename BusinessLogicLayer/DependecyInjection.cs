using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IOrdersServices, OrdersService>();
            return services;
        }
    }
}
