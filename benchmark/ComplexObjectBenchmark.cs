using BenchmarkDotNet.Attributes;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Benchmark;
[MemoryDiagnoser]
public class ComplexObjectBenchmark
{
    private readonly IConnectionMultiplexer connectionMultiplexer;
    private readonly RedisRepository repository;
    public ComplexObjectBenchmark()
    {
        connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
        repository = new RedisRepository(connectionMultiplexer);
    }

    [Benchmark(Baseline = true)]
    public async Task WithDefaultDBAsync()
    {
        for (var i = 0; i < 2; i++)
        {
            var person = new Order(i);
            await repository.SetAsync($"order:{i}", person);
        }

    }

    [Benchmark]
    public async Task PassingDbNumberAsync()
    {
        for (var i = 0; i < 2; i++)
        {
            var person = new Order(i);
            await repository.SetAsync($"order:{i}", person, databaseNumber: 1);
        }

    }
}
