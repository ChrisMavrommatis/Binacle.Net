---
description: IAlgorithmFactory — how algorithm instances are created, DI registration, and how tests construct algorithms directly
verified: 2026-05-23
check: Class names and DI registration match lib/src/Binacle.Lib/AlgorithmFactory.cs
also_update:
  - lib/algorithms.md
---

# Algorithm Factory

## Interface

```csharp
IAlgorithmFactory.Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
    → IPackingAlgorithm
```

Type constraints:
- `TBin : class, IWithID, IWithReadOnlyDimensions`
- `TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity`

The returned `IPackingAlgorithm` exposes `Algorithm`, `Version`, and `Execute(parameters)`.

## Registered Implementation

`AlgorithmFactory` (public) is the DI-registered implementation. It always uses v2:

| Enum | Implementation |
|---|---|
| `Algorithm.FFD` | `FirstFitDecreasing_v2` |
| `Algorithm.WFD` | `WorstFitDecreasing_v2` |
| `Algorithm.BFD` | `BestFitDecreasing_v2` |

Throws `NotSupportedException` for any other value.
Class files live under `lib/src/Binacle.Lib/Algorithms/<Heuristic> v<N>/` — see [algorithms.md](algorithms.md).

`AlgorithmFactory_v1` and `AlgorithmFactory_v2` are `internal` — used for benchmarks only
(`lib/test/Binacle.Lib.Benchmarks`, BenchmarkDotNet runner).

## DI Registration

Registered as a singleton in `ServiceCollectionExtensions.AddBinacleServices()`:

```csharp
services.AddSingleton<IAlgorithmFactory, AlgorithmFactory>();
```

## How Tests Use It

Unit tests do **not** go through `IAlgorithmFactory`. They construct algorithm instances directly via `TestAlgorithmFactory<IPackingAlgorithm>` delegates in `AlgorithmFactories.cs`:

```csharp
AlgorithmFactories.FFD_v2 = (bin, items) => new FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);
```

`CommonTestingFixture` runs all six versions (FFD/WFD/BFD × v1/v2) against every scenario.
This keeps old versions tested without coupling them to the factory.
See [tests/scenarios.md](../tests/scenarios.md) for how scenarios and fixtures are structured.
