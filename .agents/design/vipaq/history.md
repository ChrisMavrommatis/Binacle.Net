---
id: vipaq/history
description: ViPaq design history — superseded throwaway-prototype measurements (2026-07-05) that informed the locked decisions. Reference only, not current truth.
paths:
  - "vipaq/**"
---

# ViPaq — design history

Superseded evidence, kept as reference for *why* the format is shaped the way it is. **Current** measured truth is
`$vipaq/findings` (the permanent harness on real data); **current** decisions are `$vipaq/decisions`. The numbers
below come from the earlier throwaway-prototype harness (2026-07-05) that informed those decisions — do not cite
them as current.

## Prototype size — v1 vs v2-schemes vs protobuf
- ViPaq (any scheme) beats *idiomatic row* protobuf everywhere; only *columnar* protobuf competes.
- **8/16 + varint per-section, pick-smallest** was the best value encoding — beat "fixed-unless-overflow" and
  "varint-everywhere" in every scenario. But its win is mostly **uncompressed / small tokens**.
- **RAW encoding gain of 8/16+varint vs v1** (isolates encoding, 2000 items): ≤255 = 0%; ≤65k mixed = **−6.6%**;
  100k–1M = **−25 to −32%** (varint 3B vs fixed 4B); 2–16M = **−3 to −13%**; >268M = **+9% (worse)** (irrelevant to
  our range). So dropping 32/64 is safe up to ~268M.
- **After a matched fast codec, the compressed win shrinks to ~3%** (validator-corrected) — compression already
  crushes v1's wasted high bytes (usually zero). The earlier "~20%" was **Brotli q11 only** (see the codec table below).

## Prototype compression — codec tradeoff (16-bit real, 5000 items, on the v2 payload)
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

## Prototype compress-or-not crossover (16-bit, growing item count, base64)
- **≤ ~8–10 items (~150 B): base64-alone wins** (compression inflates via framing).
- **≥ ~13 items: brotli-Optimal wins**; gzip only pays from ~34 items (~18 B header). So if compressing, brotli
  pays earliest. Decision options: **try-both-keep-smaller** (recommended) or a **fixed ~150-byte threshold**.

## Prototype performance — surviving conclusions
The prototype's headline "decode ~10× faster" came from decompress-once-then-read-from-span. **That fix back-ports
to v1 with no format change** — it was ported and re-measured on the real lib (`$vipaq/findings`, "Decode fix"; the
real win was ~4–5×, not 10×). Two conclusions still stand and are unmeasured on the real lib:
- **Fast-codec v2 encode was never directly benchmarked** (the throwaway codec was hardwired to q11).
- **v2 allocates ~1.4–3.4× v1** on the prototype, so "encode ≈ v1" is optimistic.

## Superseded decision framings

Earlier versions of locked decisions in `$vipaq/decisions`, kept for the *why did it change* trail. The current
decision is always the one in the ledger — read these only for context.

### D4 original — the harness re-parsed the header bytes itself (2026-07-07, amended 2026-07-10)
The original rule barred the harness from library internals: it re-implemented PROTOCOL §2's bit layout and §3's
size arithmetic to read the header from raw bytes, to keep a clean boundary. That made a second copy of the spec,
which had already gone stale by the rewrite — it still read a one-byte header and treated compression as a
`Version`. Amended so the harness reads the header through the internal `Header` instead: one copy of the spec beats
the boundary. The encode/decode-through-public-API part was unaffected.

### O2 original — "name the codec and level before v2 ships" (2026-07-08, resolved 2026-07-13 → D16)
The open question was which compression codec and which level to pin, treated as a blocker for shipping v2. It
stopped being a blocker once compression became a user toggle that defaults to off: there is one codec (raw
DEFLATE), and the level never reaches the wire, so it is a free encoder-side choice.

### D5 original — the codec race was a one-off experiment (2026-07-07, reversed 2026-07-10)
The original decision scoped the codec race as a throwaway: run it once, record the answer, keep it out of the
permanent ruler — the worry being that bolting an experiment onto the ruler stops it being comparable over time.
Reversed because the race is not only about the codec: it also settles row-major vs columnar (a permanent concern)
and fixes the compressed-vs-raw-protobuf comparison bug. So the codecs became permanent harness fixtures.
