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
    public T GetHash<T>(string key, string hashField, int databaseNumber = 0)
        where T : class
    {
        var db = GetDatabase(databaseNumber);
        var data = db.HashGet(key, hashField);

        return data.IsNull ? default : Serializer.Deserialize<T>(data);
    }
    public async Task<T> GetHashAsync<T>(string key, string hashField, int databaseNumber = 0) where T : class
    {
        var db = GetDatabase(databaseNumber);
        var data = await db.HashGetAsync(key, hashField);
        return data.IsNull ? default : Serializer.Deserialize<T>(data);
    }
    public async Task<IEnumerable<T>> GetAllHashAsync<T>(string key, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);
        var hashEntries = await db.HashGetAllAsync(key);

        return hashEntries.Length == 0 ? default(IEnumerable<T>) :
            hashEntries.Select(hashEntry => Serializer.Deserialize<T>(hashEntry.Value)).ToList();
    }
    public async Task DeleteHashAsync(string key, string hashField, int databaseNumber = 0)
    {
        var db = GetDatabase(databaseNumber);

        await db.HashDeleteAsync(key, hashField);
    }
}