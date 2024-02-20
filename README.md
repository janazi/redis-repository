# Introduction 
This project intend to make easy the use of Redis. It was made as a wrapper for the StackExchange.Redis library.
Jnz Repository makes use of MessagePack to serialize and deserialize objects to and from Redis. 
MessagePack is a fast and compact binary serialization format. It is faster and smaller than JSON.
Jnz Repository also provide will a way to create distributed lock that are necessary to deal with Microservices concurrency.

# Getting Started
To initialize Jnz Repository you have two different options.
1 - Add a section on your appsettings.json file with the following configurations:
{
  "RedisOptions": {
    "Hosts": [ "127.0.0.1:6379" ],
    "SyncTimeout": 5000,
    "AsyncTimeout": 5000,
    "KeepAlive": 180,
    "AllowAdmin": false,
    "ConnectRetry": 3
  }
}
- SyncTimeout: The time in milliseconds to wait for a synchronous operation to complete.
- AsyncTimeout: The time in milliseconds to wait for an asynchronous operation to complete.
- KeepAlive: The time in seconds to keep the connection alive.
- AllowAdmin: Allow admin operations.
- ConnectRetry: The number of times to retry to connect to the server.
- Hosts: The list of hosts to connect to.
- Password: The password to connect to the server. (optional)
- Ssl: Use SSL to connect to the server. (optional default: false)

To use this option you must set up your appSettings and the call the extension method AddRedisRepository on your IServiceCollection passing
the IConfiguration as parameter.
e.g.

builder.Services.AddRedisRepository(builder.Configuration);

2 - Mannually creation of the RedisOptions object
e.g.
builder.Services.AddRedisRepository(new RedisOptions(
{
    Hosts = ["127.0.0.1:6379"],
    SyncTimeout = 5000,
    AsyncTimeout = 5000,
    KeepAlive = 180,
    ConnectRetry = 3
});

The initilization will create a singleton of the IRedisRepository that will be used to access the Redis database and IRedisLockManager that will be used to create distributed locks.

# Usage
To use the IRedisRepository you must inject it on your class constructor and use the methods to access the Redis database.
To use the IRedisLockManager you must inject it on your class constructor and call
