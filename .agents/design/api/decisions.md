---
id: api/decisions
description: API decisions ledger — why the OpenAPI `429` is gated on the RateLimiter feature and not on endpoint metadata alone, and what the generated documents are a document of.
verified: 2026-08-13
check: D1 against api/src/Binacle.Net.Kernel/OpenApi/Transformers/RateLimiterResponseOperationTransformer.cs, which must check both the "RateLimiter" feature and EnableRateLimitingAttribute
paths:
  - "api/**"
---

# API — decisions ledger

Why the API is shaped the way it is where the shape is not obvious from the code. What it *does* is `$api` and
the docs under it; this file is the reasoning, so a later session does not undo a deliberate choice.

## Locked

### D1 — the OpenAPI `429` is gated on the `"RateLimiter"` feature, not on endpoint metadata alone

`RateLimiterResponseOperationTransformer` documents `429 Too Many Requests` only when **both** hold: the
`"RateLimiter"` feature is enabled, and the operation carries `[EnableRateLimiting]`. Either guard alone
produces a wrong document.

**Why the feature guard cannot be dropped.** `.RequireRateLimiting("ApiUsage")` sits unconditionally in the
v3/v4 endpoint files, but the limiter itself is registered by `AddServiceModule`. With the module off the call
is a no-op, so the metadata on its own declares nothing a caller can observe. Documenting the `429` from the
metadata alone puts a response in the document that the build cannot emit, which is a false statement in a
contract.

**What the generated documents are a document of.** `just openapi generate` builds them from a host with no
launch profile, so the ServiceModule is off, and the output describes **the image a self-hoster runs at its
defaults** — not the hosted deployment. Two things confirm the reading: `servers` is the single relative `/`,
and `info.description` points at GitHub and Docker Hub rather than at any host. That document already omits
everything else the module contributes — the whole `v0` ServiceModule document, `/api/auth/token` — so a `429`
inside it is the one inconsistency, describing a shape that exists nowhere.

**This was reversed once, in both directions.** The feature guard was removed on 2026-07-19 in "open api
improvements", on the reasoning that `429` is part of the endpoint's contract and a generated client should be
told it can happen. That argument fails on its own terms: a client generated from the module-off document also
has no `/api/auth/token`, which is the thing it would need in order to stop being rate-limited, so the
document serves neither the self-hoster nor a caller of the hosted service. The guard went back in on
2026-08-13, before v3.0.0 shipped. The removal reached the v3.0.0 betas, whose published swagger copies carry
the `429`; regenerating them takes it out and returns the v3 document to the shape v2.1.x published.

**If a contract for the hosted service is ever wanted, it is a second document**, generated with the module on,
carrying the auth paths and the `429` together. Do not approximate it by loosening this one.
