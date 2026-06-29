---
description: Fit exits early on first failure; pack continues and returns positions. Both return the same result shape — packed items and unpacked items. Used by both Lib algorithms and API endpoints.
verified: 2026-05-23
check: Fit/pack behavior matches AlgorithmOperation usage in lib/src/Binacle.Lib/
---

# Concepts

Ideas that span multiple slices. Read these before diving into a specific slice.

## Fit vs Pack

Fit and pack use the same algorithm. The difference is what happens when an item doesn't fit.

### Fit

**Question answered:** "Do all items fit in this bin?"

- Stops as soon as an item doesn't fit
- Fast — no extra work once it finds a failure

### Pack

**Question answered:** "Pack everything you can and tell me the result."

- Runs through all items regardless
- Use this when you need to know how much packed, not just whether it all fits

### Same result shape

Both fit and pack return the same result: packed items and unpacked items.
Fit exits early, so the result reflects where it stopped — not a full run.
If you send 10 items and fit stops on item 4, items 5–10 are all "unpacked" even though they were never tried.

See [api/v4/README.md](api/v4/README.md) for how this distinction maps to API endpoints and response shapes.

Both fit and pack share the algorithm guarantee — a positive result is reliable; a negative result may be a heuristic miss.
