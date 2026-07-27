# v4 - the endpoint descriptions overclaim which algorithms run

**Status:** Not started. Small, but it must land before the v4 OpenAPI document is generated for the docs site,
or the published spec carries the wrong claim.

## The problem

`api/src/Binacle.Net/v4/SchemaDescriptions.cs:27` describes the algorithm parameter as "FFD, WFD, BFD, or Best
to run them all and keep the best result". "Run them all" is only true on the single-bin routes - `fit/bin` and
`pack/bin`. Everywhere else `Best` races FFD + BFD, so WFD never runs.

The same phrasing may appear in the individual endpoint descriptions - grep `v4/` for it rather than fixing only
the one line.

The behaviour is deliberate and settled in the lib decisions ledger; the measurements behind it are in the lib
findings. Nothing about the code needs to change. The descriptions do.

Severity is low - v4 ships experimental - but a spec that overstates what runs is the kind of thing an integrator
builds an assumption on.

## What

- Correct the descriptions on the v4 endpoints so they say which set is raced on which route.
- Do it before `swagger/v4.json` is generated for the docs.
- The v4 docs pages, when written, must say the same thing.

## Done when

The descriptions match the behaviour, and the generated document reflects it.
