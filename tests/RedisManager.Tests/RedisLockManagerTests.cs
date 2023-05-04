using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RedisManager.Tests;
using System;
using Xunit;

namespace Jnz.RedisRepository.Tests
{

    public class RedisLockManagerTests : IClassFixture<Services>
    {
        private readonly ServiceProvider _serviceProvider;

        public RedisLockManagerTests(Services services)
        {
            _serviceProvider = services.ServiceProvider;
        }

        [Fact]
        public void Should_Release_Lock()
        {
            const string key = "TestLockRelease";


            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();

            var lockAcquired = redisLockManager.GetLockAsync(key, 0, TimeSpan.FromMilliseconds(1000)).GetAwaiter()
                .GetResult();

            Assert.True(lockAcquired);

            var islockReleased = redisLockManager.ReleaseLockAsync(key, 0).GetAwaiter().GetResult();

            Assert.True(islockReleased);
        }

        [Fact]
        public void Shoud_Acquire_Lock()
        {
            const string key = "TestLockAcquire";


            var redisLockManager = _serviceProvider.GetService<IRedisLockManager>();
            var lockAcquired = redisLockManager.GetLockAsync(key, 0, TimeSpan.FromMilliseconds(150)).GetAwaiter()
                .GetResult();

            Assert.True(lockAcquired);

            var anotherLockAttempt = redisLockManager.GetLockAsync(key, 0, TimeSpan.FromMilliseconds(110)).GetAwaiter()
                .GetResult();

            Assert.False(anotherLockAttempt);
        }
    }
}