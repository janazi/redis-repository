using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisLockManager
    {
        /// <summary>
        /// Return if lock was acquired
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lockTime"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        Task<bool> GetLockAsync(string key, TimeSpan lockTime, int databaseNumber = 0);

        /// <summary>
        /// Release lock allowing new threads to acquire it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        Task<bool> ReleaseLockAsync(string key, int databaseNumber = 0);

        /// <summary>
        /// Return lock info based on the token (default Environment.MachineName)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        Task<RedisValue> GetLockInfo(string token, int databaseNumber = 0);
    }
}