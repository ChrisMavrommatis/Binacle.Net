# ViPaq integer-range spec — enforce a common ceiling across C# and TS

Status: **Deliverables 1–4 done (2026-06-30).** Both C# and TS enforce `[0, 2^53 − 1]` on encode and decode,
with spec-named width constants. The remaining ViPaq work — the shared cross-language vector tests — is its own
dedicated session: see [vipaq-cross-language-testing.md](vipaq-cross-language-testing.md).
This plan turned one cross-language decision into a written spec, code that enforces it, and tests that prove it.

## The decision

ViPaq stores dimensions and coordinates as integers. The two runtimes do not agree on how big an integer
can be exactly:

| Runtime | Largest exact integer | |
|---|---|---|
| C# `ulong` | 18,446,744,073,709,551,615 (2^64 − 1) | bigger |
| JS `number` | 9,007,199,254,740,991 (2^53 − 1) | smaller |

JS is the limiting side. A value between 2^53 and 2^64 is fine in C# but JS rounds it silently. So a valid
C#-made buffer could carry a number TS cannot read back.

**Decision: ViPaq's interoperable integer range is `[0, 2^53 − 1]` (9,007,199,254,740,991).**
Every implementation MUST reject values outside it — on encode and on decode. This is the largest integer all
target runtimes hold exactly. C# `ulong` can hold more, but anything above this ceiling is outside ViPaq.

The constant `2^53 − 1` is `Number.MAX_SAFE_INTEGER` in JS. Call it `MaxInteger` in the spec.

## Deliverable 1 — the protocol spec

Create `vipaq/PROTOCOL.md`. First normative, language-agnostic spec for the format. It sits above both
implementations. The `.agents/docs/vipaq/` files stay as agent notes and link to it.

Must contain at least:

- **Integer range** (the rule above) — the core of this plan. MUST reject outside `[0, 2^53 − 1]`, encode and decode.
- A short wire-format section (header byte, little-endian counts, bin then per-item dims+coords). Can be lifted
  from `.agents/docs/vipaq/README.md`; keep it normative and terse. Fill fully later if needed — the integer-range
  rule is the part that must land now.
- A "decisions log" section so future protocol calls (like this one) are recorded with date and rationale.

## Deliverable 2 — enforce in TS (this session)

All values are checked against `[0, MaxInteger]`. Reject negatives too. Files:

- `src/utils/sizes.ts` — add `maxInteger = 9_007_199_254_740_991` (or use `Number.MAX_SAFE_INTEGER`).
  Note: `uLongMaxValue` is currently `9223372036854775807` (that is 2^63 − 1, mislabeled). The 64-bit bucket
  stops using it — see below. Leave or fix the constant separately; it is not the ceiling anymore.
- `src/utils/getDimensionsBitSize.ts` and `getCoordinatesBitSize.ts` — the `SixtyFour` bucket caps at
  `maxInteger`, not `uLongMaxValue`. Above it, throw. Reword the final "too large" throw to name the real limit,
  e.g. `'length' exceeds the max supported value (9007199254740991)`. This also resolves deferred decision #4:
  the line IS reachable in TS (a float like 1e19 passes the old check), so the message must be true, not
  "should never be reached".
- `src/ProtocolWriter.ts` — range-check every write primitive and throw if out of range (mirrors C#
  `CreateChecked`): `write8Bits` 0..255, `write16Bits` 0..65535, `write32Bits` 0..4294967295,
  `write64Bits` 0..maxInteger. (Confirmed decision: all four throw.)
- `src/ProtocolReader.ts` — `read64Bits`: if the decoded value is greater than `maxInteger`, throw. Stops TS
  silently returning a rounded number when it decodes a C#-made buffer that used the high range.

## Deliverable 3 — TS tests

In the locked style (mirror `src/`, sentence names, named cases, `// ports C#:` where it maps):

- **Write guards** — `tests/ProtocolWriter.test.ts`: each of the four widths throws just above its ceiling and
  on a negative; the largest in-range value still writes. (New — no C# port; characterizes our `CreateChecked`.)
- **Read guard** — `tests/ProtocolReader.test.ts`: `read64Bits` throws when the 8 bytes decode above `maxInteger`;
  the largest in-range value round-trips. (Covers deferred decision #1 — now an enforced throw, not silent loss.)
- **Width-selection ceiling** — `tests/utils/getDimensionsBitSize.test.ts` / `getCoordinatesBitSize.test.ts`:
  `maxInteger` selects `SixtyFour`; one above throws with the new message.
- **Item-count boundary** — `tests/utils/createEncodingInfo.test.ts` (deferred decision #2): 65535 items ok,
  65536 throws. Build them the same cheap way C# does:
  `Array.from({length: 65536}, () => item(1, 1, 1, 0, 0, 0))`. Ports C#
  `EncodingInfoHelperBehaviorTests.CreateEncodingInfo_Enforces_Item_Count_Limit`.

## Deliverable 4 — C# conformance — DONE (2026-06-30)

C# accepted up to 2^64 − 1; now it rejects above `MaxInteger` (2^53 − 1), encode and decode. What landed:

- New `vipaq/src/Binacle.ViPaq/ViPaqLimits.cs` — the spec constants in one place: `EightBitsMax` (255),
  `SixteenBitsMax` (65 535), `ThirtyTwoBitsMax` (4 294 967 295), `MaxInteger` (2^53 − 1), and
  `CompressionThresholdBytes` (255, §6 — kept separate from `EightBitsMax` so the two 255s never conflate).
- `Helpers/BitSizeHelper.cs` — the `SixtyFour` bucket caps at `MaxInteger`; above it the per-field throw now
  reads "exceeds the max supported value (9007199254740991)". The width checks key off the new spec constants,
  not `byte/ushort/uint.MaxValue`.
- `ExtensionMethods/ProtocolReaderExtensions.cs` — decode guard. `ReadDimensions`/`ReadCoordinates` reject a
  `SixtyFour` field above `MaxInteger` via a private `EnsureWithinRange`. Placed here, not in `ProtocolReader`,
  because the codebase treats `ReadAs*` as raw "bytes → widen to T" primitives (their full-range little-endian
  tests must keep passing); the semantic field decode is the right enforcement layer (spec §7).
- `ViPaqSerializer.Serialize.cs` — compression trigger uses `ViPaqLimits.CompressionThresholdBytes`.
- Tests: `BitSizeHelperSaturationTests` 64-bit rows now assert `MaxInteger → SixtyFour`;
  `BitSizeBoundaryByTypeProvider` gains a `MaxInteger` top-of-bucket row; `BitSizeHelperBehaviorTests` gains
  `_When_Value_Exceeds_MaxInteger` throw tests and drops the obsolete UInt128 "exceeds 64 bits" tests (their
  `ulong.MaxValue` filler now trips the ceiling first); `ProtocolExtensionsBehaviorTests` gains decode-throw
  tests for a `SixtyFour` field above the ceiling. **C# 1257 pass, TS 499 pass, `tsc` clean** (verified
  2026-06-30; the earlier 1269 predates the "vipaq rethink and structure" commit that trimmed redundant tests.
  TS rose 495→499 when the "too large" throw was aligned to name the offending field per axis, deferred #4/#7).

Also done this pass (TS, to match C#): `sizes.ts` renamed to the same spec-named constants
(`eightBitsMax` / `sixteenBitsMax` / `thirtyTwoBitsMax` / `maxInteger` / `compressionThresholdBytes`), dropped the
mislabeled `uLongMaxValue`, and all usages updated. So both libraries now key off identical spec constants.

## Order of work

1. Write `vipaq/PROTOCOL.md` (Deliverable 1) — lock the rule in words first.
2. TS enforcement (Deliverable 2), then TS tests (Deliverable 3). Run the suite green + `tsc` clean.
3. Open the C# conformance task (Deliverable 4) separately.

## Notes / risk

- Tightening the ceiling only ever rejects absurd values (a dimension above 9 quadrillion). No real packing job
  hits it, so user impact is nil — the gain is no silent corruption and one number both languages agree on.
- This supersedes the "document silent precision loss" framing of deferred decision #1: we enforce and reject
  instead of documenting a quiet gotcha. Stronger and simpler.
