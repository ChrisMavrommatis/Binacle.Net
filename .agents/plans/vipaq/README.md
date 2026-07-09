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

## Where the numbers are

`reference/findings.md` holds real measurements on real data. It is the one part of `reference/` that is evidence
rather than opinion — but it measured the old implementation, so read it for magnitudes, not for facts.
