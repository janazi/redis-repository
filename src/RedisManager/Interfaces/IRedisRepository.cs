using LanguageExt.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisRepository
    {
        #region Hash

        Task SetHashAsync<T>(T obj, string key, string hashField, int databaseNumber = 0) where T : class;
        Task<T> GetHashAsync<T>(string key, string hashField, int databaseNumber = 0) where T : class;
        T GetHash<T>(string key, string hashField, int databaseNumber = 0) where T : class;
        Task<T> GetHashWithLockAsync<T>(string key, string hashField, TimeSpan lockTime, int databaseNumber = 0) where T : class;
        Task<ICollection<T>> GetAllHashAsync<T>(string key, int databaseNumber = 0) where T : class;
        Task DeleteHashAsync(string key, string hashField, int databaseNumber = 0);
        void DeleteHash(string key, string hashField, int databaseNumber = 0);

        #endregion

        #region generic

        bool SetString(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0);
        Task<bool> SetStringAsync(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0);
        bool Set<T>(T obj, string key, int databaseNumber = 0) where T : class;
        Task<bool> SetAsync<T>(T obj, string key, int databaseNumber = 0) where T : class;
        string GetString(string key, int databaseNumber = 0);
        Task<string> GetStringAsync(string key, int databaseNumber = 0);
        Task<T> GetWithLockAsync<T>(string key, TimeSpan lockTime, int databaseNumber = 0) where T : class;
        T Get<T>(T obj, string key, int databaseNumber = 0) where T : class;
        Task<T> GetAsync<T>(T obj, string key, int databaseNumber = 0) where T : class;
        Task<Result<string>> GetStringWithLockAsync(string key, TimeSpan lockTimeToLive, int databaseNumber = 0);
        Task<bool> SetExpirationAsync(string key, TimeSpan expires, int databaseNumber = 0);
        #endregion
        bool DeleteKey(string key, int databaseNumber = 0);
        Task<bool> DeleteKeyAsync(string key, int databaseNumber = 0);
        Task<decimal> IncrementByDecimal(string key, decimal value, int databaseNumber = 0);
    }

}