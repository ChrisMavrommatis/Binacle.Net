# Custom Problems

Hand-authored packing problems, for the **tests kernel** (lib algorithm tests). Not used by ViPaq.

Unlike the Bischoff suite these have no OR-Library origin — they are small, deliberately-shaped cases we
wrote to cover specific behaviours (a box that just fits, a tight multi-item pack, and so on). Grouped into:

- `baseline.json` — simplest sanity cases.
- `simple.json` — small straightforward packs.
- `complex.json` — harder, mixed cases.

## Format

Same tests-kernel compact scenario format as the [Bischoff suite](../bischoff-suite/README.md):

```json
{
  "Name": "Baseline_5x5x5-1_FitsIn_60x40x10",
  "Bin": "60x40x10",
  "Metrics": "125 24000 1 0.5",
  "Result": "FullyPacked FullyPacked",
  "Items": ["5x5x5 [1]"]
}
```

- `Bin` — `LxWxH`; `Items` — `LxWxH [Quantity]` (types with a count, no coordinates).
- `Metrics` — `ItemsVolume BinVolume ItemsCount Percentage` (totals, not packed amounts); `Result` —
  `{PackingStatus} {FittingStatus}`.

`Metrics` is pure arithmetic over `Bin` + `Items`. `Result` is the **expected** outcome the tests assert against
— hand-set per case (these problems are authored, not converted from a source). This folder is the single source:
the tests kernel embeds these files directly (via `Link`/`LogicalName` in `Binacle.TestsKernel.csproj`) under the
manifest name `Binacle.TestsKernel.Algorithms.Data.CustomProblems.<file>`, so there is no separate kernel copy.
