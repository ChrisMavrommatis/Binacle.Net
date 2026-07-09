# Packed placed-result data

Frozen, packed results read by the ViPaq test kernel's `BischoffDataProvider` and `CustomProblemsDataProvider`
(merged for curated runs by `CuratedScenarioProvider`). Each sample is a bin plus the
**placed** items a packing run produced — dimensions **and** coordinates (`L x W x H (X,Y,Z)`) — which is what
ViPaq serializes. The source problems carry only item *types* with a quantity and no coordinates, so the
coordinates only exist after packing.

## Generated — do not hand-edit

These files are produced by `vipaq/tools/Binacle.ViPaq.PackedDataGenerator`. To change them, edit the source
problems (`shared/data/bischoff-suite`, `shared/data/custom-problems`) or the tool, then regenerate:

```
dotnet run --project vipaq/tools/Binacle.ViPaq.PackedDataGenerator
```

The run is deterministic: a no-change re-run is byte-identical, so it produces no git noise.

## Layout

Split by source family, mirroring `shared/data`:

- `custom-problems/` — `baseline`, `complex`, `simple`.
- `bischoff-suite/` — `orlib_thpack1` .. `orlib_thpack7` (BR1–BR7).

The **algorithm** rides on the file name as a `.<algo>` suffix, not a folder — e.g. `orlib_thpack1.ffd.json`.
Only **FFD** is generated today; WFD/BFD can be added later (one more entry in the tool) and land as
`.wfd.json` / `.bfd.json` files beside the FFD ones. Different algorithms place items differently, so their
coordinates — and tokens — differ; the suffix keeps the sets apart without duplicating the folder tree.

The tool prints a per-file and total sample/item count on each run; that console summary is the run's report,
so there is no committed index file to keep in sync.

## File format

Each problem file is a JSON array of samples. One sample:

```json
{
  "Name": "OrLibrary_thpack1_1",
  "WidthBits": 16,
  "Bin": "587x233x220",
  "Items": ["92x81x55 (0,0,0)", "92x81x55 (92,0,0)"]
}
```

- `Name` — the source problem's name.
- `WidthBits` — the width family: `8` if every bin dimension, item dimension and coordinate fits in a byte,
  else `16`. ViPaq still chooses the actual per-section width from the values at encode time; this is only a
  label for grouping.
- `Bin` — the container as `"LxWxH"`.
- `Items` — the placed items as `"LxWxH (X,Y,Z)"`. Bischoff instances are `PartiallyPacked` by design (they
  fill ~98%, never tessellate perfectly), so not every source box appears here — only the placed ones.

Only placed geometry is stored — no ViPaq token. The token is derivable from `Bin`+`Items`, and its compressed
bytes vary by gzip encoder/runtime, so committing it would churn the files on every regen. The kernel computes
the token itself when it benchmarks. Every sample is still round-tripped (encode → decode == input) at
generation time, or the run fails.
