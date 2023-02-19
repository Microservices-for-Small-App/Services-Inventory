using CommonLibrary.MongoDB.Extensions;
using CommonLibrary.Settings;
using Inventory.API.Clients;
using Inventory.Data.Entities;
using Polly;
using Polly.Timeout;

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

        // Typed clients :: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0
        _ = services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001");
        })
          .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
              5,
              retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
              onRetry: (outcome, timespan, retryCount) =>
              {
                  Console.WriteLine($"Delaying for {timespan} seconds, before making retry {retryCount}");
                  //services?.BuildServiceProvider()?.GetService<ILogger<CatalogClient>>()
                  //      ?.LogWarning($"Delaying for {timespan} seconds, before making retry {retryCount}");
              }
           ))
          .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

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
