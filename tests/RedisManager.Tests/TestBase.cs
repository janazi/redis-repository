using Jnz.RedisRepository.Extensions;
using Jnz.RedisRepository.Interfaces;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Testcontainers.Redis;

namespace Jnz.RedisRepository.Tests
{
    public class Services
    {
        public Services()
        {
            var serviceCollection = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var redisContainer = new RedisBuilder()
                .WithImage("redis:7.0")
                .WithExposedPort(6379)
                .WithPortBinding(6379)
                .Build();

            redisContainer.StartAsync().GetAwaiter().GetResult();

            serviceCollection.AddRedisRepository(configuration);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; }
    }

    [MessagePackObject]
    public class SomeObject : IRedisCacheable, IEquatable<SomeObject>
    {
        public SomeObject()
        {
            TempoEmCache = TimeSpan.FromMilliseconds(500);
        }

        [Key(1)] public string Name { get; set; }
        [Key(2)] public DateTime? Data { get; set; }

        [IgnoreMember] public TimeSpan TempoEmCache { get; set; }

        public string GetIndex()
        {
            return "SomeObject";
        }

        public int GetDatabaseNumber()
        {
            return 0;
        }

        public TimeSpan? GetExpiration()
        {
            return TempoEmCache;
        }

        public string GetKey()
        {
            return Name;
        }

        public bool Equals(SomeObject other)
        {
            return this.Name == other.Name;
        }
    }

    public class SomeObjectWithoutInterface
    {
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}