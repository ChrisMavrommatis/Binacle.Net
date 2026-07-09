# ViPaq — findings (the measured evidence)

The honest record of what we measured. Every session links here; **no session file keeps its own numbers.**
Ranges show the effect, not a guarantee.

**Current truth is the permanent harness on real data** ("Permanent harness" below). An earlier throwaway harness
(2026-07-05) measured a v2 prototype; the sections above the divider keep only the prototype evidence that **still
governs an open decision** (codec tradeoff, raw encoding gains, ruled-out schemes). Its performance tables and the
validator's detail were dropped 2026-07-09 — superseded by the real-lib numbers below, and in git history if needed.

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

## Size — v1 vs v2-schemes vs protobuf
- ViPaq (any scheme) beats *idiomatic row* protobuf everywhere; only *columnar* protobuf competes.
- **8/16 + varint per-section, pick-smallest** was the best value encoding — beat "fixed-unless-overflow" and
  "varint-everywhere" in every scenario. But its win is mostly **uncompressed / small tokens**.
- **RAW encoding gain of 8/16+varint vs v1** (isolates encoding, 2000 items): ≤255 = 0%; ≤65k mixed = **−6.6%**;
  100k–1M = **−25 to −32%** (varint 3B vs fixed 4B); 2–16M = **−3 to −13%**; >268M = **+9% (worse)** (irrelevant to
  our range). So dropping 32/64 is safe up to ~268M.
- **After a matched fast codec, the compressed win shrinks to ~3%** (validator-corrected) — compression already
  crushes v1's wasted high bytes (usually zero). The earlier "~20%" was **Brotli q11 only** (see below).

## Compression — codec tradeoff (16-bit real, 5000 items, on the v2 payload)
| codec | stored b64 | vs raw | encode | decode |
|---|--:|--:|--:|--:|
| raw (base64-alone) | 74,032 | 1.00× | — | — |
| gzip Optimal | 57,824 | 0.78× | 0.94 ms | 0.13 ms |
| brotli Fastest | 58,408 | 0.79× | 0.19 ms | 0.20 ms |
| brotli Optimal | 57,512 | 0.78× | 0.36 ms | 0.18 ms |
| **brotli SmallestSize (q11)** | 48,416 | 0.65× | **98.5 ms** 🛑 | 0.30 ms |
- Fast codecs all land ~0.78× (≈22% off raw). **q11's extra ~16% costs ~260× the encode time** — non-starter per
  request; only for write-once **archival**. gzip-Opt ≈ brotli-Opt on size, so switching codecs buys ~nothing at
  usable speed. (.NET quirk: gzip Optimal < gzip SmallestSize on this data.)

## Compress-or-not crossover (16-bit, growing item count, base64)
- **≤ ~8–10 items (~150 B): base64-alone wins** (compression inflates via framing).
- **≥ ~13 items: brotli-Optimal wins**; gzip only pays from ~34 items (~18 B header). So if compressing, brotli
  pays earliest. Decision options: **try-both-keep-smaller** (recommended) or a **fixed ~150-byte threshold**.

## Performance — surviving conclusions (prototype numbers archived)
The prototype's headline "decode ~10× faster" came from decompress-once-then-read-from-span. **That fix back-ports
to v1 with no format change** — it was ported and re-measured on the real lib (see "Decode fix" below; the real win
was ~4–5×, not 10×). Two conclusions still stand and are unmeasured on the real lib:
- **Fast-codec v2 encode was never directly benchmarked** (the throwaway codec was hardwired to q11). **Session 4
  owns this number.**
- **v2 allocates ~1.4–3.4× v1** on the prototype, so "encode ≈ v1" is optimistic — confirm in Session 4.

## Ruled OUT — do not rebuild
- **24-bit ladder (8/16/24/32) + coords-ride-bin:** byte-identical to v1 on real data; raw win evaporates after
  gzip; coords-ride-bin **regressed up to +19%** and its only benefit (a freed header bit) doesn't shrink base64.
  A throwaway `V2Encoder` implemented this and was **never round-trip-verified** — dead; don't rebuild it.
- **Brotli q11 as default:** 98 ms encode. Archival opt-in only.
- **Byte-plane / transpose layout:** identical to plain columnar (Brotli already exploits it).
- **Raw Deflate:** ~24 B better than gzip framing, but Brotli beats it — not worth a third codec.
- **Selling "20% smaller":** it was a q11 artifact.

**Validator's bottom line (archived).** An independent 24-check pass on the prototype landed on: *fast-codec stored
win ~3%, decode win falls to ~1.3× at million-scale, v2 allocates more, and q11 is a 98 ms non-starter.* Its
optimistic claims (decode ~10×, "ties protobuf") are superseded by the real-lib numbers below.

---

# Permanent harness (2026-07-08 on) — real data, shipped v1 library

This is the current, authoritative record. Source: `Binacle.ViPaq.PerformanceTests` (size + crossover) and
`Binacle.ViPaq.Benchmarks` (BDN). Data: 716 placed scenarios, 58,834 items, FFD-packed offline by
`Binacle.ViPaq.PackedDataGenerator` from the Bischoff suite (thpack1–7) + custom problems. Round-trip green on
every scenario, both in the generator and the harness.

## The headline: random data lies about compression

This is the single most important thing the real-data harness established, and it inverts a prototype assumption.

| Data | What gzip does |
|---|---|
| Synthetic **random** payloads | Only ever **inflates** (−8% to −0% "saved") |
| Real **packed** results | Saves **45–68%** (Bischoff); **64%** on a 100-item custom pack |

Real packing results have structure — repeated item sizes, items on a coordinate grid. Random data gives gzip
nothing to grip. So the shipped fixed **255-byte threshold is wrong in both directions**: it inflates random data
and would miss small compressible data. This drives D7.

## Size vs protobuf (like-for-like: protobuf compressed only when ViPaq compressed)

- ViPaq is **smaller than protobuf on every single row**.
- **Real data: ViPaq/protobuf ≈ 67–76%.** The gap narrows because real protobuf also gains — it omits zero
  coordinates and gzips its own structured output.
- **Synthetic: ≈ 32–68%.** ViPaq wins most at small 8-bit payloads (~32–40%).

## Width selection, confirmed on real data

- **Bischoff packs to `16/8/16`** — bin and coordinates need 16-bit (positions run to ~587); item dimensions stay
  8-bit (largest box side ~113). So the three sections genuinely disagree; an independent coordinate width earns
  its keep (relevant to the Session 3 header call).
- **Boundary pair behaves:** `255` → `8/8/8`; `256` → `16/8/8` (only the bin section flips).
- **Every Bischoff pack is 16-bit.** The only 8-bit data in the repo is a custom pack. This is a coverage hole —
  see `../shared/testskernel-data-extraction.md`.

## Compression crossover (PROVISIONAL — real data is gappy)

8-bit crosses somewhere **between 16 and 100 items**; 16-bit is **already compressing at ≤57**. The exact point
is unpinnable until a count ladder exists (one problem family at ~5/13/50/200 items, only the count changing).

## Speed and memory (first BDN pass, Short job, one machine)

**Read this with the parity caveat:** protobuf is the baseline but the comparison is *not* like-for-like — on real
packs ViPaq gzips and protobuf does not. So the encode/decode gaps below are mostly **the compression, not the
layout**.

- **Memory: ViPaq allocates less everywhere** — encode 0.37–0.97×, decode 0.75–0.83× of protobuf.
- **Encode:** faster than protobuf on small uncompressed payloads (~0.45–0.53×); slower once gzip triggers (4–8×
  on real packs). Worst real case ~14 µs.
- **Decode:** slower on anything non-trivial (4–7× on real packs) — the known decode-via-span weakness that
  Session 2 fixes. Worst ~20 µs.
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
justify it (D8). No wire change.

### Uncompressed vs compressed — the two paths, size and speed (2026-07-09)

ViPaq auto-compresses once the body passes ~255 bytes, so scenarios fall into two regimes that behave very
differently. The benchmarks fan out over a curated set (`CuratedScenarioProvider`, which merges
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
Session-2 decode fix):**

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
(down to ~29% of raw proto), and decode is near parity after Session 2. Encode is the only place protobuf leads,
and only while ViPaq is paying for compression — exactly the D8 priority. Allocations: ViPaq ≤ protobuf everywhere
except the 1-item token (520 B vs 368 B — noise at that size).

**Coverage gap:** the uncompressed set is **all 8-bit** — every 16-bit problem (Bischoff) is large enough to
compress, so the uncompressed *16-bit* path is measured nowhere. Closing it needs a small 16-bit problem authored
in `shared/data`; tracked in
[../shared/testskernel-data-extraction.md](../../shared/testskernel-data-extraction.md).

## What the harness did *not* answer

- **O2 (codec + level)** — the harness deliberately has no codec knob (D4). Still open.
- **Fast-codec v2 encode cost** — still never directly measured. Session 4 owns it.
- Absolute allocation on synthetic data runs a little high (compression can't shrink a random buffer), but ViPaq
  and protobuf see the same sample, so the *ratio* stays valid.
