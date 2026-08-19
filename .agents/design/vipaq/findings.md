---
id: vipaq/findings
description: ViPaq findings — the measured evidence (base64 size, encode/decode time) behind the decisions.
verified: 2026-08-19
check: Numbers match the latest results/vipaq/compression/ size reports and results/vipaq/benchmarks/ output; every benchmark and provider class named here still exists under vipaq/test/Binacle.ViPaq.Benchmarks/ and .TestsKernel/Providers/; the dataset note below still matches the entry count in vipaq/data/packed/**/*.json
also_update:
  - vipaq/decisions
paths:
  - "vipaq/**"
---

# ViPaq — findings (the measured evidence)

The current measured truth, on real data from the permanent harness. Every session links here; **no session file
keeps its own numbers.** Ranges show the effect, not a guarantee. The earlier throwaway-prototype numbers
(2026-07-05) that informed the locked decisions are superseded and live in `$vipaq/history`.

## Context (confirmed)
- ViPaq = **storage-first** base64 text token for a packing result (bin dims + per-item dims + coords + count).
- Unit **mm** at finest, **cm** typical; **no fractions** (unsigned integers); values **≤ ~16M** ("millions"),
  billions never. **Coordinates ≤ the bin** (a position inside it). Base64 is the *stored* form (applied today
  outside the spec).
- v1 today: fixed ladder **8/16/32/64** (1/2/4/8 bytes), row layout, gzip-Optimal when body > 255 B, base64 wrap.
- Fit AND pack results **both carry coordinates** — there is no "drop coordinates" shortcut.

## The base64-quantization rule (drove most decisions)
Stored size = `base64 = ceil(N/3)*4`. **Shaving header *bits* changes stored size by zero.** Only dropping *body
bytes across many values* matters. So value-width and compression are the only real size levers; header micro-design
is noise. Measure everything in **base64 chars**.

## Ruled OUT — do not rebuild
- **24-bit ladder (8/16/24/32) + coords-ride-bin:** byte-identical to v1 on real data; raw win evaporates after
  gzip; coords-ride-bin **regressed up to +19%** and its only benefit (a freed header bit) doesn't shrink base64.
  A throwaway `V2Encoder` implemented this and was **never round-trip-verified** — dead; don't rebuild it.
- **Brotli q11 as default:** 98 ms encode. Archival opt-in only.
- **Byte-plane / transpose layout:** identical to plain columnar (Brotli already exploits it).
- **Raw Deflate:** ~24 B better than gzip framing, but Brotli beats it — not worth a third codec.
- **Selling "20% smaller":** it was a q11 artifact.

## The harness — real data, shipped v1 library

Source: `Binacle.ViPaq.PerformanceTests` (size + crossover) and
`Binacle.ViPaq.Benchmarks` (BDN). Data: 716 placed scenarios, 58,834 items, FFD-packed offline by
`Binacle.ViPaq.PackedDataGenerator` from the Bischoff suite (thpack1–7) + custom problems. Round-trip green on
every scenario, both in the generator and the harness.

**The dataset has grown since these runs, and the counts below are not renumbered.** `vipaq/data/packed/`
carried 716 scenarios / 58,834 items when this was measured; on 2026-07-16 it went to **721 / 59,106** — five
custom scenarios and 272 items. So every "of 716" split here describes the earlier set. Nothing suggests the
*shape* moved (the five are small customs, the same family as the fifteen already on the uncompressed side),
but the exact splits would have to be re-run to be restated. The live count is in `$vipaq/dependencies`.

## The headline: random data lies about compression

This is the single most important thing the real-data harness established, and it inverts a prototype assumption.

| Data | What gzip does |
|---|---|
| Synthetic **random** payloads | Only ever **inflates** (−8% to −0% "saved") |
| Real **packed** results | Saves **45–68%** (Bischoff); **64%** on a 100-item custom pack |

Real packing results have structure — repeated item sizes, items on a coordinate grid. Random data gives gzip
nothing to grip. So the shipped fixed **255-byte threshold is wrong in both directions**: it inflates random data
and would miss small compressible data. This drives `$vipaq/decisions#D7`.

## Size vs protobuf (like-for-like: protobuf compressed only when ViPaq compressed)

- ViPaq is **smaller than protobuf on every single row**.
- **Real data: ViPaq/protobuf ≈ 67–76%.** The gap narrows because real protobuf also gains — it omits zero
  coordinates and gzips its own structured output.
- **Synthetic: ≈ 32–68%.** ViPaq wins most at small 8-bit payloads (~32–40%).

## Width selection, confirmed on real data {#width-selection}

- **Bischoff packs to `16/8/16`** — bin and coordinates need 16-bit (positions run to ~587); item dimensions stay
  8-bit (largest box side ~113). So the three sections genuinely disagree; an independent coordinate width earns
  its keep (relevant to the two-byte header call, D12).
- **Boundary pair behaves:** `255` → `8/8/8`; `256` → `16/8/8` (only the bin section flips).
- **Every Bischoff pack is 16-bit**, so 8-bit coverage comes from the custom packs — the `Simple_5x5x5-N` count
  ladder and the small baseline/simple cases; a custom `Simple_16bit-4` pack adds a small all-16-bit case too.

## Compression crossover

A controlled count ladder pins it: `Simple_5x5x5-N` (N = 5/13/50/200) in a fixed 50³ bin, only the count changing
(`results/vipaq/compression/CodecCompressionCrossover.*`). For this uniform, maximally-repetitive family deflate
already wins at the smallest rung — 5 items: raw 52 → deflate 36 b64 (31% saved) — and the saving climbs with count
(45% / 64% / 66% at 13 / 50 / 200). Uniform data is gzip's best case; mixed real packs (Bischoff) are less
repetitive and cross later, in the tens of items. **So the crossover tracks how repetitive the data is, not the
item count alone.**

## Speed and memory (first BDN pass, Short job, one machine)

**Read this with the parity caveat:** protobuf is the baseline but the comparison is *not* like-for-like — on real
packs ViPaq gzips and protobuf does not. So the encode/decode gaps below are mostly **the compression, not the
layout**.

- **Memory: ViPaq allocates less everywhere** — encode 0.37–0.97×, decode 0.75–0.83× of protobuf.
- **Encode:** faster than protobuf on small uncompressed payloads (~0.45–0.53×); slower once gzip triggers (4–8×
  on real packs). Worst real case ~14 µs.
- **Decode:** slower on anything non-trivial (4–7× on real packs) — the known decode-via-span weakness that the
  decode span fix addresses. Worst ~20 µs.
- All times are microseconds. For a token written once and read rarely, this is noise.

### Decode fix — decode-via-span, option A (2026-07-09)

The shipped v1 decode read per-value off a live `GZipStream`. `Deserialize` now decompresses the gzip body once
into a `MemoryStream` so `ProtocolReader` hits its fast path. Machine: AMD Ryzen 9 9900X, .NET 10.0.9, BDN v0.15.8.
The before column is the pre-fix v1; after is the shipped fix. 1371 unit tests pass; 716/716 round-trip.

| Scenario | ViPaq before | ViPaq after | Speedup | Ratio vs proto | Alloc before → after |
|---|--:|--:|--:|--:|--:|
| OrLibrary_thpack1_2 (16-bit) | 19.82 µs | 4.84 µs | 4.1× | 6.28× → 1.53× | 5.88 → 6.98 KB |
| OrLibrary_thpack4_1 (16-bit) | 14.06 µs | 3.72 µs | 3.8× | 7.60× → 2.01× | 4.69 → 5.45 KB |
| Simple_5x5x5-100 (8-bit)     | 17.95 µs | 3.48 µs | 5.2× | 7.48× → 1.35× | 5.63 → 6.36 KB |

**~4–5× faster decode on real v1 8/16-bit data**, from 4–8× slower than protobuf to near parity. Not the ~10× the
Round-1 prototype showed — that was v2 varint+columnar with a different codec; this is what shipped v1's layout
yields. Cost: **+0.7–1.1 KB allocation** (the decompressed buffer), but ViPaq still allocates less than protobuf
(ratio 0.91–0.97). The pooled-buffer variant (A′) would reclaim that KB; deferred — a rarely-read token doesn't
justify it (`$vipaq/decisions#D8`). No wire change.

### Uncompressed vs compressed — the two paths, size and speed (2026-07-09)

**Measured against v1's automatic threshold, which no longer exists.** At the time, ViPaq compressed by itself
once the body passed ~255 bytes, so scenarios fell into two regimes. Today compression is a caller flag
defaulting off (`$vipaq/decisions#D16`) and the harness forces it, so "which regime a scenario lands in" is now
the caller's call rather than the library's. The two regimes still describe what the *data* does under
compression, which is what makes the numbers worth keeping. The benchmarks fan out over a curated set (`CuratedScenarioProvider`, which merges
`BischoffCuratedProvider` and `CustomProblemsCuratedProvider`) that includes an uncompressed ladder
(`CustomProblemsCuratedProvider.UncompressedNames`: 1 / 8 / 16-item 8-bit packs) so the raw path is measured too —
before this, all curated benchmarks compressed and the raw path had **no** performance number. The size report now
shows two ratio columns (ViPaq vs raw proto, ViPaq vs gz proto).

**Size (base64 chars).** 15 of 716 scenarios stay uncompressed (all tiny 8-bit customs); 701 compress (nearly all
16-bit Bischoff).

| Regime | ViPaq / raw proto | ViPaq / gz proto |
|---|--:|--:|
| Uncompressed (n=15, 8-bit) | 52–67% (median 60%) | — (neither side compresses) |
| Compressed (n=701) | 17–39% (median 29%) | 64–71–76% (min–median–max) |

**Speed (BDN full run via `CuratedEncodeBenchmarks` / `CuratedDecodeBenchmarks`, ratio = ViPaq / protobuf, post
decode span fix):**

| Scenario | Items | Regime | Encode | Decode |
|---|--:|---|--:|--:|
| Baseline_5x5x5-1        |   1 | uncompressed |   75 ns (1.20×) |   56 ns (0.69×) |
| Simple_15x15x15-8       |   8 | uncompressed |  190 ns (0.89×) |  187 ns (0.85×) |
| Complex_FitsInMedium_1  |  16 | uncompressed |  336 ns (0.92×) |  333 ns (0.79×) |
| OrLibrary_thpack4_1     | ~57 | compressed   |  7.0 µs (4.31×) |  4.0 µs (1.97×) |
| OrLibrary_thpack1_2     | ~80 | compressed   | 10.6 µs (4.19×) |  5.0 µs (1.69×) |
| Simple_5x5x5-100        | 100 | compressed   | 14.5 µs (7.89×) |  3.5 µs (1.35×) |

**Takeaway:** on the **uncompressed path ViPaq matches or beats protobuf on both axes** — ~40% smaller, encode at
parity, decode faster. On the **compressed path** it trades encode CPU (the gzip pass, 4–8×) for a large size win
(down to ~29% of raw proto), and decode is near parity after the span fix. Encode is the only place protobuf leads,
and only while ViPaq is paying for compression — exactly the `$vipaq/decisions#D8` priority. Allocations: ViPaq ≤ protobuf everywhere
except the 1-item token (520 B vs 368 B — noise at that size).

**Coverage:** the uncompressed 16-bit path is now measured by `Simple_16bit-4_FitIn_600x400x300` — 4 items whose
bin and item dimensions force 16/16/16 widths, small enough to skip compression (raw b64 80, ~0.95× protobuf). It
sits in the curated uncompressed set so the encode/decode benchmarks cover it; the other uncompressed picks stay
8-bit.

### Compression cost, isolated (2026-07-14)

The encode/decode gaps above fold the compression in with the format. `CompressionCostBenchmarks` prices the
squeezing on its own: NoOp (body passed straight through) against Deflate and Gzip, row-major, over the two curated
Bischoff packs. BDN Short job. `Deflate − NoOp` is what compression actually costs; `Gzip − Deflate` is the extra
framing.

| Scenario | Items | Encode NoOp | Encode Deflate | Encode Gzip | Decode NoOp | Decode Deflate | Decode Gzip |
|---|--:|--:|--:|--:|--:|--:|--:|
| OrLibrary_thpack4_1 |  70 | 3.2 µs | 8.7 µs (2.75×) | 8.7 µs (2.76×) | 2.5 µs | 4.4 µs (1.39×) | 4.5 µs (1.41×) |
| OrLibrary_thpack1_2 | 108 | 4.8 µs | 13.1 µs (2.74×) | 13.5 µs (2.83×) | 3.7 µs | 5.5 µs (1.16×) | 5.7 µs (1.20×) |

- **Deflate encode ≈ 2.75× the format-only encode** — compressing adds ~5–8 µs per pack (the `Deflate − NoOp`
  delta). This is also **`$vipaq/decisions#D7`'s try-both price**: try-both is one `Compress` more than never compressing,
  i.e. exactly this delta.
- **Deflate decode adds only ~1.2–1.4×** (inflate ~1.8–1.9 µs) — cheap.
- **Gzip never beats deflate on time** — ~2–3% slower on encode (its framing), on par on decode. Gzip is already
  larger on size, so nothing rescues it: deflate is the pick on both axes (**`$vipaq/decisions#D16`**).
- All sub-15 µs, encoded once server-side and read rarely — noise in practice.

## What the harness did *not* answer

- **`$vipaq/decisions#O2` (codec + level)** — **resolved (`$vipaq/decisions#D16`):** one codec, raw DEFLATE. The compression cost is measured just above.
- Absolute allocation on synthetic data runs a little high (compression can't shrink a random buffer), but ViPaq
  and protobuf see the same sample, so the *ratio* stays valid.
