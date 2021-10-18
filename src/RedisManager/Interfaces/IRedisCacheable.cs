using System;

namespace Jnz.RedisRepository.Interfaces
{

    public interface IRedisCacheable
    {
        string GetKey();
        string GetIndex();
        int GetDatabaseNumber();
        TimeSpan? GetExpiration();
    }
}