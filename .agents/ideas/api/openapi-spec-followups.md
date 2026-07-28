# Idea: OpenAPI spec follow-ups

**Status:** Parked. The spec audit is done and the Spectral lint (`.spectral.yaml`, `just openapi lint`)
guards it. These are the loose ends left deliberately for later — none block anything today.

## Example double-encoding (re-enable the Spectral rule)

Every OpenAPI example `value`/`example` is emitted as a **JSON string** (double encoded), e.g.
`"example": "{\"parameters\":…}"` instead of `"example": { "parameters": … }`. This comes from the
`OpenApiExamples` integration, not the contracts. Effects: Swagger UI / Scalar and generated-SDK examples show an
escaped string blob instead of a real object.

Spectral's built-in `oas3-valid-media-example` flags all of them (~165), so it is turned **off** in `.spectral.yaml`
for now. The fix is to emit examples as inline objects — likely a small document transformer that parses each
string example back into a `JsonNode` — after which `oas3-valid-media-example` should be re-enabled.

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
  `just openapi generate`) is now the source for that, and for v4's folder when it freezes.
- Run `just openapi lint` in CI so the spec standards can't regress on a PR. One call, and it generates the
  documents itself — natural to hang the SDK generation off the same job later.

## Related

- the OpenAPI doc — how the documents are defined and transformed.

## Do not

- **Do not re-enable `oas3-valid-media-example` before the examples are fixed** — it will just flag all ~165 again.
- **Do not publish SDKs to close the codegen item** — the decision is a spec plus a codegen guide, not shipped
  packages.
