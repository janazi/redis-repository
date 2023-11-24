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
        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
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
        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        await redisRepository.SetHashAsync(objectToBeCached, key, hashField, databaseNumber);
        //ACT
        var cachedObject = redisRepository.GetHash<SomeObjectWithoutInterface>(key, hashField, databaseNumber);
        redisRepository.DeleteHash(key, hashField, databaseNumber);
        //ASSERT
        Assert.Equal(objectToBeCached.Title, cachedObject.Title);
        Assert.Equal(objectToBeCached.CreatedOn, cachedObject.CreatedOn);
    }

    [Fact]
    public async Task Should_CreateLockAndGetHashAsync()
    {
        // ARRANGE
        const string key = "Key";
        const string hashField = "HashField";
        const int databaseNumber = 1;
        var objectToBeCached = new SomeObjectWithoutInterface { Title = key, CreatedOn = DateTime.Now };
        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();
        await redisRepository.SetHashAsync(objectToBeCached, key, hashField, databaseNumber);
        //ACT
        var cachedObject = await redisRepository.GetHashWithLockAsync<SomeObjectWithoutInterface>(key, hashField, TimeSpan.FromSeconds(1), databaseNumber);
        await redisRepository.DeleteHashAsync(key, hashField, databaseNumber);
        const string keyLock = $"Lock:{key}";
        var lockToken = await redisLockManager.GetLockInfo(keyLock);
        //ASSERT
        Assert.Equal(objectToBeCached.Title, cachedObject.Title);
        Assert.Equal(objectToBeCached.CreatedOn, cachedObject.CreatedOn);
        Assert.Equal(Environment.MachineName, lockToken);
    }

    [Fact]
    public async Task Should_CreateLockAndGetAllHashFromAKeyAsync()
    {
        // ARRANGE
        const string key = "Key";
        const string hashField = "HashField";
        const string hashField2 = "HashField2";
        const int databaseNumber = 1;
        var objectToBeCached = new SomeObjectWithoutInterface { Title = key, CreatedOn = DateTime.Now };
        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        await redisRepository.SetHashAsync(objectToBeCached, key, hashField, databaseNumber);
        await redisRepository.SetHashAsync(objectToBeCached, key, hashField2, databaseNumber);
        //ACT
        var results = await redisRepository.GetAllHashAsync<SomeObjectWithoutInterface>(key, databaseNumber);
        await redisRepository.DeleteHashAsync(key, hashField, databaseNumber);

        //ASSERT
        Assert.Equal(2, results.Count);
    }

}