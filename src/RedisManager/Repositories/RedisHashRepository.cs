using Jnz.RedisRepository.Interfaces;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Repositories;

public partial class RedisRepositoryNew : IRedisRepositoryNew
{
    public async Task SetHashAsync<T>(T obj, string key, string hashField, int databaseNumber = 0)
        where T : class
    {
        var bytes = Serializer.Serialize(obj);
        var db = GetDatabase(databaseNumber);
        await db.HashSetAsync(key, new HashEntry[] { new(hashField, bytes) });
    }

    public async Task SetHashAsync<T>(T obj, string key, string hash, string index, int databaseNumber)
        where T : class
    {
        var bytes = Serializer.Serialize(obj);
        var db = GetDatabase(databaseNumber);
        var fullKey = $"{index}:{key}";
        await db.HashSetAsync(fullKey, new HashEntry[] { new(hash, bytes) });
    }

    public T GetHash<T>(string key, string hash)
        where T : IRedisCacheable
    {
        var db = GetDatabase<T>();


        var dados = db.HashGet(key, hash);

        return dados.IsNull ? default : Serializer.Deserialize<T>(dados);
    }

    public T GetHash<T>(string key, string hash, string index)
        where T : IRedisCacheable
    {
        var db = GetDatabase<T>();

        var fullKey = $"{index}:{key}";
        var dados = db.HashGet(fullKey, hash);

        return dados.IsNull ? default : Serializer.Deserialize<T>(dados);
    }

    public async Task<T> GetHashAsync<T>(string key, string hashField, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var data = await db.HashGetAsync(key, hashField);
        return data.IsNull ? default : Serializer.Deserialize<T>(data);
    }
    public async Task<IEnumerable<T>> GetAllHashAsync<T>(string key, string index, int databaseNumber)
    {
        var db = GetDatabase(databaseNumber);

        var fullKey = $"{index}:{key}";

        var hashEntries = await db.HashGetAllAsync(fullKey);

        return hashEntries.Length == 0 ? default(IEnumerable<T>) :
            hashEntries.Select(hashEntry => Serializer.Deserialize<T>(hashEntry.Value)).ToList();
    }

    public async Task DeleteHashAsync(string key, string hashField, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);

        await db.HashDeleteAsync(key, hashField);
    }
}