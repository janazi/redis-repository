using Jnz.RedisRepository.Interfaces;
using System;

namespace Jnz.RedisRepository.Benchmark
{
    //[MessagePackObject]
    public class Pessoa : IRedisCacheable
    {
        public Pessoa() { }
        public Pessoa(string cpf)
        {

        }

        //[Key(0)]
        public string Nome { get; set; }

        //[Key(1)]
        public long Cpf { get; set; }

        public string GetIndex()
        {
            return "Pessoa";
        }

        public int GetDatabaseNumber()
        {
            return 1;
        }

        public TimeSpan? GetExpiration()
        {
            return TimeSpan.FromSeconds(60);
        }

        public string GetKey()
        {
            return Cpf.ToString();
        }

    }
}
