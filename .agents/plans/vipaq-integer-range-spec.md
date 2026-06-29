# ViPaq integer-range spec — enforce a common ceiling across C# and TS

Status: planned (decided 2026-06-26). Sibling of [vipaq-cross-language-testing.md](vipaq-cross-language-testing.md).
This plan turns one cross-language decision into a written spec, code that enforces it, and tests that prove it.

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
  `CreateChecked`): `writeByte` 0..255, `writeUInt16` 0..65535, `writeUInt32` 0..4294967295,
  `writeUInt64` 0..maxInteger. (Confirmed decision: all four throw.)
- `src/ProtocolReader.ts` — `readUint64`: if the decoded value is greater than `maxInteger`, throw. Stops TS
  silently returning a rounded number when it decodes a C#-made buffer that used the high range.

## Deliverable 3 — TS tests

In the locked style (mirror `src/`, sentence names, named cases, `// ports C#:` where it maps):

- **Write guards** — `tests/ProtocolWriter.test.ts`: each of the four widths throws just above its ceiling and
  on a negative; the largest in-range value still writes. (New — no C# port; characterizes our `CreateChecked`.)
- **Read guard** — `tests/ProtocolReader.test.ts`: `readUint64` throws when the 8 bytes decode above `maxInteger`;
  the largest in-range value round-trips. (Covers deferred decision #1 — now an enforced throw, not silent loss.)
- **Width-selection ceiling** — `tests/utils/getDimensionsBitSize.test.ts` / `getCoordinatesBitSize.test.ts`:
  `maxInteger` selects `SixtyFour`; one above throws with the new message.
- **Item-count boundary** — `tests/utils/createEncodingInfo.test.ts` (deferred decision #2): 65535 items ok,
  65536 throws. Build them the same cheap way C# does:
  `Array.from({length: 65536}, () => item(1, 1, 1, 0, 0, 0))`. Ports C#
  `EncodingInfoHelperBehaviorTests.CreateEncodingInfo_Enforces_Item_Count_Limit`.

## Deliverable 4 — C# conformance (follow-up, its own task)

C# accepts up to 2^64 − 1 today, so it is non-conformant with the spec. Tighten it to reject above 2^53 − 1.
Done as its own reviewed change, not folded into the TS session. Steps:

- `vipaq/src/Binacle.ViPaq/Helpers/BitSizeHelper.cs` — add a ceiling guard at `9_007_199_254_740_991` in the
  dimension and coordinate size selection (both methods). Throw the same per-field "too large" style already used.
- Check `EncodingInfoHelper.cs` — item-count guard is unrelated; likely no change.
- Tests to revisit (they currently walk up to `ulong.MaxValue`, which the new ceiling forbids):
  - `Tests/BitSizeHelperSaturationTests.cs` — rows using `ulong.MaxValue` / `long.MaxValue`.
  - `Providers/BitSizeBoundaryByTypeProvider.cs` — the `ulong`/`long` high rows.
  - Any exact-bytes or boundary test that encodes a value above the new ceiling.
  Expectation: values above `2^53 − 1` now assert a throw instead of a `SixtyFour` result.
- Add a C# test mirroring the TS read guard: decoding a buffer whose 64-bit field exceeds the ceiling throws
  (so both sides reject the same bytes).

## Order of work

1. Write `vipaq/PROTOCOL.md` (Deliverable 1) — lock the rule in words first.
2. TS enforcement (Deliverable 2), then TS tests (Deliverable 3). Run the suite green + `tsc` clean.
3. Open the C# conformance task (Deliverable 4) separately.

## Notes / risk

- Tightening the ceiling only ever rejects absurd values (a dimension above 9 quadrillion). No real packing job
  hits it, so user impact is nil — the gain is no silent corruption and one number both languages agree on.
- This supersedes the "document silent precision loss" framing of deferred decision #1: we enforce and reject
  instead of documenting a quiet gotcha. Stronger and simpler.
