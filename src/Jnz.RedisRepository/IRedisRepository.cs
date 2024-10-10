
namespace Jnz.RedisRepository;

public interface IRedisRepository
{
    Task<bool> DeleteAsync(string key, int databaseNumber = 0);
    Task<T> ExecuteWithLockAsync<T>(string key, Func<T> action, TimeSpan timeToLive, int databaseNumber = 0);
    Task<bool> ExistsAsync(string key, int databaseNumber = 0);
    Task<T?> GetAsync<T>(string key, int databaseNumber = 0);
    Task<bool> GetLockAsync(string key, TimeSpan timeToLive, int databaseNumber = 0);
    Task<bool> ReleaseLockAsync(string key, int databaseNumber = 0);
    Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null);
    Task SetAsync<T>(string key, T value, int databaseNumber, TimeSpan? timeToLive = null);
    Task<bool> SetExpirationAsync(string key, TimeSpan timeToLive, int databaseNumber = 0);
}