using BenchmarkDotNet.Attributes;
using MessagePack;

namespace Jnz.RedisRepository.Benchmark
{
    [MemoryDiagnoser]
    public class MessagePackContractlessStandarResolverBenchmark
    {
        readonly ContractlessSerializerTarget _target;
        readonly StandardSerializerTarget _targetStandard;
        public MessagePackContractlessStandarResolverBenchmark()
        {
            _target = new ContractlessSerializerTarget
            {
                MyProperty1 = 1,
                MyProperty2 = 2,
                MyProperty3 = 3,
                MyProperty4 = 4,
                MyProperty5 = 5,
                MyProperty6 = 6,
                MyProperty7 = 7,
                MyProperty8 = 8,
                MyProperty9 = 9,
            };

            _targetStandard = new StandardSerializerTarget
            {
                MyProperty1 = 1,
                MyProperty2 = 2,
                MyProperty3 = 3,
                MyProperty4 = 4,
                MyProperty5 = 5,
                MyProperty6 = 6,
                MyProperty7 = 7,
                MyProperty8 = 8,
                MyProperty9 = 9,
            };
        }

        [Benchmark(Baseline = true)]
        public void ContractlessStandardResolver()
        {
            var bytes = MessagePackSerializer.Serialize(_target, MessagePack.Resolvers.ContractlessStandardResolver.Options);

        }

        [Benchmark]
        public void StandardResolver()
        {
            var bytes = MessagePackSerializer.Serialize(_targetStandard, MessagePack.Resolvers.StandardResolver.Options);

        }
    }

    public class ContractlessSerializerTarget
    {
        public int MyProperty1 { get; set; }
        public int MyProperty2 { get; set; }
        public int MyProperty3 { get; set; }
        public int MyProperty4 { get; set; }
        public int MyProperty5 { get; set; }
        public int MyProperty6 { get; set; }
        public int MyProperty7 { get; set; }
        public int MyProperty8 { get; set; }
        public int MyProperty9 { get; set; }
    }

    [MessagePackObject]
    public class StandardSerializerTarget
    {
        [Key(0)]
        public int MyProperty1 { get; set; }
        [Key(1)]
        public int MyProperty2 { get; set; }
        [Key(2)]
        public int MyProperty3 { get; set; }
        [Key(3)]
        public int MyProperty4 { get; set; }
        [Key(4)]
        public int MyProperty5 { get; set; }
        [Key(5)]
        public int MyProperty6 { get; set; }
        [Key(6)]
        public int MyProperty7 { get; set; }
        [Key(7)]
        public int MyProperty8 { get; set; }
        [Key(8)]
        public int MyProperty9 { get; set; }
    }
}
