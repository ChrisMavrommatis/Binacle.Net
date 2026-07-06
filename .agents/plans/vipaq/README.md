# ViPaq — plan index

Everything for evolving `Binacle.ViPaq` lives here. ViPaq is a **storage-first** format: it turns a packing result
into a compact, copy-pasteable **base64 text token**. The whole point is small stored text; CPU/memory come second.

This folder replaces the earlier scattered notes. Read **[findings.md](findings.md)** first — it's the honest,
measured evidence (with the numbers the adversarial validator confirmed) that every session below draws on.

## The decision (CONFIRMED 2026-07-05)
Do **v2 for simplicity**. **Varint is deferred (session 7) and may never happen** — v2.0 is intentionally just
`8/16 + reserved codes`. The trade is accepted with eyes open:
- **Honest size effect of v2.0: ~0% vs today** on ≤16-bit data (8/16 = v1 there). The payoff is **a simpler format**
  (2 tiers, not 4; no `2^53` ceiling to reason about) and a **clean base** if varint is ever wanted. **Not a size
  play. Do not sell "20% smaller."**
- The real, separate win is the **decode-via-span fix (~10× faster reads, non-breaking)** — session 2, no format
  change. That's the standout regardless of v2.
- Note: v2.0-without-varint is a change chosen for simplicity, not measured size. Proceeding anyway.

## The worth-it gate (governs every decision from here)
Every decision in every session — a header bit, a codec, a layout, an extra feature — must be answered with
**"is it worth it?"**, written down as:
- **Cost:** effort + risk + complexity added + cross-language churn (C#, TS, interop vectors).
- **Benefit:** measured, in the terms that matter — **base64 size** (the stored form), **encode/decode ms**, or a
  concrete simplicity/maintenance gain. "Might be nice" is not a benefit.
- **Verdict + why.** **Default to NO.** The format is already good; the bias is against churn. Only a clear,
  stated benefit clears the gate. Log the verdict in the session doc so the next person sees the reasoning.

Reminder from the evidence: compression buys ~22% but only above ~150 bytes; a *fast* codec ≈ gzip on size; q11
costs 98 ms; header bits are free for size; varint helps only small/uncompressed tokens. Use these to answer the
gate honestly, not optimistically.

## Sessions (in priority order — each is one working session)
| # | Session | Breaking? | Why here |
|---|---------|-----------|----------|
| 1 | [01-benchmark-permanent.md](01-benchmark-permanent.md) — make the benchmark permanent (vs protobuf, **8/16 only**) | no | The ruler + regression guard. Everything after is measured against it. Decide the compression trigger here with data. |
| 2 | [02-decode-fix.md](02-decode-fix.md) — decode-via-span fix on v1 | no | Biggest, cheapest win (~10× decode). Back-portable to v1, no wire change. Bank it early. |
| 3 | [03-spec-v2.md](03-spec-v2.md) — write the v2 spec | (spec) | Normative `PROTOCOL.md` change first. 8/16 + reserved, columnar, compression rule, throw >65535. |
| 4 | [04-implement-csharp.md](04-implement-csharp.md) — implement v2 in C# → update benchmarks | yes | Build to spec, then re-measure vs v1 + protobuf. |
| 5 | [05-ts-mirror-tests.md](05-ts-mirror-tests.md) — TypeScript mirror + tests | yes | C# and TS must stay wire-identical. |
| 6 | [06-regenerate-vectors.md](06-regenerate-vectors.md) — regenerate interop vectors | yes | The shared answer key that stops C#/TS drift. |
| 7 | [07-additional-features.md](07-additional-features.md) — decide varint & extras | yes | varint (the deferred size lever), optional q11 archival mode, etc. — each with its measured gain. |

Binding reference for sessions 5–6: **[cross-language-testing.md](cross-language-testing.md)** — the existing
C#/TS interop apparatus and its rules.

## How to use these
- Each session doc is self-contained: **goal → context (the relevant findings) → steps → watch-outs → references**.
- **Sessions may deviate** — not every detail is final. Each doc carries the discovered info it needs so a fresh
  session can reason, not just follow. Where a doc says "DECIDE," that's a live choice, not a settled fact.
- **Never commit** (CLAUDE.md) — leave working-tree changes for the human. **Do not modify v3.**

## Decisions to confirm
1. **16-bit cap in v2.0.** 8/16 fixed caps at 65,535; v2.0 throws above it, varint (session 7) lifts the cap later.
   Confirm nothing in production exceeds 65,535 in the chosen unit (mm → 65 m; fine for physical bins).
2. **Compression trigger.** Recommend *try-both-keep-smaller* (smallest token, no threshold guesswork); fixed
   ~150-byte threshold is the simpler fallback. Locked in session 1 with data.
3. **Codec.** Fast **gzip-Optimal or brotli-Optimal** as default (they tie on size). **Never q11 as default**
   (~98 ms encode); q11 only as an opt-in archival flag (session 7).

## The benchmark harness — REMOVED
The throwaway harness that produced [findings.md](findings.md) has been **deleted** (uncommitted, never in the
solution). Its numbers and approach are captured in findings.md. **Session 1 builds the permanent benchmark fresh**
— cleaner than promoting throwaway code. Modes worth rebuilding: a **size report** (base64 headline), a **codec
tradeoff**, a **compress-crossover**, a **round-trip check**, and a **BenchmarkDotNet perf run**. See
[01-benchmark-permanent.md](01-benchmark-permanent.md).
