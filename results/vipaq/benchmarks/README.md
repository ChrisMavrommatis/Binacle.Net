# ViPaq — encode/decode speed

BenchmarkDotNet reports for encoding and decoding, per mode (raw, deflate, gzip, and the no-op path that prices
the compressed framing without the squeezing). Produced by `vipaq/test/Binacle.ViPaq.Benchmarks`, which writes to
its own build-local `BenchmarkDotNet.Artifacts`; copy the reports worth keeping into this folder by hand, like
`results/lib/benchmarks/`.

**Empty for now.** The size numbers are measured (see [../compression/](../compression/)), but the compression
*time* has not been run yet — the one open cell in the codec race
(`.agents/plans/vipaq/codec-race.md`, Table 3). Drop the first reports here when that run lands, following the
scratch-vs-curated convention in the [parent README](../../README.md).
