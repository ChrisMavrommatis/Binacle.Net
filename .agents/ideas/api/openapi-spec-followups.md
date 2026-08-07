# Idea: OpenAPI spec follow-ups

**Status:** Mostly parked. The spec audit is done and the Spectral lint (`.spectral.yaml`, `just openapi lint`)
guards it. Trimmed 2026-08-07: a third section was deleted because both its bullets had become work tracked
properly elsewhere — the `v3.0.x` swagger handover is in the docs-site plan, and running the lint in CI is a
one-liner in the plans TODO file.

Two loose ends left. The **servers block** now has a decided shape and an ordering constraint, so it is really
a small piece of scheduled work sitting in an idea file — pull it out if it stays here much longer. The
**codegen doc page** is still a genuine maybe.

## Servers block (base-URL story) — shape decided 2026-08-07, not built

The document currently has **no `servers`** — the setter was removed. Spectral's `oas3-api-servers` warns about
it, and a generated client then has no default base URL.

**The shape is settled: a single relative `/`.** Binacle ships as a docker image people self-host, so the API
lives at the root of wherever the reader deployed it. `https://api.binacle.net` is real — it serves the packing
demo on the docs and marketing sites — but naming it in the spec would bake it into every generated client as
the default, aiming other people's traffic at our host until they override it. A relative `/` is honest, clears
both warnings, and costs the reader nothing they were not going to do anyway.

Rejected: `api.binacle.net` as the default (above), and a templated `{scheme}://{host}` variable — most
ceremony, and generators handle templated servers unevenly.

**Do this before turning on the lint gate**, which is a one-liner in the plans TODO file. Clearing the two
warnings first lets that gate fail on warnings from day one instead of going green with known noise and
quietly ceasing to be read.

## Codegen "generate a client" doc page

The payoff for the spec-first, generate-on-demand direction: a short docs page with copy-paste
commands that generate a client from the published per-version spec (e.g. `hey-api` for TS, `kiota` for C#). Turns
"there is a spec" into "here is your client in 30 seconds" — most of the win for one page. No SDKs are published;
consumers generate their own.

## Related

- the OpenAPI doc — how the documents are defined and transformed.

## Do not

- **Do not publish SDKs to close the codegen item** — the decision is a spec plus a codegen guide, not shipped
  packages.
