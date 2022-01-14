using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jnz.RedisRepository
{

    public class RedisRepository : IRedisRepository
    {
        private const string LockedKeyError = "Key locked by another process";
        private const string KeyLockDefaultPrefix = "Lock";
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ISerializer _serializer;
        private readonly IRedisLockManager _redisLockManager;

        public RedisRepository(IConnectionMultiplexer connectionMultiplexer,
            ISerializer serializer, IRedisLockManager redisLockManager)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _serializer = serializer;
            _redisLockManager = redisLockManager;
        }

        private static string Token => Environment.MachineName;

        public async Task SetAsync<T>(T obj)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);

            var db = _connectionMultiplexer.GetDatabase(obj.GetDatabaseNumber());

            await db.StringSetAsync(GetFullKey<T>(obj.GetKey()), bytes, obj.GetExpiration());
        }

        public async Task SetAsync<T>(T obj, string key, string index, int databaseNumber = 0, TimeSpan? expiration = null)
            where T : class
        {
            var bytes = _serializer.Serialize(obj);

            var db = _connectionMultiplexer.GetDatabase(databaseNumber);

            var fullKey = $"{index}:{key}";

            await db.StringSetAsync(fullKey, bytes, expiration);
        }

        public void Set<T>(T obj)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);

            var db = _connectionMultiplexer.GetDatabase(obj.GetDatabaseNumber());

            db.StringSet(GetFullKey<T>(obj.GetKey()), bytes, obj.GetExpiration());
        }

        public void Set<T>(T obj, string index)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);

            var db = _connectionMultiplexer.GetDatabase(obj.GetDatabaseNumber());

            var fullKey = $"{index}:{obj.GetKey()}";

            db.StringSet(fullKey, bytes, obj.GetExpiration());
        }

        public void Set<T>(T obj, string key, string index)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);

            var db = _connectionMultiplexer.GetDatabase(obj.GetDatabaseNumber());

            var fullKey = $"{index}:{key}";

            db.StringSet(fullKey, bytes, obj.GetExpiration());
        }

        public void SetHash<T>(T obj, string key, string hash)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);
            db.HashSetAsync(fullKey, new HashEntry[] { new(hash, bytes) });
        }

        public void SetHash<T>(T obj, string key, string hash, string index)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);
            var db = GetDatabase<T>();
            var fullKey = $"{index}:{key}";
            db.HashSetAsync(fullKey, new HashEntry[] { new(hash, bytes) });
        }

        public async Task<T> GetWithLockAsync<T>(string key, TimeSpan lockTime)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);
            var obj = await GetAsync<T>(key);

            var isLocked = await _redisLockManager.GetLockAsync(fullKey, db.Database, TimeSpan.FromSeconds(1));
            if (!isLocked) throw new KeyLockedException(LockedKeyError);
            return obj;
        }

        public void DeleteKey<T>(string key)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);

            db.KeyDelete(fullKey);
        }

        public void DeleteKey<T>(string key, string index)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var fullKey = $"{index}:{key}";

            db.KeyDelete(fullKey);
        }

        public async Task DeleteKeyAsync<T>(string key)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);

            await db.KeyDeleteAsync(fullKey);
        }

        public async Task DeleteKeyAsync(string key, int dataBaseNumber)
        {
            var db = GetDatabase(dataBaseNumber);
            await db.KeyDeleteAsync(key);
        }

        public async Task DeleteHashAsync<T>(string key, string hash)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);

            await db.HashDeleteAsync(fullKey, hash);
        }

        public IEnumerable<string> GetAllKeysByPattern(int dataBaseNumber, string pattern, int pageSize = 100)
        {
            var config = _connectionMultiplexer.Configuration.Split(',')[0];
            var server = _connectionMultiplexer.GetServer(config);

            var keys = server.Keys(dataBaseNumber, pattern, pageSize);

            return keys.Select(k => k.ToString()).ToList().AsEnumerable();
        }

        private IDatabase GetDatabase<T>() where T : IRedisCacheable
        {
            var type = (T)Activator.CreateInstance(typeof(T));
            return _connectionMultiplexer.GetDatabase(type.GetDatabaseNumber());
        }

        private IDatabase GetDatabase(int databaseNumber)
        {
            return _connectionMultiplexer.GetDatabase(databaseNumber);
        }

        public async Task<T> GetAsync<T>(string key)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = GetFullKey<T>(key);
            var bytes = await db.StringGetAsync(fullKey);

            return bytes.IsNullOrEmpty ? default : _serializer.DeserializeAsync<T>(bytes);
        }

        public async Task<T> GetAsync<T>(string index, string key)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = $"{index}:{key}";
            var bytes = await db.StringGetAsync(fullKey);

            return bytes.IsNullOrEmpty ? default : _serializer.DeserializeAsync<T>(bytes);
        }

        public async Task<T> GetAsync<T>(string index, string key, int databaseNumber)
           where T : class
        {
            var db = GetDatabase(databaseNumber);

            var fullKey = $"{index}:{key}";
            var bytes = await db.StringGetAsync(fullKey);

            return bytes.IsNullOrEmpty ? default : _serializer.DeserializeAsync<T>(bytes);
        }

        public T Get<T>(string key)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = GetFullKey<T>(key);
            var bytes = db.StringGet(fullKey);

            return bytes.IsNullOrEmpty ? default : _serializer.DeserializeAsync<T>(bytes);
        }

        public T Get<T>(string key, string index)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = $"{index}:{key}";
            var bytes = db.StringGet(fullKey);

            return bytes.IsNullOrEmpty ? default : _serializer.DeserializeAsync<T>(bytes);
        }

        public T GetHash<T>(string key, string hash)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = GetFullKey<T>(key);
            var dados = db.HashGet(fullKey, hash);

            return dados.IsNull ? default : _serializer.DeserializeAsync<T>(dados);
        }

        public T GetHash<T>(string key, string hash, string index)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = $"{index}:{key}";
            var dados = db.HashGet(fullKey, hash);

            return dados.IsNull ? default : _serializer.DeserializeAsync<T>(dados);
        }

        public async Task SetHashAsync<T>(T obj, string key, string hash)
            where T : IRedisCacheable
        {
            var bytes = _serializer.Serialize(obj);
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);
            await db.HashSetAsync(fullKey, new HashEntry[] { new(hash, bytes) });
        }

        public async Task<T> GetHashWithLockAsync<T>(string key, string hash, TimeSpan lockTime)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var keyLock = $"{KeyLockDefaultPrefix}:{key}";

            var obj = await GetHashAsync<T>(key, hash);
            var isLocked = db.LockTake(keyLock, Token, lockTime);

            if (!isLocked) throw new KeyLockedException(LockedKeyError);

            return obj;
        }

        public async Task<T> GetHashAsync<T>(string key, string hash)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();

            var fullKey = GetFullKey<T>(key);
            var dados = await db.HashGetAsync(fullKey, hash);

            return dados.IsNull ? default : _serializer.DeserializeAsync<T>(dados);
        }

        public async Task<bool> SetExpiration<T>(string key, TimeSpan expires)
            where T : IRedisCacheable
        {
            var db = GetDatabase<T>();
            var fullKey = GetFullKey<T>(key);
            return await db.KeyExpireAsync(fullKey, expires);
        }

        public async Task<bool> SetExpiration(string fullKey, int databaseNumber, TimeSpan expires)
        {
            var db = GetDatabase(databaseNumber);
            return await db.KeyExpireAsync(fullKey, expires);
        }

        public async Task<decimal> IncrementByDecimal(string key, decimal value, int databaseNumber)
        {
            var db = GetDatabase(databaseNumber);
            return (decimal)await db.StringIncrementAsync(key, (float)value);
        }

        private static string GetFullKey<T>(string key)
            where T : IRedisCacheable
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            return $"{obj.GetIndex()}:{key}";
        }

        public async Task<string> GetStringAsync(string key, int dataBaseNumber)
        {
            var db = _connectionMultiplexer.GetDatabase(dataBaseNumber);
            return await db.StringGetAsync(key);
        }
    }

}