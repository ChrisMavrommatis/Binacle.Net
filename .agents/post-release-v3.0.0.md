# Post-release - after Binacle.Net v3.0.0

**Status:** Do these once v3.0.0 is out. None gate the release. Like the release plan, this coordinates other
files and nothing points back at it. Delete it once the list is clear.

**The cap.** Only work with an immediate benefit belongs here - something with a real answer to "by when", which
means the 3.0.x line or 3.1.0. Everything else stays in `ideas/` and is found through `ideas/_index.md`. If this
file turns into a second backlog it never gets deleted, and the release set never closes.

**Same index rule as the release plan.** When a plan lands its file is deleted - tick the row and drop the link
in the same change, leaving the text.

---

## In the 3.0.x line

| # | Item | Plan | Why now |
|---|---|---|---|
| 1 | One set of scripts, run by both CI and a human | [ci-shared-scripts](plans/ci-shared-scripts.md) | Everything else on the CI list gets easier once this lands. Do it first. |
| 2 | Build the docker image on every PR | [ci-docker-image-gate](plans/ci-docker-image-gate.md) | This release shipped with an image nobody had built since the restructure. Do not repeat it. |
| 3 | Run the integration tests with all modules enabled | [ci-all-modules-integration-tests](plans/ci-all-modules-integration-tests.md) | Core-only means the gate is green without being meaningful. |
| 4 | Put Sonar and coverage on the PR gate | [ci-sonar-coverage-gate](plans/ci-sonar-coverage-gate.md) | Configured, never enforced. |
| 5 | Give the build a version of its own | [version-stamp](plans/version-stamp.md) | Nothing but the docker tag knows which build it is - the first support question exposes it. |
| 6 | Decide whether the TS packages get published | [npm-package-publishing](plans/npm-package-publishing.md) | The release breaks every old token and `binacle-vipaq` is the reference decoder. A JS user has no supported way to get it. |
| 7 | Migrate the shipped UI clients off v3 | [api/ui-clients-off-v3](plans/api/ui-clients-off-v3.md) | Not urgent - v3 is frozen and they keep working. It is also the v4 adoption that 3.1.0 needs. |
| 8 | The small TODOs and the stale lockfile entry | [todos](plans/todos.md) | Cheap, and the lockfile one has already cost a session once. |

## Toward 3.1.0

| # | Item | Plan | Why now |
|---|---|---|---|
| 9 | Flip v4 from experimental to stable | [api/v4-stable-in-3.1.0](plans/api/v4-stable-in-3.1.0.md) | Needs the beta to have run and one endpoint added without reshaping an existing contract. |
| 10 | Decide what happens to the three `Parallel*` processors | [lib/parallel-processors-decision](plans/lib/parallel-processors-decision.md) | Three unreachable classes and two factory parameters that do nothing. |
| 11 | Refresh the curated lib benchmark ledger | [lib/benchmark-ledger](plans/lib/benchmark-ledger.md) | The committed numbers describe code that no longer exists after the geometry migration. Do not quote them until then. |
| 12 | Grow the shared TestsKernel fixture cases | [shared/testskernel-data-extraction](plans/shared/testskernel-data-extraction.md) | Result selection has one baseline per case, and ViPaq's compression crossover is still provisional. |

## Ideas worth a look once this is out

The one place a release file may point at `ideas/`. These are unvetted and unscheduled - listed because the
release makes each one more interesting than it was, not because they are committed.

- [ideas/api/smoke-testing-the-image.md](ideas/api/smoke-testing-the-image.md) - the beta was the first time the
  image ran anywhere, by hand. A smoke test is how that stops being a manual ritual.
- [ideas/api/openapi-spec-followups.md](ideas/api/openapi-spec-followups.md) - v4 is experimental for the whole
  3.0.x line, so spec changes are cheapest right now.
- [ideas/api/reduce-integration-friction.md](ideas/api/reduce-integration-friction.md) - this release asks every
  integrator to change something. Worth reading their side of it while that is fresh.
- [ideas/vipaq/interop-vector-coverage.md](ideas/vipaq/interop-vector-coverage.md) - the format is newly frozen
  and the C#/TS pair is the only thing holding it together.
- [ideas/api/pack-first-bin-endpoint.md](ideas/api/pack-first-bin-endpoint.md) - a candidate for the "one
  endpoint added without reshaping a contract" that v4 needs before it can be called stable.

Everything else lives in `ideas/_index.md`. The ServiceModule Azure provider removal, hinted at during this
release, has no plan and no version - it is an idea until somebody writes one.
