# ViPaq — encoded size

How small ViPaq encodes, against protobuf under matched codecs. Produced by
`vipaq/test/Binacle.ViPaq.PerformanceTests`, which writes its run to a build-local `PerformanceTests.Artifacts`
folder (gitignored scratch). These files are the **hand-curated** baseline — to record a run, diff the scratch
output against them and copy it in on a win (`.agents/docs/vipaq/decisions.md` D3).

| File | What it shows |
|---|---|
| `VipaqProtobufSizeComparison.NoOp.md` | ViPaq vs protobuf, uncompressed — one row per scenario, base64 and raw bytes |
| `VipaqProtobufSizeComparison.Deflate.md` | Same comparison with both sides deflated |
| `VipaqProtobufSizeComparison.Gzip.md` | Same comparison with both sides gzipped |
| `CodecCompressionCrossover.Row.md` | Every codec on one line per scenario, row-major layout — which mode wins, and where compression starts to pay |
| `CodecCompressionCrossover.Columnar.md` | The same crossover, columnar layout |

These five are the **current best** — overwrite them when a run is worth promoting. To mark a milestone (the
obvious first one: when the codec is pinned), also drop a dated `YYYY-MM-DD.md` snapshot here with a line on what
changed, per the scratch-vs-curated convention in the [parent README](../../README.md).

Compressed sizes are not byte-identical across engines, so these are size measurements, not golden vectors. The
cross-language byte contract lives in `vipaq/test-vectors/`, not here.
