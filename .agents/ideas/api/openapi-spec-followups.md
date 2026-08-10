# Idea: OpenAPI spec follow-ups

**Status:** Parked, and down to one item. The spec audit is done and the Spectral lint (`.spectral.yaml`,
`just openapi lint`) guards it. Trimmed 2026-08-07, trimmed again 2026-08-10.

**The servers block was built on 2026-08-10** and its section is gone from this file. Both v3 and v4 now carry a
single relative `/`, set in the shared document transform; `just openapi lint` went from two `oas3-api-servers`
warnings to a clean run. That was the ordering constraint on turning the lint gate on in CI, so the one-liner in
the plans TODO file is unblocked.

What is left is the **codegen doc page**, which is still a genuine maybe.

## Codegen "generate a client" doc page

The payoff for the spec-first, generate-on-demand direction: a short docs page with copy-paste
commands that generate a client from the published per-version spec (e.g. `hey-api` for TS, `kiota` for C#). Turns
"there is a spec" into "here is your client in 30 seconds" — most of the win for one page. No SDKs are published;
consumers generate their own.

## Related

- the OpenAPI doc — how the documents are defined and transformed.

## Do not

- **Do not publish SDKs to close the codegen item** — the deliverable is a spec plus a codegen guide, not
  shipped packages. That decision is not this file's to hold or to change: it is recorded as a memory, which is
  where it survives if this idea is ever dropped.
