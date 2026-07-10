# Idea: compare ViPaq to other formats

**Status:** The protobuf half is **done** — don't rebuild it. What's left is whether to widen the comparison.

## Answered (2026-07-08)

"How does ViPaq stack up against Protobuf?" was the original question. It is now measured continuously: protobuf is
the `[Benchmark(Baseline = true)]` row in `Binacle.ViPaq.Benchmarks` and a column in
`results/vipaq/SizeComparison.md`. Numbers live in `.agents/plans/vipaq/findings.md`.

The short answer: **ViPaq is smaller than protobuf on every row** — ~67–76% of protobuf's base64 on real packed
data, ~32–68% on synthetic. It allocates less on both encode and decode. It is slower to encode once gzip triggers
and slower to decode, though both are microseconds. Protobuf here is a plain row message (`PackedResult` with
repeated `PlacedItem`) — the honest, unoptimised baseline.

So ViPaq justifies its existence on size, which is what a storage-first format is for.

## Still open — the actual idea

- **Add a columnar protobuf variant?** It would be a harder, smaller baseline than the row message. Only worth it if
  we want to know whether ViPaq beats a *tuned* protobuf, not just an idiomatic one. Nobody has asked yet.
- **Compare against MessagePack, CBOR, or FlatBuffers?** None of them are the standard-everyone-uses that protobuf
  is, so the "should we have built this at all" question is already settled. This would be curiosity, not evidence.
- **Maintenance cost never got measured** — schema evolution, the cross-language mirror, the vector tooling. That is
  the one axis where protobuf plausibly wins, and it is the one nobody has written down.

## Related

- `.agents/plans/vipaq/findings.md` (the numbers), `.agents/plans/vipaq/reference/01-benchmark-permanent.md` (the harness)
- `.agents/docs/vipaq/README.md`, `vipaq/PROTOCOL.md`
