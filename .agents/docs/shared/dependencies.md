---
id: shared/dependencies
description: Shared slice dependency tree — Geometry (the BCL-only leaf everything geometric bottoms out on), CompactNotation, TestReporting, and the TestsKernel test hub; who references them and who sees internals.
verified: 2026-07-14
check: ProjectReference and InternalsVisibleTo entries in shared/**/*.csproj match the graph and notes below
---

# Shared — project dependencies

The foundation slice. These projects have no dependency on `api`, `lib`, or `vipaq` code (with one deliberate
exception, noted below), so everything else can lean on them.

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals.

```
Binacle.Geometry                 leaf — BCL only, no Binacle deps
   ▲                             geometry types + IWith[ReadOnly]Dimensions/Coordinates<T>
   │                             (consumed by CompactNotation, Lib.Abstractions, ViPaq)
   │
Binacle.CompactNotation ─────────┘   "LxWxH (X,Y,Z) [Q]" parser/formatter
   ▲   [IVT → CompactNotation.UnitTests]
   │
   ├── Binacle.CompactNotation.UnitTests   xUnit
   │
   └── Binacle.TestsKernel               shared TEST hub — scenario data, providers, fixtures
          refs: Binacle.Lib.Abstractions (!), Binacle.CompactNotation
          consumers: Binacle.Lib.UnitTests/Benchmarks/PerformanceTests, api IntegrationTests

Binacle.TestReporting            leaf — markdown report writer, no Binacle deps
   consumers: Binacle.Lib.PerformanceTests, ViPaq.PerformanceTests, ViPaq generators, OrLibrary.Converter

shared/tools/Binacle.OrLibrary.Converter   exe tool
   refs: Binacle.CompactNotation, Binacle.TestReporting
```

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.Geometry` | library | — (BCL only) | — | the geometry leaf: `IWith[ReadOnly]*` + `Dimensions<T>`/`Coordinates<T>`/`Item<T>` |
| `Binacle.CompactNotation` | library | Geometry | grants IVT to its UnitTests | parses/formats the `LxWxH (X,Y,Z)` compact string |
| `Binacle.CompactNotation.UnitTests` | xUnit exe | CompactNotation | yes | notation tests |
| `Binacle.TestReporting` | library | — | — | markdown report writer for the perf harnesses |
| `Binacle.TestsKernel` | library | Lib.Abstractions, CompactNotation | — | shared test data + fixtures (see the cross-slice note) |
| `Binacle.OrLibrary.Converter` | exe tool | CompactNotation, TestReporting | — | converts OR-Library benchmark data |

## Notes

1. **`Binacle.Geometry` is the true leaf.** It has no Binacle dependency and only the BCL. Every geometric type in
   the repo (lib, api, vipaq) bottoms out here, which is why it can be shared without dragging anything along.

2. **`Binacle.TestsKernel` is test-support that references production `Lib.Abstractions`.** That is the one
   inward edge from the shared slice into `lib` — deliberate, because the shared scenarios are typed against the
   lib's abstraction interfaces so lib and api tests can share them. It is a *test* library; nothing shipped
   references it. (Not to be confused with `Binacle.ViPaq.TestsKernel`, a separate ViPaq-only hub —
   see `$vipaq/dependencies`.)

3. **`Binacle.TestReporting` has no Binacle deps** — a plain writer, safe for any harness to reference.
