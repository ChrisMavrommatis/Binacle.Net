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
| 2 | Make the PR gate mean something - image build, all-modules integration tests, Sonar and coverage | [ci-gates](plans/ci-gates.md) | Three gates folded into one plan on 2026-07-28. The image is never built on a PR, the suites run core modules only, and Sonar runs by hand. |
| 3 | Migrate the shipped UI clients off v3 | [api/ui-clients-off-v3](plans/api/ui-clients-off-v3.md) | Not urgent - v3 is frozen and they keep working. It is also the v4 adoption that 3.1.0 needs. |
| 4 | Two small ServiceModule TODOs | [todos](plans/todos.md) | Cheap. The stale lockfile entry that used to sit here was cleared on 2026-07-28. |

## Toward 3.1.0

| # | Item | Plan | Why now |
|---|---|---|---|
| 5 | Flip v4 from experimental to stable | [api/v4-stable-in-3.1.0](plans/api/v4-stable-in-3.1.0.md) | Needs the beta to have run and one endpoint added without reshaping an existing contract. |
| 6 | Decide what happens to the three `Parallel*` processors | [lib/parallel-processors-decision](plans/lib/parallel-processors-decision.md) | Three unreachable classes and two factory parameters that do nothing. |
| 7 | Refresh the curated lib benchmark ledger | [lib/benchmark-ledger](plans/lib/benchmark-ledger.md) | The committed numbers describe code that no longer exists after the geometry migration. Do not quote them until then. |
| 8 | Grow the shared TestsKernel fixture cases | [shared/testskernel-data-extraction](plans/shared/testskernel-data-extraction.md) | Result selection has one baseline per case, and ViPaq's compression crossover is still provisional. |

## Ideas worth a look once this is out

The one place a release file may point at `ideas/`. These are unvetted and unscheduled - listed because the
release makes each one more interesting than it was, not because they are committed.

- [plans/api/smoke-testing-the-image.md](plans/api/smoke-testing-the-image.md) - the beta was the first time the
  image ran anywhere, by hand, and v3.0.0's verification was manual again. A smoke test is how that stops being
  a ritual. **Note this one is a plan, not an idea** - it is designed, with the OCI label prerequisite already
  done, and only the suite is unbuilt. The link here said `ideas/` and pointed at nothing (fixed 2026-08-06).
  Being designed rather than unvetted, it arguably belongs in the numbered table above rather than in this
  section - that is a scheduling call, not a filing one.
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
