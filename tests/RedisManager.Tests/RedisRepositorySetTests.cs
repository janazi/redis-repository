using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests;

[CollectionDefinition(nameof(RedisRepositoryStringTests))]
public class RedisRepositorySetTests(Services services) : IClassFixture<Services>
{
    private readonly ServiceProvider _serviceProvider = services.ServiceProvider;

    [Fact]
    public async Task ShouldCreateSetAsync()
    {
        // ARRANGE
        const string key = "Key";
        const string key2 = "Key2";
        const int databaseNumber = 1;
        var objectToBeCached = new SomeObjectWithoutInterface { Title = key, CreatedOn = DateTime.Now };
        var objectToBeCached2 = new SomeObjectWithoutInterface { Title = key2, CreatedOn = DateTime.Now };
        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        //ACT
        var result = await redisRepository.AddSetAsync(objectToBeCached, key, databaseNumber);
        var result2 = await redisRepository.AddSetAsync(objectToBeCached, key, databaseNumber);
        var result3 = await redisRepository.AddSetAsync(objectToBeCached2, key, databaseNumber);
        //await redisRepository.DeleteHashAsync(key, hashField, databaseNumber);
        //ASSERT
        Assert.True(result);
        Assert.False(result2);
        Assert.True(result3);

    }
}