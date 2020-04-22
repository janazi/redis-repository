using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Super.RedisRepository.Interfaces;

namespace Super.RedisRepository.Extensions
{
    public static class RedisRepositoryExtensions
    {
        /// <summary>
        /// Adiciona a interface IRedisRepository para utilizar o Redis como banco de dados key/value
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="formatterResolver">Por padrão irá assumir ContractlessStandardResolver que não 
        /// precisa de atributos nas models, por causa do MessagePack, porém perde em performance para 
        /// o StandardResolver. Também assume por padrão NativeDateTimeResolver</param>
        /// <returns></returns>
        public static IServiceCollection AddRedisRepository(this IServiceCollection services,
            IConfiguration configuration, IFormatterResolver formatterResolver = null)
        {
            var uri = configuration.GetConnectionString("RedisUri");

            var options = new ConfigurationOptions()
            {
                EndPoints = { uri },
                SyncTimeout = 300000,
                AsyncTimeout = 300000,
                KeepAlive = 180
            };

            if (formatterResolver == null)
                formatterResolver = ContractlessStandardResolver.Instance;

            services.AddSingleton(formatterResolver);

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
            services.AddSingleton<ISerializer, RedisSerializer>();
            services.AddSingleton<IRedisRepository, RedisRepository>();

            return services;
        }
    }
}
