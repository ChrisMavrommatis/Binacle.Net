# Release Plan — the v4 version

**Status:** Not started — this is the master checklist for cutting the next release.
**Created:** 2026-07-16

The release is built around the v4 endpoints, but v4 alone is not enough to ship. This file lists
everything that has to happen, in order, and points at the plan that owns each piece.

Related plans and trackers:
- `.agents/plans/docs-versioning.md` — the docs-site version history (owns the detail)
- `.agents/release-notes.md` — the changelog to publish
- `.agents/pending-actions.md` — the manual steps outside the repo

---

## Settled: compare ships, and v4 now covers v3

**Decided 2026-07-16 (maintainer): build the v4 endpoints.** The four compare endpoints are built, so the
question this section used to pose — ship v4 without compare? — is moot. v4 now has a successor for every v3
endpoint:

| v3 endpoint | v4 successor | Exists? |
|---|---|---|
| `POST /api/v3/fit/by-custom` | `POST /api/v4/fit/compare-bins` | Yes |
| `POST /api/v3/fit/by-preset/{preset}` | `POST /api/v4/fit/compare-bins/{preset}` | Yes |
| `POST /api/v3/pack/by-custom` | `POST /api/v4/pack/compare-bins` | Yes |
| `POST /api/v3/pack/by-preset/{preset}` | `POST /api/v4/pack/compare-bins/{preset}` | Yes |
| `GET /api/v3/presets` | `GET /api/v4/presets` | Yes |

v4 ships with **16 endpoints**, still marked experimental. Ten landed on 2026-07-16: the compare four, the
preset variants of smallest, both best-fit routes, fit/smallest, and `GET /api/v4/presets/{preset}`.
`pack/first-bin` (custom + preset, formerly `first-fit`) was cut from v4 — it needs a design call and may target v3.1 instead.
It is parked in `.agents/ideas/api/pack-first-bin-endpoint.md`. **The v4 endpoint work is complete.**

Still marked experimental at release, carrying the same banner v3 used while it was in development. Feature
coverage is not the same as a frozen contract, and the UI clients have not moved across yet.

---

## 1. Blockers — the release cannot go out without these

- [ ] **Fix the `API_PROJECT_PATH` Actions variable.** Repo Settings → Secrets and variables → Actions →
      Variables: `src/Binacle.Net/Binacle.Net.csproj` → `api/src/Binacle.Net/Binacle.Net.csproj`.
      The `src/` → `api/src/` move breaks the `release-docker-image.yml` publish step until this changes.
      **The release literally cannot publish until this is done.** Tracked in `pending-actions.md`.
- [ ] **Run a docker image build once.** Never run since the `Binacle.Geometry` extraction — every C# and
      TS suite is green but the image build was skipped by choice. Tracked in `pending-actions.md`.
- [x] **Docs site reworked to version-only folders.** Done 2026-07-16. `v1.3.x`, `v2.0.x`, `v2.1.x` now
      preserve the released versions; `latest` is gone as a folder and survives only as a redirect. Site
      builds clean. Detail: `.agents/plans/docs-versioning.md`.
- [ ] **Write the `v3.0.x` docs.** `v3.0.x` is currently a **stub** — `index.md` only, with a notice pointing
      at `v2.1.x`. Every page has to be written: `api/` (v3 + v4), `swagger/`, `configuration/`, `samples/`,
      `quick-start.md`, `release-notes.md`. Nothing was copied from `v2.1.x` — the v3.0 docs are authored
      fresh, by choice. **API v2 must not reappear**; it lives on in `v2.1.x` / `v2.0.x`.
      `vlink` raises on a missing target, so add a link only once its page exists.
- [ ] **Publish v4 in `v3.0.x`, marked experimental.** Needs `swagger/v4.json` + a `swagger/v4.md` stub
      (`layout: versions/swagger`, `swagger: 'v4'`), the `api/v4.md` prose page, and nav wiring.
      Generate the spec by running the API and fetching `/openapi/v4.json` with `SWAGGER_UI` or `SCALAR_UI`
      on. **Run on the `Normal` profile (ServiceModule OFF)** so the spec matches the committed convention —
      the committed `v3.json` has no `/api/auth/token` path, and a ServiceModule-on run would add it.
      Do this **after** the endpoint work lands, so the spec is generated once and is complete.
      Carry the experimental banner v3 used while it was in development ("This API version is experimental and
      can change at any time.") — the same shape as v1.2.0 shipping a one-endpoint v3.
      The banner text is in `v1.3.x/api/v3.md`.

## 2. Should do — the release is worse without these

- [ ] **Patch the two high-severity package vulnerabilities.** The build is green (0 errors) but reports
      `NU1903` on every project:
      - `Microsoft.OpenApi` 2.0.0 — https://github.com/advisories/GHSA-v5pm-xwqc-g5wc
      - `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 — https://github.com/advisories/GHSA-2m69-gcr7-jv3q
      Shipping a docker image with known high-severity advisories is a bad look for a release. Check
      whether the `Microsoft.OpenApi` bump moves the generated swagger — if it does, bump **before**
      generating the v4 spec so the spec is generated once.
- [ ] **Announce the ViPaq token break.** The rebuilt format rejects every token an earlier version made,
      and there is no reader for the old wire. Nothing in the repo says the old format existed, so the
      GitHub release body is the only place users will learn this. Tracked in `pending-actions.md`;
      the banner and migration text are already drafted in `release-notes.md`.
- [ ] **Fix the OpenAPI enum transformer TODO before generating the v4 spec.**
      `api/src/Binacle.Net.Kernel/OpenApi/EnumStringsSchemaTransformer.cs:35` — a required enum property
      still gets `JsonStringNullableEnumConverter` and keeps its `?`. This bakes into the published
      `v4.json`, so it is worth fixing while the spec is being cut rather than after.
- [x] **Write the v4 release-notes entries.** Done 2026-07-16 — `🔎 Overview` and `⚙️ Core Changes` cover v4,
      the three request shapes, and the experimental status; v2 removal has its own migration entry.

## 3. Follow-ups — fine to ship without, but decide consciously

- [x] **Build the four compare endpoints.** Done 2026-07-16, along with six more. See `$api/v4`.
- [ ] **Move the clients off v3** (see below). Post-release per the maintainer (2026-07-16). The web demo may
      not even need compare — check whether it uses every bin's result or just the winner, since
      `pack/smallest-bin` already covers the winner case.
- [ ] **`pack/first-bin` is out of v4** — cut rather than deferred, and may target v3.1. The open calls (what
      "first success" means, and the name colliding with the FFD algorithm) are captured in
      `.agents/ideas/api/pack-first-bin-endpoint.md`. Nothing to do for this release.
- [ ] **Remaining code TODOs** — none block the release:
      - `ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34` — review rate-limit policy JSON config
      - `ServiceModule/v0/Endpoints/AccountBindingResult.cs:57` — "no request body" returns raw `ProblemDetails`
      - three integration-test harnesses only run core modules, not all modules
- [ ] **`.agents/plans/shared/testskernel-data-extraction.md`** — growing the fixture cases. Test-quality
      work, unrelated to shipping.

---

## Clients still on v3

Both shipped UI clients call `POST /api/v3/pack/by-custom`, which is a compare-shaped call:

- `packages/binacle-net-ui/src/core/packingDemo.ts:127` — the web site packing demo
- `api/src/Binacle.Net.UIModule/Components/Pages/PackingDemo.razor.cs:135` — the Blazor UI module

They keep working, because v3 stays and is frozen (`$memory/v3-frozen`). **Migrating them is post-release**
(maintainer, 2026-07-16), so nothing in this release waits on them.

When they are migrated, check what each one actually uses the response for before assuming it needs
`pack/compare-bins`. Both call a compare-shaped endpoint, but if a client only shows the winning bin then
`pack/smallest-bin` already covers it and exists today.

---

## Suggested order

1. ~~Rework the docs site to version-only.~~ **Done 2026-07-16.**
2. ~~Settle the compare-endpoint decision.~~ **Done 2026-07-16** — compare ships; the v4 endpoint work is
   complete.
3. Bump the vulnerable packages (before any spec generation).
4. Fix the enum transformer TODO.
5. Generate `v4.json` on the `Normal` profile. **All 16 endpoints are in place**, so the spec can be cut once.
6. Write the `v3.0.x` docs — every page, plus the v4 pages marked experimental.
7. Write the v4 release-notes entries (both `.agents/release-notes.md` and `v3.0.x/release-notes.md`, which
   still holds the old v2.0.0 notes).
8. Set the `API_PROJECT_PATH` Actions variable.
9. Run the docker image build; full green sweep.
10. Cut the release; announce the ViPaq token break in the body.

Trim each item as it lands. Delete this file once the release is out.
