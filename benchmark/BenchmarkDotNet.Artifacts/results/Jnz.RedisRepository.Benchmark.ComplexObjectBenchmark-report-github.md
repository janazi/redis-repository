```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.400
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method               | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| WithDefaultDBAsync   | 619.9 μs | 12.32 μs | 24.89 μs |  1.00 |    0.06 |   2.35 KB |        1.00 |
| PassingDbNumberAsync | 654.3 μs | 15.88 μs | 45.82 μs |  1.06 |    0.09 |   2.35 KB |        1.00 |
