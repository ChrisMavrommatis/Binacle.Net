---
description: Packing heuristics, their versions, and operation types
---

# Algorithms

## Heuristics

Three heuristics, each with two versions:

| Heuristic | Versions |
|---|---|
| First Fit Decreasing (FFD) | v1, v2 |
| Best Fit Decreasing (BFD) | v1, v2 |
| Worst Fit Decreasing (WFD) | v1, v2 |

Each lives under `src/Binacle.Lib/Algorithms/<Heuristic> v<N>/`.

All versions of a heuristic produce the same results. Newer versions are faster and use less memory.
The API currently uses **v2 for all three heuristics** — this is set in `src/Binacle.Lib/AlgorithmFactory.cs`.
When writing new code, always use the latest version (currently v2).
Old versions are kept so you can benchmark without changing what the API uses — do not remove them.

## Operation Types

Set `AlgorithmOperation` on the parameters before calling the algorithm.

- `Fitting` — exits early as soon as an item doesn't fit
- `Packing` — keeps going and packs as many items as it can

See [Fit vs Pack](../concepts/fit-vs-pack.md).
