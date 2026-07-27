# Docs site - generate the v3 and v4 OpenAPI documents

**Status:** Not started. Gates the v3.0.0 release. Needs a running API, so it cannot be done from the docs
session alone - produce the files here, hand them over.

## Both documents, not just v4

Every version folder ships its own swagger set: `docs/collections/_versions/v2.1.x/swagger/` holds `v2.json` and
`v3.json` with a `.md` page each, and `v1.3.x` holds three. So `v3.0.x` needs **`v3.json` and `v4.json`**, plus
their pages.

Neither JSON can be copied forward from `v2.1.x`:

- **`v3.json` changed in this release.** The ViPaq payload in its responses is the rebuilt format, and the
  document is emitted by different OpenAPI tooling than the one committed under `v2.1.x`.
- **`v2.json` must not appear** in the `v3.0.x` folder at all. V2 is removed in this version and lives on in
  `v2.1.x` / `v2.0.x`.

Generating only v4 leaves the docs session waiting on a `v3.json` that nobody agreed to make.

## What

Generate both into the `v3.0.x` docs folder, in one run:

- Run the API with `SWAGGER_UI` or `SCALAR_UI` on and fetch `/openapi/v3.json` and `/openapi/v4.json`.
- Run it on the **`Normal` profile, ServiceModule OFF**, so the specs match the committed convention. The
  committed `v3.json` has no `/api/auth/token` path; a ServiceModule-on run adds it, and the two documents stop
  being comparable.
- Confirm the v4 document carries the experimental warning in its description. `ApiV4Document.IsExperimental`
  must be `true` for v3.0.0 - if the banner is missing, the flag was flipped early and that is a release blocker,
  not a docs problem.
- Hand both files over. The `.md` page beside each one is the docs session's job; the shape to copy is
  `v2.1.x/swagger/v3.md`.

## Watch out

The v4 endpoint descriptions currently claim `Best` races "all algorithms (FFD, WFD, BFD)", which is not true on
every route. That correction is its own piece of work and should land in the code **before** these documents are
generated, or the published spec carries the wrong claim.

## Done when

`v3.json` and `v4.json` both sit in the `v3.0.x` docs folder, generated on the Normal profile in the same run,
with the experimental banner present on v4 and the algorithm claim already corrected.
