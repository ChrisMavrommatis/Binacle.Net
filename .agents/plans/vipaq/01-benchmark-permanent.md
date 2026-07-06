# Session 1 — Build the permanent benchmark (vs protobuf, 8/16 only)

**Status:** ⬜ not started — design ready
**Depends on:** [findings.md](findings.md), [decisions.md](decisions.md) (D3, D4, D5, D6, O1, O2)

## Goal

Build the benchmark that measures ViPaq. It is the ruler we rerun after every change, to see if the change made the
token smaller or the code faster. It keeps comparing ViPaq against protobuf on the same data.

## What it must do (must-have)

- **Measure stored size in base64 characters.** That is the real stored form and the headline number.
- **Measure encode and decode time and memory, reliably.** Use BenchmarkDotNet for this half.
- **Compare against protobuf on the exact same data.**
- **Use only ViPaq's public `Serialize` / `Deserialize`.** Nothing internal. (Why: [decisions.md](decisions.md) D4.)
- **Check every result round-trips** — decode gives back the input. A smaller token that does not decode is not a
  win; reject it.
- **Cover 8-bit and 16-bit data only.**
- **Cover many payload shapes** (see the matrix below).
- **Save results under `results/`** so the next run can be diffed against the last.

## What it must NOT do

- Must not touch ViPaq internals, header bits, or layout.
- Must not add a compression or codec knob — that comes later, in v2 ([decisions.md](decisions.md) D5).
- Must not benchmark 32/64-bit (v2 drops it).
- Must not rebuild the ruled-out experiments (see [findings.md](findings.md)).

## How it's built — three projects, mirroring lib

- **`Binacle.ViPaq.TestsKernel` (shared).** The seeded data generator that makes all the shapes below, and the
  protobuf schema + protobuf serializer. Both other projects use it, so both measure the *same* data. Without one
  shared source they could drift and the comparison would be invalid.
  - **Naming:** follows lib's `Binacle.TestsKernel` (the shared test foundation). Lives under `vipaq/test/`.
  - **Payload model:** reuse `Binacle.Geometry.Dimensions<T>` and `Item<T>` — the ViPaq unit tests already build
    scenarios from these, and they implement the interfaces `Serialize` needs. Do not invent or copy a model.
  - **Do NOT fold this into the shared `Binacle.TestsKernel`** — that one is packing/algorithm-focused; ViPaq
    generators and proto don't belong there.
- **`Binacle.ViPaq.Benchmarks` (BenchmarkDotNet).** Encode/decode time and memory (`MemoryDiagnoser`). Protobuf is
  the `[Benchmark(Baseline = true)]` row, so ViPaq is reported as a ratio and stays comparable across machines and
  days ([decisions.md](decisions.md) D3).
- **`Binacle.ViPaq.PerformanceTests` (custom runner).** The base64 size table, the protobuf comparison, the
  round-trip check, and the "when does compression start to win" report. Writes markdown to `results/`. Copy lib's
  `ITest` / `TestRunner` / markdown-writer pattern; **do not refactor lib's version yet** — copy first, share later
  only if the duplication actually hurts.

Mirror `lib/test/Binacle.Lib.Benchmarks` conventions (Exe, net10.0, BenchmarkDotNet 0.15.8). Protobuf via
`Google.Protobuf` + `Grpc.Tools` (test-only; the outside baseline is the point).

## Data sources — two parts

**Part 1 — Generated (synthetic). Primary, unblocked.** The seeded generator makes the shapes in the matrix below
at any scale. It is the only source that can do the crossover sweep. Start here; it needs nothing outside the kernel.

**Part 2 — Real Bischoff problems, packed. Realistic. DEFERRED until after Part 1 lands.**

The `orlib_thpack*.json` files hold real bin + item sizes, but **no coordinates** — only sizes and quantities
(checked: `Result` is a status string, `Metrics` is volume numbers). ViPaq encodes *placed* items, so real
coordinates only exist after packing.

**Chosen approach: a standalone offline tool, not a runtime dependency.** A small tool packs the Bischoff problems
with `Binacle.Lib` **once** and writes the packed payloads (bin + items + coords) to frozen data files. The
benchmark then reads those static files. This keeps the lib dependency in the tool, out of the benchmark, and
freezes the data so results are reproducible.
- Fits the existing `vipaq/tools/Binacle.ViPaq.Generators` pattern. Keep the tool **standalone** — the benchmark
  reads the emitted files, it does not reference the tool (see memory `vipaq-generator-standalone`).
- Reading the Bischoff JSON: the format is trivial (`Bin` string + `Items` strings), parsed by the shared
  `Binacle.CompactNotation`. Do **not** reuse `Binacle.TestsKernel`'s reader — it is lib/api-coupled.

**Why it stays a realism snapshot, not a regression guard.** FFD, WFD, and BFD place items differently, so
coordinates — and the token bytes — differ by algorithm, and shift as the lib's algorithms change. So Part 2 shows
"ViPaq on realistic shapes"; the stable ruler stays **Part 1 (synthetic, deterministic, lib-independent)**. The
tool pins the algorithm at generation time; regenerate deliberately when you want fresh data.
- Nuance: *uncompressed* base64 size is set by item count + value width (from the problem, not the algorithm), so
  the headline size is roughly algorithm-independent. The algorithm mainly moves exact bytes and *compressed* size.

**Open, when Part 2 is picked up:** which algorithm(s) to freeze (lean BFD, or all three); where the frozen files
live; whether the tool emits a manifest of what it generated.

## The payload shape matrix

| Axis | Values | Why |
|---|---|---|
| Value size | 8-bit, 16-bit | The two widths v2 keeps |
| Value spread | low (near 0), high (near the max), mixed | Stress the width choice |
| Boundary | the 255 → 256 flip | Where 8-bit becomes 16-bit |
| Item count | ~5, ~13, ~50, 2000, 5000 | Scaling, and the compression crossover |
| Data shape | same-size bins vs varied | Real-world variety |
| Compression | whatever ViPaq decides | We detect it, we do not set it |

The harness cannot set 8 vs 16 directly. It makes values that it *expects* ViPaq to store as 8- or 16-bit, and
confirms from the output. (See Open below — this expectation must be checked, not assumed.)

## Protobuf fairness

- Same logical data on both sides: bin size + item sizes + coordinates + count. **No IDs. All values non-zero** (so
  protobuf can't skip zero fields for free).
- Read byte 0 of ViPaq's output to see if ViPaq compressed. If it did **not**, compare against protobuf **without**
  compression only. If it **did**, compare against protobuf **both** ways (with and without), same codec and level.

## Tracking

- [ ] Build `Binacle.ViPaq.TestsKernel`: seeded generator (the matrix) + `.proto` + protobuf serializer; reuse the
      `Binacle.Geometry` payload types.
- [ ] Register the three projects in `Binacle.Net.slnx` (near the other vipaq test projects).
- [ ] `Binacle.ViPaq.Benchmarks`: BDN encode/decode with `MemoryDiagnoser`; protobuf as `[Benchmark(Baseline=true)]`.
- [ ] `Binacle.ViPaq.PerformanceTests`: size table + protobuf compare + round-trip gate (copy lib's runner pattern).
- [ ] Add the compression-crossover report: sweep item count, find where compressed base64 first beats raw.
- [ ] Make each report runnable on its own (BDN `--filter`; the runner's DI list).
- [ ] Answer O1 (compression trigger) from the crossover data; record in [findings.md](findings.md) + [decisions.md](decisions.md).
- [ ] Save a first baseline (size report + BDN summary) under `results/`.

## Open / unknowns — do not assume

- **Protobuf message shape.** Row or columnar? findings says only *columnar* protobuf competes on size. Start with
  a plain row message; add a columnar one only if we want the harder baseline. Decide while building.
- **Do we lift the vector readers from UnitTests?** `VectorReader` / `VectorParser` live in UnitTests and read the
  hand-authored correctness vectors. The benchmark *generates* data, so it does not need them now. Lift them into
  the kernel only if a second consumer appears.
- **Exact item counts for the crossover sweep** — refine once the first numbers are in.
- **O1 (compression trigger) and O2 (codec)** are answered here *with data* — unknown until measured.

## Watch-outs

- BDN writes UTF-16 on Windows via a PowerShell redirect — decode when reading the tables.
- ShortRun (3+3) is fine for relative comparison; use full runs only when a number is close.

## References

[findings.md](findings.md) · [decisions.md](decisions.md) (D3–D6, O1, O2) ·
`lib/test/Binacle.Lib.Benchmarks`, `lib/test/Binacle.Lib.PerformanceTests`.
