---
id: vipaq/dependencies
description: ViPaq project dependency tree — who references whom, who can see internals, and the deliberate walls (UnitTests never references TestsKernel; no test project references a generator).
verified: 2026-07-24
check: ProjectReference and InternalsVisibleTo entries in vipaq/**/*.csproj match the graph and the boundary rules below
---

# ViPaq — project dependencies

One picture of how the ViPaq projects fit together, plus the boundaries that are easy to break by accident.
The *why* of the format is in `$vipaq/architecture` and the design decisions behind it; this file is
just the wiring.

## The graph

Arrows point at what a project references. `[IVT]` marks a project that can see `Binacle.ViPaq` internals
(`Header`, `ProtocolEncoder`, `Layout`, `Width`, the codecs).

```
Binacle.Geometry                    leaf — geometry types + IWith[ReadOnly]Dimensions/Coordinates
   ▲   ▲   ▲
   │   │   └── Binacle.CompactNotation      "LxWxH (X,Y,Z)" parser/formatter
   │   │
   │   └────── Binacle.ViPaq  [grants IVT]  the format (reference implementation)
   │              ▲   ▲   ▲   ▲
   │              │   │   │   │
   │              │   │   │   └── Binacle.ViPaq.UnitTests        [IVT]  xUnit — spec/correctness
   │              │   │   │           refs: ViPaq, CompactNotation
   │              │   │   │           NO ref to TestsKernel (deliberate)
   │              │   │   │
   │              │   │   └────── Binacle.ViPaq.TestsKernel      [IVT]  library — real-data hub
   │              │   │               refs: ViPaq, Geometry, CompactNotation
   │              │   │               owns: the 716 frozen packs, providers, protobuf,
   │              │   │                     ViPaqEncoder/ViPaqHeader (drives ProtocolEncoder)
   │              │   │                  ▲          ▲
   │              │   │                  │          └── Binacle.ViPaq.PerformanceTests  [IVT]  exe
   │              │   │                  │                  refs: TestsKernel, TestReporting
   │              │   │                  │                  runs RoundTripCheck + size/codec reports
   │              │   │                  │
   │              │   │                  └───────────────── Binacle.ViPaq.Benchmarks    [IVT]  exe
   │              │   │                                          refs: TestsKernel (BenchmarkDotNet)
   │              │   │
   │              │   └── Binacle.ViPaq.VectorGenerators  [IVT]  tool exe — regenerates test-vectors/
   │              │           refs: ViPaq, CompactNotation, TestReporting
   │              │
   │              └────── Binacle.ViPaq.PackedDataGenerator  (no IVT)  tool exe — freezes data/packed/
   │                          refs: Lib, Lib.Abstractions, ViPaq, CompactNotation, Geometry, TestReporting
   │
   └── lib/src/Binacle.Lib (+ .Abstractions)   the packing engine — reached only by PackedDataGenerator
```

`Binacle.TestReporting` (a shared markdown-report writer) is referenced by PerformanceTests and both generators.

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.ViPaq` | library | Geometry | grants IVT | the format; everything but the public surface is `internal` |
| `Binacle.ViPaq.UnitTests` | xUnit exe | ViPaq, CompactNotation | yes | spec/correctness — vectors + curated inputs, no real data |
| `Binacle.ViPaq.TestsKernel` | library | ViPaq, Geometry, CompactNotation | yes | real-data hub — 716 packs, providers, protobuf, its own encoder |
| `Binacle.ViPaq.PerformanceTests` | exe | TestsKernel, TestReporting | yes | `RoundTripCheck` gate + size/codec reports |
| `Binacle.ViPaq.Benchmarks` | exe | TestsKernel | yes | BenchmarkDotNet timings |
| `Binacle.ViPaq.VectorGenerators` | tool exe | ViPaq, CompactNotation, TestReporting | yes | regenerates `test-vectors/` |
| `Binacle.ViPaq.PackedDataGenerator` | tool exe | Lib(+Abstractions), ViPaq, CompactNotation, Geometry, TestReporting | **no** | packs problems offline, freezes `data/packed/` |

## The walls (easy to break, deliberate)

1. **UnitTests never references TestsKernel.** UnitTests is the spec gate: it proves the code obeys `PROTOCOL.md`
   using the shared cross-language vectors and its own curated inputs. Keeping it clear of the real-data hub means
   a data change can never turn a spec test red, and the C# vector suite reads exactly what the TypeScript suite
   reads. If a test needs the 716 real packs, it belongs on the kernel side, not here.

2. **TestsKernel is the only home for the real packs, and only the measurement harnesses consume it.**
   Benchmarks and PerformanceTests reference it; nothing else does. It reaches the internal `ProtocolEncoder`
   through its own thin `ViPaqEncoder`/`ViPaqHeader`, so every mode (each codec, each layout) is forceable.

3. **PackedDataGenerator has no internals grant.** It produces the frozen data through the public surface and the
   packing engine only. It must never reach into ViPaq internals — the data has to be generatable the way any
   caller would.

4. **Two doors into the internal `ProtocolEncoder`** — and they stay apart: the UnitTests fixture
   (`SerializationTestingFixture`, curated/vector inputs) and the kernel's `ViPaqEncoder` (real-pack inputs).

5. **No test project references a generator.** The generators are standalone tools: they write `test-vectors/`
   and `data/packed/`, and the suites read those files. A `ProjectReference` from a test project to a tool —
   or a TS test importing the generator's parser — drags a CLI tool into the test build and lets a broken tool
   fail the suite for a non-product reason. Shared grammar goes in the library both sides already reference
   (`Binacle.CompactNotation`), never across this line.

## The real-data round-trip gate

The packed-data conformance suite is `RoundTripCheck.Run()` in `PerformanceTests`, which runs before any size
report. It sweeps all 716 real packs in two passes, each codec × both layouts:

- the **report path** — natural widths through the kernel's `ViPaqEncoder`, the exact encoder the reports use;
- the **conformance path** — the same packs forced to 16-bit widths through `ProtocolEncoder` directly, which the
  wrapper cannot reach (it always picks the narrowest widths), so the 16-bit read path is exercised on real data.

Both passes assert the two header bytes round-trip (`Header.FromBytes` equals the header written) and that the pack
decodes back to the input, throwing on the first mismatch. It lives on the harness side of wall 1, not in
UnitTests, because it drives the internal `ProtocolEncoder` over the real-data kernel.
