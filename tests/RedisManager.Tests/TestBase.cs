using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Super.RedisRepository.Extensions;
using Super.RedisRepository.Interfaces;
using System;

namespace RedisManager.Tests
{
    public class Services
    {
        public ServiceProvider ServiceProvider { get; }
        public Services()
        {
            var serviceCollection = new ServiceCollection();
            

            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "RedisUri")]).Returns("127.0.0.1");
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);


            serviceCollection.AddRedisRepository(mockConfiguration.Object);

            ServiceProvider = serviceCollection.BuildServiceProvider();

        }
    }

    [MessagePackObject]
    public class MyObject : IRedisCacheable
    {
        public MyObject()
        {
            TempoEmCache = TimeSpan.FromMilliseconds(500);
        }

        [Key(1)] public string Name { get; set; }
        [Key(2)] public DateTime? Data { get; set; }
        [IgnoreMember]
        public TimeSpan TempoEmCache { get; set; }

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
    }
}
