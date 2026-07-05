# Session 1 — Make the benchmark permanent (vs protobuf, 8/16 only)

**Goal:** build the permanent benchmark project — the ruler and regression guard for everything after. Keep the
protobuf comparison. Scope the value-width side to **8/16 only** (varint/24/32/64 experiments were useful once but
aren't worth keeping in the permanent tool). Decide the **compression trigger** here, with data.

**Prereq reading:** [findings.md](findings.md).

## Context you need
- **The earlier throwaway harness was removed** — you're building fresh. It produced [findings.md](findings.md);
  reuse the approach, not the code. Rebuild these modes: **size report** (base64 headline), **codec tradeoff**
  (gzip/brotli levels, encode/decode ms), **compress-crossover** (where compression starts beating base64-alone),
  **round-trip check**, and a **BenchmarkDotNet perf run** (serialize/deserialize, MemoryDiagnoser).
- Mirror `lib/test/Binacle.Lib.Benchmarks` conventions (Exe, net10.0, BenchmarkDotNet 0.15.8). Protobuf via
  `Google.Protobuf` + `Grpc.Tools` (test-only; keep it — the external baseline is the point).
- **base64 is the headline size metric** (the stored form), not raw bytes.
- Fairness rules (locked): identical logical payload on all sides (bin dims + item dims + coords, **no IDs**);
  **all values non-zero** (so proto3 can't skip-zero for free); **same codec/level both sides**; compare ViPaq by
  its actual mode (uncompressed → proto raw; compressed → proto raw AND proto+codec).

## Steps
1. Register `Binacle.ViPaq.Benchmarks` in `Binacle.Net.slnx` (near the other vipaq test projects).
2. Build two modes: a **size report** (base64 headline) and the **BDN perf run**.
3. **Trim to 8/16 for ViPaq's own encodings** in the kept tool (v1 today = 8/16/32/64; the permanent comparison
   focuses on the 8/16 world we're moving to). Keep the protobuf row + columnar baselines.
4. Build the **codec tradeoff** and **compress-crossover** reports — they answer "which codec" and "when to
   compress." These are how you make the compression decision.
5. **DECIDE the compression trigger** from the crossover data:
   - Recommended: **try-both-keep-smaller** — compress with the fast codec, emit `min(raw, compressed)`, set the
     header flag. Smallest token always, no threshold to tune, never inflates tiny tokens.
   - Simpler fallback: **fixed ~150-byte threshold** (brotli pays from ~150 B; gzip only from ~400 B).
   - Default codec: **gzip-Optimal or brotli-Optimal** (tie on size, both fast). **Not q11.**
6. Pin a **baseline snapshot** (size report + BDN summary) so future sessions see deltas. Size numbers are
   deterministic — a wire change shows an exact byte delta.

## Watch-outs
- BDN writes UTF-16 on Windows via PowerShell redirect — decode when extracting tables.
- ShortRun (3+3) is fine for relative comparison; use full runs only when a number is close.
- Don't rebuild the ruled-out experiments (24-bit ladder, coords-ride-bin, byte-plane) — see findings.md.

## Deviation note
If you'd rather keep the full multi-scheme sweep as a research appendix, fine — but the *default* permanent report
should be lean: v1(8/16) vs protobuf, size (base64) + perf, uncompressed and compressed.

## References
[findings.md](findings.md) · [03-spec-v2.md](03-spec-v2.md) (the compression decision feeds the spec) ·
`lib/test/Binacle.Lib.Benchmarks`, `lib/test/Binacle.Lib.PerformanceTests`.
