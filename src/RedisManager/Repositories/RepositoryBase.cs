using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;
using System;

namespace Jnz.RedisRepository.Repositories
{
    public abstract class RepositoryBase
    {
        protected const string LockedKeyError = "Key locked by another process";
        protected const string KeyLockPrefix = "Lock";
        protected readonly IConnectionMultiplexer ConnectionMultiplexer;
        protected readonly ISerializer Serializer;
        protected readonly IRedisLockManager RedisLockManager;

        // TODO: MAKE THIS COMING FROM SETTINGS
        protected const int DefaultDatabaseNumber = 0;
        protected static string Token => Environment.MachineName;

        protected RepositoryBase(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer, IRedisLockManager redisLockManager)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            Serializer = serializer;
            RedisLockManager = redisLockManager;
        }

        private static string GetFullKey<T>(string key)
            where T : IRedisCacheable
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            return $"{obj.GetIndex()}:{key}";
        }

        protected IDatabase GetDatabase<T>() where T : IRedisCacheable
        {
            var type = (T)Activator.CreateInstance(typeof(T));
            return ConnectionMultiplexer.GetDatabase(type.GetDatabaseNumber());
        }

        protected IDatabase GetDatabase(int databaseNumber)
        {
            return ConnectionMultiplexer.GetDatabase(databaseNumber);
        }

    }
}
