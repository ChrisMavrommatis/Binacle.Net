# Binacle.ViPaq results

Measured output for the ViPaq wire format. Two kinds, one per folder — the same split as `results/lib/`:

| Folder | What it measures | Written by |
|---|---|---|
| [benchmarks/](benchmarks/) | Encode/decode speed and allocation, per mode | `vipaq/test/Binacle.ViPaq.Benchmarks` (BenchmarkDotNet) |
| [compression/](compression/) | Encoded size vs protobuf, and where compression starts to pay | `vipaq/test/Binacle.ViPaq.PerformanceTests` |

Both folders are **hand-curated** — the harnesses write their raw run to build-local `*.Artifacts` scratch
(gitignored), and you copy the keepers in. To record a size run, diff the perf test's `PerformanceTests.Artifacts`
against `compression/` and copy it in on a win (`.agents/docs/vipaq/decisions.md` D3); `benchmarks/` works the same
way off the BDN artifacts. See the scratch-vs-curated convention in the [parent README](../README.md).
