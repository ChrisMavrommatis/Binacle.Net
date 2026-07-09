# Session 5 — TypeScript mirror + tests

**Goal:** update the hand-maintained TypeScript mirror (`vipaq/packages/binacle-vipaq`) to the v2 spec so it stays
**wire-identical** to the C# lib, and update its tests. This is bound by the existing interop apparatus.

**Prereq reading:** [03-spec-v2.md](03-spec-v2.md), `.agents/docs/vipaq/cross-language-testing.md`.

## Do NOT (this session) — on top of the README standing fence
- Do not diverge from `PROTOCOL.md` — the spec is the C#↔TS contract.
- Do not invent a second grammar/model; mirror the C# lib's consolidated notation + models.
- Do not assert byte-equality on compressed payloads — decode-to-input only (uncompressed bytes stay byte-identical).

## Context you need
- The C#↔TS contract is the spec (`PROTOCOL.md`). Bytes written by one must be readable by the other. The shared
  reference vectors in `vipaq/test-vectors/` grade both suites against one answer key so they can't silently drift.
- TS mirror lives in `vipaq/packages/binacle-vipaq` (`src/`, `tests/`). It reads the same JSON vectors from disk.
- **Compression note (already documented, keep it true):** compressed *bytes* are NOT reproducible across runtimes
  — the interop contract for compressed payloads is **decode-to-input, never byte-equality** (see PROTOCOL.md §6 +
  test-vectors/README). Uncompressed bytes ARE byte-identical across languages. This matters for v2's codec too
  (gzip/brotli output differs C# vs Node) — so v2 compressed interop stays decode-to-input.

## Steps
1. Mirror the v2 changes in TS: header layout, 8/16 width codes (2/3 reserved), columnar body, 65,535 cap + throw,
   compression (same codec choice; the `Compressed` bit), base64 text form.
2. Match the C# width lookup, thresholds, and the decompress-then-read decode shape.
3. Update TS unit tests to v2; keep them reading the shared `test-vectors/*.json`.
4. `npx tsc --noEmit` clean; `npx jest` green.

## Watch-outs
- Brotli in Node: `zlib.brotliCompressSync` / `brotliDecompressSync` — confirm the quality mapping matches the
  chosen fast level (Optimal), and that decode handles C#-produced brotli (it will; brotli is a standard).
- Keep language-local mechanics (buffer sizing, Web-Streams gzip) out of the shared vectors — they're not wire data.
- Don't invent a second grammar/model — the lib already consolidated `CompactNotation` + models; mirror that.

## References
`.agents/docs/vipaq/cross-language-testing.md` (binding) · [03-spec-v2.md](03-spec-v2.md) ·
`vipaq/packages/binacle-vipaq/*`, `vipaq/test-vectors/`.
