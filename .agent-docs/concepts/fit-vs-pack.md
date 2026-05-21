---
description: Fit exits early on first failure; pack continues and returns positions. Used by both Lib algorithms and API endpoints.
---

# Fit vs Pack

Fit and pack use the same algorithm. The difference is what happens when an item doesn't fit.

## Fit

**Question answered:** "Do all items fit in this bin?"

- Stops as soon as an item doesn't fit
- Fast — no extra work once it finds a failure

## Pack

**Question answered:** "Pack everything you can and tell me the result."

- Runs through all items regardless
- Use this when you need to know how much packed, not just whether it all fits

See [v4.md](../api/v4.md) for how this distinction maps to API endpoints and response shapes.

Both fit and pack share the algorithm guarantee — a positive result is reliable; a negative result may be a heuristic miss.
