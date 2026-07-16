# Release Plan — the v4 version

**Status:** Not started — this is the master checklist for cutting the next release.
**Created:** 2026-07-16

The release is built around the v4 endpoints, but v4 alone is not enough to ship. This file lists
everything that has to happen, in order, and points at the plan that owns each piece.

Related plans and trackers:
- `.agents/plans/api/v4-endpoints.md` — the endpoint buildout (owns the detail)
- `.agents/plans/docs-versioning.md` — the docs-site version history (owns the detail)
- `.agents/release-notes.md` — the changelog to publish
- `.agents/pending-actions.md` — the manual steps outside the repo

---

## Open decision: do the compare endpoints ship in this release?

**Read this before planning the endpoint work — it sets the size of the release.**

v4 splits what v3 did into three request shapes. Two are built:

| Shape | Request base | v4 endpoints | Built? |
|---|---|---|---|
| one bin → one answer | `CustomBinRequestBase` / `PresetBinRequestBase` | `fit/bin`, `pack/bin`, + `{preset}/{bin}` variants | Yes |
| **many bins → one answer** | `CustomBinsRequestBase` | `pack/smallest-bin` | **Yes** |
| **many bins → all answers** | `CustomBinsRequestBase` / `PresetBinsRequestBase` | `fit/compare`, `pack/compare`, + `{preset}` variants | **No** |

v3 only ever had the last shape: all four of its fit/pack endpoints call `MultipleBinsAsync`, taking many
bins and returning a result per bin.

So the gap is **compare specifically**, not "many bins" in general — `pack/smallest-bin` already takes a bin
list. A v3 caller asking "which bin should I use" is already served by `smallest-bin`. Only a caller that
wants *every* bin's result needs compare.

| v3 endpoint | v4 successor | Exists? |
|---|---|---|
| `POST /api/v3/fit/by-custom` | `POST /api/v4/fit/compare` | No |
| `POST /api/v3/fit/by-preset/{preset}` | `POST /api/v4/fit/compare/{preset}` | No |
| `POST /api/v3/pack/by-custom` | `POST /api/v4/pack/compare` | No |
| `POST /api/v3/pack/by-preset/{preset}` | `POST /api/v4/pack/compare/{preset}` | No |
| `GET /api/v3/presets` | `GET /api/v4/presets` | Yes |

**Recommendation: ship v4 without compare, marked experimental. Compare lands in a later 3.x minor.**

- **Precedent.** v1.2.0 shipped API v3 with exactly **one** endpoint, marked experimental, and grew it over
  the line. v4 at six endpoints is further along than v3 ever was at introduction.
- **Nothing is stranded.** v3 is frozen and complete (`$memory/v3-frozen`), so it keeps serving every caller,
  including the API v2 users this release forces to move.
- **The release is already heavy** — API v2 dropped, ViPaq tokens rebuilt, packing-log config flattened.
- **Migrating the UI clients is post-release** (maintainer, 2026-07-16), so nothing waits on compare.
- **It costs no docs churn later.** Adding endpoints to v4 does not change the API version *set* the image
  serves (still v3 + v4), so it needs no new docs folder — just an edit to the current `v3.0.x` folder.
  See `.agents/plans/docs-versioning.md`.

**Not yet decided by the maintainer.** If the answer is instead "v4 must fully cover v3 at release", then the
four compare endpoints become the largest work item in the release and move into Blockers below.

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
- [ ] **Write the v4 release-notes entries.** `release-notes.md` currently covers the ViPaq break and the
      packing-log config change but says **nothing about v4** — the headline feature of the release is
      missing from its own changelog. Add an overview line, the endpoint list, and the v2-removal note.
      v2 removal is a breaking change and needs its own migration entry.

## 3. Follow-ups — fine to ship without, but decide consciously

- [ ] **Build the four compare endpoints** — assuming the open decision above lands on "ship without them".
      Follow `$api/v4/add-endpoint`. `CustomBinsRequestBase` and `PresetBinsRequestBase` already exist; no
      concrete request or endpoint classes do. Detail: `.agents/plans/api/v4-endpoints.md`.
- [ ] **Move the clients off v3** (see below). Post-release per the maintainer (2026-07-16). The web demo may
      not even need compare — check whether it uses every bin's result or just the winner, since
      `pack/smallest-bin` already covers the winner case.
- [ ] **The rest of the planned v4 endpoints** — eight more beyond the compare four (`smallest/{preset}`,
      `best-fit`, `first-fit`, `fit/smallest`, `presets/{preset}`). Pick a cut line.
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
`pack/compare`. Both call a compare-shaped endpoint, but if a client only shows the winning bin then
`pack/smallest-bin` already covers it and exists today.

---

## Suggested order

1. ~~Rework the docs site to version-only.~~ **Done 2026-07-16.**
2. Settle the compare-endpoint decision at the top — it sets the size of the release. If it lands on "ship
   without them", there is no API code work left in this release at all.
3. Bump the vulnerable packages (before any spec generation).
4. Fix the enum transformer TODO.
5. Generate `v4.json` on the `Normal` profile.
6. Write the `v3.0.x` docs — every page, plus the v4 pages marked experimental.
7. Write the v4 release-notes entries (both `.agents/release-notes.md` and `v3.0.x/release-notes.md`, which
   still holds the old v2.0.0 notes).
8. Set the `API_PROJECT_PATH` Actions variable.
9. Run the docker image build; full green sweep.
10. Cut the release; announce the ViPaq token break in the body.

Trim each item as it lands. Delete this file once the release is out.
