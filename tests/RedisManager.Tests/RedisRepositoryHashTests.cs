using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests;

[CollectionDefinition(nameof(RedisRepositoryStringTests))]
public class RedisRepositoryHashTests(Services services) : IClassFixture<Services>
{
    private readonly ServiceProvider _serviceProvider = services.ServiceProvider;

    [Fact]
    public async Task ShouldSetHashFieldTypedAsyncInTheGivenDatabase()
    {
        // ARRANGE
        const string key = "Key";
        const string hashField = "HashField";
        const int databaseNumber = 1;
        var objectToBeCached = new SomeObjectWithoutInterface { Title = key, CreatedOn = DateTime.Now };
        var redisRepository = _serviceProvider.GetService<IRedisRepositoryNew>();
        //ACT
        await redisRepository.SetHashAsync(objectToBeCached, key, hashField, databaseNumber);
        var cachedObject = await redisRepository.GetHashAsync<SomeObjectWithoutInterface>(key, hashField, databaseNumber);
        await redisRepository.DeleteHashAsync(key, hashField, databaseNumber);
        //ASSERT
        Assert.Equal(objectToBeCached.Title, cachedObject.Title);
        Assert.Equal(objectToBeCached.CreatedOn, cachedObject.CreatedOn);
    }

    [Fact]
    public async Task ShouldGetHash()
    {
        // ARRANGE
        const string key = "Key";
        const string hashField = "HashField";
        const int databaseNumber = 1;
        var objectToBeCached = new SomeObjectWithoutInterface { Title = key, CreatedOn = DateTime.Now };
        var redisRepository = _serviceProvider.GetService<IRedisRepositoryNew>();
        await redisRepository.SetHashAsync(objectToBeCached, key, hashField, databaseNumber);
        //ACT
        var cachedObject = redisRepository.GetHash<SomeObjectWithoutInterface>(key, hashField, databaseNumber);
        await redisRepository.DeleteHashAsync(key, hashField, databaseNumber);
        //ASSERT
        Assert.Equal(objectToBeCached.Title, cachedObject.Title);
        Assert.Equal(objectToBeCached.CreatedOn, cachedObject.CreatedOn);
    }

}