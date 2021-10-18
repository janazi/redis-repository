using Jnz.RedisRepository.Interfaces;
using MessagePack;
using MessagePack.Resolvers;

namespace Jnz.RedisRepository
{

    public class RedisSerializer : ISerializer
    {
        public RedisSerializer(IFormatterResolver formatterResolver)
        {
            StaticCompositeResolver.Instance.Register(NativeDateTimeResolver.Instance, formatterResolver);

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;

            //var options = MessagePackSerializerOptions.Standard.WithResolver(NativeDateTimeResolver.Instance);
            //MessagePackSerializer.DefaultOptions = options;
            //CompositeResolver.RegisterAndSetAsDefault(NativeDateTimeResolver.Instance, formatterResolver);
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