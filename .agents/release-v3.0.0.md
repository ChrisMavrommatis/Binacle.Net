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
2. **Docker image build + full green sweep** *(external — see actions)*. Never run since the `Binacle.Geometry`
   extraction, and **no CI runs tests**, so one local green sweep — all C#/TS suites plus the image build — is
   the only gate there is. (The API compiles clean — `dotnet build` 0 warnings, 2026-07-19 — but the image
   build and full suite still need one run.)
3. **Fitting results are unchanged — VERIFIED 2026-07-19.** ~5,400 fit requests (random bins/items, all three
   algorithms, weighted to the near-full boundary where heuristics diverge) ran against old (v2.1.1) and new
   side by side: identical answers every time, zero disagreements. No behaviour change in the frozen v3
   contract, and no release-notes caveat needed.
4. **Old ViPaq tokens fail loudly — VERIFIED 2026-07-19.** 250 real old tokens plus targeted adversarial cases
   (the ones whose count byte forms a *valid* new header, so they reach body parsing) all threw
   `ViPaqFormatException`; zero silent misparses — the body-length check is the backstop. Worth locking in as
   regression vectors: `plans/vipaq/old-format-rejection-tests.md`.

## Cut the release

1. All four blockers green.
2. Tag `v3.0.0`; the `release-docker-image.yml` workflow publishes the image.
3. Paste `release-notes-v3.0.0.md` into the GitHub release body, and **announce the ViPaq token break** there.
4. Once it is out, work `post-release-v3.0.0.md`.

## Not in this release — tracked elsewhere, do not pull these in

- **Docs site `v3.0.x` pages + v4 spec** — `plans/docs-versioning.md` (surfaced in `post-release-v3.0.0.md`).
- **Migrate the UI clients off v3** — `post-release-v3.0.0.md`.
- **CI / Sonar / coverage** — `plans/ci-enablement.md`.
- **Refresh the curated benchmark ledger** — `plans/lib/benchmark-ledger.md`.
- **Code TODOs** — `plans/todos.md`.
- **`Parallel*` processor cleanup** — the open question in `$lib/decisions#O1`.
- **pack/first-bin endpoint** — `ideas/api/pack-first-bin-endpoint.md`.
- **TestsKernel fixture growth** — `plans/shared/testskernel-data-extraction.md`.
