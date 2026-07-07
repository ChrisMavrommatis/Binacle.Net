# ViPaq — plan index & tracker

Everything for evolving `Binacle.ViPaq` lives here. ViPaq is a **storage-first** format: it turns a packing result
into a compact, copy-pasteable **base64 text token**. The point is small stored text; CPU/memory come second.

**Read order for a fresh session:** this file → [findings.md](findings.md) (the measured evidence) →
[decisions.md](decisions.md) (what's locked and why) → the one session file you're working. Each session file is
self-contained: it carries the context it needs so you can reason, not just follow.

## Status tracker

| # | Session | Scope | Breaking | Status |
|---|---------|-------|----------|--------|
| 1 | [01-benchmark-permanent.md](01-benchmark-permanent.md) | Permanent benchmark (ruler + regression guard), 8/16 only | no | 🟡 **Part 1 built** + real data via API precursor — standalone Part 2 tool deferred |
| 2 | [02-decode-fix.md](02-decode-fix.md) | Decode-via-span fix on v1 (~10× read) | no | ⬜ not started |
| 3 | [03-spec-v2.md](03-spec-v2.md) | Write the v2 wire spec in `PROTOCOL.md` | spec | ⬜ not started |
| 4 | [04-implement-csharp.md](04-implement-csharp.md) | Implement v2 in C# → re-measure | yes | ⬜ not started |
| 5 | [05-ts-mirror-tests.md](05-ts-mirror-tests.md) | TypeScript mirror + tests | yes | ⬜ not started |
| 6 | [06-regenerate-vectors.md](06-regenerate-vectors.md) | Regenerate interop vectors | yes | ⬜ not started |
| 7 | [07-additional-features.md](07-additional-features.md) | Decide varint & extras | yes | ⬜ not started |

Status legend: ⬜ not started · 🟡 in progress · ✅ done. Update this row **and** the `Status:` line in the session
file together. When a session fully lands, trim its file to only what remains (per `.agents` rules) and mark ✅.

## The decision in one line

Do **v2 for simplicity** — `8/16 + reserved codes`, varint deferred (maybe forever). It is a **simplicity play,
not a size play** (~0% smaller than today on ≤16-bit data). The standout win is separate: the **decode-via-span
fix** (Session 2). Full reasoning and every locked/open decision live in **[decisions.md](decisions.md)**.

## Reference docs (not sessions — they don't get "done")

- **[findings.md](findings.md)** — the measured evidence. The numbers every session draws on.
- **[decisions.md](decisions.md)** — locked decisions + open questions + the worth-it gate that governs all of them.
- **[architecture-v2.md](architecture-v2.md)** — the v2 policy/mechanism design (dumb serializer + smart chooser).
  Guides Sessions 3–4; explains why the permanent harness stays on the minimal public API.
- **[cross-language-testing.md](cross-language-testing.md)** — the C#/TS interop apparatus. Binds Sessions 5–6.

## Rules

- **Never commit, stage, or push** (CLAUDE.md). Leave working-tree changes for the human.
- **Do not modify v3.** v2 is greenfield/experimental; version-tag everything.
- Put a fact in one place; link across. Decisions go in `decisions.md`, evidence in `findings.md`, design in
  `architecture-v2.md` — not duplicated into session files.
