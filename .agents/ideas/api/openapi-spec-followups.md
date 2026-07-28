# Idea: OpenAPI spec follow-ups

**Status:** Parked. The spec audit is done and the Spectral lint (`.spectral.yaml`, `just openapi lint`)
guards it. These are the loose ends left deliberately for later — none block anything today.

## Servers block (base-URL story)

The document currently has **no `servers`** — the setter was removed. Spectral's `oas3-api-servers` warns about
it, and a generated client then has no default base URL. Decide the shape: a relative `/`, a templated server
variable, or a plain default. It is a **warning**, so it does not fail the lint; adding a server clears it.

## Codegen "generate a client" doc page

The payoff for the spec-first, generate-on-demand direction: a short docs page with copy-paste
commands that generate a client from the published per-version spec (e.g. `hey-api` for TS, `kiota` for C#). Turns
"there is a spec" into "here is your client in 30 seconds" — most of the win for one page. No SDKs are published;
consumers generate their own.

## Wire the spec into the docs freeze, and lint in CI

- The `v3.0.x` docs version folder has only `index.md` — no swagger dump. `build/openapi/` (written by
  `just openapi generate`) is now the source for that, and for v4's folder when it freezes. Regenerate before
  handing them over so the published spec carries the real example objects.
- Run `just openapi lint` in CI so the spec standards can't regress on a PR. One call, and it generates the
  documents itself — natural to hang the SDK generation off the same job later.

## Related

- the OpenAPI doc — how the documents are defined and transformed.

## Do not

- **Do not publish SDKs to close the codegen item** — the decision is a spec plus a codegen guide, not shipped
  packages.
