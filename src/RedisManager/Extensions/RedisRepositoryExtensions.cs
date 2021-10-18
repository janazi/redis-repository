using Jnz.RedisRepository.Interfaces;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Linq;

namespace Jnz.RedisRepository.Extensions
{

    public static class RedisRepositoryExtensions
    {
        /// <summary>
        ///     Adiciona a interface IRedisRepository para utilizar o Redis como banco de dados key/value
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">It will look for a section string named RedisOptions 
        /// ex: "RedisOptions": {
        /// "hosts": [ "localhost:6379" ],
        /// "SyncTimeout": 500, 
        /// "AsyncTimeout": 500,
        /// "KeepAlive": 180
        /// }</param>
        /// <param name="formatterResolver">
        ///     Por padrão irá assumir ContractlessStandardResolver que não
        ///     precisa de atributos nas models, por causa do MessagePack, porém perde em performance para
        ///     o StandardResolver. Também assume por padrão NativeDateTimeResolver
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddRedisRepository(this IServiceCollection services,
            IConfiguration configuration, IFormatterResolver formatterResolver = null)
        {
            var redisOptions = configuration.GetSection("RedisOptions").Get<RedisOptions>();

            if (redisOptions is null)
                throw new ArgumentException("RedisOptions configuration section is missing");

            var options = new ConfigurationOptions
            {
                SyncTimeout = redisOptions.SyncTimeout,
                AsyncTimeout = redisOptions.AsyncTimeout,
                KeepAlive = redisOptions.KeepAlive,
                Password = redisOptions.Password
            };
            redisOptions.Hosts.ToList().ForEach(h => options.EndPoints.Add(h));


            formatterResolver ??= ContractlessStandardResolver.Instance;

            services.AddSingleton(formatterResolver);

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
            services.AddSingleton<ISerializer, RedisSerializer>();
            services.AddSingleton<IRedisRepository, RedisRepository>();
            services.AddSingleton<IRedisLockManager, RedisLockManager>();

            return services;
        }
    }
}