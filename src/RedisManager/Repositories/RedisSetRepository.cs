using Jnz.RedisRepository.Interfaces;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Repositories
{
    public partial class RedisRepository : IRedisRepository
    {
        public async Task<bool> AddSetAsync<T>(T obj, string key, int databaseNumber = 0) where T : class
        {
            var bytes = Serializer.Serialize(obj);
            var db = GetDatabase(databaseNumber);
            return await db.SetAddAsync(key, bytes);
        }

        public async Task<bool> AddSetAsync(string key, string value, int databaseNumber = 0)
        {
            var db = GetDatabase(databaseNumber);
            return await db.SetAddAsync(key, value);
        }
    }
}
