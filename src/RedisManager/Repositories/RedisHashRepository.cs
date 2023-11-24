using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Repositories;

public partial class RedisRepository : IRedisRepository
{
    public async Task SetHashAsync<T>(T obj, string key, string hashField, int databaseNumber = 0)
        where T : class
    {
        var bytes = Serializer.Serialize(obj);
        var db = GetDatabase(databaseNumber);
        await db.HashSetAsync(key, new HashEntry[] { new(hashField, bytes) });
    }
    public T GetHash<T>(string key, string hashField, int databaseNumber = 0)
        where T : class
    {
        var db = GetDatabase(databaseNumber);
        var data = db.HashGet(key, hashField);

        return data.IsNull ? default : Serializer.Deserialize<T>(data);
    }

    public async Task<T> GetHashWithLockAsync<T>(string key, string hashField, TimeSpan lockTime, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var obj = await GetHashAsync<T>(key, hashField, databaseNumber);
        var lockKey = $"{KeyLockPrefix}:{key}";
        var isLocked = db.LockTake(lockKey, Token, lockTime);

        if (!isLocked) throw new KeyLockedException(LockedKeyError);

        return obj;
    }

    public async Task<T> GetHashAsync<T>(string key, string hashField, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var data = await db.HashGetAsync(key, hashField);
        return data.IsNull ? default : Serializer.Deserialize<T>(data);
    }
    public async Task<ICollection<T>> GetAllHashAsync<T>(string key, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var hashEntries = await db.HashGetAllAsync(key);

        return hashEntries.Length == 0 ? default(ICollection<T>) :
            hashEntries.Select(hashEntry => Serializer.Deserialize<T>(hashEntry.Value)).ToList();
    }
    public async Task DeleteHashAsync(string key, string hashField, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        await db.HashDeleteAsync(key, hashField);
    }

    public void DeleteHash(string key, string hashField, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        db.HashDelete(key, hashField);
    }
}