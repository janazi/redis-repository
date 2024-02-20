using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests;

[Collection(nameof(RedisCollection))]
public class RedisRepositoryStringTests(Services services)
{
    private readonly ServiceProvider _serviceProvider = services.ServiceProvider;

    [Fact]
    public async Task ShouldSetStringAsyncInTheGivenDatabase()
    {
        const string key = "TestStringSet";
        const string value = "TestValue";
        const int databaseNumber = 1;

        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        var isSet = await redisRepository.SetStringAsync(key, value, null, databaseNumber);
        var result = await redisRepository.GetStringAsync(key, databaseNumber);
        // clean test
        await redisRepository.DeleteKeyAsync(key);

        Assert.True(isSet);
        Assert.Equal(value, result);
    }

    [Fact]
    public void ShouldSetStringInTheGivenDatabase()
    {
        const string key = "TestStringSet";
        const string value = "TestValue";
        const int databaseNumber = 1;

        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        var isSet = redisRepository.SetString(key, value, null, databaseNumber);
        var result = redisRepository.GetString(key, databaseNumber);
        // clean test
        redisRepository.DeleteKey(key);

        Assert.True(isSet);
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task ShouldCreateLockAndReturnString()
    {
        const string key = "TestStringSet";
        const string value = "TestValue";
        const int databaseNumber = 1;

        var redisRepository = _serviceProvider.GetService<IRedisRepository>();
        var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();
        var isSet = await redisRepository.SetStringAsync(key, value, databaseNumber: databaseNumber);
        var result = await redisRepository.GetStringWithLockAsync(key, TimeSpan.FromSeconds(10), databaseNumber);

        const string keyLock = $"Lock:{key}";
        var lockToken = await redisLockManager.GetLockInfo(keyLock, databaseNumber);

        Assert.True(isSet);
        Assert.Equal(value, result);
        Assert.Equal(Environment.MachineName, lockToken);
    }
}