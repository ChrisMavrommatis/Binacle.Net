# CI - make the PR gate mean something

**Status:** Not started. After v3.0.0. Folded on 2026-07-28 from `ci-docker-image-gate`,
`ci-all-modules-integration-tests` and `ci-sonar-coverage-gate` - they share a trigger, a workflow and an
ordering, so the shared parts were being written three times.

Three gates, one story: **the PR gate is green without proving much.** The image is never built, the integration
suites run core modules only, and Sonar runs when somebody remembers. Each gate below closes one of those.

## What they share

- **`ci-shared-scripts` lands first.** Every gate here calls a recipe rather than inlining its own commands, and
  the image gate is blocked outright until the build/image split in that plan is done (see the image gate).
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

**Blocked on the build/image split.** `config/build.sh` also creates the local bind-mount folders and `chmod`s
them, one of those with `sudo` - local setup CI must not run. Publish + `docker build` have to come out as their
own entry point before CI can call them. That split lives in `ci-shared-scripts`, and it is the first step there.

## Gate 2 - run the integration tests with all modules enabled

The integration harnesses run **core modules only**. Every module combination the image actually ships is
untested end to end. Three `// TODO` comments say so:

- `api/test/Binacle.Net.IntegrationTests/BinacleApi.cs:35`
- `api/test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
- `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`

- Turn the modules on in the harnesses - Diagnostics, Service, UI.
- Decide whether that is one run with everything on, or a small matrix over the combinations that actually ship.
  Everything-on is cheaper and catches registration conflicts; a matrix catches "module A only works because
  module B registered something".

**Watch out:** test-host configuration goes through an env var the harness reads, never a `.runsettings` file -
the MTP runner ignores VSTest runsettings. `BINACLE_TEST_INFRA` already works this way.

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
