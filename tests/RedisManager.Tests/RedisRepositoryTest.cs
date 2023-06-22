using Jnz.RedisRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RedisManager.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Jnz.RedisRepository.Tests
{
    [Collection("Sequential")]
    public class RedisRepositoryTest : IClassFixture<Services>
    {
        private readonly ServiceProvider _serviceProvider;
        private const string DecimalKey = "DecimalKey:1";
        private const string KeyForSet = "SET:123";

        public RedisRepositoryTest(Services services)
        {
            _serviceProvider = services.ServiceProvider;
        }

        [Fact]
        public void Should_Save_InCache()
        {
            var obj = new MyObject
            {
                Name = "Test",
                TempoEmCache = TimeSpan.FromMilliseconds(200)
            };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj).GetAwaiter().GetResult();

            var objCache = redisRepository.GetAsync<MyObject>(obj.GetKey()).GetAwaiter()
                .GetResult();

            Assert.NotNull(objCache);
        }

        [Fact]
        public void Should_SaveACollection_ToCache()
        {
            var obj = new List<string>();

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj, "Collection", "Redis", 0).GetAwaiter().GetResult();

            var objCache = redisRepository.GetAsync<List<string>>("Redis", "Collection", 0).GetAwaiter()
                .GetResult();

            Assert.NotNull(objCache);
        }

        [Fact]
        public void Should_Get_and_Lock_Key()
        {
            var obj = new MyObject
            {
                Name = "TestLock",
                TempoEmCache = TimeSpan.FromMilliseconds(20)
            };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj).GetAwaiter().GetResult();

            var objCache = redisRepository.GetWithLockAsync<MyObject>(obj.GetKey(), TimeSpan.FromSeconds(3))
                .GetAwaiter()
                .GetResult();

            Assert.Throws<KeyLockedException>(() =>
                redisRepository.GetWithLockAsync<MyObject>(obj.GetKey(), TimeSpan.FromSeconds(1)).GetAwaiter()
                    .GetResult());
        }


        [Fact]
        public void Should_Be_Remored_After_Expiration_500ms()
        {
            var obj = new MyObject { Name = "TestLock", TempoEmCache = TimeSpan.FromMilliseconds(550) };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj).GetAwaiter().GetResult();

            Thread.Sleep(550);

            var objCache = redisRepository.GetAsync<MyObject>(obj.GetKey()).GetAwaiter()
                .GetResult();

            Assert.Null(objCache);
        }

        [Fact]
        public void Deve_Remover_Key()
        {
            const string name = "TestLock";
            var obj = new MyObject { Name = name };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj).GetAwaiter().GetResult();

            var objCache = redisRepository.GetAsync<MyObject>(obj.GetKey()).GetAwaiter()
                .GetResult();

            Assert.NotNull(objCache);

            redisRepository.DeleteKeyAsync<MyObject>(name).GetAwaiter().GetResult();

            objCache = redisRepository.GetAsync<MyObject>(obj.GetKey()).GetAwaiter()
                .GetResult();

            Assert.Null(objCache);
        }

        [Fact]
        public void Deve_Remover_apenas_uma_hash()
        {
            const string key = "TestUmaHash";
            const string hash = "Hash";
            const string hash2 = "Hash2";

            var obj = new MyObject { Name = key, TempoEmCache = TimeSpan.FromMilliseconds(20) };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetHashAsync(obj, key, hash).GetAwaiter().GetResult();
            redisRepository.SetHashAsync(obj, key, hash2).GetAwaiter().GetResult();

            var hashCache = redisRepository.GetHashAsync<MyObject>(key, hash).GetAwaiter().GetResult();

            Assert.NotNull(hashCache);

            redisRepository.DeleteHashAsync<MyObject>(key, hash).GetAwaiter().GetResult();

            hashCache = redisRepository.GetHashAsync<MyObject>(key, hash).GetAwaiter().GetResult();

            Assert.Null(hashCache);

            var hash2Obj = redisRepository.GetHashAsync<MyObject>(key, hash2).GetAwaiter().GetResult();
            Assert.NotNull(hash2Obj);
            redisRepository.DeleteHashAsync<MyObject>(key, hash2).GetAwaiter().GetResult();
        }

        [Fact]
        public void Deve_Obter_Todas_Chaves_Por_Indice()
        {
            Thread.Sleep(300); // time to expire previous tests keys
            const string key = "TestLock";
            const string key2 = "TestLock2";

            var obj = new MyObject { Name = key };
            var obj2 = new MyObject { Name = key2 };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj).GetAwaiter().GetResult();
            redisRepository.SetAsync(obj2).GetAwaiter().GetResult();

            var keys = redisRepository.GetAllKeysByPattern(obj.GetDatabaseNumber(), "MyObject:*");

            Assert.Equal(2, keys.Count());
        }


        [Fact]
        public void Deve_Gravar_e_Buscar_data()
        {
            var data = DateTime.Now;
            var obj = new MyObject
            {
                Name = "TestData",
                Data = DateTime.Now,
                TempoEmCache = TimeSpan.FromMilliseconds(300)
            };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj).GetAwaiter().GetResult();

            var objCache = redisRepository.GetAsync<MyObject>(obj.GetKey()).GetAwaiter()
                .GetResult();


            Assert.Equal(data.Hour, objCache.Data.Value.Hour);
        }

        [Fact]
        public void Deve_Gravar_Objeto_Informando_Indice()
        {
            const string index = "NovoIndice";
            var obj = new MyObject { Name = index, TempoEmCache = TimeSpan.FromMilliseconds(300) };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.Set(obj, index);

            var objIndice = redisRepository.Get<MyObject>(index, obj.GetKey());

            Assert.NotNull(objIndice);
        }

        [Fact]
        public void Deve_Gravar_Hash_Informando_Indice()
        {
            const string index = "NovoIndice";
            const string hash = "Hash";
            var obj = new MyObject { Name = "Indice", TempoEmCache = TimeSpan.FromMilliseconds(20) };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetHash(obj, obj.GetKey(), hash, index);

            var objIndice = redisRepository.GetHash<MyObject>(obj.GetKey(), hash, index);

            Assert.NotNull(objIndice);
        }

        [Fact]
        public async Task WhenCallingUsingClass_ShouldCreateHashUsingParameters()
        {
            const string index = "MyObject";
            const string hash = "UniqueIdInsideHash";
            const string key = "MyObjectUniqueKey";
            const int databaseNumber = 0;
            var obj = new MyObjectWithoutInterface { Title = "Hash", CreatedOn = DateTime.Now.ToUniversalTime() };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            await redisRepository.SetHashAsync(obj, key, hash, index, databaseNumber);

            var objSaved = await redisRepository.GetHashAsync<MyObjectWithoutInterface>(key, hash, index, 0);

            Assert.NotNull(objSaved);

            await redisRepository.DeleteHashAsync(key, hash, index, databaseNumber);

            var objDeleted = await redisRepository.GetHashAsync<MyObjectWithoutInterface>(key, hash, index, 0);

            Assert.Null(objDeleted);
        }

        [Fact]
        public async Task ShouldReturnAllHashEntities()
        {
            const string index = "MyObject";
            const string hash = "UniqueIdInsideHash";
            const string hash2 = "UniqueIdInsideHash2";
            const string key = "MyHashUniqueKey";
            const int databaseNumber = 0;

            var obj = new MyObjectWithoutInterface { Title = "HashObject", CreatedOn = DateTime.Now.ToUniversalTime() };
            var obj2 = new MyObjectWithoutInterface { Title = "HashObject2", CreatedOn = DateTime.Now.ToUniversalTime() };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            await redisRepository.SetHashAsync(obj, key, hash, index, databaseNumber);
            await redisRepository.SetHashAsync(obj2, key, hash2, index, databaseNumber);

            var myObjects = await redisRepository
                .GetAllHashAsync<MyObjectWithoutInterface>(key, index, databaseNumber);

            Assert.NotNull(myObjects);
            Assert.Equal(2, myObjects.Count());

            await redisRepository.DeleteHashAsync(key, hash, index, databaseNumber);
            await redisRepository.DeleteHashAsync(key, hash2, index, databaseNumber);

        }

        [Fact]
        public async Task ShouldCreate_FloatKeyAsync()
        {
            var dataBaseNumber = 0;
            var initialValue = 10.25f;
            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            var savedValue = await redisRepository.IncrementByDecimal(DecimalKey, (decimal)initialValue, dataBaseNumber);
            Assert.Equal((decimal)initialValue, savedValue);
            decimal valueToIncrement = 10;
            savedValue = await redisRepository.IncrementByDecimal(DecimalKey, valueToIncrement, dataBaseNumber);
            var newValue = (decimal)initialValue + valueToIncrement;
            Assert.Equal(newValue, savedValue);
            await redisRepository.DeleteKeyAsync(DecimalKey, dataBaseNumber);
        }

        [Fact]
        public async Task ShouldCreateSet_WithMembers()
        {
            var memberValue = "Value";
            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            await redisRepository.SetAddAsync(0, KeyForSet, memberValue);
            var data = await redisRepository.GetSetMembersAsync(0, KeyForSet);
            await redisRepository.RemoveEntireSetAsync(0, KeyForSet);
            Assert.Equal(memberValue, data[0].ToString());
        }

        [Fact]
        public async Task ShouldCreateSet_WithMembersUsingGenerics()
        {
            var memberValue = new MyObject() { Name = "teste" };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            await redisRepository.SetAddAsync<MyObject>(0, KeyForSet, memberValue);
            var data = await redisRepository.GetSetMembersAsync<MyObject>(0, KeyForSet);
            await redisRepository.RemoveEntireSetAsync(0, KeyForSet);
            Assert.Equal(memberValue.Name, data[0].Name);
        }

        [Fact]
        public async Task ShouldRemove_Member()
        {
            var memberValue = "Value";
            var member2Value = "Value2";
            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            await redisRepository.SetAddAsync(0, KeyForSet, memberValue);
            await redisRepository.SetAddAsync(0, KeyForSet, member2Value);


            var objects = await redisRepository.GetSetMembersAsync(0, KeyForSet);
            Assert.Equal(2, objects.Count);

            await redisRepository.RemoveMemberSetAsync(0, KeyForSet, memberValue);
            objects = await redisRepository.GetSetMembersAsync(0, KeyForSet);
            await redisRepository.RemoveEntireSetAsync(0, KeyForSet);
            Assert.Single(objects);
            Assert.Contains(member2Value, objects);

        }

        [Fact]
        public async Task ShouldRemove_MemberUsingGenerics()
        {
            var memberValue = new MyObject() { Name = "teste" };
            var member2Value = new MyObject() { Name = "teste2" };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();
            await redisRepository.SetAddAsync<MyObject>(0, KeyForSet, memberValue);
            await redisRepository.SetAddAsync<MyObject>(0, KeyForSet, member2Value);


            var objects = await redisRepository.GetSetMembersAsync<MyObject>(0, KeyForSet);
            Assert.Equal(2, objects.Count);

            await redisRepository.RemoveMemberSetAsync(0, KeyForSet, memberValue);
            objects = await redisRepository.GetSetMembersAsync<MyObject>(0, KeyForSet);
            await redisRepository.RemoveEntireSetAsync(0, KeyForSet);
            Assert.Single(objects);
            Assert.Contains(member2Value, objects);

        }

        [Fact]
        public async Task ShouldGet_WithLock()
        {
            const string indexName = "MyObj";
            const string objKey = "1";
            const string indexWithKey = $"{indexName}:{objKey}";
            var createdDate = DateTime.Now;
            var obj = new MyObjectWithoutInterface
            {
                Title = "TestLock",
                CreatedOn = createdDate
            };

            var redisRepository = _serviceProvider.GetService<IRedisRepository>();

            redisRepository.SetAsync(obj, objKey, indexName).GetAwaiter().GetResult();

            var objCache = redisRepository.GetWithLockAsync<MyObjectWithoutInterface>(indexWithKey, TimeSpan.FromSeconds(10), 0)
                .GetAwaiter()
                .GetResult();

            Assert.NotNull(objCache);
            Assert.Throws<KeyLockedException>(() =>
                redisRepository.GetWithLockAsync<MyObjectWithoutInterface>(indexWithKey, TimeSpan.FromSeconds(1), 0).GetAwaiter()
                    .GetResult());
        }

    }
}