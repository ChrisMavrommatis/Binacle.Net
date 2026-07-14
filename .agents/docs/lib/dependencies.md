---
description: Lib slice dependency tree — the Abstractions/Lib split, who sees internals (IVT), and the composition-root rule (only Binacle.Net references the concrete Binacle.Lib; everyone else uses Abstractions).
verified: 2026-07-14
check: ProjectReference and InternalsVisibleTo entries in lib/**/*.csproj match the graph below
---

# Lib — project dependencies

The bin-packing algorithm layer. It splits into interfaces (`Abstractions`) and implementations (`Lib`), and that
split is load-bearing: callers depend on the interfaces, and only the composition root pulls in the concrete lib.

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals.

```
Binacle.Geometry                         (shared leaf — see ../shared/dependencies.md)
   ▲
Binacle.Lib.Abstractions ────────────────┘   interfaces + models (Bin, Item, results, IWith*)
   ▲   [IVT → Binacle.Lib, Binacle.Lib.UnitTests, Binacle.TestsKernel]
   │       consumers beyond lib: api Kernel, api UIModule, api IntegrationTests, shared TestsKernel
   │
Binacle.Lib ─────────────────────────────┘   FFD/WFD/BFD algorithms, processors, result selection
   ▲   [IVT → UnitTests, Benchmarks, PerformanceTests]
   │       only Binacle.Net references the concrete lib (composition root)
   │
   ├── Binacle.Lib.UnitTests         xUnit   refs: Lib, TestsKernel
   ├── Binacle.Lib.Benchmarks        BDN exe refs: Lib, TestsKernel
   └── Binacle.Lib.PerformanceTests  exe     refs: Lib, TestsKernel, TestReporting
```

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.Lib.Abstractions` | library | Geometry | grants IVT | interfaces + models shared with the API layer |
| `Binacle.Lib` | library | Lib.Abstractions | grants IVT | the algorithms and processors |
| `Binacle.Lib.UnitTests` | xUnit exe | Lib, TestsKernel | yes | algorithm/result unit tests |
| `Binacle.Lib.Benchmarks` | exe | Lib, TestsKernel | yes | BenchmarkDotNet timings |
| `Binacle.Lib.PerformanceTests` | exe | Lib, TestsKernel, TestReporting | yes | markdown perf reports |

## Notes

1. **Composition-root rule.** Only `Binacle.Net` references the concrete `Binacle.Lib`. Every other consumer — the
   API `Kernel`, the `UIModule`, the integration tests — references `Binacle.Lib.Abstractions` instead. So the
   algorithms are registered in exactly one place (the API entry point) and the rest of the code depends only on
   the interfaces. Keep it that way: a new consumer should take `Abstractions`, not `Lib`.

2. **`TestsKernel` supplies the test data**, and it references `Lib.Abstractions` (see
   [../shared/dependencies.md](../shared/dependencies.md)), so lib and api tests share one scenario set.
