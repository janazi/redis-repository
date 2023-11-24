using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests
{
    [Collection(nameof(RedisCollection))]
    public class RedisRepositoryGenericsTests(Services services)
    {
        private readonly ServiceProvider _serviceProvider = services.ServiceProvider;

        [Fact]
        public async Task ShouldSetObjectAsync()
        {
            const string key = "TestStringSet";
            var myObject = new SomeObjectWithoutInterface
            {
                CreatedOn = DateTime.Now,
                Title = "MyObject"
            };
            const int databaseNumber = 1;

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            var isSet = await redisRepository.SetAsync(myObject, key, databaseNumber);
            var result = await redisRepository.GetAsync<SomeObjectWithoutInterface>(key, databaseNumber);
            // clean test
            await redisRepository.DeleteKeyAsync(key);

            Assert.True(isSet);
            Assert.Equal(myObject.Title, result.Title);
            Assert.Equal(myObject.CreatedOn, result.CreatedOn);
        }

        [Fact]
        public void ShouldSetObject()
        {
            const string key = "TestStringSet";
            var myObject = new SomeObjectWithoutInterface
            {
                CreatedOn = DateTime.Now,
                Title = "MyObject"
            };
            const int databaseNumber = 1;

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            var isSet = redisRepository.Set(myObject, key, databaseNumber);
            var result = redisRepository.Get<SomeObjectWithoutInterface>(key, databaseNumber);
            // clean test
            redisRepository.DeleteKey(key);

            Assert.True(isSet);
            Assert.Equal(myObject.Title, result.Title);
            Assert.Equal(myObject.CreatedOn, result.CreatedOn);
        }

        [Fact]
        public async Task ShouldGetObjectAndCreateLockAsync()
        {
            const string key = "TestObjectWithLock";
            var myObject = new SomeObjectWithoutInterface
            {
                CreatedOn = DateTime.Now,
                Title = "MyObject"
            };
            const int databaseNumber = 1;

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();
            var isSet = await redisRepository.SetAsync(myObject, key, databaseNumber);
            var result = await redisRepository.GetWithLockAsync<SomeObjectWithoutInterface>(key, TimeSpan.FromSeconds(1), databaseNumber);
            // clean test
            await redisRepository.DeleteKeyAsync(key);

            const string keyLock = $"Lock:{key}";
            var lockToken = await redisLockManager.GetLockInfo(keyLock);

            Assert.True(isSet);
            Assert.Equal(myObject.Title, result.Title);
            Assert.Equal(myObject.CreatedOn, result.CreatedOn);
            Assert.Equal(Environment.MachineName, lockToken);
        }
    }
}
