using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisRepository
    {
        /// <summary>
        ///     Adiciona um objeto no Redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task SetAsync<T>(T obj) where T : IRedisCacheable;
        /// <summary>
        /// Set any kind of object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task SetAsync<T>(T obj, string key, string index, int databaseNumber = 0, TimeSpan? expiration = null) where T : class;

        /// <summary>
        ///     Obtêm um objeto do Redis com Lock para uso exclusivo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="lockTime"></param>
        /// <returns></returns>
        Task<T> GetWithLockAsync<T>(string key, TimeSpan lockTime) where T : IRedisCacheable;

        Task<T> GetWithLockAsync<T>(string indexKey, TimeSpan lockTime, int dbNumber) where T : class;

        /// <summary>
        ///     Obtêm um objeto do Redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key) where T : IRedisCacheable;
        Task<T> GetAsync<T>(string index, string key, int databaseNumber);

        Task<T> GetAsync<T>(string fullKey, int databaseNumber);
        Task<string> GetStringAsync(string key, int dataBaseNumber);

        /// <summary>
        ///     Obtem o objeto informado indice e chave ex Pessoa:1234
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string index, string key) where T : IRedisCacheable;

        /// <summary>
        ///     Adiciona um objeto a uma determinada hash de uma key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task SetHashAsync<T>(T obj, string key, string hash) where T : IRedisCacheable;

        /// <summary>
        ///     Obtêm um objeto de uma hash com lock exclusivo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <param name="lockTime"></param>
        /// <returns></returns>
        Task<T> GetHashWithLockAsync<T>(string key, string hash, TimeSpan lockTime) where T : IRedisCacheable;

        /// <summary>
        ///     Obtêm um objeto de uma hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<T> GetHashAsync<T>(string key, string hash) where T : IRedisCacheable;

        Task<T> GetHashAsync<T>(string key, string hash, string index, int databaseNumber);

        void DeleteKey<T>(string key) where T : IRedisCacheable;
        Task DeleteKeyAsync(string key, int dataBaseNumber);
        void DeleteKey<T>(string key, string index) where T : IRedisCacheable;
        /// <summary>
        /// Remove completamente uma key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteKeyAsync<T>(string key) where T : IRedisCacheable;
        /// <summary>
        /// Remove uma hash de uma key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task DeleteHashAsync<T>(string key, string hash)
            where T : IRedisCacheable;
        Task DeleteHashAsync(string key, string hash, string index, int databaseNumber);

        /// <summary>
        ///     Obtem todas as chaves de um determinado padrão
        /// </summary>
        /// <param name="dataBaseNumber"></param>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<string> GetAllKeysByPattern(int dataBaseNumber, string pattern, int pageSize = 100);

        void Set<T>(T obj) where T : IRedisCacheable;
        void Set<T>(T obj, string index) where T : IRedisCacheable;
        void Set<T>(T obj, string key, string index) where T : IRedisCacheable;
        void SetHash<T>(T obj, string key, string hash) where T : IRedisCacheable;
        void SetHash<T>(T obj, string key, string hash, string index) where T : IRedisCacheable;

        Task SetHashAsync<T>(T obj, string key, string hash, string index, int databaseNumber)
            where T : class;

        T Get<T>(string key) where T : IRedisCacheable;
        T Get<T>(string key, string index) where T : IRedisCacheable;
        T GetHash<T>(string key, string hash) where T : IRedisCacheable;
        T GetHash<T>(string key, string hash, string index) where T : IRedisCacheable;
        Task<bool> SetExpiration<T>(string key, TimeSpan expires) where T : IRedisCacheable;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullKey">Index + Key</param>
        /// <param name="expires"></param>
        /// <returns></returns>
        Task<bool> SetExpiration(string fullKey, int databaseNumber, TimeSpan expires);
        /// <summary>
        /// IncrByDecimal
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        Task<decimal> IncrementByDecimal(string key, decimal value, int databaseNumber);

        /// <summary>
        /// Create or add value on a existing set
        /// </summary>
        /// <param name="dataBaseNumber"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        Task<bool> SetAddAsync<T>(int dataBaseNumber, string key, T obj);
        Task<bool> SetAddAsync(int dataBaseNumber, string key, string obj);
        Task<List<T>> GetSetMembersAsync<T>(int dataBaseNumber, string key);
        Task<List<string>> GetSetMembersAsync(int dataBaseNumber, string key);
        Task<bool> RemoveMemberSetAsync<T>(int databaseNumber, string key, T member) where T : IEquatable<T>;
        Task<bool> RemoveMemberSetAsync(int databaseNumber, string key, string value);
        Task<bool> RemoveEntireSetAsync(int databaseNumber, string key);

    }

}