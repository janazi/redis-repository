using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace Jnz.RedisRepository.Benchmark
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ComplexObjectBenchmark>();
        }
    }
}
