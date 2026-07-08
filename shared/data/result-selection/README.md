# Result Selection

Hand-authored result-selection scenarios, for the **tests kernel** (lib result-selection tests). Not used by ViPaq.

These fixtures exercise how the lib picks a single winning result out of many candidate packings. Unlike the
algorithm fixtures (Bischoff suite, custom-problems) these do not describe a packing problem — each case lists a set
of already-computed results and the one the selector is expected to choose. One folder per selector:

- `BestAlgorithm/` — pick the best result across algorithms.
- `BestBin/` — pick the best bin.
- `SmallestBin/` — pick the smallest bin that still fits.

Each folder has a single `baseline.json` today (thin coverage — see the extraction plan for growth notes).

## Format

A JSON array of scenarios. Each scenario names the expected winner and the candidate results to choose from:

```json
{
  "Name": "Best Bin - One Fully Packed winner",
  "ExpectedResult": "60x40x30",
  "Results": {
    "60x40x10": "60x40x10 FFD_v2 PartiallyPacked 40 60",
    "60x40x20": "60x40x20 FFD_v2 PartiallyPacked 80 70",
    "60x40x30": "60x40x30 FFD_v2 FullyPacked 95 100"
  }
}
```

- `ExpectedResult` — the bin key the selector under test must choose.
- `Results` — candidate results keyed by bin; each value is a compact operation result
  `Bin Algorithm PackingStatus <metric> <metric>`.

This set uses its **own** provider/reader/model (`ResultSelection/ScenarioCollectionsProvider.cs`, its own
`Scenario` model and `CollectionKeys`) — a different shape from the algorithm fixtures; the two are kept separate.

This folder is the single source: the tests kernel embeds these files directly (via `Link`/`LogicalName` in
`Binacle.TestsKernel.csproj`) under the manifest name `Binacle.TestsKernel.ResultSelection.Data.<Case>.<file>`, so
there is no separate kernel copy.
