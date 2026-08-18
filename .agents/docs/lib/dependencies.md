---
id: lib/dependencies
description: Lib slice dependency tree — Binacle.Lib as the single src project, its own result-selection tests kernel, who sees internals (IVT), and the composition-root rule (only Binacle.Net references the packer).
verified: 2026-08-13
check: ProjectReference and InternalsVisibleTo entries in lib/**/*.csproj match the graph below
paths:
  - "lib/**"
---

# Lib — project dependencies

The bin-packing algorithm layer. **`lib/src` holds one project, `Binacle.Lib`.** There is no separate
abstractions assembly: the packing vocabulary that callers need moved down into `shared/src/Binacle.Packing`,
and the engine interfaces folded into `Binacle.Lib` itself, under its `Abstractions/` folder.

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals.

```
Binacle.Geometry                         (shared leaf — see $shared/dependencies)
   ▲
Binacle.Packing ─────────────────────────┘   the packing vocabulary (shared/src)
   ▲   [IVT → Binacle.Lib, Binacle.Lib.TestsKernel]
   │
   ├── Binacle.Lib ──────────────────────┘   FFD/WFD/BFD algorithms, processors, result selection
   │      ▲   [IVT → UnitTests, Benchmarks, PerformanceTests]
   │      │       only Binacle.Net references the packer (composition root)
   │      │
   │      ├── Binacle.Lib.UnitTests         xUnit   refs: Lib, TestsKernel, Lib.TestsKernel
   │      ├── Binacle.Lib.Benchmarks        BDN exe refs: Lib, TestsKernel, Lib.TestsKernel
   │      └── Binacle.Lib.PerformanceTests  exe     refs: Lib, TestsKernel, TestReporting
   │
   └── Binacle.Lib.TestsKernel ──────────┘   result-selection fixture hub (lib/test)
          refs: Binacle.Packing, Binacle.CompactNotation
          embeds lib/data/result-selection under the manifest prefix "ResultSelection."
```

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.Lib` | library | Packing | grants IVT to its three test projects | the algorithms, processors, result selection |
| `Binacle.Lib.TestsKernel` | library | Packing, CompactNotation | sees Packing's | result-selection fixtures + providers |
| `Binacle.Lib.UnitTests` | xUnit exe | Lib, TestsKernel, Lib.TestsKernel | yes | algorithm/result unit tests |
| `Binacle.Lib.Benchmarks` | exe | Lib, TestsKernel, Lib.TestsKernel | yes | BenchmarkDotNet timings |
| `Binacle.Lib.PerformanceTests` | exe | Lib, TestsKernel, TestReporting | yes | markdown perf reports |

`TestsKernel` above is the shared algorithm kernel in `shared/test`; `Lib.TestsKernel` is this slice's own.

## Notes

1. **Composition-root rule.** Only `Binacle.Net` references `Binacle.Lib`, and only to wire the packer up. The
   api `Kernel`, both modules and the integration suite are all off lib entirely — what they need is the result
   vocabulary, and that is `Binacle.Packing` in `shared/src`. Keep it that way: a new consumer should take
   `Binacle.Packing`, not `Binacle.Lib`.

2. **Two tests kernels, split by audience.** The shared `Binacle.TestsKernel` holds the algorithm fixtures, which
   the api integration suite reads too. `Binacle.Lib.TestsKernel` holds result selection, which nothing outside
   this slice reads — so its fixtures live in `lib/data` and it embeds them itself. Each kernel owns its own
   embedded-resource reader because `Assembly.GetExecutingAssembly()` resolves to the assembly holding the data.

3. **The friend grant is what lets the kernel fabricate results.** `Binacle.Packing`'s result models have internal
   constructors; `Binacle.Lib.TestsKernel` builds them from compact strings, and uses Packing's internal
   `Dimensions` struct to satisfy `PackedBin`. That grant annotates the kernel's existing reference to Packing —
   it does not add an edge, and nothing in `shared` depends on `lib` because of it.

4. **Nothing enforces the abstractions boundary.** Only convention stops an interface under
   `Binacle.Lib/Abstractions/` naming a concrete algorithm. That is the sharpest candidate rule for a
   type-level architecture check.
