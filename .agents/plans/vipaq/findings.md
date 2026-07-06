# ViPaq — findings (the measured evidence)

The honest record of what we measured (2026-07-05), including the numbers an adversarial validator independently
reproduced and **corrected**. Every session doc points here. These came from a throwaway harness that has since
been **removed** — the numbers and approach are recorded below, reproducible once the benchmark is rebuilt
(session 1). Ranges are illustrative of the effects, not guarantees.

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

## Performance (BDN ShortRun, MemoryDiagnoser; v2 = varint+columnar+Brotli codec)
- **Decode:** v2 **~10–12× faster than v1 on 8/16-bit** compressed data (decompress-once-then-read-from-span vs v1
  reading per-value off a live GZipStream). **Collapses to ~1.3× at 32/64-bit** (large varints, brotli-decompress
  dominates). Codec-independent enough that **the decode fix back-ports to v1 with no format change**.
- **Encode:** with q11 it's **75–2290× slower** than v1 (the 98 ms problem). A *fast-codec* v2 encode was **not
  directly benchmarked** (the throwaway codec is hardwired to q11); v2 allocates **1.4–3.4× v1**, so "≈ v1" is
  optimistic — measure it in session 4.
- **Memory:** v2 decode allocs ~1.4–1.8× v1 (intermediate columnar arrays); encode ≈ v1 (q11) / carries build cost.

## Ruled OUT — do not rebuild
- **24-bit ladder (8/16/24/32) + coords-ride-bin:** byte-identical to v1 on real data; raw win evaporates after
  gzip; coords-ride-bin **regressed up to +19%** and its only benefit (a freed header bit) doesn't shrink base64.
  A throwaway `V2Encoder` implemented this and was **never round-trip-verified** — dead; don't rebuild it.
- **Brotli q11 as default:** 98 ms encode. Archival opt-in only.
- **Byte-plane / transpose layout:** identical to plain columnar (Brotli already exploits it).
- **Raw Deflate:** ~24 B better than gzip framing, but Brotli beats it — not worth a third codec.
- **Selling "20% smaller":** it was a q11 artifact.

## Validator's bottom line (independent, 24 checks, reproduced)
Strongest TRUE statement: *"v2 decodes ~10× faster than v1 on typical data with proven full reconstruction, its
encoding is ~7.5% smaller before compression, and it ties protobuf."* Strongest surviving objection: *"with a fast
matched codec the stored win is ~3%, the decode win falls to ~1.3× at million-scale, v2 allocates more, and the
shipped codec is a 98 ms q11 non-starter."* Round-trip: **7/7 PASS** for the varint+columnar codec.
