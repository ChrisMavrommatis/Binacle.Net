---
description: CI - make the PR gate mean something
paths:
  - ".github/workflows/**"
---

# CI - make the PR gate mean something

**Status:** **Gate 1 landed on 2026-08-18, with the workflow shape and the `gate` job.**
`pull-request.yml` runs `changes` -> (`test-suite`, `image`) -> `gate`, `shared-test-suite.yml` lost its
`pull_request` trigger now that a caller exists, and `release-docker-image.yml` was not touched.
**Branch protection has not been updated and must be** - the one name to require is `Pull Request / Gate`.
**Gates 2 and 3 are what is left**, and both are deferred rather than ready: gate 2 waits on the all-modules
integration tests, gate 3 on the UI test harness that would make a coverage floor honest.

Folded on 2026-07-28 from `ci-docker-image-gate`,
`ci-all-modules-integration-tests` and `ci-sonar-coverage-gate` - they share a trigger, a workflow and an
ordering, so the shared parts were being written three times.

Three gates, one story: **the PR gate is green without proving much.** The image is never built, the integration
suites run core modules only, and Sonar runs when somebody remembers. Each gate below closes one of those.

**Two gates moved out on 2026-08-07, and this file is back to the three it folded.** Linting the OpenAPI
documents was one workflow step with a known answer, so it went out as a one-liner - and it shipped on
2026-08-17 as a step in `shared-test-suite.yml`. Smoking the image runs in the release workflow rather than on a PR, so
it moved to the release-pipeline work, which owned that file.
Neither shared the checkout, the ordering or the runtime budget below - that is what made this file too big to
finish in one sitting.

## What they share

- **Nothing blocks these any more - changed 2026-08-11.** Every gate here calls a recipe rather than inlining
  its own commands, and the release pipeline now builds through `just build publish` too. The wiring these
  gates used to wait on has landed, so the image and smoke gates would prove exactly what they claim to.
- **One job or three? Answered - see the section below.** The gate becomes its own workflow that calls
  `shared-test-suite.yml`, rather than more steps inside it. The answer holds for all three gates, which is most of why
  these were folded into one file.
- **Runtime is the shared budget.** The integration suite is already the long pole, and all-modules plus coverage
  both make it longer. Whatever ordering is chosen, know the total before adding the third gate.
- **A gate that does not match what ships proves nothing.** That is the same failure in all three: the release
  Dockerfile arguments, the shipped module set, and the coverage floor all have to be the real ones.

## The gate is its own workflow, not more steps in `shared-test-suite.yml`

**Decided 2026-08-17.** `shared-test-suite.yml` is what the release workflow calls as its "this commit passed CI" gate,
and it takes no inputs on purpose. **So every step added to that file is a step the release pays for.** The
image build is the plain case: the release would build a throwaway image inside the gate and the real one two
minutes later in its own build job, for nothing.

The PR entry point therefore becomes a **new workflow that calls `shared-test-suite.yml`**, and the release keeps
calling `shared-test-suite.yml` directly and gets exactly the suite.

- **The new workflow runs `on: pull_request`, with parallel jobs.**
  - **The architecture checks** - checkout, node, `npm ci`, nothing else. Under a minute, and no SDK.
  - **The test suite** - `uses: ./.github/workflows/shared-test-suite.yml`, unchanged.
  - **The image build.** **The OpenAPI lint was going to sit beside it** - both need the API project built, so
    one checkout and one restore would have covered both. It went into `shared-test-suite.yml` as a plain step on
    2026-08-17 instead, at the maintainer's call, so the release gets it too. **That does not carry to the
    image build:** the reason the image must stay out of that file is a duplicated build, not a preference.
- **`shared-test-suite.yml` loses its `pull_request` trigger.** It keeps `workflow_call` and
  `workflow_dispatch`, and nothing else in it changes. **That trigger is still there on purpose** - the
  restructure kept it because dropping it before a caller existed would have left pull requests with no gate at
  all. **Removing it is this change's job, and it only becomes safe once the new workflow calls the file.**
- **`release-docker-image.yml` is untouched.**

**The naming rework landed with the restructure on 2026-08-17**, so what is left here is the new file's own
name and its job names - not the whole set. A called workflow still reports as
`<caller> / <job> / <job in the callee>`, which is the string a reader sees on a red check, so the new
workflow's job names have to be chosen against that whole string rather than on their own.

**Two traps.**

- **Branch protection breaks quietly.** The required status check names change with the job names. A required
  check that no longer reports leaves every PR waiting on it forever, with nothing saying why. Update
  protection in the same sitting as the split.
- **Setup is paid per job.** One job is exactly why `shared-test-suite.yml` looks the way it does, and its header says
  so. Three jobs means three checkouts and two SDK setups. Wall clock still comes down, because the image build
  stops queueing behind the integration suite - **but measure it rather than assume it.**

**What the release stops seeing** is the architecture checks. Everything on `main` went through the gate to get
there, so that is a choice, not an oversight. Say it out loud in the workflow comment.

### One `gate` job is the required check, not the jobs themselves

**This changes the job list above, which is why it is here rather than beside it.** Two problems have the same
answer.

**The first is skipping.** Roughly two thirds of recent commits touch only agent guidance, the two sites or
markdown - measured at 38 of the last 60 - and every one of them currently earns a full suite with a postgres
and an azurite container. **The obvious fix is a trap:** `on: pull_request` with `paths-ignore` means the
workflow does not trigger at all, so a required check never reports and the pull request waits on it forever.

**The second is renaming.** Required check names are job names, so the naming rework above breaks branch
protection by definition.

**Both go away with a job that always runs and always reports.**

- **`changes`** - always runs, no filter, a few seconds. Works out whether anything outside guidance and the
  sites moved, and says so as an output.
- **The expensive jobs** - `needs: changes`, with an `if:` on that output. They are *skipped* rather than
  failed when nothing relevant changed.
- **`gate`** - `needs:` all of them, `if: always()`. Fails if any dependency failed or was cancelled, passes if
  every one succeeded or was skipped.

**`gate` is the only name in branch protection.** It reports on every pull request whether the work ran or not,
and the jobs underneath can be renamed and restructured freely afterwards - which retires the branch
protection trap above rather than just working around it once.

**Get the `if: always()` condition right or the gate is decoration.** A bare `if: always()` with no result check
passes while its dependencies are red, which is a required check that can never fail. It has to read each
dependency's result explicitly and treat only `success` and `skipped` as acceptable.

## Gate 1 - build the docker image on every PR

The image is built in CI only when a release is published (`.github/workflows/release-docker-image.yml`). So a PR
never proves the image still builds, and a break is found at release time - which is exactly what happened after
the `Binacle.Geometry` extraction, where the image had not been built for the whole restructure. `shared-test-suite.yml`
builds the solution and runs every suite on each PR; it does not build the image.

- Add an image build step to the PR gate. Build only - no push, no login, no Docker Hub credentials on a PR.
- Use the same Dockerfile and the same publish arguments the release workflow uses, or the gate proves nothing.

**Unblocked, and fully so as of 2026-08-11.** The gate step is `just build image` (`tooling/build.just`), which
publishes and builds with no push, no `sudo` and nothing interactive. The release pipeline now calls
`just build publish` as well, so this gate proves the release path builds rather than just the recipe.

## Gate 2 - run the integration tests with all modules enabled

The integration harnesses run **core modules only**, so every module combination the image ships is untested
end to end. Writing those tests is its own plan - the one on integration test additions - and it owns the
decisions there: one run with everything on or a matrix, where the rate-limit tests live, and what breaks when
the modules go on.

What belongs to **this** plan is only the gate: once those tests exist, the leaves they add run on every PR
like the rest. If the answer turns out to be a matrix, the runtime budget below is what decides how wide it can
be.

## Gate 3 - put Sonar and coverage on the PR gate

Sonar and coverage are configured but never enforced. `.github/workflows/sonar-analysis.yml` is
`workflow_dispatch` only, so analysis happens when somebody remembers, which is never on the PR that introduced
the problem.

- Run Sonar analysis and coverage reporting on every PR.
- Decide the gate: which suites must pass, and the coverage floor. A floor nobody agreed on gets waived the first
  time it blocks something, so pick a number that is true today and ratchet it.
- Keep Automatic Analysis OFF. Coverage needs a CI run - Automatic Analysis only reads source, and the two fight.

### The coverage condition fails on purpose {#coverage-red-on-purpose}

**Decided 2026-08-09.** The project runs the built-in "Sonar way" gate, which asks for 80% coverage on new code.
It is read-only, and on the **Free plan there is no way around that**: custom quality gates start at the Team
plan, so 80% is not a number we can argue with, only one we can meet or fail. It is being failed - the
2026-08-08 run reads **53.3% overall, 31.4% on new code**, the only failing condition on the gate.

The gap is almost entirely one thing. Four areas sit at **exactly 0% coverage** - the Blazor `UIModule` (959
lines), and the `binacle-net-ui`, `cookies` and `theme-switcher` TS packages (612 between them). That is 1571
lines, 22.5% of the whole coverage denominator. Without them the project reads about 68%.

Excluding those four from coverage was considered and **rejected**: it moves the number without changing
anything true. The coverage condition therefore stays red until the UI has a test harness, which is its own plan
(the one on a UI test harness) rather than a line in the analysis xml. A red gate here means "the UI is
untested", which is exactly what is true.

Two consequences for this gate:

- **Do not make coverage blocking on the PR gate yet.** A condition that is red before anyone writes a line
  blocks every PR for a reason none of them caused, and gets waived within a week - the same failure this plan
  already warns about for a floor nobody agreed on.
- **When the floor is finally set, set it from a run that has settled**, and after the UI harness lands. The
  2026-08-09 numbers are a correction to what was being measured, not work anyone did.

### The two published sites are back in scope {#sites-in-scope}

**Changed 2026-08-09.** `sonar.exclusions` used to drop `docs/**` and `web/**` whole. The reason given was that
they are a separate deliverable with their own session - a workflow reason, not a scope reason, and the test in
the analysis xml is whether the code is ours to author, review and change. It is. The cost was concentrated
exactly where it hurt: those two Jekyll sites are the only public attack surface in the repo, `5e5f8c02` was an
XSS fix in one of them, and the exclusion kept Sonar from looking for the next one.

What is in scope now is small - 6 hand-written js, 15 scss, and the site yml and json. The generated and
vendored parts (`docs/js`, `web/js`, `docs/lib`, `web/lib`, the two `media` folders) are named individually and
stay out; they are gitignored, so a CI checkout would not see them anyway. `docs/**/*.html` and `web/**/*.html`
stay out too, because a Jekyll template with `---` front matter and Liquid in its attributes is not an HTML
document and Sonar's HTML analyser can only misread it.

**Findings under `docs/` and `web/` are not fixed in a coding session.** Both folders stay off limits per
`CLAUDE.md`, with the one carve-out that rule names for downloadable sample files. Whatever the next run
reports there gets written into `sonar-issue-triage.md` - which already holds the `docs/` findings from the
2026-08-09 sweep - or a new plan for the session that owns those files. Measuring and fixing are separate jobs,
and only fixing was ever restricted.

### Settings that live in the SonarCloud UI, not in the repo {#sonar-ui-settings}

Scope, coverage paths and the test/product split are all in the repo now (the analysis xml and
`Directory.Build.props`). These are the ones that cannot be, and they are what the gate actually hangs on.

- **New code period - the one that matters. Applied 2026-08-09; this entry is kept for the reasoning.** It used
  to be `previous_version`, and because the scanner is never passed `/v:` the project version never changed, so
  the period stayed pinned to the **first analysis, 2025-04-15**. Sixteen months of work counted as "new code":
  882 of 1059 code smells, and a gate asking for 80% coverage on new code was really asking for it on everything
  ever written. It is now **"Number of days = 30"** (`qualitygates/project_status` reports
  `mode: days, parameter: 30`, baseline 2026-07-30), and the new-code smell count fell from 882 to 8. Once Sonar
  runs on PRs (this gate), the textbook answer is **"reference branch = main"**, so each PR is graded on exactly
  what it changed rather than on a rolling month that keeps moving under it. **Do not assume it applies here.**
  The Free plan analyses only the main branch plus PRs targeting it, and a PR is already graded on its own diff
  automatically, whatever the new code setting says. So "reference branch = main" may be either unavailable or a
  no-op for us, and "days = 30" may be the permanent answer rather than a stopgap. Check it in
  Administration > New Code before planning around it.
- **Three findings marked in the UI**, none of which has an honest code fix: `S2245` on `SampleDataService` and
  on `getRandomInt.ts` (both pick demo data, not secrets), and `S2068` on the `AccountGetResponse` OpenAPI
  example, where `PasswordHash` is the literal `"type::hash::salt"`.
- **Automatic Analysis stays OFF**, as below.
- **No source glob in the UI.** An `sonar.inclusions` of `src/**/*` left over from a flat layout is what made the
  2026-08-07 run index 0 files and still report success. Scope is exclusions only, and they live in the xml.

**What the scope work actually did**, so nobody reads it as a regression or an improvement anyone earned. The
2026-08-08 run (revision `54a94b83`) came out at **24,931 ncloc, 53.3% coverage, 509 issues, 0 bugs, 0
vulnerabilities, 0 hotspots, 3.4% duplication**, against ~50k ncloc and 37.8% coverage before. Vendored
`assets/lib` and the `shared/data` fixtures left the line count; 2329 uncovered vendored lines and 1203 lines
of support code left the coverage denominator. Every one of those moves is a correction to what was being
measured, not work anyone did. **Do not set the coverage ratchet from either the old figure or that one** -
take it from a run that has settled, after `vipaq/test-vectors` and the docs/web scope change land.

**Watch out:**

- Build + coverage must sit between `Sonar begin` and `Sonar end`; the scanner only sees projects compiled in that
  pair. A failing suite skips `Sonar end`, so a failed run publishes nothing - that is deliberate.
- Sonar needs full git history (`fetch-depth: 0`) to tell new code from old. A shallow clone makes everything look
  new.
- `sonar-analysis.yml` pins the service suite to SQLite, so its coverage never exercises the Azure or Postgres
  provider code. Covering those means running that leaf again per backend, which the coverage recipes do not do
  today: `just coverage all` runs the infra-free set once.
- Coverage now runs all 9 C# suites; the old script ran 5, and the four API unit suites contributed nothing. The
  first analysis after that change will show a step up in the number. It is a correction, not an improvement
  anyone made to the code - **do not set the ratchet from the old figure.**

## Done when

- A PR that breaks the image build fails before it merges.
- The integration suites run against the module set the image ships, and the three TODOs are gone.
- A PR gets a coverage number and a Sonar verdict without anyone pressing a button.
