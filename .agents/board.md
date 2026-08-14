---
description: The board - what there is to work on
---

# The board - what there is to work on

**This is where you pick work from.** Everything not tied to a release lives here: plans, ideas and the
one-liners. Open it, choose a row, open that file, start.

Permanent. Unlike the release set, this file is never deleted - versions come and go underneath it.

**Created 2026-08-07. Rewritten 2026-08-14** when the v3.0.0 scope was reset and six items moved between this
file and the release.

## How to use it

- **Themes first, readiness second.** Find the area you want to work in, then look at the `State` column.
- **`ready`** means nothing blocks it - it can start today. **`blocked`** names what it waits on. **`deferred`**
  means deliberately not now, and the row says what revives it. **`in progress`** means someone is on it.
- **A row is a pointer, never a container.** The plan or idea file holds the work. If you find yourself
  explaining the work here, it belongs in that file instead.
- **Ideas are listed, not tabled.** They are maybes; a maybe does not get a `State`. An idea reaches a table
  only by being promoted to a plan first.

## How agents maintain it

- **When a plan lands, its file is deleted - so tick the row and drop the link in the same change.** Otherwise
  the board rots into dead links, which is exactly what it exists to prevent.
- **Do not place, reorder or reprioritise a row on your own judgement. Ask.** Where something goes, how urgent
  it is and when it should happen are the maintainer's calls. Adding a row for work you were told to record is
  fine; deciding it is "ready" and putting it at the top is not.

## What is not here

**The active release.** v3.0.0 is in flight. Its plan holds the gates and its post-release companion holds the
checks that follow the tag. **Do not pull board work into the release.**

**Nine items are release work as of 2026-08-14** and are not to be started here: the rate limiter test
carve-out, rate limiting owned by the ServiceModule, image verification, the Docker Hub page, the Docker Hub
immutability **rule** (not the switch), the PR gate change and its three architecture checks, the docs deploy,
the client-generation page, and more ViPaq interop vectors. **Their plan files stay under `plans/`** - the
release takes a slice of most of them and leaves the rest, and a file is deleted only when nothing is left in
it.

**Two ideas are being consumed by the release** and their files get deleted when the work lands: the OpenAPI
follow-ups idea, which was down to the client-generation page, and the ViPaq interop vector coverage idea.

**Three things were held back from v3.0.0**, with the reasoning in the release plan's scope section: the heavy
architecture tools, CI gates 2 and 3, and raising test coverage. **All three are deferred rather than ready** -
each is waiting on something named in its row.

**Reference material is not work.** Docs, design and memory have their own indexes.

---

## The order - a recommendation, not a decision {#the-order}

**Rewritten 2026-08-14 after the release scope reset, and it is a recommendation only.** Placement and priority
are the maintainer's call; this exists to make that call cheap, not to make it. Strike any line of it.

**Everything below assumes v3.0.0 is tagged and the post-release list is clear.**

### First: one decision, and it is worth more than any build here

**The ServiceModule direction.** Five ideas pull in one direction and interact. The simplification would collapse
the layering the migration runner targets, and the admin site and refresh tokens build on the same auth surface.

One answer places five ideas, two one-liners and the Azure Storage removal question. **It is the
highest-leverage hour on the board**, and it is worth taking before any of them is picked up rather than after.

**v3.0.0 shipped the rate-limiting move without waiting for it.** That is not a precedent - that plan was
self-contained and proven, and shipping it first means the simplification inherits a smaller surface. The five
ideas below are not self-contained and do not get the same treatment.

### Then: the two other decisions, still no code

Each is one sitting and each unblocks a table.

1. **The v4 endpoint.** `v4-stable` cannot move without one endpoint added to v4 that reshapes no existing
   contract. `pack/first-bin` is the only candidate written down. Promote it, pick another, or accept that v4
   stays experimental past 3.1.0 - **all three are answers; having none is not.**
2. **Does anyone run this on ARM?** `multi-arch-images` is blocked on a question, not on work. If the answer is
   no, the useful action is writing that down as a decision and deleting the plan.

### Then: 3.1.0's content

**The v4 chain, in this order and no other** - `pack/first-bin` -> `v4-stable` -> `ui-clients-off-v3` -> the
UIModule Alpine port -> **then** the Blazor half of `ui-test-harness`. Each step needs the one before it.

**Two reasons the order is not negotiable.** Porting a page you are about to rewrite is wasted work, which is
why the port sits after the v4 migration. And **the port deletes most of what the Blazor tests would cover** -
writing them first means writing them twice, in two languages, where after the port it is once, in one.

### Running alongside, not queued behind

- **`ui-test-harness`, the TypeScript half only.** The Blazor half is queued behind the Alpine port above. The
  612 lines of TypeScript can be tested today and 79 of them - `cookies` and `theme-switcher` - are untouched by
  the port, so nothing written there gets rewritten. **This does not clear the coverage gate on its own** and was
  never going to.
- **The heavy architecture tools** - ArchUnitNET, dependency-cruiser, lychee. **Read the xunit pin trap in that
  row before starting**; it decides whether the first one is an afternoon or a week.
- **`integration-test-additions`, phase 1 only.** Read what the release built for the rate limiter tests first;
  it answers one of phase 1's four questions in code.
- **`sonar-issue-triage`.** Leftovers, one of which is a decision.
- **`parallel-processors-decision`.** A measurement and a decision, self-contained.

### Deliberately last

The image work - `image-base-slimming`, `multi-arch-images` if ARM is wanted - and the maintainer tooling. They
change the shipped artifact or nothing at all, and neither is worth doing in the weeks after a major release.

---

## Architecture and quality

| Plan | State | Waiting on |
|---|---|---|
| [architecture-boundaries](plans/architecture-boundaries.md) | **deferred** - the heavy tools only | a quiet week, not a release |
| [sonar-issue-triage](plans/sonar-issue-triage.md) | ready | - |

**Most of this plan is now v3.0.0 work.** `architecture.yml`, the 27 comment fixes and the restructure shipped
on the merged branch; **three checks ship with the PR gate change** - the comment check, a graph check that
re-derives every `ProjectReference` and compares it to the file, and an `InternalsVisibleTo` check. All three
read files the repo already has and land green.

**What is left here is the tooling that needs a new toolchain**, and that is the only reason it was held back.

- **ArchUnitNET.** **Lead with the two rules a graph walk can never see** - the api module boundary and
  v3-frozen - not the thirty slice edges that will be green forever. **Check its transitive xunit dependency
  first.** This repo pins `xunit.v3.mtp-v2` on purpose because mixing the MTP v1 and v2 adapters throws
  `TypeLoadException` before a test runs. If `.xUnitV3` pulls plain `xunit.v3`, the arch leaf reproduces that.
  **That is the trap that decides whether this is an afternoon or a week.** Also settle where the test project
  lives - it references every slice it inspects, so it becomes a node with an edge to everything, and that
  exemption belongs in `architecture.yml` rather than in the test.
- **dependency-cruiser.** "Reads the YAML" is the easy half. **There is no root `tsconfig.json`** - there are
  four, and `web/` has none despite running `ts-loader` - and imports are bare specifiers resolved through npm
  workspace symlinks. Rules must be written against resolved real paths with symlink handling pinned.
- **lychee** for dead links.

**One loose end that is not a check yet.** 19 global `Using` declarations across 13 projects have no matching
`ProjectReference`. Every one resolves transitively today, so they all compile - and every one breaks the day
the project it borrows from stops referencing what it borrows. **Whether the fix is 19 added references or a
decision that transitive resolution is fine here has never been settled**, which is why no check was built for
it. Settle it, then it is a cheap check.

**Say plainly which edges no tool will ever check** - the Gemfile `path:` gems, the webpack `splitChunks`
regexes, the path strings in just recipes, the gulp copy. They belong in the file so a person can read it, but
mark them documentation-only or a green run gets read as "all of this is checked".

**Do not go looking for comment sites to fix.** All 27 are already fixed.

**`sonar-issue-triage` is the 2026-08-09 sweep's leftovers.** The sweep is done; the file still carries the
CA1816 decision, the frozen-copies question and what the quality gate hangs on. **The gate hangs on
`new_coverage`, which is `ui-test-harness` under Testing** - the two rows are the same problem seen from two
directions.

## Testing

| Plan | State | Waiting on |
|---|---|---|
| [ui-test-harness](plans/ui-test-harness.md) | **TypeScript half ready. Blazor half blocked** | the Alpine port |
| [api/integration-test-additions](plans/api/integration-test-additions.md) | ready - phase 1 first | - |
| [shared/testskernel-data-extraction](plans/shared/testskernel-data-extraction.md) | ready | - |

**`ui-test-harness` is why the Sonar coverage gate is red**, and it is the honest answer to "raise test
coverage". The UI is the only code with no harness at all - 1571 lines at 0%, 22.5% of the coverage denominator.
Overall coverage reads 53%; without those four areas it would read about 68%. **Excluding them from coverage was
considered and rejected** - it moves the number without changing anything true.

**It split in two on 2026-08-14 and only one half is ready.**

- **The TypeScript packages, 612 lines - ready.** jest already runs here, so the runner is not the question; the
  DOM is. **Start with `cookies` and `theme-switcher`** - 79 lines, stable, and the Alpine port does not touch
  either.
- **The Blazor UIModule, 959 lines - blocked on the Alpine port.** The port deletes `PackingDemo.razor.cs`,
  `ProtocolDecoder.razor.cs`, `PackingVisualizer.razor.cs`, `BinacleVisualizerService` and `MessagingService`,
  and moves their logic into TypeScript that already exists. **Testing them first is writing the same tests
  twice in two languages.**

**The gate does not move until both halves are done**, and that is expected rather than a failure - 612 of 1571
lines does not clear an 80% new-code condition. **The port also removes one of this plan's own open questions:**
"bUnit or a browser for the demo page" only exists because the demo spans two stacks with a seam between them.
After the port there is one stack and no seam.

**`integration-test-additions` is two sessions.** Phase 1 investigates and stops; the maintainer picks the shape;
phase 2 writes the tests. **Do not run it as one job** - the plan says why.

**Its rate limiter cases went into v3.0.0 and are not to be built here.** The release takes one finding - that
nothing anywhere asserts a 429 ever happens - and covers the two limiters plus the auth throttle's partition.
**Everything else stays:** the four phase-1 questions, the module matrix, CORS, and the hunt for other core
behaviour that only works because an optional module registered something.

**Read the release's answer before phase 1 runs.** One of phase 1's four questions is "where do the rate-limit
tests live so a live limiter does not make everything else flaky", and the release answered it in code. Phase 1
inherits that answer instead of re-deciding it.

**Two of these three collide with the architecture work, and one collides with a pin.** The ArchUnitNET phase
adds a test leaf that references every slice it inspects, and `testskernel-data-extraction` reshapes the fixture
kernels the architecture branch just split - **whichever runs second reads the other's result.** The xunit runner
pin bites both.

Ideas: [mutation-testing](ideas/mutation-testing.md) - one contained experiment before any opinion.
[testing-techniques](ideas/testing-techniques.md) - a survey, nothing decided.

**`vipaq/interop-vector-coverage` went into v3.0.0 on 2026-08-14** and its file is deleted when the rows land.
The idea's own argument against - that a future wire-format change regenerates the vectors anyway - is spent,
because the format froze in this release.

## ServiceModule

**No plan rows here - the one that was is release work.** Rate limiting owned by the ServiceModule was pulled
into v3.0.0 on 2026-08-14. Its file stays under `plans/api/` until it lands.

Two one-liners in [todos](plans/todos.md): the rate-limit policy config review, and the raw `ProblemDetails` on
the no-body path.

**Five ideas that all pull in one direction and interact** -
[servicemodule-simplification](ideas/api/servicemodule-simplification.md) ·
[schema-migrations](ideas/api/schema-migrations.md) ·
[refresh-token-endpoint](ideas/api/refresh-token-endpoint.md) ·
[admin-user-management-site](ideas/api/admin-user-management-site.md) ·
[per-user-packing-logs](ideas/api/per-user-packing-logs.md). **Worth one direction decision before any of them
is picked up** - and that decision now also unblocks the plan above.

Also unresolved: the **Azure Storage provider** has no dedicated sample, no CI coverage and no smoke profile
since `service-azure` was folded into `service`. **Removal is an option nobody has written down.**

## CI and the release pipeline

| Plan | State | Waiting on |
|---|---|---|
| [ci-cd/ci-gates](plans/ci-cd/ci-gates.md) | **deferred** - gates 2 and 3 only | gate 2: the all-modules tests. gate 3: the UI harness |
| [ci-cd/release-pipeline-rebuild](plans/ci-cd/release-pipeline-rebuild.md) | **nearly done** | one docs decision, held in the release plan |
| [ci-cd/dockerhub-tag-immutability](plans/ci-cd/dockerhub-tag-immutability.md) | blocked until after v3.0.0 | a shipped release behind the rule |
| [ci-cd/multi-arch-images](plans/ci-cd/multi-arch-images.md) | **not scheduled** | does anyone run it on ARM? |
| [image-base-slimming](plans/image-base-slimming.md) | ready - **timing not decided** | - |

**`ci-gates` lost gate 1 to v3.0.0 and the other two were deferred on 2026-08-14.** The image-build gate ships
with the release's PR gate change, alongside the OpenAPI lint one-liner and three architecture checks - all in
one workflow edit, all green on arrival.

**Both remaining gates are deferred because neither has anything to gate yet**, not because they are unwanted.

- **Gate 2** runs the integration suites with all modules on. Those tests are `integration-test-additions` under
  Testing and they are not written. **Gate 2 follows that plan, not the other way round.**
- **Gate 3** puts Sonar and coverage on the PR gate. Its coverage condition asks 80% on new code, the project
  runs the read-only "Sonar way" gate, and custom gates start at a paid plan - so it is a number to meet or
  fail, not one to argue with. **It is red before anyone writes a line**, and a gate like that gets waived
  within a week. **Set the floor only after the UI harness lands, and from a run that has settled.**

**`release-pipeline-rebuild` is one decision from deletable.** The pipeline is built and proven end to end by
beta 2. What is left is the question of whether the docs-site release-notes page is generated from
`CHANGELOG.md` or stays hand-copied - **a docs decision, so it cannot be settled in a coding session**, and the
release plan is holding it for handover. Plus one post-release check: whether the deployment host pulls without
a stored credential.

**`dockerhub-tag-immutability` keeps the switch; v3.0.0 takes the rule.** The rule is corrected before the tag
because the current `".*"` would freeze `latest` and `3.0`, the two tags designed to move. **The switch waits
for a release to have shipped behind the corrected rule** - and the post-release list carries the decision
point. **There is no undo:** an immutable tag cannot be deleted either, so a release tag pushed by mistake is
permanent. Test on a scratch repo, never the live one.

**`multi-arch-images` is blocked on a question, not on work.** The published image is `linux/amd64` only. **If
nobody runs Binacle.Net on ARM, that is a defensible choice** and the useful action is writing it down as a
decision rather than building anything.

**`image-base-slimming` is about the shipped artifact.** Its opening finding landed in v3.0.0 - dropping
`--self-contained` took the image from 150.2 MB to 103.2 MB. The file is now about the base itself, which is
~90% of what remains: chiseled, and whether Docker Hardened Images earn their subscription. **Not scheduled.**

One-liner, in [todos](plans/todos.md): the `Dockerfile` comment that says "from the 'build' stage" when **there
is no build stage** - the publish happens in `just build publish`, and the path is hardcoded and allowlisted in
`.dockerignore`, so publishing elsewhere builds an empty image.

## API

| Plan | State | Waiting on |
|---|---|---|
| [api/ui-clients-off-v3](plans/api/ui-clients-off-v3.md) | ready | - |
| [api/v4-stable](plans/api/v4-stable.md) | blocked | no endpoint chosen |

**`api/openapi-spec-followups` went into v3.0.0 on 2026-08-14.** It was down to one item - the
client-generation page - and its file is deleted when that page ships. The page covers **every** published
version, not just v3.0.x.

**`v4-stable` cannot move until an endpoint is chosen.** It requires one endpoint added to v4 without reshaping
an existing contract, and no such endpoint is planned. The only candidate is `pack/first-bin` below. **Promote
it, pick another, or the flip has no path.** v4 stays experimental in v3.0.0 by design; the flip is 3.1.0 work.

Ideas: [api/pack-first-bin-endpoint](ideas/api/pack-first-bin-endpoint.md) - **the candidate `v4-stable`
needs.** [api/packing-only-image](ideas/api/packing-only-image.md) ·
[api/reduce-integration-friction](ideas/api/reduce-integration-friction.md) - direction settled, nothing to
build. [api/uimodule-alpine-port](ideas/api/uimodule-alpine-port.md) - do it **after** the v4 migration above,
not before.

## Lib and ViPaq

| Plan | State | Waiting on |
|---|---|---|
| [lib/parallel-processors-decision](plans/lib/parallel-processors-decision.md) | ready | - |
| [lib/benchmark-ledger](plans/lib/benchmark-ledger.md) | deferred | someone needing the numbers |

**`parallel-processors-decision` is a decision, not a build** - wire the threshold up or delete three
unreachable classes. **Measure `ParallelBinProcessor` first either way**; it is the one that was never measured.

**`benchmark-ledger` is deferred, not forgotten.** The committed numbers describe code that no longer exists
after the geometry migration - **do not quote them until it is re-run.**

Idea: [shared/extend-shared-models](ideas/shared/extend-shared-models.md) - parked leftovers from the
`Binacle.Geometry` extraction, with a recommendation to leave them alone.

## Tooling

| Plan | State | Waiting on |
|---|---|---|
| [tooling/image-module-stacks](plans/tooling/image-module-stacks.md) | ready | - |
| [tooling/scripts-to-just-recipes](plans/tooling/scripts-to-just-recipes.md) | ready | - |

Both are maintainer tooling - no user sees either, and nothing in CI calls them. Neither is urgent; **both are
the kind of thing that fills a day that should have gone somewhere else.**

**`image-module-stacks` lost its coupling on 2026-08-14.** It asks what the `image` module is still for, and the
verification-recipe placement question used to be tangled with it. **The release settles that placement
independently**, so this plan is now free-standing. **Whoever picks it up reads where the verification recipes
ended up first**, rather than re-opening the question.

**`scripts-to-just-recipes` is discoverability only, and should not be allowed to grow past that.** It owns a
comment one-liner: the ~40 restating lines in `tooling/tmux.sh`, plus two banners that name the wrong window.
**Do them together or not at all** - if the script moves into a recipe body whole, the noise moves with it.
