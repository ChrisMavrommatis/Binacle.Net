# ViPaq — plan index & tracker

Everything for evolving `Binacle.ViPaq` lives here. ViPaq is a **storage-first** format: it turns a packing result
into a compact, copy-pasteable **base64 text token**. The point is small stored text; CPU/memory come second.

## Start here

**The test-kernel restructure is largely done** ([testskernel-restructure.md](testskernel-restructure.md)). The
ViPaq kernel now uses the house words (`Scenario`, not `Sample`), owns its own file provider, and splits its real
data into `BischoffDataProvider` + `CustomProblemsDataProvider`. The earlier attempt to share the file plumbing via
`shared/test/Binacle.TestFiles` was **reverted** — see [decisions.md](decisions.md) D10. A few items remain
(synthetic rebuild, second-algorithm handling, curated-pick refinement); they are listed in that file.

**Session 2 (decode-via-span) is done** — ~4–5× faster decode on v1, no format change; the before/after is in
[findings.md](findings.md) ("Decode fix") and the call is [decisions.md](decisions.md) D8. **The next session is
Session 3** — write the v2 wire spec in `PROTOCOL.md` ([03-spec-v2.md](03-spec-v2.md)).

**Read order for a fresh session:** this file → [findings.md](findings.md) (the measured evidence) →
[decisions.md](decisions.md) (what's locked, and why not to re-argue it) → the one session file you're working.

## Where a fact lives — check here before you write one down

Sessions have repeatedly re-derived things that were already settled, and recorded new measurements in session files
where they got lost. One home per kind of fact:

| Kind of fact | Home | Never |
|---|---|---|
| A number we measured | [findings.md](findings.md) | in a session file |
| A decision + why | [decisions.md](decisions.md) | in a session file |
| The v2 policy/mechanism design | [architecture-v2.md](architecture-v2.md) | duplicated into 03/04 |
| How C#/TS interop is tested | `.agents/docs/vipaq/cross-language-testing.md` | it's a doc, not a plan |
| The normative wire format | `vipaq/PROTOCOL.md` | anywhere in `.agents/` |
| What a session must still *do* | that session's file | anywhere else |

A session file carries context and steps. If you find yourself writing a measurement or settling a question in one,
put it in findings/decisions and link to it.

## Status tracker

| # | Session | Scope | Breaking | Status | Blocked by |
|---|---------|-------|----------|--------|------------|
| 1 | [01-benchmark-permanent.md](01-benchmark-permanent.md) | Permanent benchmark (ruler + regression guard) | no | ✅ **built** — 2 follow-ups | — |
| 2 | _done — folded into findings + D8_ | Decode-via-span fix on v1 (~4–5× read) | no | ✅ **done** | — |
| 3 | [03-spec-v2.md](03-spec-v2.md) | Write the v2 wire spec in `PROTOCOL.md` | spec | ⬜ not started | — (O1 now locked as D7) |
| 4 | [04-implement-csharp.md](04-implement-csharp.md) | Implement v2 in C# → re-measure | yes | ⬜ not started | 3 |
| 5 | [05-ts-mirror-tests.md](05-ts-mirror-tests.md) | TypeScript mirror + tests | yes | ⬜ not started | 4 |
| 6 | [06-regenerate-vectors.md](06-regenerate-vectors.md) | Regenerate interop vectors | yes | ⬜ not started | 5 |
| 7 | [07-additional-features.md](07-additional-features.md) | Decide varint & extras (a menu, not a commitment) | yes | ⬜ not started | 4 |

Sessions 3–7 are a chain: each needs the one before. Sessions 1 and 2 stand alone.

Status legend: ⬜ not started · 🟡 in progress · ✅ done. Update this row **and** the `Status:` line in the session
file together. When a session fully lands, trim its file to only what remains and mark ✅.

**Not a session:** [testskernel-restructure.md](testskernel-restructure.md) — bring `Binacle.ViPaq.TestsKernel`
in line with the repo's test-kernel conventions. Core alignment done (2026-07-09); a few items remain, listed
there. Delete the file once they land.

## The decision in one line

Do **v2 for simplicity** — `8/16 + reserved codes`, varint deferred (maybe forever). It is a **simplicity play,
not a size play** (~0% smaller than today on ≤16-bit data). The standout win was separate: the **decode-via-span
fix** (Session 2, done — ~4–5× faster decode). Full reasoning and every locked/open decision live in
**[decisions.md](decisions.md)**.

## Reference docs (not sessions — they don't get "done")

- **[findings.md](findings.md)** — the measured evidence. Current truth is the permanent harness on real data;
  earlier throwaway-harness prototype numbers are archived to git, only their surviving conclusions kept.
- **[decisions.md](decisions.md)** — D1–D9 locked, O2 open, plus the worth-it gate that governs all of them, and a
  "ruled out — do not rebuild" list.
- **[architecture-v2.md](architecture-v2.md)** — the v2 policy/mechanism design (dumb serializer + smart chooser).
  Guides Sessions 3–4; explains why the permanent harness stays on the minimal public API.
- **`.agents/docs/vipaq/cross-language-testing.md`** — the C#/TS interop apparatus (built, green). Binds Sessions
  5–6: they change what the vectors contain, never how the apparatus works.

## Standing fence — applies to every session

Read this before any session. Each session file adds its own short "Do NOT (this session)" on top.

- **Never commit, stage, or push** (CLAUDE.md). Leave working-tree changes for the human.
- **Do not modify v3.** v2 is greenfield/experimental; version-tag everything.
- **Nothing ships without a measured gain** — smaller base64, or faster/less memory, or a concrete simplicity win.
  Default to NO. See the worth-it gate in [decisions.md](decisions.md).
- **Do not add a new project, shared library, or abstraction to solve a local problem.** Prefer a standalone copy;
  share only when a third consumer actually appears. The shared-`TestFiles` extraction was reverted for exactly this
  ([decisions.md](decisions.md) D10), as was coupling the reader to the generator (memory `vipaq-generator-standalone`).
- **Do only what the session scopes — no more.** If a fix pulls you into restructuring, stop and note it; don't ride
  the tangent.
- Put a fact in one place; link across. See the table above.
