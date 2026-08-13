# Release - Binacle.Net v3.0.0

**Status:** In progress. **Gate A green. Beta 1 and beta 2 published, verified and deployed. Docs written. The
pipeline is rebuilt and proven end to end.** What is left is the architecture branch, the rate limiter tests, a
beta 3 to cover both, one Azure run, the pin bump, the tag, and the docs deploy - plus three registry and
verification items that do not gate any of it.

**Created:** 2026-07-16. **Rewritten for the GHCR pipeline:** 2026-08-11. **Beta 2 re-cut and verified:**
2026-08-13. **Pruned to what remains:** 2026-08-14. **Architecture branch pulled in, beta 3 added:** 2026-08-14.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to the
reference rules: it may point at any file to coordinate the release, and **nothing points back at it**. Delete it
once v3.0.0 is out.

Companion: `post-release-v3.0.0.md` - what to do once the release is out.

## What is left

| # | Item | State |
|---|---|---|
| B10 | Merge `features/arch_tests` - the architecture restructure and the rules layer | open, do this first |
| B11 | Cut, verify and deploy beta 3 - it carries B2X's two open checks | open, after B10 |
| B15 | Rate limiter integration tests, and a coverage bump | open, after B10 |
| B6 | One Azure Storage run, **after the merge** | open |
| B12 | Image verification - the recipe and the surfaces | open, not a gate |
| B13 | The Docker Hub page - credential, file, workflow | open, not a gate |
| B14 | Docker Hub tag immutability - the rule, not the switch | open, not a gate |
| B5 | Bump nine files to `3.0`, in the last commit before the tag | open |
| - | Commit the `429` OpenAPI guard, re-run the suites, rename the changelog section, tag `v3.0.0` | open |
| B8 | Deploy the docs, with three edits that go out with it | open, after the tag |

Everything else in Gate A and Gate B is done. The done items are collapsed to one line each below; the sections
that remain carry their full instructions.

**Six items are new on 2026-08-14** and they reorder everything under them.

- **B10 and B11** - the architecture branch was standing work until the maintainer pulled it into this release.
  It changes shipping code, so beta 2's image evidence stops covering the tree that ships, and the two checks
  left open on the beta 2 host move to beta 3 rather than being done twice.
- **B15** - a carve-out from the integration-test plan. Nothing anywhere asserts a 429 ever happens, and two of
  this release's claims rest on that.
- **B12, B13 and B14** came off the board, spent an afternoon in the post-release list, and landed here. Each
  has tooling or a decision that has to be settled before the tag; only the confirmation is post-release. **They
  do not gate the tag** - if one is not ready, the release goes without it.

**What still gates the tag:** B10, B15, B6, B11, B5 and the changelog rename, in that order. That is all - B12,
B13 and B14 run alongside and B8 comes after.

## How to work this file

Two gates. **Gate A** had to be green before the first beta image; **Gate B** before the final tag. Each row is
either a link to a plan under `.agents/plans/` that holds the whole item, or a checkbox for a one-line action.

**When a plan lands, its file is deleted.** Tick the row here and drop the link in the same change, leaving the
text. Otherwise this index rots into a list of dead links within a fortnight.

**How the pipeline works now**, because it shapes every item below. The workflow stages every build on GHCR,
smokes it there, and **copies the smoked digest to Docker Hub** - so nothing unsmoked reaches the registry users
pull from. Every tag is copied across, betas included; a prerelease just gets its immutable tag, never `3.0` or
`latest`. The release body is extracted from `CHANGELOG.md` by the workflow.

---

## Gate A - green

- **A1** - publish paths hardcoded in the workflow, no Actions variable needed. Done 2026-07-27.
- **A2** - a prerelease moves neither `latest` nor the minor tag. Observed on Docker Hub 2026-08-06, after beta 1.
- **A3** - health check IP restrictions, four defects and the missing tests. Done 2026-07-27.
- **A4** - forwarded headers, warn-once diagnostics and the missing tests. Done 2026-07-27.

A3 and A4 gated the beta rather than the tag because the beta is deployed behind a proxy with a health check
allow-list: A3 was the allow-list, A4 is what makes its failure modes visible instead of silent.

### Already verified - do not re-audit

- **Fitting results are unchanged.** Differential-tested 2026-07-19 against the real `binacle/binacle-net:2.1.1`
  image across all three algorithms, zero disagreements. No release-notes caveat needed.
- **Old ViPaq tokens fail loudly.** Verified 2026-07-19, locked 2026-07-20 with four regression vectors in
  `vipaq/test-vectors/serialization/decode-invalid.json`. Zero silent misparses.
- **The login throttle no longer partitions on a caller-supplied header.** `GetClientIp()` deleted 2026-07-24;
  `AuthTokenRateLimitingPolicy` partitions on `Connection.RemoteIpAddress`.
- **ViPaq's wire format did not move after beta 1** - source changed comments only, so beta 1's ViPaq evidence
  covers the shipped image.
- **The Dockerfile did not change after beta 1 at all.** `/app/data`, `libgssapi-krb5-2` and the OCI labels were
  all in that image already.

---

## Gate B

| # | Item | State |
|---|---|---|
| B0 | Unfreeze the docs site, point `current` back at `v2.1.x`, deploy | **done 2026-08-06** |
| B1 | Beta 1 verification on the deployed image | **done 2026-08-06** - all boxes pass, no defects |
| B2 | Write the `v3.0.x` docs pages | **done 2026-08-10** - site builds, `/version/latest/` lands on v3.0.x |
| B2X | Publish, verify and deploy beta 2 | **done bar two boxes, which moved to B11** - see below |
| B3 | Fix the ViPaq protocol page | **done 2026-08-07** - split landed, all four versions written |
| B4 | Generate `swagger/v3.json` and `swagger/v4.json` | **done 2026-08-06** - regenerated again under B8 |
| B5 | Move the sample image pins | **open** - see below |
| B6 | Run the ServiceModule suite once against Azure Storage | **open** - see below |
| B7 | v4 stays experimental; announce all four breaking changes | **done, re-confirm at the tag** |
| B8 | Flip `current` forward and redeploy the docs | **open, after the tag** - see below |
| B9 | Strip the `AI-GENERATED` review tokens | **done 2026-08-11** - zero left, whole repo |
| B10 | Merge `features/arch_tests` | **open** - see below |
| B11 | Beta 3 | **open**, after B10 - see below |
| B12 | Image verification - the recipe, and the surfaces that tell users | **open** - see below |
| B13 | The Docker Hub page - credential, file and workflow | **open** - see below |
| B14 | Docker Hub tag immutability - correct the rule | **open** - see below |
| B15 | Rate limiter integration tests, and a coverage bump | **open** - see below |

**On B9, one thing a later session must not undo.** The pass was never "revert everything an agent wrote", and
several agent comments were kept on purpose because they were judged better than the line they replaced - the
unchecked-multiply overflow note on the packing algorithms, the empty-catch explanation in `ConnectionString.cs`,
the curated-scenario table in `BischoffCuratedProblemsProvider.cs`. A surviving agent comment is not damage.

**On B7, what still has to hold at the tag.** `ApiV4Document.IsExperimental` must still be `true` - shipping v4
as stable would lock contracts meant to keep moving, and the flip is 3.1.0 work. And all four breaking changes
plus the six migration steps must survive the `## [Unreleased]` -> `## 3.0.0` rename; `just changelog extract
3.0.0` after the rename is the same preview that was checked byte-for-byte against the published beta 2 body.

### B2X - beta 2 {#beta-2}

Beta 2 exists because 27 commits landed after beta 1 and the assumption that they miss the image was **not
true** - the Sonar sweep changed shipping code in `api/src`, `lib/src` and `vipaq/src`, and the image went
framework-dependent on 2026-08-10 (150.2 MB -> 103.2 MB, 172 `System.*.dll` -> 4). It was also the first run of
the rebuilt pipeline, which was deliberate: a prerelease tag is the only free test that pipeline gets, and a
failure costs a deleted tag instead of a bad release.

**Published 2026-08-13** from `d317cd2b`, digest `sha256:ccce2a44`, and **deployed to the test server
2026-08-14**. Verified: all six jobs green with `publish` included, the GHCR package public to an anonymous
puller, Docker Hub took `3.0.0-beta.2` and nothing else moved, the copy preserved the digest, `BINACLE_VERSION`
reads `3.0.0-beta.2`, the image is signed on both registries, SBOM and provenance are in the index, 31/31
structure assertions and all five smoke profiles pass against the published image, and the release body is
byte-identical to `just changelog extract Unreleased`.

- [x] **The deployment host pulled it.** Deployed on the test server 2026-08-14. The package is public to an
      anonymous puller, proven from another machine. If that host has a `ghcr.io` entry in its docker config,
      the credential-free half of this check is still unproven - it costs a `docker logout ghcr.io` and a
      re-pull to close.
- [x] **`DEBUG_ENDPOINT` is off on the deployment** - 2026-08-14. It had been on and answering publicly since
      beta 1, echoing the caller's `Authorization` header back, and it was the only real exposure B1 left
      behind. It did not survive beta 2, which is what that run was for.
**The last two boxes moved to B11 on 2026-08-14.** They are live checks on a deployed host, and beta 3 replaces
the host they would run against. Doing them on beta 2 first would mean doing them twice. They are stated in
full under B11; nothing about what they check has changed.

- **Exercise the auth token endpoint** - moved to B11.
- **Re-confirm the resolved caller** - moved to B11.

**What beta 2 does not need to re-do:** the fitting differential, the old-ViPaq-token rejection, the health
check allow-list, or the login throttle partition. All four are in "Already verified" or closed by B1, and
nothing behind them changed.

**Two things beta 2 still does not prove**, both structural rather than untried. **The moving tags** - a
prerelease withholds `{{major}}.{{minor}}` and `latest`, so `3.0` and `latest` are written for the first time by
v3.0.0 itself; that is one extra argument to an `imagetools create` call that has run several times. And
**`latest=auto` does not consult the registry** - it reads the git ref, so any non-prerelease semver takes
`latest`. Right for v3.0.0, and the reason a throwaway `v0.0.1` against the real repo would have moved `latest`
off `2.1.1`. Recorded because the trap outlives the plan that found it.

### B10 - merge `features/arch_tests` {#arch-merge}

**Pulled into the release on 2026-08-14.** It was standing work; the maintainer decided it ships in v3.0.0. It
is the third exception taken to "not in this release", and the largest.

**What is on the branch.** Five commits off `2471ec0d`, 520 files. Three separate things in one branch:

- **The packing-contract extraction.** `Binacle.Lib.Abstractions` is broken up: the geometry half folds into
  `Binacle.Geometry`, the packing vocabulary becomes a new `shared/src/Binacle.Packing`, the engine interfaces
  go into `Binacle.Lib`, which is left as the only project in `lib/src`. `shared/test/Binacle.TestsKernel`
  splits, its result-selection half becoming `lib/test/Binacle.Lib.TestsKernel`, with the fixtures moving from
  `shared/data/` to `lib/data/`. The repo ends with **no upward edge** - nothing under `shared/` references
  `lib/` or `api/`.
- **`architecture.yml` at the repo root**, plus the 27 comment fixes that stop code pointing into the agent
  guidance. The **check** that enforces it is not built and is not release work.
- **The `.agents/rules/` layer and a rewritten `CLAUDE.md`.** Process only, invisible to users.

**Why it is safe enough to take this late.** The shipping-code change is namespaces, `using` lines and
`ProjectReference` entries - 84 non-comment changed lines across `api/src`, against 356 total. No type was
renamed, no contract type changed shape, `Auth/Token.cs` is untouched, and the `Dockerfile`, the workflows,
`CHANGELOG.md` and `samples/` are all untouched. The rest is comment thinning.

**The merge is clean against the committed tree and conflicts with the staged one.** `git merge-tree main
features/arch_tests` comes back clean today only because the pending `.agents` work is staged rather than
committed. Twelve files are modified on both sides:

`board.md`, `release-v3.0.0.md`, `post-release-v3.0.0.md`, `design/_index.md`, `design/ci-cd/decisions.md`,
`docs/api/modules/service.md`, `docs/api/openapi.md`, `docs/api/v4/README.md`, `docs/api/v4/add-endpoint.md`,
`docs/ci-cd/release-pipeline.md`, `plans/_index.md`, `plans/ci-cd/release-pipeline-rebuild.md`.

**Resolve them by rule, not file by file.** The branch forked before all of that work, so its copies are the
older text plus one systematic addition:

- **This file, `post-release-v3.0.0.md`, `board.md` and the four docs/design/plan files: take the current
  content.** The branch has nothing new in them except front matter.
- **Then re-apply the branch's `description:` front matter**, which is the convention it introduces across
  every `.agents` file. Port the branch's "Architecture and quality" board section too - that part is real.
- **Never hand-resolve an `_index.md`.** Run `just agents all` after the merge; it writes six indexes now, not
  five.

**Three chores the merge does not do for itself:**

- [ ] **Front matter on the five files added since the fork** - the four new plans under `plans/api/`,
      `plans/ci-cd/` and `plans/tooling/`, and `design/api/decisions.md`. The design one already has it; the
      four plans do not, and they are invisible to the new convention until they do.
- [ ] **Regenerate and diff the OpenAPI documents.** The contract types moved namespace. Schema names come off
      the short type name so nothing should move, but "should" is not a check - `just openapi generate` and
      diff against `docs/collections/_versions/v3.0.x/swagger/`. Do it **after** the `429` guard is committed,
      or the guard's change and the merge's change are indistinguishable in the diff.
- [ ] **`just test all`, then `just openapi lint`.** Eleven leaves, about two minutes. The branch renames
      projects that `tooling/coverage.just` and `Directory.Build.props` both describe.

### B11 - beta 3 {#beta-3}

**Decided 2026-08-14.** B10 changes shipping code, so beta 2's image evidence describes a tree that is not the
one shipping. This is the same reasoning that produced beta 2 out of beta 1, and it costs one tag.

**What beta 3 is for, and it is narrower than beta 2 was.** Beta 2 was proving the rebuilt pipeline. That is
proven. Beta 3 exists to put the restructured tree on a real host and run the two live checks against the
image that actually ships.

- [ ] **Cut it from the merge commit**, once `just test all` is green and the OpenAPI diff is understood.
- [ ] **Exercise the auth token endpoint.** `ServiceModule/v0/Endpoints/Auth/Token.cs` is the single most
      restructured shipping file and its rejection chain is now one extracted `Reject` helper. A wrong branch
      returns the wrong status code to a real client, which no unit test shape catches as well as one live call.
      **The merge does not touch this file** - the check is owed from beta 2 and is being paid here.
- [ ] **Re-confirm the resolved caller** once the forwarded-headers source header settles. During B1 it moved
      between boots (`CF-Connecting-IP` on one, `X-Forwarded-For` on a later one). The resolved caller was
      correct whenever it was observed, but the two are not equivalent behind a CDN and the health check
      allow-list is compared against whatever they resolve to.
- [ ] **Confirm the image still runs and the version stamp reads `3.0.0-beta.3`.** The pipeline smokes the GHCR
      copy before anything is copied, so a broken image cannot reach Docker Hub; this is the deployment half,
      which the pipeline does not see.

**What beta 3 does not need to re-do:** everything in "Already verified", plus every structural thing beta 2
proved - the six jobs, the digest-preserving copy, the signature on both registries, the SBOM and provenance,
the release body extraction. None of it moves because a namespace did.

**Still not proven, same as after beta 2.** A prerelease withholds `{{major}}.{{minor}}` and `latest`, so `3.0`
and `latest` are written for the first time by v3.0.0 itself.

### B12, B13, B14 - the three that came off the board {#off-the-board}

**Pulled in on 2026-08-14.** They were briefly in `post-release-v3.0.0.md` and that was wrong: each carries
tooling and a decision that has to be figured out **before** the tag, and only the confirmation belongs after.
That file is checks only. **None of these three gates the tag** - if one is not ready, it ships without it -
but each is cheaper now than after.

#### B12 - image verification

[tooling/image-verification-recipes](plans/tooling/image-verification-recipes.md). The release notes say the
image is signed and carries an SBOM and provenance, and nothing tells a user how to check any of it. As of the
tag we advertise a property no user can confirm.

**Before the tag:**

- [ ] **The `just` recipe.** A prototype was built on 2026-08-13, run green against `3.0.0-beta.2`, then
      reverted, so the tree is clean and the plan holds what it learned. Five checks, and the order matters -
      each answers something the next assumes.
- [ ] **Decide the placement first.** It is the plan's own open question and it changes what gets written. It
      is coupled to `image-module-stacks`, which is still on the board.
- [ ] **Write the two surfaces a coding session owns.** The plan names five; two are writable here.
- [ ] **Hand the docs-site surface to B8.** Repo-root `docs/` is off limits from a coding session, so write
      down what the page must say and let the docs deploy carry it. **This is the deadline that makes B12
      pre-tag rather than post-tag** - B8 runs once, straight after the tag, and a surface that misses it waits
      for the next docs deploy.
- [ ] **The fifth surface is the Docker Hub page**, so B12 goes before or with B13, never after. Otherwise the
      page gets written twice.

#### B13 - the Docker Hub page

[ci-cd/dockerhub-overview](plans/ci-cd/dockerhub-overview.md). The description advertises `2.1.1` as latest and
hand-lists fifteen tags, none of them 3.x. For a lot of people that page is the first thing they read about the
project, and the tag is what makes it wrong rather than merely stale.

**Before the tag:**

- [ ] **Test the credential first - five minutes, and it decides the whole item.** The plan has the exact
      calls. Two traps in it: test `POST /v2/auth/token`, not the legacy `/v2/users/login/` whose 403s would
      condemn the plan for no reason; and **back the page up and PATCH the current text straight back**, never
      a placeholder - a green result and a defaced public page in the same second. If the widest scope still
      403s with Admin confirmed, the plan says do **not** fall back to an account password. Record the answer
      and the date in the plan either way.
- [ ] **Write `.github/dockerhub-overview.md`** and the `update-dockerhub-overview.yml` workflow.
- [ ] **The logo and the categories** - pure web form, nothing gated.

**What is not done before the tag: publishing it.** The text names `3.0`, a tag that does not exist and will
not point at a signed image until v3.0.0. **Do not let the file reach `main` early** - the workflow triggers on
a push touching that path, so landing it is publishing it. Hold the file, or land it and run the workflow by
`workflow_dispatch` after the tag.

#### B14 - tag immutability

[ci-cd/dockerhub-tag-immutability](plans/ci-cd/dockerhub-tag-immutability.md). Not caused by the release, but
v3.0.0 is the first run that writes moving tags, which is when the trap in it becomes real.

**Before the tag - the rule only, not the switch:**

- [ ] **Confirm the setting is even offered** in the repository's settings UI. It appears in the API response,
      so plan availability is probably not the blocker, but the sponsored org's entitlements decide it.
- [ ] **Correct the rule.** It reads `".*"` today with the switch off. **A rule marks matching tags immutable;
      it does not exempt them** - so that value would freeze `latest` and `3.0`, the two tags designed to move.
      Write a rule that matches exact-version tags only, and confirm the value took.

**Leave the switch off until after v3.0.0.** Turning it on with a wrong rule fails the publish job *after* the
image has been built, smoked and copied - a red at the last step of an otherwise good release, with the moving
tags half written. There is no version of this worth risking the release for.

### B15 - rate limiter integration tests, and a coverage bump {#rate-limiter-tests}

**Added 2026-08-14 at the maintainer's call.** This is a **carve-out** from
[api/integration-test-additions](plans/api/integration-test-additions.md), not the whole plan. That plan's four
phase-1 questions, the module matrix and the CORS gap all stay on the board; what comes into the release is its
sharpest single finding.

**The finding, verified 2026-07-29 and still true:** **no test anywhere asserts that a 429 ever happens.**

- The core harnesses boot with modules off, so the `"ApiUsage"` policy is never registered, the
  `.RequireRateLimiting("ApiUsage")` metadata on all 18 endpoints is inert, and the middleware is not in the
  pipeline. That harness cannot see rate limiting at all.
- The ServiceModule harness turns the module on and then sets `RateLimiter:AuthToken` to `NoLimiter::0`
  (`api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs`), deliberately disabling the limiter so
  the auth tests are not throttled.

So someone could reorganise module registration until nothing is limited, and every suite would stay green
while the shipped API stopped limiting anyone.

**Why it is release work and not board work.** Two of this release's own claims rest on behaviour nothing
tests. The login throttle change - it partitions on `Connection.RemoteIpAddress` instead of a caller-supplied
header, so varying a header no longer resets your own throttle - is in "Already verified" on the strength of
**one hand-check on 2026-07-24**, and it is the one security-relevant change in the release with no regression
behind it. And the `429` OpenAPI guard says the module-off document must not document a status the build cannot
emit; that is asserted by a decision record and a transformer, and by nothing executable.

**What to build:**

- [ ] **A 429 arrives when the module is on.** The shipped limits are in
      `Config_Files/ServiceModule/RateLimiter.json` - `ApiUsageAnonymous` is `SlidingWindow::60/3600-30`,
      `AuthToken` is `SlidingWindow::20/3600-30`. **The test must read the configured number, not guess it.**
- [ ] **No limiting when the module is off.** The pair is the point: one test each side proves the behaviour is
      the module's, which is what the `429` guard and the decision behind it claim.
- [ ] **The auth throttle partitions on the connection address.** Vary the caller-supplied header across
      requests and assert the throttle does **not** reset. This is the one that turns a hand-check into a
      regression test, and it is the highest-value test in the list.
- [ ] **Do not turn the limiter on in the shared harness.** A live limiter is a shared bucket across a suite
      that hits endpoints repeatedly, and the failure mode is random 429s that read as flakiness rather than as
      a limit. Use a dedicated harness instance with a tiny limit and its own tests. The existing auth tests
      keep `NoLimiter::0`.

**On coverage, and keep this modest.** Cover what this release changed, not the global number. **The gate is a
different problem and is not this item** - the Sonar quality gate hangs on `new_coverage`, and the UI is 1571
lines at 0%, 22.5% of the denominator. That is `ui-test-harness`, it is a whole harness, and it stays on the
board. Do not let this grow into it.

**Sequencing: after B10.** The merge renames projects and moves the types these tests touch. Writing them first
means writing them twice.

### B5 - move the sample image pins

- [ ] **The pins move once, to `3.0`, as the last change before the tag.** They sit at `3.0.0-beta.1` today and
  **they stay there until then.** The rule that drives it: **a pin on `main` must name an image that exists on
  Docker Hub.** An intermediate bump to `3.0.0-beta.2` is possible now that betas reach Docker Hub, but it is
  not worth churning nine files twice to land on `3.0` a week later.

  **It is nine files, not six.** The six are the pin itself; three more carry the beta in prose, and they are
  the ones that get missed:

  | File | What to change |
  |---|---|
  | `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` | the `image:` line |
  | `samples/kubernetes/minimal/binacle-deployment.yaml` | the `image:` line |
  | `README.md` | the pin warning under Quick Start - **the repo landing page** |
  | `samples/README.md` | the pin paragraph |
  | `samples/docker/README.md` | the pin paragraph |

  **The prose goes with the number.** All three READMEs explain the beta pin with some version of "since `3.0`
  does not exist on Docker Hub yet". That reason expires the moment v3.0.0 publishes.

  The two-line comment above each `image:` line is already dropped - all six read exactly `# Pinned on purpose -
  a copied sample must not jump to a new major on the next pull.`, matching the published docs copies. Only the
  `image:` lines are left to move.

  Two more mention the beta as an **example** rather than a pin - `tooling/README.md` and `tooling/smoke.just`,
  both showing "smoke what is actually on Docker Hub". Neither is wrong today, both read as stale once the tag
  is out. Sweep them at the same time.

  **The caveat.** The pins moved early once before, on 2026-08-07, and sat on `main` naming an image that did
  not exist. **Do not leave the `3.0` bump on `main` long before tagging.**

### B6 - Azure Storage

- [ ] CI covers SQLite and Postgres only, so the Azure provider ships on trust. One deliberate run before
  tagging: `just serve services -d`, then `just test api-service-integration AzureStorage`.

  **This got more important on 2026-08-07, not less.** The `service-azure` sample is gone, folded into
  `service`, where Azure is one commented connection string among three. So Azure ships with no dedicated
  sample, no CI coverage and no smoke profile (smoke is SQLite-only by design). This one run is the only thing
  standing behind it. Removal is a stronger idea than it was, but not in this release.

### B8 - deploy the docs, after the tag

**The config half is done**: `main` carries `current: v3.0.x`, `- id: v3.0.x` back at the top of `list`, the
stub's `sitemap: exclude` gone, and `docs/collections/_sitemaps/version-3-0-x.xml` restored. What is left is the
deploy plus three edits that must go out with it. Repo-root `docs/` is off limits to a coding session - this is
the docs session's work, written here for it.

- [ ] **Put the real date and release link in `v3.0.x/release-notes.md`.** The `## v3.0.0` section carries
  interim wording - it asserts no date and links the releases *list* - because the tag did not exist when the
  pages were written. Swap that italic line for *"Released &lt;date&gt; - [release on
  GitHub](.../releases/tag/v3.0.0)"*, matching every other version folder.
- [ ] **Carry three additions from `CHANGELOG.md` into `v3.0.x/release-notes.md`.** They are the same notes in
  two places, and the release body gained content on 2026-08-10 that the page does not have. Run `just changelog
  extract Unreleased` to see the current text. All three go in the `## v3.0.0` section, in the page's
  plain-ASCII style:
  - **Overview**, one bullet after the health check line: the image creates `/app/data` and gives it to the app
    user, so a volume mounted there is writable.
  - **Core Changes**, replacing "The `Dockerfile` and existing environment variables are unchanged" (false - the
    Dockerfile changed three times this release): the `/app/data` fix spelled out - docker used to create the
    mount point as root, the app does not run as root, so packing logs and the SQLite database could not be
    written to a fresh named volume; `libgssapi-krb5-2` now ships, so Npgsql stops printing "Cannot load library
    libgssapi_krb5.so.2" at every start, which was harmless but read as fatal; OCI labels on the image; and only
    then "existing environment variables are unchanged".
  - **A `🔌 Service Module` section**, between Diagnostics and UI Module: the auth token rate limit partitions on
    the connection's remote address instead of a caller-supplied header, so varying the header no longer resets
    your own login throttle. The page already carries the exemption note at the top.
- [ ] **Replace the two stale swagger documents under `docs/collections/_versions/v3.0.x/swagger/`.** They are
  frozen copies and no longer match `just openapi generate`. Regenerate and copy
  `artifacts/openapi/Binacle.Net_v3.json` -> `swagger/v3.json` and `artifacts/openapi/Binacle.Net_v4.json` ->
  `swagger/v4.json`; the generator's file names differ from what the site expects, so the rename is part of the
  handover. Two things changed since the copies were taken: both documents now carry a `servers` entry with the
  single relative `/`, and **the `429` responses come out** - the transformer that documents `429` had lost its
  feature-flag guard, so it was documenting the status in a document generated with the ServiceModule off, which
  has no limiter and cannot emit one. The copies currently carry 14 `429` mentions in v4 and 4 in v3;
  regenerated, both are zero, which is what v2.1.x shipped. For v3 that **restores** the published shape rather
  than changing it, so nothing about the frozen v3 contract moves. It is still a visible change to the published
  spec, so mention it wherever the update is described.
- [ ] **Carry B12's signature-verification page.** The images are signed and carry an SBOM and provenance, and
  the docs site says so nowhere. B12 writes down what the page must make - the verified `cosign verify`
  invocation, the three points it has to make, and the rule that any example tag names a signed image - and
  this deploy is what puts it live. **Check B12 actually left that text before deploying**; if it did not, the
  page waits for the next docs deploy, which is the reason B12 is pre-tag work.
- [ ] **Deploy.** It is `workflow_dispatch` only.

**This is the single most losable item in the release** - nothing fails if the deploy is skipped, the site just
quietly keeps serving v2.1.x as current. **It has to run after the tag**, for two reasons: the notes need the
date and the `releases/tag/v3.0.0` link, and `main` already says v3.0.x is current, so deploying earlier presents
an unreleased version as current. It has to land before anything is announced, because the announcement points
at pages that must be live. Tag, then deploy the docs, then announce.

**One deliberate 404, do not "fix" it.** The `v3.0.x` ViPaq page links the wire spec at
`github.com/ChrisMavrommatis/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md`, which 404s until the tag is pushed. A
versioned page should pin the spec it describes; do not repoint it at `main`.

---

## The release notes {#release-notes}

**They live in `CHANGELOG.md`, in the `## [Unreleased]` section**, and the workflow extracts that section as the
release body. The content is complete and was checked byte-for-byte against the published beta 2 body on
2026-08-13 - all four breaking changes, the six migration steps, the signing and SBOM bullet, the image-size
drop, and `RetentionDays`. Nothing since then changes what a user can observe.

Three mechanics to keep in mind:

- **`[Unreleased]` is renamed to `## 3.0.0` as the last change before the tag**, alongside the B5 pin bump.
  Every beta publishes `[Unreleased]`. If you forget, the `notes` job fails in under a minute and nothing is
  built - which is why that gate runs first.
- **A section's own headings are `###`**, nested under the `##` version heading. `just changelog extract` shifts
  them back to `##` on the way out. Do not "fix" the file to use `##` throughout - that breaks the nesting under
  `# Changelog` and the extractor's terminator both.
- **Preview with `just changelog extract 3.0.0`** after the rename. That is exactly what gets published.

**The compare link at the bottom already reads `v2.1.1...v3.0.0`** - correct from the tag onward, and a 404 on
every beta release page until then. Left as it is deliberately; the alternative is editing it twice.

---

## The sequence

Steps 1-11 are done: Gate A, both betas, B0 through B4, the docs pages, the pipeline rebuild, the public GHCR
package. Beta 2 is deployed on the test server as of 2026-08-14. What is left, in order:

1. **Commit the staged `.agents` work and the `429` OpenAPI guard.**
   `RateLimiterResponseOperationTransformer.cs` is shipping code and the restored guard is still only in the
   index. It has to be in before the tag - B8's swagger regeneration depends on it, and an uncommitted fix is
   one that ships by accident or not at all. Committing first is also what makes the B10 merge resolvable:
   twelve `.agents` files are modified on both sides, and a staged change is not something git can merge.
2. **B10 - merge `features/arch_tests`**, resolving by the rule above, then the three chores: front matter on
   the five new files, `just agents all`, and the OpenAPI regenerate-and-diff.
3. **`just test all`** - eleven leaves, about two minutes, nothing to bring up. Both the guard and the merge
   changed shipping code after beta 2, so the last green run does not cover the tree as it stands.
4. **B15 - the rate limiter tests.** After the merge, so they are written once against the final project
   layout.
5. **B6** - the one Azure Storage run. **After the merge**, not before: it is the only evidence the Azure
   provider works at all, and the merge moves the types underneath it.
6. **B11 - cut, deploy and verify beta 3**, and clear the two checks carried over from B2X.
7. **B12, B13 and B14, in that order**, any time from here. B12 before B13 because one of its five surfaces is
   the Docker Hub page. **They run alongside, and none of them holds the tag.** B12 has one hard deadline
   inside it: the docs-site surface has to reach B8's handover list before the tag, because B8 runs once.
8. **The last commit before the tag, all in one:** rename `## [Unreleased]` to `## 3.0.0`, bump the nine files
   from `3.0.0-beta.1` to `3.0`, sweep the "since `3.0` does not exist yet" prose in the three READMEs, and
   sweep the two `tooling/` examples that name a beta tag. Then re-confirm `ApiV4Document.IsExperimental` is
   still `true`, and tag `v3.0.0`.
7. **The pipeline does the rest.** The tag triggers it: the changelog gate, the suite, the GHCR build, the
   smoke, the Docker Hub copy under all three tags, the signature, and the release created from the `3.0.0`
   section. **Nothing here is manual any more.** Watch the run, then check the rendered body and `docker buildx
   imagetools inspect`.
8. **B8 - deploy the docs**, with the three edits above.
9. **Work `post-release-v3.0.0.md`.**

**The pins bump once, at step 6, and beta 3 does not move them.** They sit at `3.0.0-beta.1` through the whole
sequence. B5 already says why: the rule is that a pin on `main` names an image that exists on Docker Hub, and
`3.0.0-beta.1` still does. Churning nine files to reach `3.0.0-beta.3` for a few days is the trade B5 already
refused once.

**Does the restructure need a changelog line?** No user-observable behaviour changes, nothing is published to
NuGet, and no contract moves - so the four breaking changes stay four and the `[Unreleased]` section is
already correct as it stands. Anyone building from source sees `Binacle.Lib.Abstractions` disappear. If that is
worth a line it is a housekeeping bullet, not a fifth breaking change; **the maintainer's call, and the default
is to leave it out.**

## Not in this release

Everything else has a plan of its own and is on the board, grouped by area with its blockers named. Do not pull
any of it in: the version stamp, the npm publishing decision, the `Parallel*` processors, migrating the UI
clients off v3, the benchmark ledger, TestsKernel fixtures, and v4 going stable in 3.1.0.

**Three exceptions have been taken.** Two on 2026-08-10, both already in: **dropping `--self-contained`**
(150.2 MB -> 103.2 MB, all suites green, and beta 2 is the run that proved it on a real deployment) and **the
release-pipeline rebuild** (a prerelease tag is the only free test that pipeline will ever get; deferring it
meant the first run of a rebuilt publish path would be v3.0.0 itself). The third, on 2026-08-14, is **B10** -
the architecture restructure. The rest of the image work - chiseled bases and the hardened-image question -
stays out.

**Three more came off the board on 2026-08-14** and are B12, B13 and B14: the Docker Hub repository page, the
image-verification work and tag immutability. They spent an afternoon in `post-release-v3.0.0.md` and that was
wrong - each carries tooling or a decision that has to be settled before the tag, and that file is checks only.
**None of the three gates the tag.** A fourth, B15, is a carve-out from the integration-test plan rather than
the whole thing.

**The architecture check does not come with B10.** The branch fixes all 27 comment sites; the check that keeps
them fixed is CI work and stays on the board. A check landing days before a tag is a red build with nothing to
gain.
