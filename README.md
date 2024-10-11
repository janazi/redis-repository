# Introduction 
Simple abstraction to access Redis database and create distributed locks.

# Getting Started
Add StackExchange.Redis to your project.
```bash
To initialize Jnz Repository you have two different options.
1 - Set up ConnectionMultiplexer on your Startup.cs
    e.g.
    var options = new ConfigurationOptions
            {
                SyncTimeout = 5000,
                AsyncTimeout = 5000,
                KeepAlive = true / false ,
                Password = {{your-redis-password}}, // optional
                AbortOnConnectFail = true / false,
                AllowAdmin = true / false,
                ConnectRetry = true / false,
                Ssl = true / false
            };
            options.EndPoints.Add("localhost:6379");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options)); // must be singleton
2 - Use the extension method AddRedisRepository or manually add it to your IServiceCollection
    e.g.
    services.AddRedisRepository();
    or
    services.AddScoped<IRedisRepository, RedisRepository>();
