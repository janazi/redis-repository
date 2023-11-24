using Jnz.RedisRepository.Interfaces;
using LanguageExt.Common;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Repositories;

public partial class RedisRepository : RepositoryBase, IRedisRepository
{
    public RedisRepository(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer, IRedisLockManager redisLockManager) :
        base(connectionMultiplexer, serializer, redisLockManager)
    { }

    public bool SetString(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return db.StringSet(key, value);
    }

    public async Task<bool> SetStringAsync(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return await db.StringSetAsync(key, value);
    }

    public bool Set<T>(T obj, string key, int databaseNumber = 0) where T : class
    {
        var bytes = Serializer.Serialize(obj);
        var db = GetDatabase(databaseNumber);
        return db.StringSet(key, bytes);
    }

    public async Task<bool> SetAsync<T>(T obj, string key, int databaseNumber = 0) where T : class
    {
        var bytes = Serializer.Serialize(obj);
        var db = GetDatabase(databaseNumber);
        return await db.StringSetAsync(key, bytes);
    }

    public string GetString(string key, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return db.StringGet(key);
    }

    public async Task<string> GetStringAsync(string key, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return await db.StringGetAsync(key);
    }

    public async Task<T> GetWithLockAsync<T>(string key, TimeSpan lockTimeToLive, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var bytes = await db.StringGetAsync(key);
        var keyLock = $"{KeyLockPrefix}:{key}";
        var isLocked = await db.LockTakeAsync(keyLock, Token, lockTimeToLive);

        return isLocked is false ? default : Serializer.Deserialize<T>(bytes);
    }

    public T Get<T>(string key, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var bytes = db.StringGet(key);
        return bytes.IsNullOrEmpty ? default : Serializer.Deserialize<T>(bytes);
    }

    public async Task<T> GetAsync<T>(string key, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var bytes = await db.StringGetAsync(key);
        return bytes.IsNullOrEmpty ? default : Serializer.Deserialize<T>(bytes);
    }

    public async Task<Result<string>> GetStringWithLockAsync(string key, TimeSpan lockTimeToLive, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        var keyLock = $"{KeyLockPrefix}:{key}";
        var isLocked = await db.LockTakeAsync(keyLock, Token, lockTimeToLive);

        return isLocked is false ? new Result<string>(new KeyLockedException(LockedKeyError)) : await GetStringAsync(key, databaseNumber);
    }

    public async Task<bool> SetExpirationAsync(string key, TimeSpan timeToLive, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return await db.KeyExpireAsync(key, timeToLive);
    }

    public bool DeleteKey(string key, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return db.KeyDelete(key);
    }

    public async Task<bool> DeleteKeyAsync(string key, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return await db.KeyDeleteAsync(key);
    }

    public async Task<decimal> IncrementByDecimal(string key, decimal value, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        return (decimal)await db.StringIncrementAsync(key, (float)value);
    }
}