using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisLockManager
    {
        Task<bool> GetLockAsync(string key, int databaseNumber, TimeSpan lockTime);
        Task<bool> ReleaseLockAsync(string key, int databaseNumber);
        Task<RedisValue> GetLockInfo(string teststringset);
    }
}