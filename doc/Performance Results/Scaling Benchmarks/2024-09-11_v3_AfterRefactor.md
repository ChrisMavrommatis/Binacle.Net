# Scaling Benchmark Results: 2024-09-03

Part of V3 experiment after refactoring the algorithms

![2024-09-11_v3_AfterRefactor](https://github.com/user-attachments/assets/2c3b52d8-317c-4a20-b3bd-b4abce7c8e53)


Chart made with https://chartbenchmark.net/

----


```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4412/22H2/2022Update) (VirtualBox)
Intel Core i5-4570 CPU 3.20GHz (Haswell), 1 CPU, 2 logical and 2 physical cores
.NET SDK 8.0.400-preview.0.24324.5
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method              | NoOfItems | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0    | Allocated | Alloc Ratio |
|-------------------- |---------- |----------:|---------:|---------:|------:|--------:|--------:|----------:|------------:|
| **Fitting_FFD_V1**      | **10**        |  **19.46 μs** | **0.282 μs** | **0.250 μs** |  **1.00** |    **0.02** |  **1.7395** |   **5.41 KB** |        **1.00** |
| Fitting_FFD_V1_Full | 10        |  20.03 μs | 0.232 μs | 0.217 μs |  1.03 |    0.02 |  1.9836 |   6.16 KB |        1.14 |
| Fitting_FFD_V2      | 10        |  12.98 μs | 0.215 μs | 0.201 μs |  0.67 |    0.01 |  1.2360 |    3.8 KB |        0.70 |
| Fitting_FFD_V2_Full | 10        |  13.85 μs | 0.169 μs | 0.149 μs |  0.71 |    0.01 |  1.4801 |   4.55 KB |        0.84 |
| Packing_FFD_V1      | 10        |  18.98 μs | 0.353 μs | 0.330 μs |  0.98 |    0.02 |  2.1667 |   6.73 KB |        1.24 |
| Packing_FFD_V1_Full | 10        |  16.00 μs | 0.178 μs | 0.158 μs |  0.82 |    0.01 |  1.7700 |   5.45 KB |        1.01 |
|                     |           |           |          |          |       |         |         |           |             |
| **Fitting_FFD_V1**      | **192**       | **293.61 μs** | **4.605 μs** | **4.082 μs** |  **1.00** |    **0.02** | **23.4375** |   **72.2 KB** |        **1.00** |
| Fitting_FFD_V1_Full | 192       | 309.60 μs | 4.868 μs | 4.315 μs |  1.05 |    0.02 | 27.3438 |   83.9 KB |        1.16 |
| Fitting_FFD_V2      | 192       | 190.95 μs | 3.789 μs | 3.544 μs |  0.65 |    0.01 | 13.6719 |  42.22 KB |        0.58 |
| Fitting_FFD_V2_Full | 192       | 204.22 μs | 2.795 μs | 2.615 μs |  0.70 |    0.01 | 17.5781 |  53.91 KB |        0.75 |
| Packing_FFD_V1      | 192       | 367.30 μs | 6.802 μs | 6.362 μs |  1.25 |    0.03 | 25.8789 |  79.66 KB |        1.10 |
| Packing_FFD_V1_Full | 192       | 316.33 μs | 3.510 μs | 2.931 μs |  1.08 |    0.02 | 19.0430 |   58.9 KB |        0.82 |
|                     |           |           |          |          |       |         |         |           |             |
| **Fitting_FFD_V1**      | **202**       |  **50.94 μs** | **0.479 μs** | **0.425 μs** |  **1.00** |    **0.01** |  **1.5259** |   **4.75 KB** |        **1.00** |
| Fitting_FFD_V1_Full | 202       |  66.60 μs | 1.308 μs | 1.343 μs |  1.31 |    0.03 |  5.4932 |  16.84 KB |        3.54 |
| Fitting_FFD_V2      | 202       |  43.71 μs | 0.734 μs | 0.651 μs |  0.86 |    0.01 |  5.0659 |  15.63 KB |        3.29 |
| Fitting_FFD_V2_Full | 202       |  59.03 μs | 0.975 μs | 1.160 μs |  1.16 |    0.02 |  9.0332 |  27.71 KB |        5.83 |
| Packing_FFD_V1      | 202       |  32.40 μs | 0.405 μs | 0.359 μs |  0.64 |    0.01 |  5.2490 |   16.1 KB |        3.39 |
| Packing_FFD_V1_Full | 202       | 354.42 μs | 4.494 μs | 3.983 μs |  6.96 |    0.09 | 20.0195 |  61.41 KB |       12.93 |
