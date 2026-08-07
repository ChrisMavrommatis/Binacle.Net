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
