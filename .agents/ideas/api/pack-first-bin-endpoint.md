---
description: pack/first-bin endpoint
paths:
  - "api/**"
---

# Idea: pack/first-bin endpoint

**Status:** Unvetted idea. Was planned for v4, then pulled out — it may target v3.1 instead. Nothing is decided.

## What

A packing endpoint that answers with the first bin that succeeds, rather than the best or the smallest one.

- `POST .../pack/first-bin` — custom bins → first success
- `POST .../pack/first-bin/{preset}` — preset bins → first success

The caller supplies bins in the order they care about, and gets back the first one the items fit into.

**Do not call this `first-fit`.** That name collides with First Fit Decreasing, an algorithm already selectable
through `Parameters.Algorithm`. v4 shipped the same mistake once as `pack/best-fit` and had to rename it to
`pack/best-bin` — see the naming rule in the v4 API doc. A route names the bin it returns; the algorithm is a
parameter. `first-bin` follows `smallest-bin` / `best-bin`, and a `FirstBin` strategy class would match.

## Why

The existing selecting endpoints (`pack/smallest-bin`, `pack/best-bin`) both optimize. Neither lets the caller
say "I have a preference order — give me the first that works". A warehouse with a stack of box sizes it wants
to consume in a set order can't express that today.

## The two meanings of "first success"

This is the decision that blocked it, and it is still open. The name covers two different endpoints:

- **Selection only** — run every bin, return the first successful one in request order. A small strategy class
  next to `BestBin` / `SmallestBin` in `lib/src/Binacle.Lib/ResultSelection/`, consistent with how the other
  selecting endpoints work. But it saves no compute, so the name promises something the endpoint doesn't do.
- **Short-circuit** — stop at the first bin that packs. This is the version that earns the name and the only
  one with a performance story. Needs a new bin processor: `IBinProcessor.Process` runs all bins today.

The second is the interesting one and the expensive one. Pick before writing code.

## Open questions

- Which version does this land in? It was cut from v4. If it goes to v3.1, that reopens a frozen surface —
  worth checking that against how v3 is treated in the v3 API doc.
- Does short-circuit change the response shape? Every other selecting endpoint has results for all bins
  available; this one wouldn't.
- "First" is caller-supplied order, so the answer depends on request order in a way no other selecting endpoint
  does. Worth saying so in the endpoint description if this ever gets built.

## Related

- the result-selection doc — where a selection-only strategy would live
- the v4 API doc — the selecting endpoints it would sit beside
- the v4 add-endpoint guide — the build steps, once decided
