using LanguageExt.Common;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisRepositoryNew
    {
        #region Hash

        Task SetHashAsync<T>(T obj, string key, string hashField, int databaseNumber = 0) where T : class;
        Task<T> GetHashAsync<T>(string key, string hashField, int databaseNumber = 0) where T : class;
        T GetHash<T>(string key, string hashField, int databaseNumber = 0) where T : class;
        Task DeleteHashAsync(string key, string hashField, int databaseNumber = 0);

        #endregion

        #region strings

        bool SetString(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0);
        Task<bool> SetStringAsync(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0);
        string GetString(string key, int databaseNumber = 0);
        Task<string> GetStringAsync(string key, int databaseNumber = 0);
        Task<Result<string>> GetStringWithLockAsync(string key, TimeSpan lockTimeToLive, int databaseNumber = 0);

        #endregion
        bool DeleteKey(string key, int databaseNumber = 0);
        Task<bool> DeleteKeyAsync(string key, int databaseNumber = 0);
    }

}