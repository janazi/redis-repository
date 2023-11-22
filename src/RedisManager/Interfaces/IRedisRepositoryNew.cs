using LanguageExt.Common;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisRepositoryNew
    {
        #region strings

        bool SetString(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0);
        Task<bool> SetStringAsync(string key, string value, TimeSpan? timeToLive, int databaseNumber = 0);
        string GetString(string key, int databaseNumber = 0);
        Task<string> GetStringAsync(string key, int databaseNumber = 0);
        Task<Result<string>> GetStringWithLockAsync(string key, TimeSpan lockTimeToLive, int databaseNumber = 0);

        #endregion
    }

}