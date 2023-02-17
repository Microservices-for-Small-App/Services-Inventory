using CommonLibrary.MongoDB.Extensions;
using CommonLibrary.Settings;
using Inventory.Data.Entities;

namespace Inventory.API.Extensions;

public static class DependedServicesExtensions
{

    public static IServiceCollection ConfigureDependedServices(this IServiceCollection services)
    {
        _ = services.AddSingleton(serviceProvider =>
        {
            return serviceProvider.GetService<IConfiguration>()
                    ?.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()!;
        });

        _ = services.AddSingleton(serviceProvider =>
        {
            return serviceProvider.GetService<IConfiguration>()
                    ?.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()!;
        });

        _ = services.AddSingleton(serviceProvider =>
        {
            return serviceProvider.GetService<IConfiguration>()
                    ?.GetSection(nameof(MongoDbCollectionSettings)).Get<MongoDbCollectionSettings>()!;
        });

        _ = services.AddMongo().AddMongoRepository<InventoryItem>();

        _ = services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();

        _ = services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
        });

        return services;
    }

}
