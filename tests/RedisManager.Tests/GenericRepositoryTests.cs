using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests;

[CollectionDefinition(nameof(RedisCollection))]
public class RedisCollection : ICollectionFixture<Services>;

[Collection(nameof(RedisCollection))]
public class GenericRepositoryTests(Services services)
{
    private readonly ServiceProvider _serviceProvider = services.ServiceProvider;

    [Fact]
    public async Task SetAsync_WithObject_ShouldSetAndGetObject()
    {
        // Arrange
        var repository = _serviceProvider.GetRequiredService<IRedisRepository>();
        var key = "key";
        var value = new Person("John", new DateTime(1990, 1, 1));
        // Act
        await repository.SetAsync(key, value);
        var result = await repository.GetAsync<Person>(key);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(value.Name, result!.Name);
        Assert.Equal(value.Birthday, result!.Birthday);
    }

    [Fact]
    public async Task SetAsync_WithObject_ShouldSetObject()
    {
        // Arrange
        var repository = _serviceProvider.GetRequiredService<IRedisRepository>();
        var key = "key";
        var value = new Person("John", new DateTime(1990, 1, 1));
        // Act
        await repository.SetAsync(key, value);
        var result = await repository.SetLockAndGetAsync<Person>(key, TimeSpan.FromSeconds(60));

        // the object has an active lock, so it should throw an exception
        await Assert.ThrowsAsync<KeyLockedException>(async () =>
            await repository.SetLockAndGetAsync<Person>(key, TimeSpan.FromSeconds(60)));

        // release the lock
        await repository.ReleaseLockAsync(key);

        // should be able to get the object now
        var result2 = await repository.SetLockAndGetAsync<Person>(key, TimeSpan.FromSeconds(60));

        // release the lock
        await repository.ReleaseLockAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(value.Name, result!.Name);
        Assert.Equal(value.Birthday, result!.Birthday);

        Assert.NotNull(result2);
        Assert.Equal(value.Name, result2!.Name);
        Assert.Equal(value.Birthday, result2!.Birthday);
    }
}

public class Person(string name, DateTime birthday)
{

    public string Name { get; set; } = name;
    public DateTime Birthday { get; set; } = birthday;
}
