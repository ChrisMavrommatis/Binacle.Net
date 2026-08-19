---
id: lib/algorithm-factory
description: IAlgorithmFactory — how algorithm instances are created, DI registration, and how tests construct algorithms directly
verified: 2026-08-19
check: The signature, constraints and switch arms match lib/src/Binacle.Lib/AlgorithmFactory.cs and both files under AlgorithmFactories/; DI registration matches api/src/Binacle.Net/ExtensionMethods/ServiceCollectionExtensions.cs; a grep for AlgorithmFactory_v1 and AlgorithmFactory_v2 over lib/ lands only in the benchmarks
also_update:
  - lib/algorithms
paths:
  - "lib/src/Binacle.Lib/AlgorithmFactor*"

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

`Algorithm` is `Binacle.Packing.Algorithm` — the packing vocabulary in `shared/src`, not a lib type
(`$lib/models`). The returned `IPackingAlgorithm` (`Binacle.Lib.Abstractions.Algorithms`) exposes `Algorithm`,
`Version`, and `Execute(IOperationParameters parameters)`.

## Registered Implementation

`AlgorithmFactory` (public) is the DI-registered implementation. It always uses v2:

| Enum | Implementation |
|---|---|
| `Algorithm.FFD` | `FirstFitDecreasing_v2` |
| `Algorithm.WFD` | `WorstFitDecreasing_v2` |
| `Algorithm.BFD` | `BestFitDecreasing_v2` |

Throws `NotSupportedException` for any other value.
Class files live under `lib/src/Binacle.Lib/Algorithms/<Heuristic> v<N>/` — see `$lib/algorithms`.

`AlgorithmFactory_v1` and `AlgorithmFactory_v2` (`lib/src/Binacle.Lib/AlgorithmFactories/`) are the same
switch pinned to one version each. Both are `internal` and used for benchmarks only
(`lib/test/Binacle.Lib.Benchmarks`, BenchmarkDotNet runner), which reaches them through the
`InternalsVisibleTo` in `Binacle.Lib.csproj`.

## DI Registration

Registered as a singleton in `AddBinacleServices()`. Note this lives in the **API** project, not in `lib/` —
`api/src/Binacle.Net/ExtensionMethods/ServiceCollectionExtensions.cs`. The `lib/` projects contain no DI wiring.

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
See Lib Tests (`$lib/tests`) for `AlgorithmFactories` and `CommonTestingFixture`, and Shared (`$shared`)
for how scenarios are structured.
