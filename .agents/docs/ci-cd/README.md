---
id: ci-cd
description: CI/CD — the six GitHub Actions workflows in .github/workflows, what triggers each, the conventions they all follow, and the repo variables, secrets and environments they need
verified: 2026-08-11
check: The workflow table matches the files in .github/workflows; the vars/secrets tables match every ${{ vars.* }} and ${{ secrets.* }} reference in them; the pinned just version and runner labels still match
also_update:
  - ci-cd/release-pipeline
  - tooling
  - commands
paths:
  - ".github/workflows/**"
---

# CI/CD

Six workflows in `.github/workflows/`. They test a pull request, analyse it, release the Docker image, and
deploy the two Jekyll sites. This doc covers what runs, when, and the conventions every one of them follows.
The release pipeline has its own page (`$ci-cd/release-pipeline`) because it is six jobs with an ordering
that matters.

**Where the line with `tooling/` sits.** `tooling/` (`$tooling`) owns *what a command does* — the `just` modules
for build, test, coverage, openapi and smoke. This slice owns *what runs on a runner and in what order*. A
workflow step says `just build publish` and stops; how that publishes is decided in `tooling/build.just` and
described in `$tooling`. Nothing about a recipe's behaviour is repeated here.

## What runs, and when

| Workflow | Trigger | What it does |
|---|---|---|
| `run-tests.yml` | `pull_request`, `workflow_dispatch`, `workflow_call` | Every test leaf. One job so setup and build happen once; one step per leaf so a red check names the suite. Postgres runs as a job service. Called by the release pipeline as its "this commit passed CI" gate |
| `sonar-analysis.yml` | `workflow_dispatch` | Build plus `just coverage all sonar` between `Sonar begin`/`Sonar end`, published to SonarCloud |
| `release-docker-image.yml` | `push` on tags `v[0-9]*` | The release pipeline — check the changelog section, run the suite, build and push to GHCR, smoke it there, copy to Docker Hub, create the GitHub release. See `$ci-cd/release-pipeline` |
| `smoke-image.yml` | `workflow_dispatch`, `workflow_call` | Pulls a published image and runs the structure check plus all five smoke profiles. Called by the release pipeline as its gate, or run by hand against any tag |
| `deploy-binacle-net-docs.yml` | `workflow_dispatch` | Tags the commit `docs-release-<run>`, deploys repo-root `docs/` (`$docs-site`) to DigitalOcean App Platform |
| `deploy-binacle-net-web.yml` | `workflow_dispatch` | Tags the commit `web-release-<run>`, deploys repo-root `web/` (`$web-site`) the same way |

**Only two of the six run on their own.** `run-tests.yml` runs on every pull request and the release pipeline
runs on a tag. The other four are `workflow_dispatch` — somebody presses a button. That is the current state,
not an end state.

**Three workflows push git tags, and the namespaces must not overlap.** The release pipeline fires on
`v[0-9]*`; the two deploy workflows create `docs-release-<run>` and `web-release-<run>`. That is the whole
reason the release trigger is not the looser `v*` — a deploy tag must never build and publish an image. Any new
workflow that pushes a tag has to stay out of the `v<digit>` namespace.

## Conventions every workflow follows

- **A step calls a `just` recipe; it does not inline the command.** `tooling/*.just` is the only place that
  knows which project a leaf maps to, what the publish flags are, or what a smoke profile brings up. This is
  what keeps "green in CI" and "green on a laptop" the same claim, and it means the `run:` line of a red step
  is what you paste into a terminal to reproduce it.
- **One step per thing that can break, and every step is named.** A red check should name the suite or the
  profile, not make you open a log.
- **Adding a test leaf or a smoke profile is two edits: one in the module, one step in the workflow.** The
  module owns what the thing does; the workflow owns that it runs on a PR. Neither half implies the other, and
  the pairing is what stops a suite from being "green in CI" while it does not exist locally, or existing
  locally while CI never runs it. Add the leaf to `tests.just` or the profile to `smoke.just`, then add the
  matching step.
- **Repeated leaf steps carry `if: ${{ !cancelled() }}`**, so one failure does not hide the rest. You see all
  the red at once. In `smoke-image.yml` the same condition also gates on the pull having succeeded — six
  failures that all mean "no such image" is noise.
- **Every action is pinned by commit SHA**, first-party included, with the version in a trailing comment
  (`extractions/setup-just@53165ef... # v4.0.0`). `.github/dependabot.yml` raises a weekly PR per action and
  rewrites the SHA and the comment together, which is what keeps a pin from quietly going stale.
- **`just` is pinned to `^1.45`** everywhere it is installed. Modules and `set working-directory` need a recent
  just, and Ubuntu's apt ships one too old to parse the module files.
- **A tool with no maintained action is installed by `curl` from its own release, one step per tool, pinned by
  version and by SHA-256.** `container-structure-test` and `hurl` are both like this — the only hurl action is
  archived and neither project ships an `action.yml`, so a generic third-party installer would add a dependency
  without removing one. Each step ends by printing the version it installed, so the log names the tool that
  failed rather than "smoke tools". The checksums make a swapped release asset fail the build instead of run;
  hurl publishes its own, and the `container-structure-test` one was taken from the binary in use because
  upstream publishes none.
- **`permissions:` is declared per job**, least privilege. `contents: read` almost everywhere; the release job
  takes `contents: write` to create the release, and the two deploy workflows take it to push their marker tag.
  The build job takes `packages: write` for GHCR, and the publish job declares no `contents` at all because it
  never checks out. Both take `id-token: write`, which is what keyless cosign signing needs and the only
  reason either has it.
- **Every job declares `timeout-minutes`.** The default is six hours, which is what a hung container or a
  wedged smoke profile would otherwise burn.
- **`npm ci --ignore-scripts`**, so an install-time lifecycle hook cannot run arbitrary code. Nothing in the
  workspaces declares `prepare`/`postinstall`.
- **Runners are `ubuntu-latest`, with one deliberate exception**: `smoke-image.yml` pins `ubuntu-24.04`,
  because hurl links `libxml2.so.2` and Ubuntu 26.04 ships only `libxml2.so.16` with no compat package.

## Repo variables

Set in GitHub repo settings, read as `${{ vars.* }}`.

| Variable | Used by | What it is |
|---|---|---|
| `DONET_VERSION` | `run-tests`, `sonar-analysis`, `release-docker-image` | The .NET SDK version for `actions/setup-dotnet`. **The name is misspelled** ("DONET"). It matches the repo setting, so do not correct it in one file only |
| `DOCKERHUB_ORGNAME` | `release-docker-image` | Docker Hub org, the first half of the image name |
| `DOCKERHUB_REPO` | `release-docker-image` | Docker Hub repo, the second half. Also the lever for testing the `publish` job without touching the real repo: point it at a scratch repo, tag a non-prerelease version, then point it back |
| `SONAR_PROJECT_KEY` | `sonar-analysis` | SonarCloud project key |
| `SONAR_ORGANIZATION` | `sonar-analysis` | SonarCloud organisation |

**Three variables were removed and must not come back.** `API_PROJECT_PATH` and `BUILD_OUTPUT` are decided in
`tooling/build.just`; `BUILD_DOCKERFILE` is the literal `Dockerfile` at the repo root. A repo setting that
duplicates a fact in the repo can only drift from it, and one of them did — `API_PROJECT_PATH` still pointed at
the pre-move `src/` path after the layout change and broke the publish.

## Secrets

| Secret | Used by |
|---|---|
| `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN` | `release-docker-image` — the `publish` job only, which is the only job that touches Docker Hub |
| `SONAR_TOKEN` | `sonar-analysis` |
| `DIGITALOCEAN_ACCESS_TOKEN` | both deploy workflows |
| `GITHUB_TOKEN` | `release-docker-image` — GHCR login in `build` and `publish`, and creating the release |

**Signing needs no secret either.** cosign runs keyless — it exchanges the job's OIDC token for a short-lived
certificate — so there is no signing key in the repo and none to rotate. That is what `id-token: write` buys.

**GHCR needs no new secret, and that is a reason it was chosen.** `GITHUB_TOKEN` is minted for a single run and
expires with it, so the staging registry has no stored credential to rotate and no classic personal access
token to depend on. Pulling a staged image needs no credential at all, because the package is public.

## Environments

The two deploy workflows declare a GitHub environment, which is what carries the deployment URL in the Actions
UI: `binacle-net-docs` (https://docs.binacle.net) and `binacle-net-web` (https://www.binacle.net). Nothing
else uses an environment.

Both deploy workflows also push a marker tag before deploying — `docs-release-<run_number>` /
`web-release-<run_number>` — so a deployed site maps back to a commit.

## What CI does not cover

Stated plainly, because the gaps are not obvious from a green check.

- **The image is never built on a pull request.** It is built only by the release pipeline, on a tag. A break
  in the `Dockerfile` or the publish chain is found at release time.
- **The integration suites run core modules only.** Every module combination the image actually ships is
  untested end to end.
- **Sonar runs when somebody presses the button**, so analysis never lands on the pull request that caused the
  finding. Automatic Analysis is deliberately off — it only reads source, and it fights a CI run that uploads
  coverage.
- **Only two of the three storage backends are exercised.** `run-tests.yml` runs the ServiceModule suite
  against SQLite and Postgres. The Azure Storage provider has no CI coverage at all, and `sonar-analysis.yml`
  runs the service suite against SQLite only, so its coverage never reaches the Postgres or Azure provider code.
- **One architecture.** The image is `linux/amd64` only. It does ship an SPDX SBOM and SLSA provenance, and is
  cosign-signed — see `$ci-cd/release-pipeline`.
- **Nothing tells users how to verify any of that.** The signature and attestations are published; the
  `cosign verify` invocation, with the certificate identity and OIDC issuer to match, is not documented
  anywhere a user would look. Until it is, the signing is not doing the job it was added for.
