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

  **`sonar-issue-triage` has never had a row here**, and it should: the sweep is done but it still carries the
  CA1816 decision, the frozen-copies question and what the quality gate hangs on. **Placement and priority are
  the maintainer's call**, so it has not been put in a table.
- **Reference material.** Docs, design and memory are not work. Find them through their own indexes.

---

## CI and the release pipeline

| Plan | State | Waiting on |
|---|---|---|
| [ci-cd/multi-arch-images](plans/ci-cd/multi-arch-images.md) | **not scheduled** | does anyone run it on ARM? |
| [ci-gates](plans/ci-cd/ci-gates.md) | ready - **was blocked, unblocked 2026-08-11** | - |
| [ci-cd/release-pipeline-rebuild](plans/ci-cd/release-pipeline-rebuild.md) | **in progress - pipeline landed** | the GHCR visibility flip, after beta 2 |
| [image-base-slimming](plans/image-base-slimming.md) | ready - **timing not decided** | - |

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

**`release-pipeline-rebuild` landed on 2026-08-11**, before v3.0.0 rather than after it - the maintainer's
call, which makes beta 2 the first run of the new pipeline. The workflows, `CHANGELOG.md`, the changelog
module and dependabot are all in the working tree. What is left in the file is the GHCR visibility flip (which
can only happen after the first run creates the package), two open questions, and telling users how to verify
the signature. **The v3.0.0 release plan owns the scheduling from here.**

One-liner, in [todos](plans/todos.md): **lint the OpenAPI documents on every PR.** Ready, and no longer
blocked - the `servers` block landed on 2026-08-10 and the lint is clean, so the gate can fail on warnings
from day one.

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
[api/openapi-spec-followups](ideas/api/openapi-spec-followups.md) - the `servers` block has a decided shape and
is really scheduled work sitting in an idea file. [api/packing-only-image](ideas/api/packing-only-image.md) ·
[api/reduce-integration-friction](ideas/api/reduce-integration-friction.md) - direction settled, nothing to
build. [api/uimodule-alpine-port](ideas/api/uimodule-alpine-port.md) - do it **after** the v4 migration above,
not before; porting a page you are about to rewrite is wasted work.

## ServiceModule

Nothing planned. Two one-liners in [todos](plans/todos.md): the rate-limit policy config review, and the raw
`ProblemDetails` on the no-body path.

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
| [image-module-stacks](plans/config/image-module-stacks.md) | ready | - |
| [scripts-to-just-recipes](plans/config/scripts-to-just-recipes.md) | ready | - |

Both are maintainer tooling - no user sees either, and nothing in CI calls them. `image-module-stacks` is a
decision about two compose stacks the smoke suite has made redundant. `scripts-to-just-recipes` is
discoverability only, and should not be allowed to grow past that.
