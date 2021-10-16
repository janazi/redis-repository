using System;
using System.Threading.Tasks;
using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;

namespace Jnz.RedisRepository;

public class RedisLockManager : IRedisLockManager
{
    private const string KeyLockDefaultPrefix = "Lock";
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisLockManager(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    private static string Token => Environment.MachineName;


    /// <summary>
    ///     Key must make explicit the reason of taking a lock ex: Person-123456
    ///     Bad keys are just "123456" or "person"
    /// </summary>
    /// <param name="key"></param>
    /// <param name="databaseNumber"></param>
    /// <param name="lockTime"></param>
    /// <returns></returns>
    public async Task<bool> GetLockAsync(string key, int databaseNumber, TimeSpan lockTime)
    {
        var keyLock = $"{KeyLockDefaultPrefix}:{key}";

        var db = _connectionMultiplexer.GetDatabase(databaseNumber);
        var isLockedAcquired = await db.LockTakeAsync(keyLock, Token, lockTime);

        return isLockedAcquired;
    }

    public async Task<bool> ReleaseLockAsync(string key, int databaseNumber)
    {
        var db = _connectionMultiplexer.GetDatabase(databaseNumber);
        var token = Environment.MachineName;
        var keyLock = $"{KeyLockDefaultPrefix}:{key}";
        var isLockRelesead = await db.LockReleaseAsync(keyLock, token);

        return isLockRelesead;
    }
}