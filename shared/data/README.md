# Data

Problem datasets used by the benchmarks and performance/regression tests. A set lives here when **more than one
slice reads it**; a set with a single consumer lives in that slice (result-selection is in `lib/data`, ViPaq's
packed data in `vipaq/data`). Three folders:

| Folder | What | Consumer |
| --- | --- | --- |
| [`or-library/`](or-library/README.md) | Raw OR-Library container-loading text, exactly as published. Untouched source. | The converter (produces `bischoff-suite`). |
| [`bischoff-suite/`](bischoff-suite/README.md) | Converted Bischoff & Ratcliff (BR) instances — `thpack1–7` only — in the tests-kernel scenario format. | The **tests kernel** (lib algorithm tests). |
| [`custom-problems/`](custom-problems/README.md) | Hand-authored problems (baseline / complex / simple), same tests-kernel format. | The **tests kernel**. |

`bischoff-suite` and `custom-problems` share the tests-kernel **scenario compact format**: a JSON array where
each entry is `Name`, `Bin` (`"LxWxH"`), `Metrics`, `Result`, and `Items` (`["LxWxH [Q]"]`). The tests kernel
embeds these very files by `Link`, so there is no second copy to keep in step.

`Items` are item **types** with a quantity (`"108x76x30 [40]"`), never *placed* items — there are no x/y/z
coordinates here. `Metrics` (`ItemsVolume BinVolume ItemsCount Percentage`) is pure arithmetic over `Bin` +
`Items`. `Result` (`{PackingStatus} {FittingStatus}`) is the **expected** outcome the tests assert against —
for the Bischoff suite it is always `PartiallyPacked`, written directly, not by running the packer. So the
converter (see `shared/tools/Binacle.OrLibrary.Converter`) has no dependency on the packing algorithms.

**ViPaq data is not here.** ViPaq needs *placed* items (with x/y/z coordinates), which this format does not
carry. That data lives in the ViPaq slice and is addressed separately.
