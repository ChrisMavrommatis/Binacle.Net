---
description: Packing heuristics (FFD/WFD/BFD) — versions, operation types, trade-offs, and the fit/pack guarantee
---

# Algorithms

## Heuristics

Three heuristics, each with two versions:

| Heuristic | Versions |
|---|---|
| First Fit Decreasing (FFD) | v1, v2 |
| Best Fit Decreasing (BFD) | v1, v2 |
| Worst Fit Decreasing (WFD) | v1, v2 |

Each lives under `lib/src/Binacle.Lib/Algorithms/<Heuristic> v<N>/`.
See [Algorithm Factory](algorithm-factory.md) for the concrete class names (`FirstFitDecreasing_v2`, etc.).

All versions of a heuristic produce the same results. Newer versions are faster and use less memory.
The API currently uses **v2 for all three heuristics** — this is set in `lib/src/Binacle.Lib/AlgorithmFactory.cs`.
When writing new code, always use the latest version (currently v2).
Old versions are kept so you can benchmark without changing what the API uses — do not remove them.

## Trade-offs

<!-- sourced from docs site; verify against current code if behaviour changes -->

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

Set `AlgorithmOperation` on the parameters before calling the algorithm.

- `Fitting` — exits early as soon as an item doesn't fit
- `Packing` — keeps going and packs as many items as it can

See [Fit vs Pack](../concepts.md).
