using Microsoft.Extensions.DependencyInjection;

namespace Jnz.RedisRepository;
public static class RedisRepositoryExtensions
{
    public static void AddRedisRepository(this IServiceCollection services)
    {
        services.AddScoped<IRedisRepository, RedisRepository>();
    }
}
