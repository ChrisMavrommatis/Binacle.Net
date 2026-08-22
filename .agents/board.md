---
description: The board - what there is to work on
---

# The board - what there is to work on

**This is where you pick work from.** Everything not tied to a release lives here: plans, ideas and the
one-liners. Open it, choose a row, open that file, start.

Permanent. Unlike the release set, this file is never deleted - versions come and go underneath it.

## How to use it

- **Themes first, readiness second.** Find the area you want to work in, then look at the `State` column.
- **`ready`** means nothing blocks it - it can start today. **`blocked`** names what it waits on. **`deferred`**
  means deliberately not now, and the row says what revives it. **`in progress`** means someone is on it.
- **A row is a pointer, never a container.** The plan or idea file holds the work. If you find yourself
  explaining the work here, it belongs in that file instead.
- **Ideas are listed, not tabled.** They are maybes; a maybe does not get a `State`. An idea reaches a table
  only by being promoted to a plan first.

`rules/the-board-and-the-release-set.md` holds how this file is kept and who decides what goes on it.

## What is not here

**The active release.** v3.0.0 is in flight; its plan holds the gates and its post-release companion holds the
checks that follow the tag. **Do not pull board work into the release, and do not start a release item here.**
Their plan files stay under `plans/` - the release takes a slice of most of them and leaves the rest.

**Reference material is not work.** Docs, design and memory have their own indexes.

---

## The order - a recommendation, not a decision {#the-order}

**A recommendation only.** Placement and priority are the maintainer's call; this exists to make that call
cheap, not to make it. Strike any line of it. **Everything below assumes v3.0.0 is tagged and the post-release
list is clear.**

### First: three decisions, no code

1. **The ServiceModule direction.** Five ideas pull one way and interact - one answer places all five, two
   one-liners and the Azure Storage question. **The highest-leverage hour on the board**, and worth taking
   before any of them is picked up rather than after.
2. **The v4 endpoint.** `v4-stable` cannot move without one endpoint added to v4 that reshapes no existing
   contract. `pack/first-bin` is the only candidate written down. Promote it, pick another, or accept that v4
   stays experimental past 3.1.0 - **all three are answers; having none is not.**
3. **Does anyone run this on ARM?** `multi-arch-images` is blocked on a question, not on work. If the answer is
   no, the useful action is writing that down as a decision and deleting the plan.

### Then: 3.1.0's content

**The v4 chain, in this order and no other** - `pack/first-bin` -> `v4-stable` -> `ui-clients-off-v3`. Each
step needs the one before it: there is nothing to migrate the clients to until v4 has the endpoint.

**The UIModule rebuild landed on 21-22 Aug 2026 and is out of this chain.** Blazor is gone, both demos come
from `packages/binacle-net-ui`, and the module is Razor Pages. It used to sit between `ui-clients-off-v3` and
the tests, and both reasons for that are spent - there is no page left to rewrite twice, and there is no
Blazor to test. **It shipped with the v3 call still hardcoded**, so `ui-clients-off-v3` is unaffected by it and
still has exactly one call site.

### Running alongside, not queued behind

`ui-test-harness` · `architecture-checks` · `comment-lint` ·
`integration-test-additions` (phase 1 only) · `sonar-issue-triage` · `parallel-processors-decision`.

### Deliberately last

The image work - `image-base-slimming`, `multi-arch-images` if ARM is wanted - and the maintainer tooling. They
change the shipped artifact or nothing at all, and neither is worth doing in the weeks after a major release.

---

## Architecture and quality

| Plan | State | Waiting on |
|---|---|---|
| [architecture-checks](plans/architecture-checks.md) | ready - **state chosen by an agent, strike it if wrong** | - |
| [comment-lint](plans/comment-lint.md) | ready - **state chosen by an agent, strike it if wrong** | - |
| [sonar-issue-triage](plans/sonar-issue-triage.md) | ready | - |

**Ordering, and it is the only thing this section decides.** `architecture-checks` and
`testskernel-data-extraction` under Testing collide - **whichever runs second reads the other's result** - and
the xunit runner pin bites both. Only the heavy-tool half of `architecture-checks` is affected; its generator
and ruleset collide with neither. **`sonar-issue-triage`'s quality gate hangs on `new_coverage`, which is
`ui-test-harness` under Testing** - the same problem seen from two directions.

## Testing

| Plan | State | Waiting on |
|---|---|---|
| [ui-test-harness](plans/ui-test-harness.md) | **harness built. One verification left** | a Sonar run |
| [api/integration-test-additions](plans/api/integration-test-additions.md) | ready - phase 1 first | - |
| [shared/testskernel-data-extraction](plans/shared/testskernel-data-extraction.md) | ready | - |

**`ui-test-harness` is why the Sonar coverage gate is red.** All four suites landed on 2026-08-22; what is
left is that nobody has dispatched a Sonar run since, so nobody has seen the coverage arrive there. **The
state and blocker in that row were written by an agent from what is in the tree** - strike them if the call
is different.

Ideas: [mutation-testing](ideas/mutation-testing.md) · [testing-techniques](ideas/testing-techniques.md).

## ServiceModule

**No plan rows.** Two one-liners in [todos](plans/todos.md): the rate-limit policy config review, and the raw
`ProblemDetails` on the no-body path.

**Five ideas that all pull in one direction and interact** -
[servicemodule-simplification](ideas/api/servicemodule-simplification.md) ·
[schema-migrations](ideas/api/schema-migrations.md) ·
[refresh-token-endpoint](ideas/api/refresh-token-endpoint.md) ·
[admin-user-management-site](ideas/api/admin-user-management-site.md) ·
[per-user-packing-logs](ideas/api/per-user-packing-logs.md). **Worth one direction decision before any of them
is picked up.**

Also unresolved: the **Azure Storage provider** has no dedicated sample, no CI coverage and no smoke profile
since `service-azure` was folded into `service`. **Removal is an option nobody has written down.**

## CI and the release pipeline

| Plan | State | Waiting on |
|---|---|---|
| [ci-cd/workflow-restructure](plans/ci-cd/workflow-restructure.md) | **built; one hand item and one gap left** - state chosen by an agent, strike it if wrong | branch protection |
| [ci-cd/ci-gates](plans/ci-cd/ci-gates.md) | **deferred** - gates 2 and 3 only | gate 2: the all-modules tests. gate 3: the UI harness |
| [ci-cd/dockerhub-overview](plans/ci-cd/dockerhub-overview.md) | **the logo and the categories only** - the release takes the rest | - |
| [ci-cd/dockerhub-tag-immutability](plans/ci-cd/dockerhub-tag-immutability.md) | **the switch only** - the release takes the rule | a shipped release behind the rule |
| [ci-cd/multi-arch-images](plans/ci-cd/multi-arch-images.md) | **not scheduled** | does anyone run it on ARM? |
| [image-base-slimming](plans/image-base-slimming.md) | ready - **timing not decided** | - |

**`release-pipeline-rebuild` is gone - the plan was finished and deleted on 2026-08-20.** What outlives it is
in the CI/CD docs and the decisions ledger; the moving-tag gap it tracked closes on the v3.0.0 release itself.

**Both remaining CI gates are deferred because neither has anything to gate yet**, not because they are
unwanted. **Gate 2 follows `integration-test-additions`, not the other way round.** Gate 3's coverage floor is
set only after the UI harness lands, from a run that has settled - set before that, it is red on arrival and
gets waived within a week.

One-liner, in [todos](plans/todos.md): the `Dockerfile` comment that says "from the 'build' stage" when there
is no build stage.

## API

| Plan | State | Waiting on |
|---|---|---|
| [api/ui-clients-off-v3](plans/api/ui-clients-off-v3.md) | **module half ready; site half blocked** | the site half waits on `api.binacle.net` serving v4 |
| [api/uimodule-instance-presets](plans/api/uimodule-instance-presets.md) | ready - one design question first | - |
| [api/v4-stable](plans/api/v4-stable.md) | blocked | no endpoint chosen |

**`ui-clients-off-v3` is two halves and only one is ready.** The demo inside the image calls its own
instance on a relative URL and is unaffected. **The demo site's copy calls `api.binacle.net`, which serves
image `2.1.1` and answers 404 on v4** - probed 22 Aug 2026. Moving that copy to v4 breaks the live demo until
that host serves a v3.0.x image. The module half can go today.

**`uimodule-instance-presets` came in from outside the repository on 22 Aug 2026.** It touches no site, no
host and no published word, so it was never anything but repository work.

Ideas: [api/pack-first-bin-endpoint](ideas/api/pack-first-bin-endpoint.md) - **the candidate `v4-stable`
needs.** [api/show-me-the-request](ideas/api/show-me-the-request.md) - the demo prints the call it just made ·
[api/packing-only-image](ideas/api/packing-only-image.md) ·
[api/reduce-integration-friction](ideas/api/reduce-integration-friction.md) - direction settled, nothing to
build.

## Lib and ViPaq

| Plan | State | Waiting on |
|---|---|---|
| [lib/parallel-processors-decision](plans/lib/parallel-processors-decision.md) | ready | - |
| [lib/benchmark-ledger](plans/lib/benchmark-ledger.md) | deferred | someone needing the numbers |

Idea: [shared/extend-shared-models](ideas/shared/extend-shared-models.md).

## Tooling

| Plan | State | Waiting on |
|---|---|---|
| [tooling/scripts-to-just-recipes](plans/tooling/scripts-to-just-recipes.md) | ready | - |

Maintainer tooling - no user sees it, and nothing in CI calls it. Not urgent; **it is the kind of
thing that fills a day that should have gone somewhere else.**
