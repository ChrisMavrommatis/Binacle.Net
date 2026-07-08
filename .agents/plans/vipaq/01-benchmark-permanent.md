# Session 1 — Build the permanent benchmark (vs protobuf, 8/16 only)

**Status:** 🟢 Part 1 built + Track B / Part 2 built — offline `Binacle.ViPaq.PackedDataGenerator` (FFD) freezes
placed data under `vipaq/data/packed/{bischoff-suite,custom-problems}/*.ffd.json`; `RealDataProvider` now reads
those embedded files (was API-captured hardcode). Count-ladders and a curated fast-subset provider deferred until
we see results.
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
- Fits the existing `vipaq/tools/Binacle.ViPaq.VectorGenerators` pattern. Keep the tool **standalone** — the benchmark
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

- [x] Build `Binacle.ViPaq.TestsKernel`: seeded generator (the matrix) + `.proto` + protobuf serializer; reuse the
      `Binacle.Geometry` payload types. Also holds `ViPaqHeader` (reads byte 0 with the public `Version`/`BitSize`
      enums, no internals) and `ViPaqCodec` (the public `Serialize`/`Deserialize` door + round-trip check).
- [x] Register the three projects in `Binacle.Net.slnx` (under `/vipaq/test/`).
- [x] `Binacle.ViPaq.Benchmarks`: BDN encode/decode with `MemoryDiagnoser`; protobuf as `[Benchmark(Baseline=true)]`;
      `ParamsSource` over `BenchmarkCatalog.Names`. Confirmed it discovers and executes (Dry job).
- [x] `Binacle.ViPaq.PerformanceTests`: size table + protobuf compare (compression-parity rule) + round-trip gate
      (copied lib's runner pattern). Writes `results/vipaq/SizeComparison.md`.
- [~] Add the compression-crossover report: sweep item count, compare the token ViPaq emits against the exact
      uncompressed size. Writes `results/vipaq/CompressionCrossover.md`. **Now fed the real samples** (was
      synthetic — which showed compression "never pays", the opposite of the truth). Real data: 8-bit crosses
      between 16 and 100 items (saves 64%), 16-bit already compressing at ≤57 (saves 45–68%). **Marked
      PROVISIONAL / may be phased out** — the size report already shows where compression starts to pay, and
      the real data has gaps so the crossover point is coarse. Keep for now; revisit when v2 lands.
- [x] Report selection: BDN benchmarks take `--filter`. The PerformanceTests runner runs **all** reports on a
      plain `dotnet run` (the old report-name arg was dropped — everything runs every time).
- [~] Answer O1 (compression trigger) — **both sides now measured; ready to lock as try-both-keep-smaller.**
      Synthetic *random* payloads: gzip only ever inflated (−8% to −0% "saved"). Real packed data (the API
      precursor): gzip saved 45–68%. So compression's value depends entirely on the data, and the fixed
      255-byte threshold is wrong in both directions — it inflates random data and would miss small
      compressible data. The evidence points cleanly at **try-both-keep-smaller** (compress, keep whichever
      is shorter, never inflate). Lock in `decisions.md` when v2 is written (Session 3/4).
- [x] Save a first baseline (size report) under `results/vipaq/`. BDN summary is produced on demand (a full run
      is minutes); the size + crossover markdown is the committed baseline.

## Real-data precursor (API) — SUPERSEDED by the offline tool

**History.** This was the shortcut before the standalone tool existed. It is now replaced by
`Binacle.ViPaq.PackedDataGenerator` (see "Track B — built" below): the same placed data is produced offline
and committed, so `RealDataProvider` no longer carries an API-captured hardcode. Kept here for the findings it
established (all still hold). The old `vs API`/MATCH cross-check is retired — the committed data no longer stores
a token (it was derivable and its compressed bytes drifted by runtime); the kernel computes tokens itself.

Before building the full offline tool, we took a shortcut to get *real placed data* in front of the harness:
packed 20 problems (custom + Bischoff `thpack1..7`, two each) through the running API (`v4 pack/bin`, FFD,
`includeViPaqData`), archived the responses under `results/packed-responses/`, and froze them as a hardcoded
`RealDataProvider` in the kernel (bin + placed items, L/W/H and X/Y/Z). No files read at run time, no lib
dependency in the harness. The API's own token is stored per sample and used **only** to validate our
re-encode. `SizeComparison.md` now has two tables: synthetic and real.

What it showed:
- **Round-trip OK on every real sample, and `vs API` = MATCH on all 20** — our UInt16 re-encode is
  byte-identical to the token the API emitted (the API serialises from Int32, but width is chosen per
  section from the values, so the bytes are the same). Strong evidence the kernel path is correct.
- **Compression pays on real data — the opposite of synthetic.** Structured results (repeated item sizes,
  items on a coordinate grid) gzip well: the Bischoff tokens are 45–68% smaller than their uncompressed
  size, and the 100-item custom pack 64% smaller. On synthetic *random* data gzip only ever inflated.
- **ViPaq still wins on real data, but by less.** ViPaq/Proto is ~67–76% (vs 32–68% on synthetic). Real
  protobuf gains from omitted zero coordinates and gzip on its own structured output, so the gap narrows —
  but ViPaq is smaller on every row.
- **Width split confirmed on real data:** Bischoff packs to `16/8/16` — bin and coordinates need 16-bit
  (positions run to ~587), item dimensions stay 8-bit (largest box side ~113).

This is a precursor, not the deferred Part 2. The standalone offline tool (freeze many problems, pin the
algorithm, emit a manifest) is still the plan; this just proved the pipeline and answered the "does real
data compress?" question early.

## Next step: offline data tooling (replaces the API capture)

Two **separate** tracks — do not conflate them. The tests-kernel data (Track A) is problem *definitions* used
by the lib's algorithm tests; ViPaq's data (Track B) is *placed* results with coordinates. Different formats,
different homes, different consumers.

### Track A — tests-kernel data under `shared/data` (converter done)

Home for the lib's algorithm-test datasets. Each folder has a README. Raw is never edited.

- `shared/data/or-library/` — **raw** OR-Library text, as published. **Provenance (verified):**
  the **Bischoff suite = `thpack1`–`thpack7` only** = Bischoff & Ratcliff (1995, OMEGA) "BR instances"
  (BR1–BR7), a well-known benchmark. **`thpack8` (Loh & Nee, 1992) and `thpack9` (Ivancic et al., 1989,
  multi-container) are NOT Bischoff** — different authors, and thpack9 is a different problem class. Sweep
  only 1–7 for the suite; never fold 8/9 in.
- `shared/data/bischoff-suite/` — converted BR instances (1–7), tests-kernel scenario format.
- `shared/data/custom-problems/` — hand-authored problems, same format.

The tests-kernel **scenario format** is `Name` / `Bin` (`"LxWxH"`) / `Metrics` / `Result` / `Items`
(`["LxWxH [Q]"]`) — item *types* with a count, **no coordinates**.

**Converter built: `shared/tools/Binacle.OrLibrary.Converter`** (mirrors the ViPaq generator's style —
`IConverter` + a per-suite class, `Program` iterates, shared `RepositoryRoot.Bind().Find(...)`, no-arg
deterministic run). It reads the
raw thpack1–7 text and writes the bischoff-suite JSON. Key facts settled while building it:
- `Metrics` is **pure arithmetic** over `Bin` + `Items` (items volume / bin volume / count / %) — not a
  pack-run output, so no packer needed.
- `Result` is a **fixed expected baseline**: every Bischoff instance is always `PartiallyPacked` (fills ~98%,
  never tessellates perfectly), so the converter writes `"PartiallyPacked PartiallyPacked"` directly. The
  tests kernel runs the real packer and **asserts** against it — a `FullyPacked`/`NotPacked` would fail the
  test (packed unusually well, or nothing fit). So the tool depends on **`Binacle.CompactNotation` only** — no
  `Binacle.Lib`, no packer.
- Output goes to **`shared/data/bischoff-suite/` only** — the converter does **not** touch the kernel's embedded
  copies. It reproduces them byte-for-byte for **thpack1–4**. For **thpack5–7** it differs by one chosen
  normalization: those files' `Metrics` % were historically 1-decimal; chose **2 decimals everywhere**
  (option B) — test-safe (0.1% tolerance). The kernel copies are deliberately left at HEAD.

Open mechanics (unchanged): the kernel loads this data as *embedded resources* from inside its own project.
The converter writes to `shared/data/bischoff-suite`; the kernel still reads its own committed copies. Deciding
**keep-in-place vs relocate** (rewire the kernel to source from `shared/data`) is what reconciles the two — and
is when the thpack5–7 2-decimal normalization would actually reach the tests. Do that before wiring further.

### Track B — ViPaq placed data (BUILT)

ViPaq serializes *placed* results, so it needs items with **L/W/H and X/Y/Z**, which Track A's format does
not carry. This is its own thing, living in the **ViPaq slice** (not `shared/data`).

**Built: `vipaq/tools/Binacle.ViPaq.PackedDataGenerator`** (mirrors `Binacle.ViPaq.VectorGenerators`: no-arg `Exe`,
CompactNotation output, run once, output committed, deterministic byte-identical re-run; uses the shared
`Binacle.TestReporting.RepositoryRoot` to find the repo root, no bespoke per-tool locator). It reads the
Bischoff suite (`thpack1..7`) + custom problems, packs each in full with **FFD** via the lib's
`AlgorithmFactory` + `Execute(Packing)` (the API's exact call path), and emits the placed results in compact
notation for the ViPaq tests kernel to read — replacing the hardcoded, API-captured `RealDataProvider`.

- **Output:** `vipaq/data/packed/`, split by source family — `bischoff-suite/orlib_thpack1..7.ffd.json` and
  `custom-problems/{baseline,complex,simple}.ffd.json` — plus a `README.md`. The algorithm rides on the file name
  as a `.<algo>` suffix, not a folder. Each sample is **placed geometry only**: `Name`, `WidthBits` (8 if every
  value ≤255 else 16, a grouping label), `Bin` (`"LxWxH"`), `Items` (`"LxWxH (X,Y,Z)"`). Per-file/total counts
  print to the console on each run (no committed index file).
- **First run:** 716 samples, 58,834 placed items. All Bischoff instances are `PartiallyPacked` by design
  (fill ~98%, leftovers flagged in the log); two custom `DoesNotFit` scenarios place 0 items (emitted, flagged).
- **Round-trip gate:** every sample encodes → decodes back to its placed input or the tool exits non-zero. All
  716 passed.
- **No token stored.** The frozen data is pure geometry — deterministic, no gzip drift. The token is derivable
  and its compressed bytes vary by runtime, so the kernel computes it when it benchmarks (the old committed
  `SourceToken` and the `vs API`/MATCH column are gone).
- **Consumer rewired:** `RealDataProvider` reads the embedded `PackedData.*` (`*.ffd.json` from both family
  folders) at static init (kept `All`/`Names`/`GetByName`/`BenchmarkNames`); `BenchmarkNames` unchanged.

**FFD is pinned.** The tool is structured to add WFD/BFD later (one entry in a list; each algorithm lands as
`.wfd.json` / `.bfd.json` files beside the FFD ones, so the sets never mix). Why offline over the API capture:
deterministic, repo-contained, and can produce **any item count** — so the clean crossover ladder (pack
prefixes of one family: first 5, 13, 50, 200 items, only the count changing) is now possible. **Count-ladders
and a curated fast-subset provider are deferred** until we see results from the full frozen set.

### Speed and memory (first benchmark pass, Short job, this machine)

- **Memory: ViPaq allocates less everywhere** — encode 0.37–0.97×, decode 0.75–0.83× of protobuf.
- **Encode:** faster than protobuf on small uncompressed payloads (~0.45–0.53×); slower once gzip triggers
  (4–8× on real packs) — the compression is the cost, not the layout. Worst real case ~14µs.
- **Decode:** slower on anything non-trivial (4–7× on real packs) — the known decode-via-span weakness that
  Session 2 fixes (~10× read). Worst ~20µs.
- All times are microseconds; irrelevant for a storage token that is written once and read rarely.

### Steers to surface later (recorded now, lock in decisions.md when v2 is written)

- **Encode speed is the priority; decode is second.** ViPaq's job is to produce a token *fast and store it*;
  reads are rarer. Optimise encode first. Take decode wins only when they are cheap — Session 2's span fix
  is exactly that, so it still belongs. This changes how we read the benchmark: encode is the number that
  matters, decode is a watch-not-block figure.
- **Scope synthetic to speed/memory; use real for size.** (Refined — earlier this said "stop benchmarking
  synthetic"; that was too broad.) The two things we measure depend on different properties of the data:
  - **CPU and memory** (BDN encode/decode) depend on **item count and byte-width**, not on whether values
    repeat — encode/decode do the same work either way. So **synthetic random is fine, and preferred, for
    the speed/memory benchmarks**: it is deterministic, scales freely to counts we have no real packs for
    (2000, 5000), and it deliberately exercises the expensive path — compression runs but does not help, so
    the encoder pays the gzip cost and (under try-both-keep-smaller) discards it. That "wasted gzip" cost is
    real and worth measuring. Caveat: ViPaq's *absolute* allocation on random data runs a little high
    (compression does not shrink the buffer), but ViPaq and protobuf see the same sample so the
    ViPaq-vs-proto comparison stays valid.
  - **Size and compression** are the one place random lies (gzip has nothing to grip, so it reports the
    opposite of real behaviour). So **size and crossover use real data only.** The *contrast itself*
    (synthetic inflates −8→0%, real saves 45–68%) stays a keep-it finding.
  - **Done:** the crossover report now sweeps the real samples (was synthetic). Its one limit is that real
    data is gappy, so the crossover point is coarse (8-bit between 16 and 100 items; 16-bit ≤57) — marked
    PROVISIONAL, may be phased out once the size report is judged to cover it. The offline tool below is what
    gives real data at any count, which would make crossover exact again if we keep it.
  - **Done:** dropped the synthetic table from the size runner — the size report is now real placed data only,
    split into custom and Bischoff tables. The synthetic generator + matrix were removed too; `SampleProvider`/
    `SampleGenerator` are replaced by a stubbed `SyntheticDataProvider` (returns nothing) so `BenchmarkCatalog`
    still compiles and BDN runs real-only. Rebuild `SyntheticDataProvider` when synthetic speed/memory coverage
    (large item counts) is wanted again.

## Results so far (first run, this machine)

- **Round-trip: OK on every sample.** The gate passes across the whole matrix, both boundary samples included.
- **ViPaq is smaller than protobuf everywhere: ~32–68% of protobuf's base64**, comparing like against like
  (protobuf compressed only when ViPaq compressed). ViPaq wins most at small 8-bit payloads (~32–40%).
- **The boundary pair behaves:** `boundary-255-stays-8bit` → widths `8/8/8`; `boundary-256-flips-16bit` →
  `16/8/8` (only the bin section flips, items/coords stay 8-bit).
- **Baked-in compression hurts on random data.** ViPaq compresses once the body passes 255 bytes even when that
  makes the token bigger. On these random payloads it always did. Real packing data has structure and should
  compress — that is exactly why Part 2 matters before O1 is locked.
- Numbers are a snapshot of one machine; the protobuf baseline ratio is the stable figure to track over time.

## Open / unknowns — do not assume

- **Protobuf message shape.** Decided: **plain row message** (`PackedResult` with repeated `PlacedItem`). It is the
  honest, unoptimised baseline and ViPaq already beats it 2–3×. A columnar variant is only worth adding if we want
  the harder, smaller baseline — still open, not needed yet.
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
