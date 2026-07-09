# Session 4 — Implement v2 in C# → update benchmarks

**Goal:** implement the session-3 spec in the C# library `Binacle.ViPaq`, then re-point the (now permanent)
benchmark at the real lib v2 and re-measure vs v1 + protobuf. Confirm the gains are what the spec promised.

**Prereq reading:** [03-spec-v2.md](03-spec-v2.md), [findings.md](findings.md).

## Do NOT (this session) — on top of the README standing fence
- Do not change the permanent harness — it stays on the public `Serialize`/`Deserialize` (D4) and measures v2 with no edit.
- Do not grow the public API. The dumb directive entry stays `internal` + `InternalsVisibleTo`.
- Do not sell "20% smaller" — ~0% on ≤16-bit is expected; the wins are decode speed + simplicity.

## Context you need
- Files: `ViPaqSerializer.Serialize.cs` / `.Deserialize.cs`, `ProtocolReader.cs` / `ProtocolWriter.cs`,
  `Models/BitSize.cs`, `Models/EncodingInfo.cs`, `Models/Version.cs`, `Helpers/EncodingInfoHelper.cs`,
  `Helpers/BitSizeHelper.cs`, `ViPaqLimits.cs`.
- A throwaway codec (since removed) round-trip-verified the encoding shape — per-section codes, columnar body,
  decompress-then-span decode. Build the real thing to the session-3 spec; findings.md describes the shape.

## What changes vs v1
- **Width codes remap:** `EncodingInfoHelper.ToByte/FromByte` to the v2 header layout; `BitSize` code→width becomes
  a lookup (8-bit→1, 16-bit→2; codes 2/3 reserved). Drop the 32/64 handling from the width selection.
- **`ViPaqLimits`:** new ceiling **65,535** for v2.0 (retire/repoint the `2^53-1 MaxInteger`); throw above it.
- **Compression:** implement the session-1 rule (try-both-keep-smaller, or threshold) with the chosen fast codec;
  set the `Compressed` bit. Keep it **optional** (small tokens raw).
- **Layout:** columnar body per spec.
- **Decode:** use the **decompress-once-then-span** path from session 2 (or reuse if already applied to v1).
- **Version:** stamp v2; keep decoders rejecting unknown versions.

## Steps
1. Implement encode + decode to spec. Keep the independent coord-width field.
2. The benchmark already calls only the public `Serialize`/`Deserialize` (D4), so it needs **no change** to measure
   v2 — that is the whole point of keeping it on the minimal API. Just rebuild and rerun.
3. Rerun `dotnet run` on `Binacle.ViPaq.PerformanceTests` (it runs every report; the old per-report CLI arg is gone)
   and the BDN suite. Diff against the committed baseline in `results/vipaq/`.
4. **Measure the fast-codec encode cost.** This is the one number never directly benchmarked — findings flag "≈ v1"
   as unproven, and v2 allocs 1.4–3.4× v1. It now matters twice over: D7 (try-both-keep-smaller) adds a second
   compress pass, and D8 makes encode the priority metric. Confirm acceptable, or tune (buffer pooling).
5. **Answer O2 here** (codec + level) with the one-off experiment — the permanent harness has no codec knob by
   design (D5), so this is a separate throwaway. Record the answer in findings.md, lock it in decisions.md.
6. Honest expectation: v2.0 ≈ v1 on size for ≤16-bit data; the wins are decode speed + simplicity. Don't be
   surprised by ~0% size — that's expected and fine. Do not sell "20% smaller"; that was a q11 artifact.

## Watch-outs
- **Never modify v3.** **Never commit.**
- Keep round-trip green (`-- check`-style tests, plus the real unit suite in `vipaq/test/Binacle.ViPaq.UnitTests`).
- `EncodingInfoHelper` is `internal` to the lib — the benchmark can't call it (that bit us before; the harness
  computes widths locally). Keep lib internals internal; expose only what's needed.

## References
[03-spec-v2.md](03-spec-v2.md) · [findings.md](findings.md) (the decode fix is already in shipped v1) ·
`vipaq/src/Binacle.ViPaq/*`.
