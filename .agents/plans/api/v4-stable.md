# v4 — flip from experimental to stable

**Status:** Waiting on 3.0.0 shipping. v4 goes out experimental in 3.0.0 and stays that way for the whole
3.0.x line. 3.1.0 is the intent, not a commitment — the flip needs the second condition below, and no endpoint
is planned that would satisfy it yet.

## Why it ships experimental first

v4 has never been called by a real user. Shipping it stable would lock its contracts on the strength of a design
nobody has used yet, and every later reshape becomes a breaking change against people who trusted the document.
Experimental costs one boolean and some adoption for one release; getting it wrong the other way costs a
breaking change in a version that promised none.

The API declares this itself: `ApiV4Document.IsExperimental` drives a warning banner into the published OpenAPI
description ("This API version is experimental and may change any time, introducing breaking changes"). v3 sets
it false, v4 sets it true.

## Flip when both are true

1. v4 has run in a real deployment — the 3.0.0 beta counts.
2. At least one endpoint has been added to v4 **without** reshaping an existing request or response. That is the
   evidence that the contract shape holds, which is the whole claim "stable" makes.

If the second one fails — if adding an endpoint forces an existing contract to change — v4 is not ready, and the
right move is to make that change while it is still experimental.

## What the flip involves

- `api/src/Binacle.Net/v4/ApiV4Document.cs` — `IsExperimental => false`, and drop the comment above it.
- Regenerate the docs-site `swagger/v4.json` so the banner disappears from the published spec, and drop the
  "experimental" marking from the v4 docs pages.
- Say it in the 3.1.0 release notes: v4 is now stable and grows by adding endpoints.
- Update the v4 agent doc, which records the experimental marking as current truth.
