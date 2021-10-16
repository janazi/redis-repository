namespace Jnz.RedisRepository.Interfaces;

public interface ISerializer
{
    byte[] Serialize<T>(T obj);
    T DeserializeAsync<T>(byte[] bytes);
}