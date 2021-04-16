using MessagePack.Resolvers;
using Polly;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<MessagePackContractlessStandarResolverBenchmark>();
            TestRedisAsync().GetAwaiter().GetResult();
        }

        public static async Task TestRedisAsync()
        {
            try
            {
                var options = new ConfigurationOptions()
                {
                    EndPoints = { "127.0.0.1" },
                    KeepAlive = 18
                };
                var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(options);
                var redisSerializer = new RedisSerializer(ContractlessStandardResolver.Instance);
                var redisLockManager = new RedisLockManager(connectionMultiplexer);
                var redisRepository = new RedisRepository(connectionMultiplexer, redisSerializer, redisLockManager);



                //var c = new CacheService();
                var cpf = 27805028868;
                var p = new Pessoa(cpf.ToString())
                {
                    Cpf = cpf,
                    Nome = "Marcelo"
                };

                var bin = redisSerializer.Serialize(p);

                var pessoa = redisSerializer.DeserializeAsync<Pessoa>(bin);


                await redisRepository.SetAsync(p);


                var p2 = await redisRepository.GetWithLockAsync<Pessoa>(cpf.ToString(), TimeSpan.FromMilliseconds(3000));

                Console.WriteLine(p2.Nome);


                var policy = Policy
                    .Handle<KeyLockedException>()
                    .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromMilliseconds(20),
                        TimeSpan.FromMilliseconds(50),
                        TimeSpan.FromMilliseconds(3040)

                   });


                var pessoaDoCache = await policy.ExecuteAsync(() => redisRepository.GetWithLockAsync<Pessoa>(cpf.ToString(), TimeSpan.FromMilliseconds(50000)));

                Console.WriteLine($"CPF: {pessoaDoCache.Cpf}");
                //await redisRepository.ReleaseLockAsync<Pessoa>(pessoaDoCache.Cpf.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}
