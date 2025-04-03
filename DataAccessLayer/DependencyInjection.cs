using AutoMapper.Configuration;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer
            (this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: Add data access layer services into the IOc container
            string connectionStringTemplate = 
                configuration.GetConnectionString("MongoDB")!;

            connectionStringTemplate.Replace("$MONGO_HOST",
                Environment.GetEnvironmentVariable("MONGODB_HOST"))
                .Replace("$MONGO_PORT",
                Environment.GetEnvironmentVariable("MONGODB_HOST"));

            services.AddSingleton<IMongoClient>(new MongoClient(connectionStringTemplate));

            services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase("OrdersDatabase");
            });

            services.AddScoped<IOrdersRepositoriy, OrderRepository>();
            return services;
        }
    }
}
