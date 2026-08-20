---
description: Release - Binacle.Net v3.0.0
---

# Release - Binacle.Net v3.0.0

**Status:** In progress. Betas 1 to 4 published; `v3.0.0-beta.4` is the tag deployed on the test server. The
pipeline is rebuilt and proven end to end, the architecture branch is merged, the suite is green and the
OpenAPI documents are proven unmoved. **What is left is the last commit and the tag**, plus the immutability
rule that holds nothing up, and the docs deploy that follows.

**Betas 1 and 2 are deleted from Docker Hub.** Only `3.0.0-beta.3` and `3.0.0-beta.4` still resolve. Anything
that quotes a real tag or a real response has to name one of those two, or `3.0.0` once it exists.

**`3.0.0-beta.4` is the reference tag.** `just image verify 3.0.0-beta.4` passed all four checks on
2026-08-20 - tags, signature, attestations and metadata - under the `binacle-labs` certificate identity.
`3.0.0-beta.3` passed the same run on 2026-08-17 and is the fallback. **Both are safe to name in an example;
nothing else is.**

**Created:** 2026-07-16. **Rewritten for the GHCR pipeline:** 2026-08-11. **Scope reset:** 2026-08-14.
**Pruned to pending work only:** 2026-08-20.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to
the reference rules: it may point at any file to coordinate the release, and **nothing points back at it**.
Delete it once v3.0.0 is out and verified.

Companion: `post-release-v3.0.0.md` - the checks to run once the release is out.

---

## How to work this file

**This file holds pending work only.** A finished item is a tick and a date, with the detail gone - what
outlives the release is in the docs and the decisions ledgers, not here.

**Where a plan does the work, this file names the slice it took and nothing more.** The plan file itself never
says what ships when. When a slice lands, cut that part out of the plan; when nothing is left in the plan,
delete it. Whatever the release did not take stays on the board.

**Two lists.** The gate is what must be green before the tag. "Runs alongside" does **not** hold the tag - if
one is not ready, the release goes without it.

**How the pipeline works**, because it shapes every item below. Every build is staged on GHCR, smoked there,
and the smoked digest is **copied** to Docker Hub - so nothing unsmoked reaches the registry users pull from.
A prerelease gets its immutable tag only, never `3.0` or `latest`. The release body is extracted from
`CHANGELOG.md` by the workflow, and the last job writes the Docker Hub page.

---

## The gate - all of this before the tag

| # | Item | State |
|---|---|---|
| 1 | Rate limiter tests | done - 2026-08-14 |
| 2 | Rate limiting owned by the ServiceModule | done - 2026-08-14 |
| 3 | The Azure Storage run | done - 2026-08-14, and now on the PR gate |
| 4 | Beta 3, and its three live checks | done - 2026-08-19 |
| 5 | Admin read endpoints | done - 2026-08-19 |
| 6 | Beta 4, deployed to the test server | done - 2026-08-19 |
| 7 | The Docker Hub page's quick start | open |
| 8 | The last commit: pins, prose and the changelog rename | open |
| 9 | Tag `v3.0.0` | open |

### 7. The Docker Hub page's quick start

**The slice this release takes from [ci-cd/dockerhub-overview](plans/ci-cd/dockerhub-overview.md): section 1,
the `curl` example.** Everything else in that plan stays on the board.

**Why it is on the gate rather than alongside - state chosen by an agent, strike it if wrong.** The `page` job
runs at the end of the release and publishes the file as it stands, so there is no "goes without it". The tag
is the moment the page becomes public.

- [ ] **Re-run the quick start against `3.0.0-beta.4` and paste the real response back.** The response in
      `.github/dockerhub-overview.md` came from a run against a tag that is now deleted. A broken first command
      is the whole first impression. The pinned tag in the page is a placeholder, so only the response body
      changes.
- [ ] **Read the rendered page locally:** `just image dockerhub-overview 3.0.0`. That is exactly what the
      release publishes.

**Do not run `Shared / Docker Hub Page` by hand before the tag - confirmed 2026-08-20, it has not been run.**
Both ways of running it publish something wrong. An empty version input takes the latest **non-prerelease**
release, which is still `v2.1.1`, so the page would describe 2.1. Typing `3.0.0` renders correctly but tells
every reader to pull `binacle/binacle-net:3.0`, which does not resolve yet - the recipe substitutes
placeholders, it does not check the tag exists. **The stale 2.x page is the lesser wrong until the tag.** The
local render above is the pre-tag check.

### 8. The last commit before the tag - all in one

- [ ] **Rename `## [Unreleased]` to `## 3.0.0`** in `CHANGELOG.md`.
- [ ] **Move six pins from `3.0.0-beta.4` to `3.0`:**

  | File | What to change |
  |---|---|
  | `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` | the `image:` line |
  | `samples/kubernetes/minimal/binacle-deployment.yaml` | the `image:` line |

- [ ] **Drop the expiring comment in the same six files.** Each `image:` line carries two extra lines - *"Pinned
      to the beta patch for now because `binacle/binacle-net:3.0` does not exist on Docker Hub yet - move to
      the 3.0 minor tag once v3.0.0 is published."* Delete those two, leaving only *"Pinned on purpose - a
      copied sample must not jump to a new major on the next pull."* **That reason expires the moment v3.0.0
      publishes.**
- [ ] **Rewrite the same reason in prose in `samples/README.md` and `samples/docker/README.md`.** Both name
      `3.0.0-beta.4` and explain why; both become `3.0` with the explanation cut.
- [ ] **Re-confirm `ApiV4Document.IsExperimental` is still `true`.** Shipping v4 as stable would lock contracts
      meant to keep moving. The flip is 3.1.0 work.
- [ ] **Preview the body:** `just changelog extract 3.0.0` after the rename. That is exactly what publishes.

**One decision, and it is open.** `README.md` was moved to `binacle/binacle-net:3.0` early, on 2026-08-17, when
the beta names came off the public surfaces. That tag does not exist on Docker Hub yet, so **the most read file
in the repo currently names an image nobody can pull**. Either revert it until the tag or accept it. The same
early-move trade was taken deliberately for `tooling/README.md` and `tooling/smoke.just`, which read `3.0.0`.

**The rule that drives the pin timing: a pin on `main` must name an image that exists on Docker Hub.** They
moved early once before, on 2026-08-07, and sat on `main` naming an image that did not exist. **Do not leave
the `3.0` bump on `main` long before tagging.**

### 9. Tag

- [ ] **Tag `v3.0.0`.** The pipeline does the rest: the changelog gate, the suite, the GHCR build, the smoke,
      the Docker Hub copy under all three tags, the signature, the release created from the `3.0.0` section,
      and the Docker Hub page. **Nothing here is manual any more.** Watch the run, then work
      `post-release-v3.0.0.md`.

---

## Runs alongside - does not hold the tag

**The Docker Hub logo and categories are not here.** They are the rest of the `ci-cd/dockerhub-overview` plan
and they sit on the board, not in this release - the release only takes that plan's `curl` example. Recorded so
they are not mistaken for release work that got dropped.

### Docker Hub tag immutability - the rule only

**The slice this release takes from
[ci-cd/dockerhub-tag-immutability](plans/ci-cd/dockerhub-tag-immutability.md): correct the rule, leave the
switch off.** The plan holds the trap, the regexp and why prereleases are excluded; the switch and the
scratch-repo test stay in it and are on the board.

- [ ] **Correct the rule to released versions only, and read the value back from the API.** The stored value
      on 2026-08-13 was `".*"`, which would freeze `latest` and `3.0` - the two tags the release moves. **The
      switch is off, so nothing breaks either way; correcting it now is what makes the post-release decision a
      flip rather than a project.**

**Unclear, and it needs an answer:** `post-release-v3.0.0.md` was written as though this had already been done.
Nothing here records a run. Read the API and tick it or leave it.

---

## The release notes

**They live in `CHANGELOG.md`, in the `## [Unreleased]` section**, and the workflow extracts that section as
the release body. The content was checked byte-for-byte against the published beta 2 body on 2026-08-13 - all
four breaking changes, the six migration steps, the signing and SBOM bullet, the image-size drop, and
`RetentionDays`.

Three mechanics:

- **`[Unreleased]` is renamed to `## 3.0.0` as the last change before the tag.** Every beta publishes
  `[Unreleased]`. If you forget, the `notes` job fails in under a minute and nothing is built - which is why
  that gate runs first.
- **A section's own headings are `###`**, nested under the `##` version heading. `just changelog extract` shifts
  them back to `##` on the way out. **Do not "fix" the file to use `##` throughout** - that breaks the nesting
  under `# Changelog` and the extractor's terminator both.
- **The compare link at the bottom already reads `v2.1.1...v3.0.0`** - correct from the tag onward, and a 404
  on every beta release page until then. Left as it is deliberately.

**This section is also what the docs site copies.** The `## 3.0.0` body is hand-carried into the v3.0.x
release-notes page, and a v3.0.1 appends rather than replaces. The three additions that page is missing are in
the docs deploy checklist below.

**The restructure gets no changelog line - decided 2026-08-14.** No user-observable behaviour changes, nothing
is published to NuGet, and no contract moves - the OpenAPI diff proves the last one. The four breaking changes
stay four.

---

## The docs deploy - after the tag

**The config half is done:** `main` carries `current: v3.0.x`, `- id: v3.0.x` back at the top of `list`, and
`sites/docs/collections/_sitemaps/version-3-0-x.xml` restored - all verified 2026-08-14. What is left is the deploy
plus five edits that must go out with it.

**`sites/docs/` is off limits to a coding session.** This is the docs session's work, written here for it.

- [ ] **Re-cut the worked example in `v3.0.x/verifying-a-release.md` against the real `3.0.0`.** It is the
      **last place any public surface still names a beta image**, and it cannot be fixed before the tag because
      it quotes real output. Run `just image verify 3.0.0` and replace three things with what it prints: the
      Docker Hub digest, the package count, and the provenance run URL.

      **It is now broken, not merely stale.** The example verifies `3.0.0-beta.2`, which is **deleted from
      Docker Hub** and was signed under the old owner anyway - so the block asks the reader to run a command
      that cannot succeed. **This page must not deploy before this row is done.**

      **The rule that decided this, worth keeping for future releases: name a version where the version is the
      fact, never as a floor or an example.** A floor or a sample tag goes stale on its own; a record of what
      was signed does not.

- [ ] **Put the real date and release link in `v3.0.x/release-notes.md`.** The `## v3.0.0` section carries
      interim wording because the tag did not exist when the pages were written. Swap the italic line for
      *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*, matching every other version
      folder.
- [ ] **Carry three additions from `CHANGELOG.md` into `v3.0.x/release-notes.md`.** Same notes in two places,
      and the release body gained content on 2026-08-10 the page does not have.

      **Decided 2026-08-14: this page stays hand-copied.** It is not generated from `CHANGELOG.md`. The drift is
      the accepted cost, so **this checklist is the control** - every future release's docs handover has to list
      what the changelog gained since the page was last written. Run `just changelog extract Unreleased` to see
      the current text. All three go in the `## v3.0.0` section, in the page's plain-ASCII style:
  - **Overview**, one bullet after the health check line: the image creates `/app/data` and gives it to the app
    user, so a volume mounted there is writable.
  - **Core Changes**, replacing *"The `Dockerfile` and existing environment variables are unchanged"* - which is
    false, the Dockerfile changed three times this release. Spell out the `/app/data` fix (docker created the
    mount point as root, the app does not run as root, so packing logs and the SQLite database could not be
    written to a fresh named volume); `libgssapi-krb5-2` now shipping, so Npgsql stops printing "Cannot load
    library libgssapi_krb5.so.2" at every start, which was harmless but read as fatal; OCI labels on the image;
    and only then "existing environment variables are unchanged".
  - **A `🔌 Service Module` section**, between Diagnostics and UI Module: the auth token rate limit partitions
    on the connection's remote address instead of a caller-supplied header, so varying the header no longer
    resets your own login throttle.
- [ ] **Replace the two swagger documents under `sites/docs/collections/_versions/v3.0.x/swagger/`.** Copy
      `artifacts/openapi/Binacle.Net_v3.json` -> `swagger/v3.json` and `artifacts/openapi/Binacle.Net_v4.json`
      -> `swagger/v4.json`; the generator's file names differ from what the site expects, so the rename is part
      of the handover.

      **The diff is already measured - 2026-08-14, do not re-derive it.** The **only** differences are: both
      documents gain a `servers` entry with the single relative `/`, and **the `429` responses come out** - 4
      mentions in v3 and 14 in v4 go to zero. Nothing else moves. No schema name changed despite the namespace
      restructure. For v3 this **restores** the shape v2.1.x shipped, so nothing about the frozen v3 contract
      moves. It is still a visible change to the published spec, so mention it wherever the update is described.
- [ ] **Write the client-generation page.** **Pulled in on 2026-08-14 at the maintainer's call.** It was the
      last item in the `api/openapi-spec-followups` idea, which held nothing else - that file was deleted on
      2026-08-20 and this row is now the only place the work is written down.

      A short page with copy-paste commands that generate a client from the published per-version spec -
      `hey-api` for TypeScript, `kiota` for C#, and whatever else is worth naming. Today the spec is published
      and nothing tells anyone they can do this.

      **It applies to every version, not just v3.0.x** - the maintainer's call. Each version folder publishes
      its own `swagger/v3.json` and `swagger/v4.json`, so the commands work against v1.3.x, v2.0.x, v2.1.x and
      v3.0.x alike. **Write it so the version is a placeholder the reader substitutes**, rather than four
      near-identical pages that drift apart - which means deciding once where a cross-version page lives on
      that site.

      **Do not publish SDKs to close this.** The deliverable is a spec plus a generation guide, not shipped
      packages.
- [ ] **Deploy.** It is `workflow_dispatch` only.

**This is the single most losable item in the release** - nothing fails if the deploy is skipped, the site just
quietly keeps serving v2.1.x as current. **It has to run after the tag**, because the notes need the date and
the `releases/tag/v3.0.0` link, and `main` already says v3.0.x is current, so deploying earlier presents an
unreleased version as current. It has to land before anything is announced. **Tag, then deploy the docs, then
announce.**

**One deliberate 404, do not "fix" it.** The `v3.0.x` ViPaq page links the wire spec at
`github.com/binacle-labs/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md`, which 404s until the tag is pushed. A
versioned page should pin the spec it describes; do not repoint it at `main`.

---

## The sequence

1. **The Docker Hub page's quick start**, and the immutability rule whenever it suits.
2. **The last commit:** changelog rename, six pins, six comment blocks, two READMEs, and the `IsExperimental`
   re-confirm.
3. **Tag `v3.0.0`.** The pipeline does the rest, page included.
4. **Deploy the docs**, with the five edits above.
5. **Announce.**
6. **Work `post-release-v3.0.0.md`**, then delete both files.

---

## Not in this release

Everything else has a plan or an idea of its own and is on the board, grouped by area with its blockers named.
**Do not pull any of it in.**

**Held back on 2026-08-14, with reasons that still hold:**

| Item | The blocker |
|---|---|
| **The architecture checks** | The heavy tools - ArchUnitNET, dependency-cruiser, lychee - need a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose. dependency-cruiser has no root `tsconfig.json`; there are four, and `sites/web/` has none. **The three lighter checks joined them on 2026-08-17**, when a better design turned a ready item into a fresh one. |
| **CI gates 2 and 3** | Gate 2 runs the all-modules integration tests, which are not being written here. Gate 3 is Sonar and coverage, and its own plan says do not make coverage blocking yet. Gate 1 ships; these two have nothing to gate. |
| **Raising test coverage** | **Decided 2026-08-14: do not test the Blazor UIModule.** The Alpine port deletes most of what would be tested, so writing bUnit tests now means writing them twice, in two languages. What shipped here is the modest bump the rate limiter tests brought, and nothing more. |
| **The workflow restructure's last item** | The branch protection edit. It landed 2026-08-18 and left the release on 2026-08-19 - it gates nothing here. |
