using Jnz.RedisRepository.Extensions;
using Jnz.RedisRepository.Interfaces;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RedisManager.Tests
{
    public class Services
    {
        public Services()
        {
            var serviceCollection = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();


            serviceCollection.AddRedisRepository(configuration);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; }
    }

    [MessagePackObject]
    public class MyObject : IRedisCacheable, IEquatable<MyObject>
    {
        public MyObject()
        {
            TempoEmCache = TimeSpan.FromMilliseconds(500);
        }

        [Key(1)] public string Name { get; set; }
        [Key(2)] public DateTime? Data { get; set; }

        [IgnoreMember] public TimeSpan TempoEmCache { get; set; }

        public string GetIndex()
        {
            return "MyObject";
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

        public bool Equals(MyObject other)
        {
            return this.Name == other.Name;
        }
    }

    public class MyObjectWithoutInterface
    {
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}