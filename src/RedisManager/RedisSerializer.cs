using MessagePack;
using MessagePack.Resolvers;
using Super.RedisRepository.Interfaces;

namespace Super.RedisRepository
{
    public class RedisSerializer : ISerializer
    {
        public RedisSerializer(IFormatterResolver formatterResolver)
        {
            CompositeResolver.RegisterAndSetAsDefault(NativeDateTimeResolver.Instance, formatterResolver);
        }

        public T DeserializeAsync<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }
    }
}
