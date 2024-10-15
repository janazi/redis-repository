using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace Jnz.RedisRepository.Tests;
public class Services
{
    public Services()
    {
        var serviceCollection = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var redisContainer = new RedisBuilder()
            .WithImage("redis:7.0")
            .WithExposedPort(6379)
            .WithPortBinding(6379)
            .Build();

        redisContainer.StartAsync().GetAwaiter().GetResult();

        serviceCollection.AddRedisRepository();
        serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public ServiceProvider ServiceProvider { get; }
}