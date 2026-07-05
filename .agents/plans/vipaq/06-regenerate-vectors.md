# Session 6 — Regenerate interop vectors

**Goal:** regenerate the shared reference vectors so the C# and TS v2 implementations are graded against one answer
key and can't drift. This is the safety net that proves C#↔TS wire compatibility.

**Prereq reading:** [cross-language-testing.md](cross-language-testing.md) (the whole apparatus).

## Context you need
- Shared vectors live in `vipaq/test-vectors/` (byte-golden, header oracle, round-trip scenarios, invalid cases).
- Interop artifacts + generators: C# `vipaq/tools/Binacle.ViPaq.Generators`, TS `binacle-vipaq/tools`. Single entry
  point: `npm run regen:interop` (runs the C# generator then the TS one; deterministic, no-arg).
- Contract: **uncompressed bytes are byte-identical** across languages; **compressed payloads decode-to-input only**
  (not byte-equal — cross-runtime gzip/brotli differ). Keep both invariants.

## Steps
1. Update `interop/input.json` for v2: add boundary rows at the **v2** flips — the 8→16-bit flip at 255/256, and
   the >65,535 **throw** case (encode-invalid). Coordinate-section coverage too.
2. Update the generators to the v2 wire (they carry standalone encoders — keep them lib-only-dep).
3. `npm run regen:interop`; then run both suites: C# `dotnet test` in `Binacle.ViPaq.UnitTests`, TS `tsc --noEmit`
   + `jest`. The decode-to-input matrix + uncompressed byte-identity + per-file integrity must be green.
4. Update `PROTOCOL.md` decisions log / `test-vectors/README.md` if any convention changed.

## Watch-outs
- Don't assert byte-equality on compressed artifacts (it will flake across runtimes) — decode-to-input only.
- A stale/half-run regen bites: the integrity check (each artifact's `Name` set == input's) guards it — keep it.
- Add a boundary row for the **65,535 → throw** so both languages reject at the same value.

## References
[cross-language-testing.md](cross-language-testing.md) (binding, has the full vector inventory + rules) ·
[03-spec-v2.md](03-spec-v2.md) · `vipaq/test-vectors/`, `vipaq/tools/`, `binacle-vipaq/tools/`.
