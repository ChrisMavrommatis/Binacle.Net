# Session 5 — TypeScript mirror + tests

**Goal:** rebuild the hand-maintained TypeScript mirror (`vipaq/packages/binacle-vipaq`) to `vipaq/PROTOCOL.md` so
it stays wire-compatible with the C# lib, and update its tests. Bound by the existing interop apparatus.

**Prereq reading:** `vipaq/PROTOCOL.md` (the spec — normative), `.agents/docs/vipaq/cross-language-testing.md`.

**Blocked on:** the codec. `PROTOCOL.md` §6 does not name one yet; Session 4 must (D5/O2). You cannot mirror an
unnamed codec.

## Do NOT (this session) — on top of the README standing fence
- Do not diverge from `PROTOCOL.md` — the spec is the contract, and it is language-neutral on purpose.
- Do not invent a second grammar/model; mirror the C# lib's consolidated notation + models.
- Do not assert byte-equality on compressed payloads — decode-to-input only.
- **Do not assert byte-equality on uncompressed payloads across two *encoders* either**, unless the header is
  pinned. See below — this is a correction, the old blanket claim was wrong.

## Context you need
- The C#↔TS contract is the spec. Bytes written by one must be **readable** by the other. The shared reference
  vectors in `vipaq/test-vectors/` grade both suites against one answer key so they can't silently drift.
- TS mirror lives in `vipaq/packages/binacle-vipaq` (`src/`, `tests/`). It reads the same JSON vectors from disk.
- **Byte-identity is narrower than it used to be (D14, `PROTOCOL.md` §6.1).** Widths, `Layout` and `Compressed`
  are all the *encoder's* choice, so C# and TS may legally emit different blobs for the same input. What holds:
  - same header + uncompressed → **byte-identical**, always. Golden vectors must state their header.
  - compressed → never byte-compare. Compressor builds differ.
  - anything → **decode-to-input**, always. This is the real interop contract.

## Steps
1. Mirror the spec in TS: 2-byte header, 8/16 width codes (2/3 rejected), **both** body layouts, 65,535 cap +
   error, reserved-bit rejection, the `Compressed` bit, base64 text form.
2. Match the C# width lookup and the decompress-then-read decode shape. There is no threshold to match (D7).
3. Update TS unit tests; keep them reading the shared `test-vectors/*.json`.
4. `npx tsc --noEmit` clean; `npx jest` green.

## Watch-outs
- Whatever codec Session 4 picks must exist in Node **and** the browser. Check both before mirroring.
- Keep language-local mechanics (buffer sizing, stream APIs) out of the shared vectors — they're not wire data.
- Don't invent a second grammar/model — the lib already consolidated `CompactNotation` + models; mirror that.

## References
`.agents/docs/vipaq/cross-language-testing.md` (binding) · `vipaq/PROTOCOL.md` ·
`vipaq/packages/binacle-vipaq/*`, `vipaq/test-vectors/`.
