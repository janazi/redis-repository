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

        [Fact]
        public async Task Should_Release_Lock()
        {
            const string key = "TestLockRelease:112";


            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();

            var lockAcquired = await redisLockManager.GetLockAsync(key, 0, TimeSpan.FromMilliseconds(10000000));

            Assert.True(lockAcquired);

            var isLockReleased = await redisLockManager.ReleaseLockAsync(key, 0);

            Assert.True(isLockReleased);
        }

        [Fact]
        public void Shoud_Acquire_Lock()
        {
            const string key = "TestLockAcquire";
            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();
            var lockAcquired = redisLockManager.GetLockAsync(key, 0, TimeSpan.FromMilliseconds(11150)).GetAwaiter()
                .GetResult();

            Assert.True(lockAcquired);

            var anotherLockAttempt = redisLockManager.GetLockAsync(key, 0, TimeSpan.FromMilliseconds(110)).GetAwaiter()
                .GetResult();

            Assert.False(anotherLockAttempt);
        }
    }
}