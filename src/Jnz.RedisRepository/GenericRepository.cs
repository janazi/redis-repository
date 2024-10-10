using StackExchange.Redis;
using System.Text.Json;

namespace Jnz.RedisRepository;

public partial class RedisRepository(IConnectionMultiplexer connectionMultiplexer) : IRedisRepository
{
    private const int DefaultDatabaseNumber = 0;
    private const string KeyLockDefaultPrefix = "lock";

    public async Task<T?> GetAsync<T>(string key, int databaseNumber = DefaultDatabaseNumber)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        var value = await db.StringGetAsync(key);

        if (string.IsNullOrWhiteSpace(value))
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null)
    {
        await SetAsync(key, value, DefaultDatabaseNumber, timeToLive);
    }

    public async Task SetAsync<T>(string key, T value, int databaseNumber, TimeSpan? timeToLive = null)
    {
        var personString = JsonSerializer.Serialize(value);
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        await db.StringSetAsync(key, personString, expiry: timeToLive);
    }

    public async Task<bool> DeleteAsync(string key, int databaseNumber = DefaultDatabaseNumber)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        return await db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key, int databaseNumber = DefaultDatabaseNumber)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        return await db.KeyExistsAsync(key);
    }

    public async Task<bool> SetExpirationAsync(string key, TimeSpan timeToLive, int databaseNumber = DefaultDatabaseNumber)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        return await db.KeyExpireAsync(key, timeToLive);
    }

    public async Task<T> ExecuteWithLockAsync<T>(string key, Func<T> action, TimeSpan timeToLive, int databaseNumber = DefaultDatabaseNumber)
    {
        var lockKey = $"lock:{key}";
        var lockValue = Guid.NewGuid().ToString();
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        var lockTaken = await db.LockTakeAsync(lockKey, lockValue, timeToLive);
        if (!lockTaken)
            KeyLockedException.Throw($"Failed to acquire lock for key {key}");

        try
        {
            return action();
        }
        finally
        {
            db.LockRelease(lockKey, lockValue);
        }
    }

    public async Task<bool> GetLockAsync(string key, TimeSpan timeToLive, int databaseNumber = DefaultDatabaseNumber)
    {
        var keyLock = $"{KeyLockDefaultPrefix}:{key}";
        var lockValue = Environment.MachineName;
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        var lockAcquired = await db.LockTakeAsync(keyLock, lockValue, timeToLive);

        return lockAcquired;
    }

    public async Task<bool> ReleaseLockAsync(string key, int databaseNumber = DefaultDatabaseNumber)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        var lockValue = Environment.MachineName;
        var keyLock = $"{KeyLockDefaultPrefix}:{key}";
        var isLockReleased = await db.LockReleaseAsync(keyLock, lockValue);

        return isLockReleased;
    }

}
