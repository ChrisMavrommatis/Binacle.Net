---
id: api/decisions
description: API decisions ledger — why a module-off document carries no `429` and what guarantees it, what the generated documents are a document of, and why the API sends no HSTS header.
verified: 2026-08-22
check: D1 against api/src/Binacle.Net.Kernel/OpenApi/Transformers/RateLimiterResponseOperationTransformer.cs, which must check EnableRateLimitingAttribute and nothing else; against a grep for EnableRateLimitingAttribute and RequireRateLimiting over api/src, which must land only inside Binacle.Net.ServiceModule; and against ApiDocument.Transform for the relative servers entry and the GitHub/Docker Hub description. D2 against a grep for UseHsts over api/src, which must return nothing
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

### D2 — the API sends no HSTS header, and TLS is the proxy's job

**`UseHsts()` was deleted from `UseUIModule` on 2026-08-22.** Nothing in the API sends
`Strict-Transport-Security` now, and nothing should add it back without reversing this entry.

**Where it came from.** It is the ASP.NET Core Razor Pages and MVC template block, `!IsDevelopment()` guard
and all, and it arrived with the module when the module became Razor Pages. **Microsoft's own API templates
leave HSTS out**, and say why: "The default API projects don't include HSTS because HSTS is generally a
browser only instruction. Other callers, such as phone or desktop apps, do not obey the instruction."
(<https://learn.microsoft.com/aspnet/core/security/enforcing-ssl>.) This repo is an API that grew a Razor
Pages module, so it inherited the web-app template's answer to a question the API template had already
answered the other way.

**It was inert where it stood.** The published image terminates no TLS — it has no certificate, and no sample
or smoke profile sets an HTTPS port; every one of them listens on plain 8080. A browser ignores
`Strict-Transport-Security` on a plain-HTTP response, so the call could only ever fire for someone who gave
Kestrel a certificate **and** switched the demo on, a combination nothing in the repo describes.

**Three reasons it is the wrong thing for this image to send, and they outlive the placement:**

- **The header belongs to whatever owns the hostname.** Where there is TLS it is terminated by a proxy,
  ingress or CDN, and that is the only thing that knows whether `includeSubDomains` and `preload` are safe.
  ASP.NET's default — 30 days, no subdomains — is a guess made by the wrong party.
- **It is the one header that cannot be taken back.** A browser honours it for its max-age, and the only undo
  is serving `max-age=0` over HTTPS from the same host. An image people run on internal hostnames must not
  pin that by default.
- **The callers are servers.** This is the API behind someone's backend; HSTS is a browser mechanism and an
  HTTP client ignores it.

**It was also on the wrong switch.** Inside `UseUIModule` it existed only when the demo UI was on — absent
from exactly the deployments most likely to face the internet — and it ran after eight other middlewares, so
anything answering earlier never got it either. Both faults predate the module rebuild; the call moved with
the file rather than being introduced by it.

**Expect an analyser to ask for it.** A missing HSTS call is a standard ASP.NET finding. The answer is this
entry: the repo answers findings where a reader can see the answer, never with a suppression rule.

**The rejected alternative was an opt-in `Hsts__Enabled`** in `Program.cs` beside `UseHttpsRedirection`. That
is an options class, a config file, a documented key and a feature switch, for a header the proxy in front
sets in one line.

**The same page says the proxy settles it**: "If the proxy server also handles writing HSTS headers ... HSTS
middleware isn't required by the app."

**`UseHttpsRedirection()` stays**, untouched and unexamined by this decision. With no HTTPS port configured it
logs that it cannot determine one and does nothing, so it costs nothing today. **What is worth knowing before
anyone changes that**: Microsoft warns that an app calling both `UseHttpsRedirection` and `UseHsts` "put[s] a
site into an infinite loop if deployed to an Azure Linux App Service, Azure Linux virtual machine (VM), or
behind any other reverse proxy besides IIS. The reverse proxy terminates TLS, and Kestrel isn't made aware of
the correct request scheme." (<https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer>.)
`ForwardedHeaders` ships **off** in this image, so anyone who configures an HTTPS port behind a proxy walks
into exactly that. Deleting the HSTS half removes one leg of it.
