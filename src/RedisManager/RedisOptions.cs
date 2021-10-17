namespace Jnz.RedisRepository
{
    public class RedisOptions
    {
        public string[] Hosts { get; set; }
        public int SyncTimeout { get; set; }
        public int AsyncTimeout { get; set; }
        public int KeepAlive { get; set; }
    }
}
