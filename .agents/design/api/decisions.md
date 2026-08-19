---
id: api/decisions
description: API decisions ledger — why a module-off document carries no `429` and what guarantees it, and what the generated documents are a document of.
verified: 2026-08-19
check: D1 against api/src/Binacle.Net.Kernel/OpenApi/Transformers/RateLimiterResponseOperationTransformer.cs, which must check EnableRateLimitingAttribute and nothing else; against a grep for EnableRateLimitingAttribute and RequireRateLimiting over api/src, which must land only inside Binacle.Net.ServiceModule; and against ApiDocument.Transform for the relative servers entry and the GitHub/Docker Hub description
paths:
  - "api/**"
---

# API — decisions ledger

Why the API is shaped the way it is where the shape is not obvious from the code. What it *does* is `$api` and
the docs under it; this file is the reasoning, so a later session does not undo a deliberate choice.

## Locked

### D1 — a module-off document must carry no `429`, and the metadata is what guarantees it

`RateLimiterResponseOperationTransformer` documents `429 Too Many Requests` when the operation carries
`[EnableRateLimiting]`, and that single check is enough because **only `AddServiceModule` ever attaches it**.
The core endpoints call `.RateLimited()`, a marker naming no policy; the module's `IEndpointConvention` turns
that marker into the attribute. Metadata present therefore *means* a limiter is registered.

**The invariant is the assembly, not the convention.** The module's own `/api/auth/token` calls
`.RequireRateLimiting("AuthToken")` directly, which attaches the same attribute without going through the
convention — and that is fine, because the endpoint only exists in a build where the module is on. What must
stay true is that no attach point sits outside `Binacle.Net.ServiceModule`. A `.RequireRateLimiting(...)` added
to a core v3 or v4 endpoint file is the exact regression this decision exists to prevent, and it would pass a
check that only watched the convention.

**It took two guards until 2026-08-14, and the second one was load-bearing.** The v3/v4 endpoint files used to
call `.RequireRateLimiting("ApiUsage")` directly, naming a policy only the module supplies. With the module off
the call was a no-op but the metadata was still there, so the transformer also had to check the `"RateLimiter"`
feature — otherwise the document advertised a response the build cannot emit, which is a false statement in a
contract. **Moving the policy name into the module removed the need for that guard rather than the guard
alone.** Do not read its deletion as a decision that metadata was always sufficient: it was not, and the
difference is which assembly puts the attribute there.

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
the `429`; regenerating them takes it out and returns the v3 document to the shape v2.1.x published. On
2026-08-14 the guard came out for the last time, because by then nothing was left for it to guard.

**If a contract for the hosted service is ever wanted, it is a second document**, generated with the module on,
carrying the auth paths and the `429` together. Do not approximate it by loosening this one.
