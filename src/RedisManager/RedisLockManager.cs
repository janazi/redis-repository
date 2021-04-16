using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository
{
    public class RedisLockManager : IRedisLockManager
    {
        private const string KeyLockDefaultPrefix = "Lock";
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        private static string Token => Environment.MachineName;

        public RedisLockManager(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }


        /// <summary>
        /// Key must make explicit the reason of taking a lock ex: Person-123456 
        /// Bad keys are just "123456" or "person"
        /// </summary>
        /// <param name="key"></param>
        /// <param name="databaseNumber"></param>
        /// <param name="lockTime"></param>
        /// <returns></returns>
        public async Task<bool> GetLockAsync(string key, int databaseNumber, TimeSpan lockTime)
        {
            var keyLock = $"{KeyLockDefaultPrefix}:{key}";

            var db = _connectionMultiplexer.GetDatabase(databaseNumber);
            var isLocked = await db.LockTakeAsync(keyLock, Token, lockTime);

            return isLocked;
        }

        public async Task<bool> ReleaseLockAsync(string key, int databaseNumber)
        {
            var db = _connectionMultiplexer.GetDatabase(databaseNumber);
            var token = Environment.MachineName;
            var keyLock = $"{KeyLockDefaultPrefix}:{key}";
            var lockRelesead = await db.LockReleaseAsync(keyLock, token);

            return lockRelesead;
        }
    }
}
