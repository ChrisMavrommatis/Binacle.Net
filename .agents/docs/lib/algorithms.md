---
id: lib/algorithms
description: Packing heuristics (FFD/WFD/BFD) — versions, operation types, trade-offs, and the fit/pack guarantee
verified: 2026-08-19
check: The six directories and their five-file layout match lib/src/Binacle.Lib/Algorithms/; v2 default confirmed in AlgorithmFactory.cs; both early-exit paths still read AlgorithmOperation.Fitting in every heuristic's AlgorithmOperation.cs
also_update:
  - lib/algorithm-factory
paths:
  - "lib/src/Binacle.Lib/Algorithms/**"

---

# Algorithms

## Heuristics

Three heuristics, each with two versions:

| Heuristic | Versions |
|---|---|
| First Fit Decreasing (FFD) | v1, v2 |
| Best Fit Decreasing (BFD) | v1, v2 |
| Worst Fit Decreasing (WFD) | v1, v2 |

Each lives under `lib/src/Binacle.Lib/Algorithms/<Heuristic> v<N>/`, and every one of the six holds the same
five files:

| File | Holds |
|---|---|
| `Algorithm.cs` | the `internal partial` class, its `Algorithm`/`Version`, and the constructor with its guard clauses |
| `AlgorithmOperation.cs` | `Execute(IOperationParameters)` — the packing loop itself |
| `Bin.cs`, `Item.cs`, `SpaceVolume.cs` | that version's private working types |

The class is split across the first two on purpose: construction validates and flattens the input, `Execute`
does the work, and a version's loop can be read without the setup around it.
See Algorithm Factory (`$lib/algorithm-factory`) for the concrete class names (`FirstFitDecreasing_v2`, etc.).

All versions of a heuristic produce the same results, and that is held up by the test suite rather than by
convention: `CommonTestingFixture` puts all six factories in `AlgorithmsUnderTest[]` and asserts each against
the same scenario expectations (`$lib/tests`). Newer versions are faster and use less memory.
The API currently uses **v2 for all three heuristics** — this is set in `lib/src/Binacle.Lib/AlgorithmFactory.cs`.
When writing new code, always use the latest version (currently v2).
Old versions are kept so you can benchmark without changing what the API uses — do not remove them.

## Trade-offs

- **FFD** — fast; places each item in the first available space. Default algorithm. Not always optimal.
- **WFD** — places each item in the space leaving the most unused room. Useful for spread/distribution,
  but generally slower and less efficient than FFD or BFD.
- **BFD** — places each item in the space leaving the least unused room. Often slightly better packing
  efficiency than FFD or WFD. Middle ground on speed.

## Guarantee

When Binacle confirms a bin fits, all items will fit — no false positives (precision = 1).
Heuristics may miss possible fits in rare cases — a negative result is not a guarantee (recall < 1).
This is a deliberate trade-off in favour of speed.

## Operation Types

Set `AlgorithmOperation` (`Binacle.Packing`) on the parameters before calling the algorithm.

- `Fitting` — stops at the first item that does not fit
- `Packing` — keeps going and packs as many items as it can

**Two different things are called "early", and only one of them shows in the result.** Before the loop runs,
`Fitting` checks whether the items outvolume the bin or whether any item is longer than the bin's longest
dimension; either returns straight away with status `EarlyExit` and an `EarlyExitReason`. Inside the loop,
`Fitting` simply breaks on the first item it cannot place, and the result completes normally with unpacked
items and no early-exit reason. Both checks are guarded by `parameters.Operation == AlgorithmOperation.Fitting`,
which is why a packing run always reaches the end (`$api/v3` traces what that costs the v3 contract).

See Fit vs Pack (`$concepts`).
