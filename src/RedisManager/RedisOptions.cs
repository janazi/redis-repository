namespace Jnz.RedisRepository
{
    public class RedisOptions
    {

        public string[] Hosts { get; set; }
        public int SyncTimeout { get; set; }
        public int AsyncTimeout { get; set; }
        public int KeepAlive { get; set; }
        public string Password { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public bool AllowAdmin { get; set; }
        public bool Ssl { get; set; }
        public int ConnectRetry { get; set; }
    }
}
