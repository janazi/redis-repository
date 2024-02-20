using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository;

public class RedisLockManager(IConnectionMultiplexer connectionMultiplexer) : IRedisLockManager
{
    private const string KeyLockDefaultPrefix = "Lock";

    private static string Token => Environment.MachineName;


    /// <summary>
    ///     Key must make explicit the reason of taking a lock ex: Person-123456
    ///     Bad keys are just "123456" or "person"
    /// </summary>
    /// <param name="key"></param>
    /// <param name="databaseNumber"></param>
    /// <param name="lockTime"></param>
    /// <returns></returns>
    public async Task<bool> GetLockAsync(string key, TimeSpan lockTime, int databaseNumber = 0)
    {
        var keyLock = $"{KeyLockDefaultPrefix}:{key}";

        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        var isLockedAcquired = await db.LockTakeAsync(keyLock, Token, lockTime);

        return isLockedAcquired;
    }

    public async Task<bool> ReleaseLockAsync(string key, int databaseNumber = 0)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        var token = Environment.MachineName;
        var keyLock = $"{KeyLockDefaultPrefix}:{key}";
        var isLockReleased = await db.LockReleaseAsync(keyLock, token);

        return isLockReleased;
    }

    public async Task<RedisValue> GetLockInfo(string token, int databaseNumber = 0)
    {
        var db = connectionMultiplexer.GetDatabase(databaseNumber);
        return await db.LockQueryAsync(token);
    }
}