# ViPaq — build plan

Start with [prompt.md](prompt.md) — it carries the mission, the rules, and where the last session left off.
This file is just the work list.

## Build order

1. **C#** (`vipaq/src/Binacle.ViPaq/`) — the reference implementation, produces the golden bytes.
2. **TypeScript** (`vipaq/packages/binacle-vipaq/`) — mirrors it.
3. **Vectors** (`vipaq/test-vectors/`) — regenerate; they grade both.

Each needs the one before.

## Decisions the code will force

Not answers — just where you will have to stop and choose.

- **The compression codec.** `PROTOCOL.md` §6 leaves it unnamed and `Version` pins it, so the spec is not final
  until you pick one. It must exist wherever the format gets implemented.
- **Row vs columnar.** Both ship (the `Layout` bit). Which one the encoder *chooses* is measured, not decided here.
- **How the encoder is told what to do.** Widths, layout and compression are all encoder policy, and the header
  records them. Every combination has to be forceable, or the round-trip tests can't cover them.

## Known open work

Verified, and not in the spec.

- `ViPaqHeader` in the test kernel reads **one** header byte. The wire has two.
- The benchmark is not like-for-like: ViPaq compresses, protobuf does not, so the reported gap is mostly the
  compression. Either mirror it on protobuf, add a `ProtobufGzip` row, or rename the rows to say so.
- `results/vipaq/CompressionCrossover.md` is marked PROVISIONAL and has been for a while. Make it exact, or delete it.
- `SyntheticDataProvider` returns nothing. It is a stub.
- The test kernel keys scenarios by name and globs `*.ffd.json`. A second algorithm's `.bfd.json` would collide and
  would not be embedded. No such data exists today — fix it when it does.
- **Nothing checks that `vipaq/data/packed/` survives a ViPaq encode → decode.** See below. Not urgent: the data is
  pure geometry, holds no ViPaq bytes, and cannot rot as the format changes.

## Conformance checks looking for a home

`PackedDataGenerator` briefly grew a round-trip gate over its 716 real packed samples, then lost it: that tool packs
problems and freezes geometry, and a conformance suite has no business living in a data generator (it also forced an
`InternalsVisibleTo` grant, since the interesting paths are `internal`).

The checks were sound and were **all passing** when the gate came out, so any later failure is a regression, not an
unknown. Where they should live is undecided — unit tests, the test kernel, or a dedicated conformance project. The
data is one file read away: `vipaq/data/packed/**.ffd.json`, 716 samples, 58,834 placed items.

Whoever takes them: drive `ProtocolEncoder` directly, not `ViPaqSerializer`. The public serializer only ever writes
one shape (RowMajor, uncompressed, narrowest widths), so it cannot reach most of these.

1. **Decode-to-input over the forceable matrix.** D14 makes widths, `Layout` and `Compressed` encoder policy,
   recorded in the header, every combination conformant. Walk `{RowMajor, Columnar} × {natural widths, forced
   16-bit}` and assert the decoded geometry equals the input. Never compare blobs byte-for-byte — decode-to-input is
   the oracle (`PROTOCOL.md` §6.1).
2. **Header round-trip.** Assert the decoded `Header` equals the encoded one. Right geometry read off a wrong header
   is still wrong, and a `Header.FromBytes` bug hides behind a geometry-only assert.
3. **Forced 16-bit on sub-255 data.** Real packed data is mostly 8-bit, so those sections never exercise the 16-bit
   read path otherwise. Forcing a wider width is conformant (D14). Skip it when the item list is empty — §4 requires
   both item widths stay `Eight` there.
4. **Every codec, once one is pinned.** `{raw, deflate, gzip}` all round-tripped this data when tried. After the
   codec race, keep `raw` + the winner.
5. **Mixed read-only / settable types.** Encode from a read-only type (`PackedBin` / `PackedItem` off a packing
   result) and decode into a settable one (`Dimensions<int>` / `Item<int>`). Encode now takes `IReadOnlyList<TItem>`
   over the read-only geometry interfaces, and nothing else covers that shape.

A cheap way to keep them honest: mutate one thing and confirm the suite goes red. Encoding `Columnar` while decoding
`RowMajor` is the canonical one — it failed loudly every time it was tried.

## Related files

The reference — the settled "why" — lives in `.agents/docs/vipaq/` (this folder is work-to-be-done only):

- [architecture.md](../../docs/vipaq/architecture.md) — the policy/mechanism split the rebuild must keep. A constraint, not a suggestion.
- [decisions.md](../../docs/vipaq/decisions.md) — what was settled and why (D1–D16). Do not re-litigate these.
- [findings.md](../../docs/vipaq/findings.md) — real measurements on real data. It measured the old implementation, so read it for
  magnitudes, not for facts.

The open work, here in `plans/vipaq/`:

- [codec-race.md](codec-race.md) — the report that picks the compression codec and settles the layout question.
  Build to it; it names the modes, tables and columns.
- [testskernel-restructure.md](testskernel-restructure.md) — the test-kernel work still outstanding.
- [migration-api-followups.md](migration-api-followups.md) — the consumer migration is **done** (all six projects
  green, C# and TS suites pass); this tracks the three things it left behind that don't break a build — stale
  OpenAPI examples, undecodable saved browser tokens, and the v3 payload break awaiting a maintainer's word.
