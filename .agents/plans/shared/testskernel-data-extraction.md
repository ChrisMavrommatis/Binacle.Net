# TestsKernel — grow the shared fixture cases

**Status (2026-07-15):** The data move is **done** — Bischoff suite, custom-problems, and result-selection are all
out of the kernel and embedded from `shared/data/` by manifest name (the kernel's `Algorithms/Data/` tree is empty,
tests green). Only the "review and grow the cases" work below remains. When nothing pending is left, delete this file.

Fixtures now live in `shared/data/` — one place they're generated or hand-authored, one place they live. A new JSON
file dropped into the right `shared/data/*` folder is picked up automatically (the `.csproj` embeds each set with a
`*.json` glob), so growing coverage is just authoring files. Provenance and the thpack1–7 vs thpack8/9 caveat live
in the `shared/data/*/README.md` files — read those before touching the data.

## Pending — review and grow result-selection

Result selection has the thinnest coverage: a single `baseline.json` per case (BestAlgorithm, BestBin, SmallestBin).

- Add scenarios that exercise the tie-breaks and edge picks each selector is meant to make (e.g. equal-fit bins
  where the smallest wins; algorithms that tie on fit but differ on efficiency; a bin that only one algorithm can
  fill). Name cases so the intent is obvious.
- Cross-check against the selectors in `ResultSelection/Providers/` so every branch has at least one scenario.

## Pending — add more problems to `custom-problems`

Bischoff is seven fixed instances, all 16-bit, all `PartiallyPacked`, so it cannot supply these. `custom-problems`
is the only hand-authored set, and adding a problem here reaches lib's algorithm tests **and** ViPaq's packed data
(regenerated from these definitions), so one addition serves both.

- **8-bit coverage.** Every Bischoff pack is 16-bit (coordinates to ~587). The only 8-bit scenario is a custom
  pack, and ViPaq's curated Bischoff slice is all 16-bit, so a real, size-measured 8-bit problem has to come from
  here. (Benchmarks get 8-bit from `SyntheticDataProvider`; this is about real, measured data.)
- **Uncompressed 16-bit coverage.** ViPaq's uncompressed set is all 8-bit — every 16-bit problem (Bischoff) is big
  enough that ViPaq compresses it, so there is no uncompressed-16-bit scenario to size or benchmark. Author a small
  16-bit problem — coordinates over 255 but few enough items to stay under the compression threshold (16-bit body:
  `2 + 6 + items*(3*2 + 3*2)` bytes ≤ 255 → ~20 items). See `$vipaq/findings`.
- **A count ladder.** One problem family at ~5, ~13, ~50, ~200 items, with **only the item count changing**. This
  pins ViPaq's compression-crossover report, which is otherwise provisional ("8-bit crosses somewhere between 16
  and 100 items").
- **Shape variety.** `simple`/`complex`/`baseline` are small and same-ish. Consider varied bin sizes, a single-item
  bin, and a near-perfect tessellation, so the algorithm tests exercise more than one regime.

**The cost, before you start.** Each scenario carries `Metrics` and `Result`, and both are *asserted*:

```json
{ "Name": "...", "Bin": "60x40x10", "Metrics": "125 24000 1 0.5",
  "Result": "FullyPacked FullyPacked", "Items": ["5x5x5 [1]"] }
```

`Metrics` is pure arithmetic (items volume, bin volume, item count, fill %) — computable, no packer needed.
`Result` is the **expected** outcome, and the tests run the real packer and check against it. So you must know what
the packer will do before you write the file — a new problem is a small piece of reasoning, not a paste.

Consider whether Bischoff already covers the algorithm cases well enough that `custom-problems` can stay small and
targeted, growing only for the reasons above.

## Watch out

- **Keep manifest names exact.** A fixture filename must have no extra dots beyond the extension — the embedder
  splits the dotted path on `.`, so an extra dot corrupts the manifest name. Verify with
  `strings <dll> | grep <prefix>` after building.
- **Never commit** — leave changes in the working tree for the human.
- Trim this plan as each item lands; delete the file when nothing pending remains.
