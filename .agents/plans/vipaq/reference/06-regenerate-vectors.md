# Session 6 — Regenerate interop vectors

**Goal:** regenerate the shared reference vectors so the C# and TS implementations are graded against one answer
key and can't drift. This is the safety net that proves C#↔TS wire compatibility.

**Prereq reading:** `vipaq/PROTOCOL.md` (§6.1 determinism, §8 errors), `.agents/docs/vipaq/cross-language-testing.md`.

**Before adding vector rows,** read `.agents/ideas/vipaq/interop-vector-coverage.md` — it records four candidate
rows we skipped, and one experiment (cross-runtime compressed blobs) that was built and deliberately dropped.

## Do NOT (this session) — on top of the README standing fence
- Do not change the interop apparatus — this session changes what the vectors contain, never how the harness works.
- Do not assert byte-equality on compressed artifacts — it flakes across runtimes; decode-to-input only.

## Context you need
- Shared vectors live in `vipaq/test-vectors/` (byte-golden, header oracle, round-trip scenarios, invalid cases).
- Interop artifacts + generators: C# `vipaq/tools/Binacle.ViPaq.VectorGenerators`, TS `vipaq/packages/binacle-vipaq/tools`. Single entry
  point: `npm run regen:interop` (runs the C# generator then the TS one; deterministic, no-arg).
- **Every byte-golden vector must now pin its full header** — `Version`, `Compressed`, `Layout`, and all three
  widths. Widths and `Layout` are encoder policy (D14, §6.1), so "the uncompressed bytes are identical" is only
  true once the header is fixed. A vector that omits the header is not a test, it's a coin flip.

## Steps
1. Update `interop/input.json`: boundary rows at the 8→16-bit flip (255/256), the >65,535 encode-error case, and
   coordinate-section coverage.
2. Add rows the new header demands: **both `Layout` values** on the same input, a forced-wide width (16-bit on
   sub-255 data) to prove it round-trips, and decode-invalid rows for **non-zero reserved bits**, **`Version` ≠ 0**,
   **width code 2/3**, and **trailing bytes after the last item**.
3. Update the generators to the new wire (they carry standalone encoders — keep them lib-only-dep).
4. `npm run regen:interop`; then run both suites: C# `dotnet test` in `Binacle.ViPaq.UnitTests`, TS `tsc --noEmit`
   + `jest`. The decode-to-input matrix + header-pinned byte-identity + per-file integrity must be green.
5. Update `test-vectors/README.md` if any convention changed. (`PROTOCOL.md` has no decisions log — the *why*
   lives in [decisions.md](decisions.md).)

## Watch-outs
- Don't assert byte-equality on compressed artifacts (it will flake across runtimes) — decode-to-input only.
- A stale/half-run regen bites: the integrity check (each artifact's `Name` set == input's) guards it — keep it.
- Add a boundary row for **65,535 → error** so both languages reject at the same value.

## References
`.agents/docs/vipaq/cross-language-testing.md` (binding — the full vector inventory + rules) ·
`vipaq/PROTOCOL.md` · `vipaq/test-vectors/`, `vipaq/tools/`, `vipaq/packages/binacle-vipaq/tools/`.
