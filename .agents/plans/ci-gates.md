# CI - make the PR gate mean something

**Status:** Not started. After v3.0.0. Folded on 2026-07-28 from `ci-docker-image-gate`,
`ci-all-modules-integration-tests` and `ci-sonar-coverage-gate` - they share a trigger, a workflow and an
ordering, so the shared parts were being written three times.

Three gates, one story: **the PR gate is green without proving much.** The image is never built, the integration
suites run core modules only, and Sonar runs when somebody remembers. Each gate below closes one of those.

**Two gates moved out on 2026-08-07, and this file is back to the three it folded.** Linting the OpenAPI
documents is one workflow step with a known answer, so it is a line in `todos.md`. Smoking the image runs in
the release workflow rather than on a PR, so it moved to `ci-release-workflow-build`, which owns that file.
Neither shared the checkout, the ordering or the runtime budget below - that is what made this file too big to
finish in one sitting.

## What they share

- **`ci-release-workflow-build` lands first.** Every gate here calls a recipe rather than inlining its own
  commands. The build/image split it used to wait on has landed, so no gate is blocked outright any more - but
  until the release workflow builds through the same recipe, the image and smoke gates prove less than they
  look like they prove. Both say so in their own sections.
- **One job or three?** All three want a checkout and an SDK setup. Folding them into `run-tests.yml` pays for
  that once; separate workflows can each run on a schedule as well. Decide once, for all three, rather than per
  gate - that choice is most of why these were folded.
- **Runtime is the shared budget.** The integration suite is already the long pole, and all-modules plus coverage
  both make it longer. Whatever ordering is chosen, know the total before adding the third gate.
- **A gate that does not match what ships proves nothing.** That is the same failure in all three: the release
  Dockerfile arguments, the shipped module set, and the coverage floor all have to be the real ones.

## Gate 1 - build the docker image on every PR

The image is built in CI only when a release is published (`.github/workflows/release-docker-image.yml`). So a PR
never proves the image still builds, and a break is found at release time - which is exactly what happened after
the `Binacle.Geometry` extraction, where the image had not been built for the whole restructure. `run-tests.yml`
builds the solution and runs every suite on each PR; it does not build the image.

- Add an image build step to the PR gate. Build only - no push, no login, no Docker Hub credentials on a PR.
- Use the same Dockerfile and the same publish arguments the release workflow uses, or the gate proves nothing.

**Unblocked.** The split landed: the gate step is `just build image` (`config/build.just`), which publishes and
builds with no push, no `sudo` and nothing interactive. The release workflow still inlines its own publish, so
until it calls `just build publish` too, the gate proves the recipe builds - not that the release path does.
That wiring is in `ci-release-workflow-build`.

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
`CLAUDE.md`. Whatever the next run reports there gets written into `docs-v3-pages.md` or a new plan for the
session that owns those files. Measuring and fixing are separate jobs, and only fixing was ever restricted.

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
