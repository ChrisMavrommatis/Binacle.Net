# Idea: reduce integration friction

**Status:** Direction decided (2026-07-19) — **spec-first, generate-on-demand.** Plugins for non-developers stay
a separate, demand-driven bet. What is left here is the plugin thread and the open questions; the SDK half is
settled and has moved.

## The decision — moved out of this file 2026-08-07

**"We ship a spec, not SDKs" is now recorded as a memory,** with its rationale and what would reverse it. It
was living only here and in one other idea, which is the layer that gets deleted when an idea is built or
dropped — a standing decision cannot survive there. Read it there rather than restating it; this file must not
become a second copy that can disagree.

What that leaves for this idea: the spec side is done and guarded by the lint, so the remaining question is
**everything that is not a developer with a code generator.**

## What makes generate-on-demand actually work

The spec has to be SDK-grade or people generate junk and give up. That work is done: stable **operationIds**
(v3 flat, v4 resource-grouped), **plain-prose descriptions** (no Swagger markdown leaking into generated code),
request/response **examples**, one **frozen document per version**, and **build-time emission** so specs never
require a running server (`OpenApiGenerateDocumentsOnBuild`). See the OpenAPI doc.

The cheap, high-value next step is a **"generate a client" doc page** — copy-paste commands pointing at the
published per-version spec, e.g. `npx @hey-api/openapi-ts -i …/openapi/v4.json -o ./binacle` (TS) and
`kiota generate -l CSharp -d …/openapi/v4.json -o ./Binacle` (C#, no Java needed). That turns "there is a spec
somewhere" into "here is your client in 30 seconds" — most of the win for the price of one page.

## Generator notes (empirical, from a real spike)

Findings that matter if we ever do publish, or when writing the codegen guide:

- **Grouping comes from tags, method names from operationId.** Every generator groups operations by their tag
  (`Pack`/`Fit`/`Presets` here). The operationId is the method. So the `client.pack.…()` shape is driven by tags,
  which we already have — not by anything exotic.
- **v4 uses dot-notation operationIds** (`pack.customBin`). This pays off with dot/nesting-aware generators
  (hey-api's nesting function, Speakeasy's grouping) and reads as `client.pack.customBin()`. With the free
  `openapi-generator` the dot is sanitized, and `_` is the more portable delimiter — but the call site is the same.
- **Kiota ignores operationIds** — it builds method names from the URL path (`client.Api.V4.Pack.Bin.PostAsync`).
  Cleaner code, no Java, but it does not use our operationId naming. A trade of call-style for output quality.
- **Tool picks:** TS → hey-api (uses the dotted ids). C# → Kiota (free, clean, path-style) or a paid tool
  (Speakeasy/Stainless) for idiomatic `client.Pack.CustomBin()`. `openapi-generator` is the free multi-language
  option but needs Java. NSwag works but emits one very verbose file.

## Platform plugins — the non-developer bet

A plugin meets a store owner where they already sell: no wiring, they install and get packing answers. Widest
reach, but it is a **product**, not a library, and it is the only path that serves people who will never run a
generator.

- **WooCommerce first, if we do one.** Largest raw install base, open publishing (WordPress.org, no gated review),
  self-hosted PHP that calls our API — so we host nothing per merchant.
- **Shopify is the flashiest and the most work.** Hosted, reaches non-developers directly, but demands app review,
  OAuth, HMAC webhook verification, an embedded (Polaris) UI, and it ties into ServiceModule auth and billing.

Either way, **pin the integration point first**: where in a checkout/fulfillment flow does a packing answer get
used (shipping-rate step, fulfillment box pick)? The plugin is thin once that is clear, and pointless until it is.

## Open questions

- When is demand "enough" to publish an official SDK, and for which language first (TS is the likeliest)?
- Plugins that call a hosted Binacle need auth and per-tenant identity — leans on the ServiceModule rework and auth
  story.
- A packing-only image is the natural thing a self-hosting plugin user would run.

## Related

- the OpenAPI doc — the spec that makes generate-on-demand work.

## Do not

- **Do not build a plugin before the integration point is pinned.** A plugin that calls the API at the wrong step
  in checkout is worse than no plugin.

The SDK rule that used to sit here is a memory now — see the note at the top.
