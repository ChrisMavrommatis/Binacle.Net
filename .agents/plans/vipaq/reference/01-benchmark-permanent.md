# Session 1 — The permanent benchmark (vs protobuf, 8/16 only)

**Status: ✅ built.** Three follow-ups remain; only the first belongs to this session. The rest have owners below.

The benchmark is the ruler we rerun after every change, to see if a change made the token smaller or the code
faster. It compares ViPaq against protobuf on the same data, and it must never churn as the library evolves
([decisions.md](../decisions.md) D4).

**All measurements from this session live in [findings.md](../findings.md) (Round 2).** Don't copy numbers back here.
The decisions it produced are locked in [decisions.md](../decisions.md) as D7 (try-both-keep-smaller), D8 (encode
first), and D9 (synthetic = CPU/memory, real = size).

## What exists

Three projects under `vipaq/test/`, mirroring lib's layout:

- **`Binacle.ViPaq.TestsKernel`** — shared data + the protobuf schema/serializer, so the benchmark and the size
  runner measure the *same* bytes. Also `ViPaqHeader` and `ViPaqCodec` (the public `Serialize`/`Deserialize` door
  + round-trip check). `ViPaqHeader` reads byte 0 today; the new wire has a **2-byte** header and moves compression
  to its own bit (`vipaq/PROTOCOL.md` §2) — it changes in Session 4.
- **`Binacle.ViPaq.Benchmarks`** — BDN encode/decode + `MemoryDiagnoser`, protobuf as `[Benchmark(Baseline=true)]`.
- **`Binacle.ViPaq.PerformanceTests`** — the base64 size table, protobuf comparison (compression-parity rule),
  round-trip gate, and the crossover report. Plain `dotnet run` runs everything. Writes
  `results/vipaq/SizeComparison.md` and `results/vipaq/CompressionCrossover.md` — the committed baseline.

Data is real placed results, frozen offline: `vipaq/data/packed/` (see its README), produced by
`vipaq/tools/Binacle.ViPaq.PackedDataGenerator` from `shared/data/` problems, FFD only, deterministic. The
synthetic generator was removed; `SyntheticDataProvider` is a stub that returns nothing.

**Round-trip is the gate.** A smaller token that does not decode is not a win. 716/716 samples pass.

## Remaining — owned by this session

- [ ] **The BDN benchmarks are not a like-for-like comparison.** Protobuf is the baseline, but on real packs ViPaq
      gzips and protobuf does not, so the reported 4–8× encode gap and 4–7× decode gap are mostly *the compression,
      not the layout*. `PerformanceTests` applies a compression-parity rule; the benchmarks do not. Either apply it,
      or rename the rows to say plainly "ViPaq including gzip vs raw protobuf". Also:
      `CuratedEncodeBenchmarks`/`CuratedDecodeBenchmarks` return different shapes (a protobuf message vs a tuple with
      an `IList`), so some measured allocation is the return value, not the decode. Revisit alongside
      the decode fix (done — see [findings.md](../findings.md)), which moved these numbers anyway.
      *(Markers: `Benchmarks/CuratedEncodeBenchmarks.cs:9`, `Benchmarks/CuratedDecodeBenchmarks.cs:9`.)*

- [ ] **Decide the fate of `CompressionCrossover.md`.** It is marked PROVISIONAL. The real data is gappy, so it can
      only say "8-bit crosses between 16 and 100 items". Two ways out: a count ladder makes it exact (blocked, see
      below), or the size report is judged to already show where compression starts to pay and the report is deleted.
      Don't leave it provisional forever — pick one when v2 lands.

## Remaining — owned elsewhere

- **Rebuild `SyntheticDataProvider`** (CPU/memory only, scaling to 2000/5000 items — D9) and **add a curated
  fast-subset provider**: both are steps in [testskernel-restructure.md](../testskernel-restructure.md).
- **The count ladder** — one problem family at ~5/13/50/200 items, only the count changing. The packed data is
  regenerated from `shared/data/`, so the problems must be authored there first. Tracked in
  [../shared/testskernel-data-extraction.md](../../shared/testskernel-data-extraction.md).

## Open — not needed yet, don't build speculatively

- **Protobuf message shape.** It is a plain row message (`PackedResult` with repeated `PlacedItem`) — the honest,
  unoptimised baseline, and ViPaq already beats it. A columnar variant would be a harder, smaller baseline. Only
  worth adding if we want that; we don't yet.
- **Lifting `VectorReader`/`VectorParser` out of UnitTests.** The kernel has its own reader. Deliberate (see memory
  `vipaq-generator-standalone`). Lift only if a third consumer appears.
- **WFD/BFD packed data.** The generator pins FFD; adding an algorithm is one list entry, and the files land as
  `.wfd.json`/`.bfd.json` siblings. But a second algorithm **crashes the current reader** — fix
  [testskernel-restructure.md](../testskernel-restructure.md) first.

## Watch-outs

- BDN writes UTF-16 on Windows via a PowerShell redirect — decode when reading the tables.
- ShortRun (3+3) is fine for relative comparison; use full runs only when a number is close.
- The harness cannot set 8 vs 16 directly. It makes values it *expects* ViPaq to store at a width, then confirms
  from byte 0. Never assume the width — read it.

## References

[findings.md](../findings.md) (Round 2 = this session's numbers) · [decisions.md](../decisions.md) (D3–D9, O2) ·
`vipaq/data/packed/README.md` · `lib/test/Binacle.Lib.Benchmarks`, `lib/test/Binacle.Lib.PerformanceTests`.
