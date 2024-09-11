# Scaling Benchmark Results: 2024-09-03

Part of V3 experiment after refactoring the algorithms

![2024-09-11_v3_AfterRefactor](https://github.com/user-attachments/assets/0d1028b1-5ced-4d78-9c07-1b782ab5d298)

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
| **Fitting_FFD_V1**      | **10**        |  **19.32 μs** | **0.302 μs** | **0.283 μs** |  **1.00** |    **0.02** |  **1.7395** |   **5.41 KB** |        **1.00** |
| Fitting_FFD_V1_Full | 10        |  19.33 μs | 0.258 μs | 0.216 μs |  1.00 |    0.02 |  1.9836 |   6.16 KB |        1.14 |
| Fitting_FFD_V2      | 10        |  12.47 μs | 0.105 μs | 0.093 μs |  0.65 |    0.01 |  1.2360 |    3.8 KB |        0.70 |
| Fitting_FFD_V2_Full | 10        |  14.44 μs | 0.283 μs | 0.503 μs |  0.75 |    0.03 |  1.4801 |   4.55 KB |        0.84 |
| Packing_FFD_V1      | 10        |  19.09 μs | 0.213 μs | 0.189 μs |  0.99 |    0.02 |  2.1667 |   6.73 KB |        1.24 |
| Packing_FFD_V1_Full | 10        |  19.63 μs | 0.392 μs | 0.495 μs |  1.02 |    0.03 |  2.1973 |   6.76 KB |        1.25 |
|                     |           |           |          |          |       |         |         |           |             |
| **Fitting_FFD_V1**      | **192**       | **300.83 μs** | **5.344 μs** | **5.718 μs** |  **1.00** |    **0.03** | **23.4375** |   **72.2 KB** |        **1.00** |
| Fitting_FFD_V1_Full | 192       | 313.35 μs | 2.783 μs | 2.603 μs |  1.04 |    0.02 | 27.3438 |   83.9 KB |        1.16 |
| Fitting_FFD_V2      | 192       | 198.79 μs | 2.879 μs | 2.693 μs |  0.66 |    0.01 | 13.6719 |  42.22 KB |        0.58 |
| Fitting_FFD_V2_Full | 192       | 208.56 μs | 2.982 μs | 2.789 μs |  0.69 |    0.02 | 17.5781 |  53.91 KB |        0.75 |
| Packing_FFD_V1      | 192       | 354.73 μs | 5.545 μs | 4.916 μs |  1.18 |    0.03 | 25.8789 |  79.66 KB |        1.10 |
| Packing_FFD_V1_Full | 192       | 358.16 μs | 6.763 μs | 5.995 μs |  1.19 |    0.03 | 25.8789 |   79.7 KB |        1.10 |
|                     |           |           |          |          |       |         |         |           |             |
| **Fitting_FFD_V1**      | **202**       |  **52.13 μs** | **0.690 μs** | **0.576 μs** |  **1.00** |    **0.02** |  **1.5259** |   **4.75 KB** |        **1.00** |
| Fitting_FFD_V1_Full | 202       |  67.77 μs | 1.027 μs | 0.961 μs |  1.30 |    0.02 |  5.4932 |  16.84 KB |        3.54 |
| Fitting_FFD_V2      | 202       |  44.16 μs | 0.232 μs | 0.193 μs |  0.85 |    0.01 |  5.0659 |  15.63 KB |        3.29 |
| Fitting_FFD_V2_Full | 202       |  59.43 μs | 0.779 μs | 0.691 μs |  1.14 |    0.02 |  9.0332 |  27.71 KB |        5.83 |
| Packing_FFD_V1      | 202       |  32.27 μs | 0.244 μs | 0.216 μs |  0.62 |    0.01 |  5.2490 |   16.1 KB |        3.39 |
| Packing_FFD_V1_Full | 202       |  50.75 μs | 0.993 μs | 1.063 μs |  0.97 |    0.02 | 12.2681 |  37.66 KB |        7.93 |
