using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests
{
    [Collection(nameof(RedisCollection))]
    public class RedisLockManagerTests(Services services)
    {
        private readonly ServiceProvider _serviceProvider = services.ServiceProvider;
        private const int DatabaseNumber = 0;

        [Fact]
        public async Task ShouldReleaseLock()
        {
            const string key = "TestLockRelease:112";


            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();

            var lockAcquired = await redisLockManager.GetLockAsync(key, TimeSpan.FromMilliseconds(10000000), DatabaseNumber);

            Assert.True(lockAcquired);

            var isLockReleased = await redisLockManager.ReleaseLockAsync(key, 0);

            Assert.True(isLockReleased);
        }

        [Fact]
        public async Task ShouldAcquireLock()
        {
            const string key = "TestLockAcquire";
            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();
            var lockAcquired = await redisLockManager.GetLockAsync(key, TimeSpan.FromSeconds(1), DatabaseNumber);

            Assert.True(lockAcquired);

            var anotherLockAttempt = await redisLockManager.GetLockAsync(key, TimeSpan.FromSeconds(1), DatabaseNumber);

            Assert.False(anotherLockAttempt);
        }
    }
}