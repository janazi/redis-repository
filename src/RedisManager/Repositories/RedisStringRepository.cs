using Jnz.RedisRepository.Interfaces;
using LanguageExt.Common;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Repositories;

public partial class RedisRepository : RepositoryBase, IRedisRepositoryNew
{
    public RedisRepository(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer, IRedisLockManager redisLockManager) : base(connectionMultiplexer, serializer, redisLockManager)
    {
    }

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

    public async Task<Result<string>> GetStringWithLockAsync(string key, TimeSpan lockTimeToLive, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        var keyLock = $"{KeyLockPrefix}:{key}";
        var isLocked = await db.LockTakeAsync(keyLock, Token, lockTimeToLive);

        return isLocked is false ? new Result<string>(new KeyLockedException(LockedKeyError)) : await GetStringAsync(key, databaseNumber);
    }
}