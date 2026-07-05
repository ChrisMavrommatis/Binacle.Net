# Session 2 — Decode-via-span fix (on v1, non-breaking)

**Goal:** make ViPaq decoding read from a decompressed buffer/span instead of per-value off a live `GZipStream`.
This is the **biggest, cheapest win found** (~10–12× faster decode on typical 8/16-bit data), and it needs **no
wire change** — it applies to today's v1. Bank it before any format work.

**Prereq reading:** [findings.md](findings.md) (Performance section).

## Context you need
- **Root cause:** `ViPaqSerializer.Deserialize` (`vipaq/src/Binacle.ViPaq/ViPaqSerializer.Deserialize.cs`) builds a
  `GZipStream` and reads values through `ProtocolReader<T>` one/two/four bytes at a time. `GZipStream` is not a
  `MemoryStream`, so it misses the reader's fast path and pays a virtual read per value — per-byte reads on a
  decompression stream are notoriously slow.
- **Fix:** when compressed, decompress the whole body **once** into a pooled `byte[]`, then read values from a
  `ReadOnlySpan<byte>` (via `BinaryPrimitives`). A throwaway codec (since removed) implemented exactly this and
  measured **~10× faster** — the approach is proven; see findings.md.
- **Measured caveat:** the ~10× holds for 8/16-bit data; at 32/64-bit it's ~1.3× (decompress dominates there). Our
  target data is 8/16-bit, so the win is real for the common case.

## Steps
1. In `Deserialize`, if `Version == CompressedGzip`, decompress fully into a pooled buffer (`ArrayPool<byte>`),
   then read from a span — don't read values straight off the `GZipStream`.
2. Keep the uncompressed path reading from the existing `MemoryStream` fast path (already fine).
3. Prove it with the harness: `-- check` (round-trip must stay green) + the BDN deserialize run (compare before/
   after against the session-1 baseline).
4. Consider applying the same "read from span" shape to the uncompressed path for consistency (minor).

## Watch-outs
- Preserve exact behavior/error paths (truncated body must still throw — see `PROTOCOL.md §7`).
- This is v1 code — **do not** change the wire format here; that's session 3+. Pure decoder-internals change.
- Don't regress allocations badly; pool the decompression buffer.

## Deviation note
If profiling shows the generic-math `T.CreateChecked` per value is also hot after this, specializing the common
widths (byte/ushort/uint) is a follow-up — but the span fix is the 90% win; do it first, measure, then decide.

## References
[findings.md](findings.md) (the span approach + the ~10× measurement) · `ViPaqSerializer.Deserialize.cs`,
`ProtocolReader.cs`.
