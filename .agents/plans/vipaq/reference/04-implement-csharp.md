# Session 4 — Implement v2 in C# → update benchmarks

**Goal:** implement `vipaq/PROTOCOL.md` in the C# library `Binacle.ViPaq`, then rerun the permanent benchmark and
re-measure against protobuf. Confirm the gains the spec implies.

**Prereq reading:** `vipaq/PROTOCOL.md` (the spec — normative), [decisions.md](decisions.md) (D11–D14).

## Answer the codec first
The spec is **not final**: §6 leaves the codec for `Version = 0` unchosen, and `Version` pins it, so it cannot be
deferred past this session. Answer O2, write the codec into `PROTOCOL.md` §6, delete it from §12, lock it in
[decisions.md](decisions.md). Session 5 cannot mirror a codec that has no name.

## Do NOT (this session) — on top of the README standing fence
- Do not change the permanent harness — it stays on the public `Serialize`/`Deserialize` (D4).
- Do not grow the *public* API. The directive entry stays `internal` + `InternalsVisibleTo`.
- Do not sell "20% smaller" — ~0% on ≤16-bit data is expected; the wins are decode speed + simplicity.
- **There is no v1 to compare against** (D11). Protobuf is the only baseline. Do not plan a v1-vs-v2 race.

## Context you need
- Files: `ViPaqSerializer.Serialize.cs` / `.Deserialize.cs`, `ProtocolReader.cs` / `ProtocolWriter.cs`,
  `Models/BitSize.cs`, `Models/EncodingInfo.cs`, `Models/Version.cs`, `Helpers/EncodingInfoHelper.cs`,
  `Helpers/BitSizeHelper.cs`, `ViPaqLimits.cs`.
- The header is now **2 bytes**, so `EncodingInfoHelper.ToByte/FromByte` become two-byte reads/writes and
  `EncodingInfo` grows `Compressed` and `Layout`. The harness's `ViPaqHeader` reads the same two bytes.

## What the spec requires
- **Header (§2):** 2 bytes. Byte 0 = `Version`(7-6) + `Compressed`(5) + `Layout`(4) + 4 reserved bits. Byte 1 =
  three 2-bit widths + 2 reserved bits. Reserved bits are written `0` and **rejected** non-zero on decode.
- **`Version = 0`** — not "v2". The field restarts. Decoders reject any other value.
- **Widths (§4):** codes `0` = 8-bit, `1` = 16-bit, `2`/`3` reserved and rejected. Drop all 32/64 handling.
  Selection is **policy** (SHOULD pick smallest), so a wider width must still encode and round-trip — D14.
- **`ViPaqLimits`:** ceiling **65,535** (retire the `2^53-1 MaxInteger` and the decode-side range check — §5 says
  a decoder has nothing to range-check). Error above the ceiling on encode.
- **Both layouts (§3).** Row-major *and* columnar, chosen per blob and recorded in the `Layout` bit. This is not
  "columnar only" — the point of the flag is to race them on real data (D13).
- **Compression (§6):** try-both-keep-smaller (D7). No threshold — D7 killed it. Set the `Compressed` bit to say
  what was kept. Phase 1 also exposes an override so raw and compressed can be measured (D13).
- **Decode (§7):** decompress once, then read over a span (the technique from the decode fix — see findings).
  Reject leftover body bytes after the last item.

## Steps
1. Answer O2 (see above) and update the spec.
2. Implement encode + decode to spec, both layouts, forceable widths.
3. The benchmark calls only the public `Serialize`/`Deserialize` (D4), so it needs **no change**. Rebuild, rerun.
4. Rerun `dotnet run` on `Binacle.ViPaq.PerformanceTests` (runs every report) and the BDN suite. Diff against the
   committed baseline in `results/vipaq/`.
5. **Measure the codec's encode cost.** Never directly benchmarked, and it now matters twice over: D7 adds a second
   compress pass, and D8 makes encode the priority metric. Confirm acceptable, or tune (buffer pooling).
6. **Race row vs columnar** on real packs, both raw and compressed. This is the question the `Layout` flag exists
   to answer. Record in findings.md.
7. Honest expectation: ~0% size change on ≤16-bit data; the wins are decode speed + simplicity.

## Watch-outs
- **Never modify v3.** **Never commit.**
- Keep round-trip green (`-- check`-style tests, plus the real unit suite in `vipaq/test/Binacle.ViPaq.UnitTests`).
- `EncodingInfoHelper` is `internal` to the lib — the benchmark can't call it (that bit us before; the harness
  computes widths locally). Keep lib internals internal; expose only what's needed.
- `InternalsVisibleTo` today grants **only** `.UnitTests` and `.VectorGenerators` — **not** `.Benchmarks`. If the
  phase-1 compression override (D13) is `internal`, decide who gets to see it before writing the experiment.
- Round-trip is the only oracle for a forced combo. Do not assert bytes across two encoders unless the header is
  pinned (§6.1, D14).

## References
`vipaq/PROTOCOL.md` · [findings.md](findings.md) (the decode-via-span technique) · `vipaq/src/Binacle.ViPaq/*`.
