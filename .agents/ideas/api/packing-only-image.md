# Idea: a packing-only image variant, without the ServiceModule assemblies

**Status:** Unvetted idea. `SERVICE_MODULE` already defaults off — this is about shipping *less*, not changing a
default.

## What

Today one image ships everything and gates the ServiceModule behind the `SERVICE_MODULE` runtime flag, which is
already off by default (`$api/modules/service`). The flag stops the code running, but the ServiceModule DLLs are
still in the image.

Ship a **second tag/image that does not contain the ServiceModule assemblies at all** — packing/fitting only. Two
variants from one codebase:

- **full** (current) — everything, ServiceModule gated by the flag.
- **packing-only** — no auth, no accounts, no ServiceModule DLLs shipped.

## Why

A self-hoster who only wants packing gets a smaller image and a smaller surface — **no unused DLLs, no bloat**.
None of the auth/account/DB code is even present, not just switched off, so there is nothing there to misconfigure
into exposure.

## The approach — conditional compilation

Drop the ServiceModule assemblies at build time: a **build configuration / MSBuild condition** omits the
ServiceModule project reference (and its wiring) from the packing-only publish, so its DLLs are never built into
that image. The catch is that `Program.cs` and the DI wiring reference ServiceModule types directly behind the
flag today, so those references have to become conditional too (`#if`/partial wiring) for the API to compile with
ServiceModule absent.

The runtime-plugin alternative (load ServiceModule at runtime instead of referencing it at build time) is cleaner
but a much bigger change — noted, not chosen.

## What it touches

- `release-docker-image.yml` — a second build + tag.
- Project references / conditional compilation so the API builds without ServiceModule.
- The `SERVICE_MODULE` flag becomes a no-op (or is absent) in the packing-only variant.

## Trade-off

Two build targets to maintain vs. today's single image with a flag. The flag already gives full runtime
isolation; this buys a smaller image and a smaller attack surface at the cost of a second build path and keeping
the code compilable with ServiceModule removed.

## Open questions

- Is the smaller surface worth a second build path, or is the flag (off by default) enough.
- How much `#if`/partial-wiring plumbing `Program.cs` and the DI setup actually need to compile without
  ServiceModule — decides how invasive this is.
- Tag naming (`:core` / `:slim` / `:packing-only`).

## Related

- `$api/modules/service` (the flag, what it gates, the current compile-time reference)
