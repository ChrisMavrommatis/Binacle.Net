---
description: The board - what there is to work on
---

# The board - what there is to work on

**This is where you pick work from.** Everything not tied to a release lives here: plans, ideas and the
one-liners. Open it, choose a row, open that file, start.

Permanent. Unlike the release set, this file is never deleted - versions come and go underneath it.

**Created 2026-08-07.**

## How to use it

- **Themes first, readiness second.** Find the area you want to work in, then look at the `State` column.
- **`ready`** means nothing blocks it - it can start today. **`blocked`** names what it waits on.
  **`deferred`** means deliberately not now, and the row says what revives it. **`in progress`** means someone
  is on it.
- **A row is a pointer, never a container.** The plan or idea file holds the work. If you find yourself
  explaining the work here, it belongs in that file instead.
- **Ideas are listed, not tabled.** They are maybes; a maybe does not get a `State`. An idea reaches a table
  only by being promoted to a plan first.

## How agents maintain it

Agents keep this file current. Two rules:

- **When a plan lands, its file is deleted - so tick the row and drop the link in the same change.** Otherwise
  the board rots into dead links, which is exactly what it exists to prevent.
- **Do not place, reorder or reprioritise a row on your own judgement. Ask.** Where something goes, how urgent
  it is, and when it should happen are the maintainer's calls, not an agent's. Adding a row for work you were
  told to record is fine; deciding it is "ready" and putting it at the top is not.

## What is not here

- **The active release.** v3.0.0 is in flight - `release-v3.0.0.md` holds its gates and
  `post-release-v3.0.0.md` holds the days right after it. **Do not pull board work into the release.**

  `docs-v3-pages` was release work (item B2). B2 landed on 2026-08-10 and the plan was deleted. The one
  question that outlived it - whether the frozen versioned sample copies get corrected or annotated - moved
  into `sonar-issue-triage`, which already held the findings it came from.

  `sonar-issue-triage` and `architecture-boundaries` used to be listed here as having no row. They now have
  one, under Architecture and quality.

  **`lib/extract-packing-contracts` is done and its file is deleted** (2026-08-13). The code, `architecture.yml`,
  `lib/README.md` and all fourteen agent-reference files landed; its durable reasoning moved into the lib
  decisions ledger as D2 and D3.

- **Five things left this board on 2026-08-14, all of them into the release.** The `features/arch_tests`
  branch, which carries `architecture.yml`, the packing-contract extraction and the 27 comment fixes. The
  Docker Hub repository page. The image-verification work. Docker Hub tag immutability. And a **carve-out**
  from the integration tests - the rate limiter cases only.

  **Three rows went; two items left without their row.** The three Docker Hub and verification plans lost their
  rows here and are release items now - they were briefly put in the post-release list and moved on the same
  day, because each has tooling or a decision that must be settled before the tag and that file is checks only.
  The architecture plan and the integration-test plan **keep their rows**, because in both cases the release
  takes a slice and leaves the rest. Every plan file stays under `plans/`.

  **What stayed behind matters more than what left.** The architecture plan is not done - only its *file* half
  and the comment fixes ship in v3.0.0. The check that keeps those fixes fixed, and every phase after it, is
  still here and is still the biggest single item on this board.

- **Reference material.** Docs, design and memory are not work. Find them through their own indexes.

---

## The order - a recommendation, not a decision {#the-order}

**Written 2026-08-14 because the maintainer asked for direction, and it is a recommendation only.** Placement
and priority are the maintainer's call; this section exists to make that call cheap, not to make it. Strike any
line of it.

**Everything below assumes v3.0.0 is tagged and the post-release list is clear.** Nothing here starts before
that.

### First: three decisions, no code

Each is one sitting and each unblocks a whole table. Doing any *build* before these risks building the thing
the decision would have cancelled.

1. **The ServiceModule direction.** Six files pull in one direction and interact - the simplification would
   collapse the layering the migration runner targets, the admin site and refresh tokens build on the same auth
   surface, and rate-limiting ownership overlaps the simplification. **This is the highest-leverage hour on the
   board**: one answer places seven items, two one-liners and the Azure Storage removal question.
2. **The v4 endpoint.** `v4-stable` cannot move without one endpoint added to v4 that reshapes no existing
   contract. `pack/first-bin` is the only candidate written down. Promote it, pick another, or accept that v4
   stays experimental past 3.1.0 - all three are answers; having none is not.
3. **Does anyone run this on ARM?** `multi-arch-images` is blocked on a question, not on work. If the answer is
   no, the useful action is writing that down as a decision and deleting the plan.

### Then: the first build, and it is one change, not three

**Architecture phase 1 - the comment check - plus `ci-gates` plus the OpenAPI lint, in a single workflow
change.** All three add steps to the same PR gate, all three are ready, and doing them separately means
touching `run-tests.yml` three times and arguing about job ordering three times.

**It lands green.** The branch fixed all 27 comment sites before the check exists, which is the state a new
gate wants: a check that is red on arrival teaches everyone to ignore it.

### Then: 3.1.0's content

The v4 chain, in this order and no other - `pack/first-bin` -> `v4-stable` -> `ui-clients-off-v3` -> the
UIModule Alpine port. Each step needs the one before it, and porting a page you are about to rewrite is wasted
work.

### Running alongside, not queued behind

- **`ui-test-harness`.** It is the only reason the Sonar coverage gate is red and it touches nothing above.
  v3.0.0 adds rate limiter tests and a modest coverage bump; **that does not move this gate** and was not meant
  to. The UI is 1571 lines at 0% and only a harness fixes it.
- **`integration-test-additions`, phase 1 only.** Read what the release built for the rate limiter tests first
  - it answers one of phase 1's four questions in code.
- **`sonar-issue-triage`.** Leftovers, and one of them - the frozen-copies question - is a decision.
- **`parallel-processors-decision`.** A measurement and a decision, self-contained.

### Deliberately last

The image work - `image-base-slimming`, `multi-arch-images` if ARM is wanted - and the maintainer tooling. They
change the shipped artifact or nothing at all, and neither is worth doing in the weeks after a major release.

---

## CI and the release pipeline

| Plan | State | Waiting on |
|---|---|---|
| [ci-cd/multi-arch-images](plans/ci-cd/multi-arch-images.md) | **not scheduled** | does anyone run it on ARM? |
| [ci-gates](plans/ci-cd/ci-gates.md) | ready - **was blocked, unblocked 2026-08-11** | - |
| [ci-cd/release-pipeline-rebuild](plans/ci-cd/release-pipeline-rebuild.md) | **in progress - proven end to end by beta 2, deployed 2026-08-14** | one open question for the maintainer |
| [image-base-slimming](plans/image-base-slimming.md) | ready - **timing not decided** | - |

**The two Docker Hub plans left this table on 2026-08-14** and are release work now. The page one was gated on
the `3.0` tag existing, but everything before the publish - the credential test, the file, the workflow, the
logo - is pre-tag. The immutability one is not caused by the release, but v3.0.0 is the first run that writes
moving tags, and the rule sitting on the repository today would freeze the two that move. Their plan files are
unchanged and still under `plans/ci-cd/`; when the release consumes them, the files get deleted with the rest
of the release set.

**`ci-gates` should not be built on its own.** It, the architecture comment check and the OpenAPI lint one-liner
all add steps to the same PR gate. See the order section above.

**`ci-release-workflow-build` is gone - it landed as the GHCR rebuild on 2026-08-11**, including the SBOM and
provenance it had listed as later work. Its one unbuilt section became `multi-arch-images`; the rest is
described in the CI/CD docs and decisions ledger now.

**`multi-arch-images` is blocked on a question, not on work.** The published image is `linux/amd64` only. If
nobody runs Binacle.Net on ARM, that is a defensible choice and the useful action is writing it down as a
decision rather than building anything.

**`ci-gates` is no longer blocked.** The release workflow builds through `just build publish` now, so the PR
gates below it would prove what they claim to prove.

**`image-base-slimming` is here because it is about the shipped artifact** - move it if you would rather it sat
elsewhere. The duplicated-runtime finding it opened with **landed on 2026-08-10 and went into v3.0.0**: dropping
`--self-contained` took the image from 150.2 MB to 103.2 MB, and beta 2 is the first built that way. The file is
now about the base itself, which is ~90% of what remains - chiseled, and whether Docker Hardened Images earn
their subscription. **Not scheduled.**

**It stopped being standing work on 2026-08-10.** The maintainer pulled the first half into the v3.0.0 release,
because a prerelease tag is the only free test that pipeline gets and beta 2 is the run. The release plan owns
the scheduling; the plan file owns the how. Only the parts a beta can prove moved - SBOM, provenance and
multi-arch are still here and still unscheduled.

**`release-pipeline-rebuild` landed and was proven by `v3.0.0-beta.2`** - first on 2026-08-11, then end to end
when the tag was **re-cut on 2026-08-13** and `publish` ran too: the Docker Hub copy preserved the digest, the
image is signed on both registries, and the release body came out of `CHANGELOG.md`. The GHCR package is
public, and **beta 2 was deployed to the test server on 2026-08-14**, so the host reaches GHCR. What is left in
the file is one open question for the maintainer, plus the narrow question of whether that host pulled without a
stored credential. **The throwaway-tag check was dropped**, because beta 2 covered what it was for.
**Telling users how to verify the signature moved out on 2026-08-14** into the image-verification work under
Tooling, which now owns the invocation and all five surfaces that carry it. **The v3.0.0 release plan owns the
scheduling from here.**

One-liner, in [todos](plans/todos.md): **lint the OpenAPI documents on every PR.** Ready, and no longer
blocked - the `servers` block landed on 2026-08-10 and the lint is clean, so the gate can fail on warnings
from day one. **Do it inside the gate change above**, not as its own commit.

## Architecture and quality

Cross-cutting work that belongs to no single slice.

| Plan | State | Waiting on |
|---|---|---|
| [architecture-boundaries](plans/architecture-boundaries.md) | **in progress - the file half ships in v3.0.0** | - |
| [sonar-issue-triage](plans/sonar-issue-triage.md) | ready | - |

**Both rows are new here on 2026-08-14**, and the architecture one arrives with its own file split in two.
`architecture.yml` and the 27 comment fixes are on `features/arch_tests` and were pulled into the v3.0.0
release; the plan file **stays here** because everything that enforces any of it is unbuilt.

**`architecture-boundaries` is what gets picked up next - the maintainer's call, 2026-08-13.** That predates
the order section above and agrees with it. Nothing blocks it: the file half is done and the graph was
re-derived from every `ProjectReference` to prove it.

**What is left in `architecture-boundaries`, in the plan's own order.** Phase 1's check - three arms, the
derived filename list, the `$id` references and the bare ref codes, each blind to the other two. Then
ArchUnitNET, leading with the two rules a graph walk can never see: the api module boundary, and v3-frozen.
Then dependency-cruiser, then lychee. **Do not go looking for comment sites to fix** - all 27 are already fixed,
so the check lands green, which is the state a new gate wants.

**`sonar-issue-triage` is the sweep's leftovers.** The sweep itself is done; the file still carries the CA1816
decision, the frozen-copies question and what the quality gate hangs on. The quality gate hangs on
`new_coverage`, which is `ui-test-harness` under Testing - the two rows are the same problem seen from two
directions.

## Testing

| Plan | State | Waiting on |
|---|---|---|
| [api/integration-test-additions](plans/api/integration-test-additions.md) | ready | - |
| [shared/testskernel-data-extraction](plans/shared/testskernel-data-extraction.md) | ready | - |
| [ui-test-harness](plans/ui-test-harness.md) | ready | - |

**`ui-test-harness` is why the Sonar coverage gate is red.** The UI is the only code with no harness at all -
1571 lines at 0%, 22.5% of the coverage denominator. Recorded 2026-08-09; state and priority are the
maintainer's call, not one an agent made.

**`integration-test-additions` is two sessions.** Phase 1 investigates and stops; the maintainer picks the
shape; phase 2 writes the tests. Do not run it as one job - the plan says why.

**Its rate limiter cases were carved out into v3.0.0 on 2026-08-14 and are not to be built here.** The release
takes one finding - that nothing anywhere asserts a 429 ever happens - and covers the two limiters plus the
auth throttle's partition. **Everything else in the plan stays**: the four phase-1 questions, the module
matrix, CORS, and the hunt for other core behaviour that only works because an optional module registered
something. The gate is unchanged - phase 1 still investigates and stops.

**Read the release's answer before phase 1 runs.** One of phase 1's four questions is "where do the rate-limit
tests live so a live limiter does not make everything else flaky", and the release will have answered it in
code. Phase 1 inherits that answer instead of re-deciding it.

**Two of these three collide with the architecture work, and one collides with a pin.** The ArchUnitNET phase
adds a test leaf that references every slice it inspects, and `testskernel-data-extraction` reshapes the
fixture kernels the architecture branch just split - **whichever runs second reads the other's result.** The
xunit runner pin is the trap that bites both: this repo pins `xunit.v3.mtp-v2` on purpose, and a new test
project that pulls plain `xunit.v3` throws before a single test runs.

Ideas: [mutation-testing](ideas/mutation-testing.md) - one contained experiment before any opinion.
[testing-techniques](ideas/testing-techniques.md) - a survey, nothing decided.
[vipaq/interop-vector-coverage](ideas/vipaq/interop-vector-coverage.md) - self-assessed low value.

## API

| Plan | State | Waiting on |
|---|---|---|
| [api/ui-clients-off-v3](plans/api/ui-clients-off-v3.md) | ready | - |
| [api/v4-stable](plans/api/v4-stable.md) | blocked | no endpoint chosen - see below |

**`v4-stable` cannot move until an endpoint is chosen.** It requires one endpoint added to v4 without reshaping
an existing contract, and no such endpoint is planned. The only candidate is the `pack/first-bin` idea below.
Promote it, or pick another, or the flip has no path.

Ideas: [api/pack-first-bin-endpoint](ideas/api/pack-first-bin-endpoint.md) - **the candidate `v4-stable` needs.**
[api/openapi-spec-followups](ideas/api/openapi-spec-followups.md) - down to one item, the codegen doc page,
since the `servers` block landed on 2026-08-10. [api/packing-only-image](ideas/api/packing-only-image.md) ·
[api/reduce-integration-friction](ideas/api/reduce-integration-friction.md) - direction settled, nothing to
build. [api/uimodule-alpine-port](ideas/api/uimodule-alpine-port.md) - do it **after** the v4 migration above,
not before; porting a page you are about to rewrite is wasted work.

## ServiceModule

| Plan | State | Waiting on |
|---|---|---|
| [api/rate-limiting-owned-by-servicemodule](plans/api/rate-limiting-owned-by-servicemodule.md) | unplaced | maintainer to rank it |

Two one-liners in [todos](plans/todos.md): the rate-limit policy config review, and the raw
`ProblemDetails` on the no-body path.

**`rate-limiting-owned-by-servicemodule` was added 2026-08-13 and has not been ranked.** It moves
`.RequireRateLimiting("ApiUsage")` out of the 18 core endpoint files and lets the module attach it, which makes
the OpenAPI `429` structurally impossible in a module-off document instead of guard-dependent. The mechanism is
proven; it overlaps the simplification idea below, so the direction decision covers it too.

**It is downstream of a v3.0.0 fix, and the order matters.** The two-guard version of the `429` transformer
ships in v3.0.0 and the reasoning is locked in the API decisions ledger. This plan is what would let one of
those guards go away - so it is a follow-up to that fix, not a competitor to it. **Do not touch the transformer
before this plan is picked up**, or the ledger and the code disagree.

Five ideas that all pull in one direction and interact -
[servicemodule-simplification](ideas/api/servicemodule-simplification.md) ·
[schema-migrations](ideas/api/schema-migrations.md) · [refresh-token-endpoint](ideas/api/refresh-token-endpoint.md) ·
[admin-user-management-site](ideas/api/admin-user-management-site.md) ·
[per-user-packing-logs](ideas/api/per-user-packing-logs.md). The simplification would collapse the layering the
migration runner targets, and the admin site and refresh tokens build on the same auth surface. **Worth one
direction decision before any of them is picked up.**

Also unresolved: the **Azure Storage provider** has no dedicated sample, no CI coverage and no smoke profile
since `service-azure` was folded into `service`. Removal is an option nobody has written down.

## Lib and ViPaq

| Plan | State | Waiting on |
|---|---|---|
| [lib/parallel-processors-decision](plans/lib/parallel-processors-decision.md) | ready | - |
| [lib/benchmark-ledger](plans/lib/benchmark-ledger.md) | deferred | someone needing the numbers |

**`parallel-processors-decision` is a decision, not a build** - wire the threshold up or delete three
unreachable classes. Measure `ParallelBinProcessor` first either way; it is the one that was never measured.

**`benchmark-ledger` is deferred, not forgotten.** The committed numbers describe code that no longer exists
after the geometry migration - do not quote them until it is re-run.

Idea: [shared/extend-shared-models](ideas/shared/extend-shared-models.md) - parked leftovers from the
`Binacle.Geometry` extraction, with a recommendation to leave them alone.

## Tooling

| Plan | State | Waiting on |
|---|---|---|
| [image-module-stacks](plans/tooling/image-module-stacks.md) | ready | - |
| [scripts-to-just-recipes](plans/tooling/scripts-to-just-recipes.md) | ready | - |

Both are maintainer tooling - no user sees either, and nothing in CI calls them. `image-module-stacks` is a
decision about two compose stacks the smoke suite has made redundant. `scripts-to-just-recipes` is
discoverability only, and should not be allowed to grow past that. Neither is urgent; both are the kind of
thing that fills a day that should have gone somewhere else.

**`image-verification-recipes` left this table on 2026-08-14** and is release work. It never belonged here: the
recipe is a convenience, but the other half of that file is the only place that says how users get told the
images are signed at all, and an unverified signature is decoration. One of its five surfaces is the docs site,
which deploys once straight after the tag - that deadline is what made it pre-tag rather than later.

**It takes `image-module-stacks`'s decision with it.** The placement question was coupled to that plan; the
release now answers it. **Whoever picks up `image-module-stacks` reads where the verification recipes ended up
first**, rather than re-opening the question.

`scripts-to-just-recipes` also owns a comment one-liner: the ~40 restating lines in `tooling/tmux.sh`. Do them
together or not at all - if the script moves into a recipe body whole, the noise moves with it.
