# Docs Site — write the v3.0.x pages

**Status (2026-07-21):** The version-only restructure is **DONE** — folders are `v1.3.x`, `v2.0.x`, `v2.1.x`,
`v3.0.x`; `latest` is a redirect only; the site builds clean. The model, the standing rule for opening a new
line, and the history/rationale now live in the canonical doc: **`.agents/docs/docs-site/README.md` → "Versioning
model"**. What remains is writing the `v3.0.x` docs.

## What is left

`v3.0.x` is a **stub**: `index.md` only, carrying the intro prose and a notice pointing at `v2.1.x`. Every other
page must be written fresh for the release — nothing is carried over from `v2.1.x` by choice.

- [ ] Write the `v3.0.x` pages: `api/` (v3 + v4), `swagger/`, `configuration/`, `samples/`, `quick-start.md`,
      `release-notes.md`. **API v2 must not reappear** — it is removed in this version and lives on in
      `v2.1.x` / `v2.0.x`.
- [ ] Two `configuration/` pages are **new in v3.0.x** and have no `v2.1.x` equivalent to copy:
      - `configuration/core/forwarded-headers.md` — running behind a proxy or CDN. Source of truth is
        the API configuration doc; needs the trust settings, the container and tunnel cases, using `/_debug` to read
        the proxy's address, and why `ASPNETCORE_FORWARDEDHEADERS_ENABLED` is ignored.
      - `configuration/diagnostics-module/health-checks.md` — carry over from `v2.1.x`, then document the three
        breaking `RestrictedIPs` changes — CIDR now means a prefix length, IPv4-mapped callers now match, and the
        `start-end` range form is gone — and that the list needs forwarded headers to match behind a proxy.
- [ ] Remove the notice block at the bottom of `v3.0.x/index.md` once the pages exist, and restore its section
      links (copy the shape from `v2.1.x/index.md`, minus V2).
- [ ] Generate `swagger/v4.json` — run the API and fetch `/openapi/v4.json` with `SWAGGER_UI` or `SCALAR_UI`
      on, on the **`Normal` profile (ServiceModule OFF)** so the spec matches the committed convention (the
      committed `v3.json` has no `/api/auth/token` path; a ServiceModule-on run adds it).
- [ ] Mark API v4 **experimental** — reuse the banner v3 carried in `v1.3.x/api/v3.md`.

**`vlink` raises and fails the build on a missing target** — that is why the stub has no section links yet. Add a
link only when its target lands. (More in the canonical doc's "Watch out".)

Delete this file once the `v3.0.x` pages are written.
