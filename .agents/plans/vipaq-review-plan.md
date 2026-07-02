# ViPaq test review plan — gate before commit & before interconnected tests

Status: **not started.** This is a review-only plan. Do the three passes below, fix or file findings, then do
the plan-maintenance step. **Do not start the gzip cross-decode ("interconnected") work until this passes.**
This file is disposable — delete it once the review is acted on (see Plan maintenance).

## Why

The C# ViPaq test suite was refactored and the TS mirror was fully ported to read the shared vectors in
`vipaq/test-vectors/`. Both are green in isolation. Before committing and before building the cross-language
serialize↔deserialize (gzip) tests, confirm each side is right on its own **and** that they agree on the parts
that must agree. A wrong-but-green test on either side would let the two drift silently — the exact thing the
shared vectors exist to prevent.

## Scope

- **C#:** `vipaq/test/Binacle.ViPaq.UnitTests/` (VectorParser, VectorReader, BitSizeKind, all `Providers/`, all
  `Tests/`, the `.csproj` embedding) and, where a test asserts lib behavior, the referenced
  `vipaq/src/Binacle.ViPaq` surface (do not change the lib unless a test proves a bug).
- **TS:** `vipaq/binacle-vipaq/` — `src/` (only where a test depends on it) and all of `tests/`
  (`support/vectorReader.ts`, `support/vectorParser/`, `providers/`, the `*.test.ts`), plus `tsconfig.json`,
  `package.json`, `jest.config`.
- **Shared:** `vipaq/test-vectors/` — the 8 data files + `README.md` (the single source of truth both sides read).

## The three passes

Run them in order. Passes 1 and 2 are per-language and independent; pass 3 depends on both. Keep each pass's
findings in a ranked list (most severe first): correctness/accuracy bugs, then style/consistency, then nits.
For every finding give file:line, what's wrong, and the concrete failure or drift it allows.

### Pass 1 — C# (accuracy · correctness · C# style)

Verify against C# idioms only (naming `Method_Result_When`, `[Trait]`, static-ctor providers, one-theory-or
thin-callers, `Scenario`/`Vector` split).

- **Correctness:** `VectorParser` — `ParseThree(sep)` split + count guard; `ParseCoordinates` splits `,`,
  `ParseDimensions`/`ParseBin` split `x`; `ParseItems` composes dims+coords, strips parens, expands `:Q`; the
  `ParseItems(IEnumerable)` overload actually adds (`result.AddRange(items)` — this was a real bug once, confirm
  it's fixed); `ParseEncodingInfo` word maps; negatives via `long.Parse`.
- **Accuracy:** each provider resolves the row it claims; `ExactBytesProvider.Blob.ToByteArray` flattens
  Header::Count::Bin::(Dims::Coords) in the right order; `EncodingInfoByteProvider` keyed by the EncodingInfo
  string; bit-size providers split by `Kind` into `dimensions`/`coordinates`, `Scenario<TValue>` carries the
  typed value + `Field`/`Expected`; `LittleEndianProvider` per-width + the two C#-local wide rows; `FileName`
  consts point at the right files.
- **Tests:** `BitSizeInvalidTests`/`BitSizeSelectionTests` have two theories (one per kind) each running only
  its picker; `RoundTripScenarioTests` pins byte 0 then round-trips; `DecodeInvalidTests` asserts broad throw;
  `EncodeInvalidTests` pins `ArgumentOutOfRangeException`; golden bytes asserted both directions.
- **Hygiene:** `.csproj` embeds `test-vectors/*.json` + `little-endian/*.json` with the `Data.` logical names;
  no dead/duplicate providers left from the refactor.
- **Run:** `cd vipaq/test/Binacle.ViPaq.UnitTests && dotnet run -c Debug` — record the count, expect green.

### Pass 2 — TS (accuracy · correctness · TS style)

Verify against TS idioms only (free functions, one-function-per-file + `index.ts` barrel, PascalCase filename
when the file exports a PascalCase type, `test.each($name)`, `expectBytes`, curated literals).

- **Correctness:** `vectorParser/` — `parseThree(sep)` (internal, NOT re-exported from `index.ts`);
  `parseCoordinates` splits `,`, `parseDimensions`/`parseBin` split `x`; `parseItems` composes + expands `:Q`;
  `parseEncodingInfo`/`parseBitSize` maps; import depth `../../../src/models`. `vectorReader.readVectors` fs
  path resolves from `__dirname`.
- **Accuracy:** providers are uniform — module-private `Vector`, exported `Scenario` (or generic
  `Scenario<TValue>` for the bit-size pair), `load(file)` called with the literal at the export; bit-size
  coordinate cases carry a full-item probe (dims defaulted to 1, since TS `getCoordinatesBitSize` needs a full
  item where C# takes a bare `Coordinates`); `exactBytes` flatten matches the C# blob; `littleEndian`
  `Number(value)` rows are all ≤ 2^53.
- **Tests:** `serialize`/`deserialize` asserted with `await …rejects.toThrow()` (never bare); picker rejects
  assert the **lowercased** field (`'length'`, `'x'`); items compared with `toEqual` (not `toStrictEqual`);
  bit-size tests have two `test.each` blocks; encodingInfo covers 256 × 3.
- **Hygiene:** `forceConsistentCasingInFileNames` holds (import casing matches PascalCase files); no leftover
  `EXAMPLES-README.md`, old camelCase providers, or stray `support/vectorParser.ts`; `@types/node` in
  `package.json`.
- **Run:** `cd vipaq/binacle-vipaq && npx tsc --noEmit && npm test` — expect tsc clean, 18 suites / 949 tests.

### Pass 3 — Correlation (do the agreed parts actually agree?)

This is the point of the exercise. Diff the two sides on what MUST match; confirm what is deliberately
language-local is NOT flagged as drift.

- **Parser parity:** the compact-string grammar is read identically — dims/bin `x`, coords `,`, item
  `"LxWxH (X,Y,Z):Q"`, byte tokens `0x`/`0b` (strip `_`), EncodingInfo `Version_Bin_ItemDim_ItemCoord`,
  bit-size `Values` parsed by `Kind`. Any separator/grammar difference is a bug on one side.
- **Coverage parity:** the same scenarios are consumed on both sides — exact-bytes (8), encoding-info (256),
  round-trip, decode-invalid, encode-invalid, bit-size split by `Kind`, little-endian per width. Counts and
  names line up.
- **Wire parity:** uncompressed golden bytes are language-neutral — TS `serialize` == golden == C# serialize;
  reserved-version / truncated / above-MaxInteger / invalid-gzip blobs reject on both.
- **Invariants both enforce:** MaxInteger ceiling `2^53−1`, item-count cap `65535`, compression trigger (body
  `> 255`). Confirm both, and that `test-vectors/README.md` still describes the actual behavior (esp. coords are
  now comma-separated).
- **Agreed vs local:** confirm the language-local items are correctly OUT of the shared set and not treated as
  drift — C# generic-`T` matrices / saturation / dispose; TS `getByteSize`/`getBufferSize`/gzip mechanics; the
  two C#-local wide little-endian rows.

## Output & severity

One ranked findings list per pass. Rank: (1) correctness/accuracy that produces a wrong pass or a wire drift,
(2) cross-language drift risk, (3) style/consistency within a language, (4) nits. For each: file:line, the
defect, and the concrete input→wrong-result it allows. Both suites must be green to pass; a red suite is an
automatic fail regardless of findings.

## Plan maintenance (the reviewer MUST do this as the last step)

Only after the three passes pass and both suites are green:

- **Update the master plan** `vipaq-cross-language-testing.md`: mark C# unit + TS unit + shared-vector wiring
  DONE (both read `vipaq/test-vectors/`); the only remaining goal is the gzip cross-decode matrix. Fix its
  top "Status" and the session-plan so it reflects reality.
- **Delete `vipaq-vectors-ts-port.md`** once its still-relevant decisions are confirmed captured in the master
  plan and in the code/agent-docs. Its job (port TS to the shared vectors) is done; keeping it invites drift.
  Read it first and lift anything unique before deleting.
- **Reconcile `vipaq-integer-range-spec.md`:** if `PROTOCOL.md` exists and both sides enforce `[0, 2^53−1]`,
  the spec is delivered — mark it done or delete it (the durable record is `PROTOCOL.md`, keep that).
- **Delete this file** (`vipaq-review-plan.md`) once the review is acted on.
- Do **not** delete `vipaq/test-vectors/README.md` or `PROTOCOL.md` — those are durable, not plans.
- If any agent doc drifted, update it and bump its `verified:` date (e.g. `.agents/docs/vipaq/typescript.md`,
  `vipaq/README.md`).

## Exit criteria → next phase

Review passes when: both suites green, all pass-3 correlation checks hold, findings are fixed or explicitly
deferred, and the plan docs are updated/deleted per above. Only then start the **interconnected tests** — the
gzip cross-decode 2×2 matrix (C#→TS and TS→C# decode-to-input), per the master plan's "gzip requirement".

## How to run (suggestion, not part of scope)

Three focused passes, in order (1 and 2 can be parallel, 3 after). Each pass may be a separate reviewer session
or subagent; whoever runs it owns the findings list for that pass and, collectively, the plan-maintenance step.
