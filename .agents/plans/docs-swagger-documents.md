# Docs site - generate the v3 and v4 OpenAPI documents

**Status:** Not started. Gates the v3.0.0 release. Needs a build of the API, so it cannot be done from the docs
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

- `just openapi generate` writes both into `build/openapi/`. No server to bring up and no UI flag to set - the
  build starts the app host itself and dumps every registered document.
- That run has **no launch profile, so ServiceModule is OFF**, which is the committed convention: the committed
  `v3.json` has no `/api/auth/token` path, a ServiceModule-on run adds it, and the two documents stop being
  comparable. Check the generated files for that path before handing them over.
- Confirm the v4 document carries the experimental warning in its description. `ApiV4Document.IsExperimental`
  must be `true` for v3.0.0 - if the banner is missing, the flag was flipped early and that is a release blocker,
  not a docs problem.
- Hand both files over. The `.md` page beside each one is the docs session's job; the shape to copy is
  `v2.1.x/swagger/v3.md`.

## Watch out

**`build/openapi/` is gitignored**, so a generated pair does not survive a commit. Generate at hand-over time and
copy the two files across in the same sitting.

## Verified on 2026-07-28

A full `just openapi generate` run was done and every check above passed, so only the copy-across is left:

- Both documents are written: `build/openapi/Binacle.Net_v3.json` and `Binacle.Net_v4.json`. Note the file names
  carry the assembly prefix - they are renamed to `v3.json` / `v4.json` on the way into the docs folder.
- ServiceModule is off as intended: neither document has an `/api/auth/token` path.
- `ApiV4Document.IsExperimental` is `true` and the banner is in the v4 description.
- The algorithm claim was corrected in the code, so both documents now carry the right wording. That plan is
  closed.
- `just openapi lint` gives 0 errors and the 2 parked `oas3-api-servers` warnings.

## Done when

`v3.json` and `v4.json` both sit in the `v3.0.x` docs folder, generated on the Normal profile in the same run,
with the experimental banner present on v4.
