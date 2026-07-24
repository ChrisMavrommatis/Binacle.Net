# Release — Binacle.Net v3.0.0

**Status:** Not started — the checklist for cutting v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq).
**Created:** 2026-07-16

This is the master release plan and the **one exception** to the reference rules: it may point at any file to
coordinate the release, but **nothing points back at it**. Delete it once v3.0.0 is out.

Companions for this version:
- `release-actions-v3.0.0.md` — the manual/external steps to run.
- `release-notes-v3.0.0.md` — the GitHub release body, ready to paste.
- `post-release-v3.0.0.md` — what to do right after the release is out.

---

## Blockers — v3.0.0 cannot ship until these are green

The two correctness questions are **now verified** (2026-07-19, differential-tested against the real
`binacle/binacle-net:2.1.1` image). Only the two infrastructure steps remain.

1. **`API_PROJECT_PATH` Actions variable** *(external — see actions)*. The `src/` → `api/src/` move breaks the
   `release-docker-image.yml` publish step until this is set. The release cannot publish otherwise.
2. **Docker image build** *(external — see actions)*. The test suites are covered — `run-tests.yml` (landed
   2026-07-20) builds the solution and runs every C# suite plus the TS suites on each PR, including
   ServiceModule against both SQLite and Postgres. It does **not** build the docker image, and it only triggers
   on `pull_request` / `workflow_dispatch` — never on a tag. So the remaining gate is one image build, which has
   not run since the `Binacle.Geometry` extraction, plus a green CI run on the PR that lands the release.
3. **Fitting results are unchanged — VERIFIED 2026-07-19.** Differential-tested against the v2.1.1 image, zero
   disagreements. No release-notes caveat needed. Evidence folded into `$lib/findings#F3`.
4. **Old ViPaq tokens fail loudly — VERIFIED 2026-07-19, locked 2026-07-20.** Zero silent misparses; four
   regression vectors committed in `vipaq/test-vectors/serialization/decode-invalid.json` (C# + TS green).
   Format detail in `vipaq/PROTOCOL.md`. Still **announce the token break** in the release body (below).

## v4 ships experimental

`ApiV4Document.IsExperimental` is **true** (set 2026-07-25), so the published OpenAPI document carries the
warning that v4 may change at any time. Everything else in this release set already said "experimental"; the
code was the one place that said otherwise, and it is what users actually see.

Keep it that way for 3.0.0. The flip to stable is 3.1.0 work — `plans/api/v4-stable-in-3.1.0.md` holds the
criteria. Check the flag is still `true` before tagging: shipping v4 as stable would lock contracts that are
meant to keep moving.

## Breaking changes to announce

Both are already written into `release-notes-v3.0.0.md`; this is the checklist that they are covered.

1. **ViPaq tokens** — old tokens no longer decode, no fallback reader. Verified to fail loudly (2026-07-20).
2. **Health check `RestrictedIPs`** — three changes, one of which **narrows existing allow-lists**:
   - CIDR now means a prefix length. The value after `/` was read as an address mask, so `192.168.1.0/24`
     matched nearly the whole IPv4 range. Anyone relying on a CIDR entry must re-check who is inside it or risk
     locking themselves out.
   - IPv4 callers arriving in IPv4-mapped IPv6 form are now unmapped before matching, so the list works in a
     container at all. It previously could not match any IPv4 entry.
   - The `start-end` range form is removed and now fails startup validation.

   `IPAddressRange` was deleted; matching is `System.Net.IPNetwork` via `RestrictedIPNetwork`.

Also new in this release, not breaking: **forwarded headers** (`Config_Files/ForwardedHeaders.json`, disabled by
default) and the **`/_debug` endpoint** (`DEBUG_ENDPOINT`, disabled by default). `ASPNETCORE_FORWARDEDHEADERS_ENABLED`
is deliberately ignored — see `$api/configuration`.

## Cut the release — beta image first

The release goes out in stages. A **beta image ships first** and gets deployed; the `v3.0.x` docs are written
while it is running, and released after. Docs are therefore **not a blocker** for the beta.

1. All four blockers green.
2. **Publish a beta image** and deploy it. This is the first time the image runs anywhere, so it is also the
   real test of the new forwarded-headers and health-check behaviour.
3. **Write the `v3.0.x` docs** while the beta is deployed — owned by `plans/docs-versioning.md`, including the
   two pages that are new in this version (forwarded headers, health checks).
4. **Release the docs**, then tag `v3.0.0`; the `release-docker-image.yml` workflow publishes the final image.
5. Paste `release-notes-v3.0.0.md` into the GitHub release body. Announce **both** breaking changes there — the
   **ViPaq token break** and the **health check `RestrictedIPs` change** (see above).
6. Once it is out, work `post-release-v3.0.0.md`.

## Not in this release — tracked elsewhere, do not pull these in

- **Docs site `v3.0.x` pages + v4 spec** — `plans/docs-versioning.md` (surfaced in `post-release-v3.0.0.md`).
- **Migrate the UI clients off v3** — `post-release-v3.0.0.md`.
- **CI / Sonar / coverage** — `plans/ci-enablement.md`.
- **Refresh the curated benchmark ledger** — `plans/lib/benchmark-ledger.md`.
- **Code TODOs** — `plans/todos.md`.
- **`Parallel*` processor cleanup** — the open question in `$lib/decisions#O1`.
- **pack/first-bin endpoint** — `ideas/api/pack-first-bin-endpoint.md`.
- **TestsKernel fixture growth** — `plans/shared/testskernel-data-extraction.md`.
