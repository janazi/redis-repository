# Introduction 
This project is about a way to use Redis as a NoSQL Database, not only as cache. It make easy to create keys in the right way, with index.
It intend to make the use of hash and distributed lock easier.

# Getting Started
There are an extesion method named AddRedisRepository that receives an IConfiguration and look for a section called RedisOptions with the following configurations:
"RedisOptions": {
    "hosts": [ "localhost:6379" ],
    "SyncTimeout": 5000, 
    "AsyncTimeout": 5000,
    "KeepAlive": 180,
    "AbortOnConnectFail": false
  }
  ps: Sample values in Sync and Async Timeout.

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://www.visualstudio.com/en-us/docs/git/create-a-readme). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)
