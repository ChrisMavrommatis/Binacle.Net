---
id: ci-cd
description: CI/CD — the nine GitHub Actions workflows in .github/workflows and the nine shared actions in .github/actions, what triggers each, the conventions they all follow, and the repo variables, secrets and environments they need
verified: 2026-08-22
check: The workflow table matches the files in .github/workflows and the action table matches .github/actions; the vars/secrets tables match every ${{ vars.* }} and ${{ secrets.* }} reference in them; the pinned just version and runner labels still match; the SHAs named as living only in .github/actions still appear in no workflow file; every .github/actions folder holding an outside SHA pin has its own entry in .github/dependabot.yml
also_update:
  - ci-cd/release-pipeline
  - tooling
  - commands
paths:
  - ".github/workflows/**"
  - ".github/actions/**"
---

# CI/CD

Nine workflows in `.github/workflows/`, over nine shared actions in `.github/actions/`. They gate a pull
request, analyse it, release the Docker image, write its Docker Hub page, and deploy the two Jekyll sites. This doc covers what runs,
when, and the conventions every one of them follows. The release pipeline has its own page
(`$ci-cd/release-pipeline`) because it is seven jobs with an ordering that matters.

**Three of the nine carry a `shared-` prefix**, which means something else calls them — the release pipeline
calls all three, and the pull request gate calls the test suite. It does **not** mean private: both keep
`workflow_dispatch`, so both can be run by hand. Nothing here is a workflow nobody can press.

**Where the line with `tooling/` sits.** `tooling/` (`$tooling`) owns *what a command does* — the `just` modules
for build, test, coverage, openapi and smoke. This slice owns *what runs on a runner and in what order*. A
workflow step says `just build publish` and stops; how that publishes is decided in `tooling/build.just` and
described in `$tooling`. Nothing about a recipe's behaviour is repeated here.

## What runs, and when

| Workflow | Trigger | What it does |
|---|---|---|
| `pull-request.yml` | `pull_request` | The gate. Works out whether anything outside guidance and the two sites changed, then runs the test suite, an image build and the two `.github/` lints if it did. `gate` reports either way — see below |
| `shared-test-suite.yml` | `workflow_dispatch`, `workflow_call` | Every test leaf, plus `just openapi lint`. One job so setup and build happen once; one step per leaf so a red check names the suite. Postgres and Azurite run as job services, one ServiceModule step per storage backend. Called by the release pipeline as its "this commit passed CI" gate, so the release gets the lint too |
| `sonar-analysis.yml` | `workflow_dispatch` | Build plus `just coverage all sonar` between `Sonar begin`/`Sonar end`, published to SonarCloud. **By hand, and never on a schedule** — a nightly run re-analyses a commit nothing changed and reports the same numbers |
| `codeql-analysis.yml` | `push` on `main`, weekly `schedule`, `workflow_dispatch` | CodeQL over four languages — `actions`, `csharp`, `javascript-typescript`, `ruby` — one matrix job each, all buildless. Findings land in the Security tab, not on a check. **On a schedule as well as on merge** — the query packs change, so an untouched commit reports new findings later. That is the one analysis a schedule earns |
| `release-docker-image.yml` | `push` on tags `v[0-9]*` | The release pipeline — check the changelog section, run the suite, build and push to GHCR, smoke it there, copy to Docker Hub, create the GitHub release, write the Docker Hub page. See `$ci-cd/release-pipeline` |
| `shared-dockerhub-overview.yml` | `workflow_dispatch`, `workflow_call` | Renders `.github/dockerhub-overview.md` with `just image dockerhub-overview <version>` and PATCHes it onto the Docker Hub repository page. Called by the release pipeline as its last job, or run by hand for a wording fix — an empty version input takes the latest release, so a typo fix needs nothing typed |
| `shared-smoke-image.yml` | `workflow_dispatch`, `workflow_call` | Pulls a published image and runs the structure check plus all five smoke profiles. Called by the release pipeline as its gate, or run by hand against any tag |
| `deploy-docs-site.yml` | `workflow_dispatch` | Two jobs — build `sites/docs/` (`$sites/docs`), check its links, hand the built directory to the host, then tag the commit `docs-<run>` |
| `deploy-demo-site.yml` | `workflow_dispatch` | The same for `sites/demo/` (`$sites/demo`), tagging `demo-<run>` |

**Three of the nine run on their own.** `pull-request.yml` on every pull request, `codeql-analysis.yml` on
every merge to `main` and weekly, and the release pipeline on a tag. The other six are `workflow_dispatch` —
somebody presses a button.

**For the two site deploys that is the end state, not the current one** — publishing to the internet is a
deliberate act and never a side effect of a commit, which is `$ci-cd/decisions#D17`. The other three are
open to changing.

## `gate` is the only required check

**Branch protection names `Pull Request / Gate` and nothing else.** Everything under it can be renamed, split
or reordered without touching protection, which retires a trap rather than working around it: required check
names *are* job names, so every rename silently breaks protection, and a required check that stops reporting
leaves every pull request waiting on it forever with nothing saying why.

**It also makes skipping safe, which is the whole reason the shape exists.** Roughly two thirds of recent
commits touch only `.agents/`, the two sites or markdown — 38 of the last 60 — and a full suite with a Postgres
and an Azurite container is wasted on every one of them. The obvious fix is the trap above: `on: pull_request` with
`paths-ignore` means the workflow never triggers, so the required check never reports. Instead:

- **`changes`** always runs, filters nothing, and takes seconds. It diffs the branch against its merge base and
  outputs whether anything outside guidance and the sites moved.
- **`test-suite`, `image` and `workflows`** carry `needs: changes` and an `if:` on that output, so they are
  *skipped*, not failed, when nothing relevant changed.
- **`gate`** carries `needs:` on all four and `if: always()`, and passes only if every one of them succeeded
  or was skipped. It writes the job-by-job table to the run summary, so a red gate says which job did it
  without anyone opening a log.

**Get the `always()` condition right or the gate is decoration.** A bare `if: always()` with no result check
passes while its dependencies are red — a required check that can never fail. The step reads each
dependency's `result` and treats only `success` and `skipped` as acceptable.

**The diff is three-dot, `base...head`.** Two dots asks what the two tips differ by, so a commit landing on
`main` would read as a change in every open pull request.

**The image build is in this workflow rather than in the test suite** because the release calls that file whole
and takes it as it is. A build step there would build a throwaway image on every release, two minutes before
the real one. A conditional step is not the fix — that gives the release a gate it does not exercise.

**What the release does not get is this workflow's own jobs.** Everything on `main` passed through the gate to
reach it, so that is a choice rather than an oversight.

**Three workflows push git tags, and the namespaces must not overlap.** The release pipeline fires on
`v[0-9]*`; the two deploy workflows create `docs-<run>` and `demo-<run>`. That is the whole
reason the release trigger is not the looser `v*` — a deploy tag must never build and publish an image. Any new
workflow that pushes a tag has to stay out of the `v<digit>` namespace.

**The two marker tags are pushed after a successful deploy, not before it.** The tag exists so a live site maps
back to a commit; pushed first, it claims that of a deploy that then failed. Each deploy workflow is two jobs
in that order — **build-and-deploy, then tag** — chained by `needs:`, which is the gate: a job with an
unsatisfied `needs:` is skipped, so a failed deploy never reaches the tag. No `if:` is written for this.
`if: success()` is already the default on a `needs:` job, and writing it out invites the reader to think
something unusual is being expressed.

**Building and deploying are one job because the built directory is the deliverable.** The host is handed a
finished `artifacts/<site>`, so splitting them would only mean uploading that directory out of one job and
downloading it into the next to reach the same place. **The tag stays its own job**, which is what keeps
`contents: write` off the run that builds and deploys.

**The `build` job also runs `just check links <site>`**, because a site with forty dead links builds
perfectly — that is the failure a build cannot see. It runs there rather than in `deploy`, so a dead link stops
the deploy instead of being found after it. **It is the offline check, never `links-external`**: the absolute
URLs on every page point at where that page *will* live, so at this moment they 404 on every page the run is
about to create. The external run is a manual tool, not a gate — see `$commands`.

## Concurrency, and where cancelling is wrong

**Every entry point declares a `concurrency` group; no `shared-` workflow does.** A called workflow runs inside
its caller's run, so a group of its own would have it queue behind the caller that is waiting for it.

| Workflow | Group | `cancel-in-progress` |
|---|---|---|
| `pull-request.yml` | `${{ github.workflow }}-${{ github.ref }}` | **true** |
| `sonar-analysis.yml` | `${{ github.workflow }}-${{ github.ref }}` | **true** |
| `codeql-analysis.yml` | `${{ github.workflow }}-${{ github.ref }}` | **true** |
| `release-docker-image.yml` | `${{ github.workflow }}-${{ github.ref }}` | **false** |
| `deploy-docs-site.yml` | `${{ github.workflow }}` | **false** |
| `deploy-demo-site.yml` | `${{ github.workflow }}` | **false** |

**Cancelling is right for the first three and wrong for the last three, and the difference is whether a
half-done run leaves anything behind.** A cancelled pull request run leaves nothing — a newer push supersedes it
and nobody was going to read the old result. A cancelled Sonar or CodeQL run is the same: both show the
branch's latest state, so the superseded findings were going to be overwritten anyway.

**A cancelled release or deploy leaves wreckage.** Stopped between `build` and `publish`, the release has a
staged image on GHCR that nothing copies, or a half-written set of moving tags on Docker Hub. A stopped deploy
leaves a deploy half-done with no marker tag, so the live site maps back to no commit. Those queue
instead.

**`github.ref` is what makes the key specific** and it differs per event: `refs/pull/<n>/merge` on a pull
request, `refs/heads/main` on a merge, `refs/tags/<tag>` on a release. The two deploys leave it out on purpose
— they are `workflow_dispatch` from any branch, and two deploys of the same site must not run at once whichever
branch fired them. **`sonar-analysis.yml` is `workflow_dispatch` too and keeps `github.ref` anyway**, because
SonarCloud tracks a branch at a time: two branches analysing at once is fine, the same branch twice is not.

## What the run page says without opening a log

**A summary is a plain markdown append to `$GITHUB_STEP_SUMMARY`** — no job output to declare, no step to
consume it. **Seven workflows write one, and each writes it from its last job**, carrying only what the log does
not already say. A table that restates the job list is a heading with no fact in it, which is why
`shared-test-suite.yml` writes nothing: its step names already are that table.

| Workflow | Written by | What it carries |
|---|---|---|
| `pull-request.yml` | `gate` | The job-by-job result table. On a red gate it names the job that did it; on a green one it shows what was skipped, which is how you tell "nothing relevant changed" from "the suite ran". |
| `release-docker-image.yml` | `release` | Version, digest, every public tag, the release link and the verify command. |
| `deploy-docs-site.yml`, `deploy-demo-site.yml` | `tag` | The commit and its subject, the marker tag and the site URL — three greps through a log otherwise. |
| `sonar-analysis.yml` | `analyze` | The quality gate and every condition with its value. The numbers live on another site and no gate blocks on them yet, so this is where they get read. |
| `codeql-analysis.yml` | `summary` | Open alert counts by severity. The matrix has no last job, so this one exists for the summary; it does not repeat the per-language results the job list already shows. |
| `shared-smoke-image.yml` | `smoke` | The image, the digest its tag resolved to, and each profile's result — **only on a `workflow_dispatch`**, where this workflow is the whole run. The release calls it too, and a block there would sit beside the one `release` writes for no new fact. |
| `shared-dockerhub-overview.yml` | `publish` | The version the page describes and a link to it — **only on a `workflow_dispatch`**, for the same reason as the row above. |

**A summary is a signed-in view.** An anonymous visitor to a run page gets the page shell and none of the
summary content, on a public repo included. So nothing a user needs may live only here: the digest they verify
against and what shipped have to reach them through the release body, `CHANGELOG.md` and the docs site.

**Two of them need real work to produce a fact**, and both do it rather than print something cheaper.
`sonar-analysis.yml` polls the SonarCloud `ceTask` from `report-task.txt` before reading the gate — `end`
returns when the upload finishes and the analysis is processed after, so reading it straight away returns the
**previous** run's numbers. `codeql-analysis.yml` pages the code-scanning API, because the per-page cap of 100
truncates silently and that reads as "fewer alerts now".

**Only one job output exists to serve a summary:** `publish.outputs.tags`, read by `release`. An output is
otherwise read by another job while a summary is read by a person, so adding one nothing consumes is plumbing
for its own sake — the exception is here because the release URL only exists after `release` has run, and the
tag set is computed in `publish`.

## Naming

**Files.** `<verb>-<object>.yml` for an entry point — `release-docker-image.yml`, `deploy-docs-site.yml`.
`shared-<noun>.yml` for one that something else calls — `shared-test-suite.yml`, `shared-smoke-image.yml`.
`pull-request.yml` is the one exception, named for the event it gates rather than for an action, because the
required check then reads `Pull Request / Gate`.

**Workflow `name:`** is the sidebar string, so it is where the grouping is actually visible. Entry points get a
plain name; shared ones get one prefix, consistently — `Shared / Test Suite`, `Shared / Smoke Image`.

**Job `name:`.** A called workflow reports as `<caller workflow> / <caller job> / <job in the callee>`, and
that whole string is what a reader sees on a red check and what branch protection matches. **Keep every half
short**, or the required check name becomes unreadable at exactly the moment somebody needs to read it.

**Steps.** One shape across every file, rather than a per-workflow style: `Setup - <tool>`,
`Install - <tool>`, `Build - <thing>`, `Check - <thing>`, `Lint - <thing>`, `Test - <leaf>`,
`Smoke - <profile>`, `Deploy to <target>`.

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
  the red at once. In `shared-smoke-image.yml` the same condition also gates on the pull having succeeded — six
  failures that all mean "no such image" is noise.
- **Every action is pinned by commit SHA**, first-party included, with the version in a trailing comment
  (`extractions/setup-just@53165ef... # v4.0.0`). `.github/dependabot.yml` raises a weekly PR per action and
  rewrites the SHA and the comment together, which is what keeps a pin from quietly going stale.
  **Four of those pins live in `.github/actions/` rather than in a workflow**, and that folder needs its own
  Dependabot entry per action — see the composite actions section below.
- **`just` is pinned to `^1.45`, in one place**: `.github/actions/setup-just`. Modules and
  `set working-directory` need a recent just, and Ubuntu's apt ships one too old to parse the module files.
- **A binary is installed by `curl` from its own release, one action per tool, pinned by version and by
  SHA-256.** `container-structure-test`, `hurl` and `lychee` are all like this, and **all three live in
  `.github/actions/install-*`**, which is where their versions and checksums are written. Each ends by printing
  the version it installed, so the log names the tool that failed rather than "smoke tools". The checksums make
  a swapped release asset fail the build instead of run; hurl and lychee publish their own, and the
  `container-structure-test` one was taken from the binary in use because upstream publishes none.

  **Every one of those `curl` calls carries `--proto '=https' --proto-redir '=https'`.** `-L` follows a
  redirect wherever it points, and without those two the hop can land on plain HTTP. Dropping them is what
  Sonar's `githubactions:S6506` reports.

  **For the first two there is no maintained action to use.** lychee has one, and is installed this way anyway:
  the action runs lychee itself from YAML arguments, so the check would stop being `just check links <site>` and CI
  would configure it separately from a laptop. **A step calls a recipe** — that convention is the stronger one,
  and it is what keeps "green in CI" and "green here" the same claim.
- **`permissions:` is declared per job**, least privilege. `contents: read` almost everywhere; the release job
  takes `contents: write` to create the release, and in each deploy workflow the `tag` job takes it — that job
  only. The build job takes `packages: write` for GHCR, and the publish job declares no `contents` at all
  because it never checks out. Both take `id-token: write`, which is what keyless cosign signing needs and the
  only reason either has it. The CodeQL job takes `security-events: write` to upload its findings — GitHub's
  template also lists `actions: read` and `packages: read`, which are for private repositories and private
  query packs and so are left out here. A job that needs nothing says `permissions: {}` rather than leaving it
  out — the two deploy jobs do, since neither reads the repository.
- **Every job declares `timeout-minutes`.** The default is six hours, which is what a hung container or a
  wedged smoke profile would otherwise burn.
- **`npm ci --ignore-scripts`**, so an install-time lifecycle hook cannot run arbitrary code. Nothing in the
  workspaces declares `prepare`/`postinstall`. It is a step in the job that needs packages, next to the
  `setup-node` that precedes it — three places today, and the flag belongs in all three.
- **An interpolated value goes through `env:`, never into a `run:` body.** `${{ }}` pasted into a script is
  substituted before the shell sees it, so the value becomes part of the command rather than an argument to
  it. Every one of them in this repo is a tag name, a repo variable or a step output — none of them
  attacker-controlled — but the pattern is uniform so that stays true of the next one added.
- **Runners are `ubuntu-latest`, with one deliberate exception**: `shared-smoke-image.yml` pins `ubuntu-24.04`,
  because hurl links `libxml2.so.2` and Ubuntu 26.04 ships only `libxml2.so.16` with no compat package.

## The shared actions in `.github/actions/`

**Nine composite actions, and the reason they are actions rather than workflows.** A composite action is a run
of steps inside somebody else's job. It never appears in the Actions tab and cannot be run on its own — which
is what makes it the right home for a repeated step sequence. A **reusable workflow** brings its own job, so it
is the only option when the shared thing needs its own runner or service containers. That is the whole test:
**does it need its own machine.**

| Action | Used by | What it does |
|---|---|---|
| `setup-just` | six jobs | The `just` install and the `^1.45` range, once |
| `setup-dotnet` | `shared-test-suite`, `sonar-analysis`, the release `build` job | SDK plus the NuGet package cache. Takes the SDK version as an input |
| `setup-node` | `shared-test-suite`, `sonar-analysis`, `build-jekyll-site` | Node and the npm cache. It does not install packages |
| `setup-ruby` | `build-jekyll-site` | Ruby and the site's gems. Takes the Gemfile directory as an input |
| `install-container-structure-test` | `shared-smoke-image` | The binary, curled and pinned by version and SHA-256 |
| `install-hurl` | `shared-smoke-image` | The same, and it carries the `libxml2` note that explains its caller's runner pin |
| `install-lychee` | both deploy workflows | The same. The musl build, so it links nothing from the runner |
| `install-actionlint` | `pull-request` | The same, for the workflow linter |
| `build-jekyll-site` | both deploy workflows | The toolchain and `just build <site>`. Takes the site's name and its directory, which are two inputs because they are two things. It does not deploy |

**A `setup-` action installs a toolchain and stops there.** Neither `setup-dotnet` nor `setup-node` installs
packages — the SDK and the cache, then the caller runs `npm ci` or lets `dotnet build` restore. An action named
for setup that also installs is the kind of thing you only discover from a log.

**`setup-ruby` is the exception, and it is upstream's doing.** `bundler-cache: true` is a single flag on
`ruby/setup-ruby` that installs the gems *and* caches them off `Gemfile.lock`. Matching the others means
turning it off and hand-rolling `bundle install` plus an `actions/cache` with the same key — more code, to
reimplement what the flag already does. The action's `description` says it installs gems, so the surprise is
declared where a reader meets it.

**The deploy is in the workflow, not in an action.** `build-jekyll-site` covers the part that is the same for
both sites and would drift if copied; deploying is one `uses:` of a vendor action, and wrapping it buys
nothing. Two things it costs, though, and both matter more: the marker tag's `git push` is visible next to the
`contents: write` that allows it, and the host is named where you would look for it. Changing host is then an
edit to one step in each of two workflows, not to the inside of something called "deploy site".

**`actionlint` does not cover these files, and there is no flag that makes it.** Hand it an `action.yml` and it
reports `"jobs" section is missing` — it treats every input as a workflow. What it *does* check from the
caller's side is their **inputs**: a missing required one or a misspelled name is reported against the `uses:`
line, naming the action and listing what it accepts.

**What is left unchecked is their shell — 38 lines**, against 132 lines in the workflows that get actionlint
and shellcheck. Four of the five blocks are the near-identical `install-*` download-and-checksum scripts. The
gap is small and it is real; closing it means extracting `runs.steps[].run` and piping it to shellcheck, which
is a tool to build rather than one to install.

**`setup-` wraps an upstream action; `install-` curls a pinned binary.** The prefix is the distinction, and it
is the one that matters when something breaks: a `setup-` failure is somebody else's action, an `install-`
failure is a download or a checksum.

**One action per tool, not one for "smoke tools".** Each ends by printing the version it installed, so a red
step names the tool rather than the pair — which is why these are two files and two steps in the caller.

**Four constraints shape them, all of them GitHub's rather than ours.**

- **Neither `secrets` nor `vars` exists inside a composite action.** An action that needs one takes it as an
  input — half the reason the deploy step stayed in the workflow, and why `DONET_VERSION` is passed to
  `setup-dotnet`. **This bites harder than "the value comes out empty".** The runner evaluates the whole
  manifest before it runs a step, so the expression fails the action *load* — every job calling it dies with
  `Unrecognized named-value: 'vars'`. **And it counts anywhere in the file, `description:` fields included**:
  the first CI run after these actions landed failed on a `${{ vars.DONET_VERSION }}` written inside the prose
  that explained why you must not write it. `just check actions` greps for this now, because actionlint
  cannot.
- **Job keys stay with the caller.** `runs-on`, `services:`, `environment:`, `permissions:` and
  `timeout-minutes` cannot be set from an action. This is why the two deploy workflows still declare their own
  `environment:` — it carries the deployment URL and differs per site — and why the test suite is a workflow at
  all, since Postgres and Azurite are `services:`.
- **An action is read out of the working copy**, so a job must check out before it can use one. The release
  `publish` job deliberately never checks out, so it gets none.
- **Dependabot does not reach this folder on its own.** For the `github-actions` ecosystem, `directory: /`
  covers `.github/workflows` and an `action.yml` at the repo root — nothing below `.github/actions/`, and a
  glob in `directories:` is unreliable here. So `.github/dependabot.yml` carries **one entry per action folder
  that pins an outside SHA**: `setup-dotnet`, `setup-just`, `setup-node`, `setup-ruby`. Give a new action an
  outside pin and it needs its own entry, or that pin stops being updated in silence — worse than no pin,
  because nothing reports it. **One side effect:** `actions/cache` is pinned in two covered places,
  `setup-dotnet` and `sonar-analysis.yml`, so a bump arrives as two pull requests and both must merge or the
  two go out of step.

## Repo variables

Set in GitHub repo settings, read as `${{ vars.* }}`.

| Variable | Used by | What it is |
|---|---|---|
| `DONET_VERSION` | `shared-test-suite`, `sonar-analysis`, `release-docker-image` | The .NET SDK version, passed into the `setup-dotnet` action. **The name is misspelled** ("DONET"). It matches the repo setting, so do not correct it in one file only. **Read in the workflow and passed as an input**, never read inside the action — the `vars` context is not dependably available there, and an empty value installs a default SDK and looks like it worked |
| `DOCKERHUB_ORGNAME` | `release-docker-image`, `shared-dockerhub-overview` | Docker Hub org, the first half of the image name |
| `DOCKERHUB_REPO` | `release-docker-image`, `shared-dockerhub-overview` | Docker Hub repo, the second half. Also the lever for testing the `publish` job without touching the real repo: point it at a scratch repo, tag a non-prerelease version, then point it back |
| `SONAR_PROJECT_KEY` | `sonar-analysis` | SonarCloud project key |
| `SONAR_ORGANIZATION` | `sonar-analysis` | SonarCloud organisation |

**Three variables were removed and must not come back.** `API_PROJECT_PATH` and `BUILD_OUTPUT` are decided in
`tooling/build.just`; `BUILD_DOCKERFILE` is the literal `Dockerfile` at the repo root. A repo setting that
duplicates a fact in the repo can only drift from it, and one of them did — `API_PROJECT_PATH` still pointed at
the pre-move `src/` path after the layout change and broke the publish.

## Secrets

| Secret | Used by |
|---|---|
| `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN` | `release-docker-image` — the `publish` job, and `shared-dockerhub-overview` which the `page` job calls. One token does both: the same registry push credential also writes the repository description, confirmed 2026-08-19. **Passed to the called workflow by name, never `secrets: inherit`**, which would hand the runner every secret the repo has |
| `SONAR_TOKEN` | `sonar-analysis` |
| `CLOUDFLARE_API_TOKEN`, `CLOUDFLARE_ACCOUNT_ID` | both deploy workflows |
| `GITHUB_TOKEN` | `release-docker-image` — GHCR login in `build` and `publish`, and creating the release |

**Signing needs no secret either.** cosign runs keyless — it exchanges the job's OIDC token for a short-lived
certificate — so there is no signing key in the repo and none to rotate. That is what `id-token: write` buys.

**GHCR needs no new secret, and that is a reason it was chosen.** `GITHUB_TOKEN` is minted for a single run and
expires with it, so the staging registry has no stored credential to rotate and no classic personal access
token to depend on.

**The staging package is private.** A package created under an organization starts private even when the repo
is public, and this one was created fresh when the repo moved to `binacle-labs`. Nothing outside the release
workflow reads it — `just image verify` reads Docker Hub alone — and all three jobs that touch it log in with
`GITHUB_TOKEN`, so private costs nothing. **Docker Hub is the only registry named on any surface a user
reads**, and every tag the pipeline publishes lands there.

## Environments

The two deploy workflows declare a GitHub environment, which is what carries the deployment URL in the Actions
UI: `binacle-net-docs` (https://docs.binacle.net) and `binacle-net-demo` (https://demo.binacle.net). Nothing
else uses an environment.

**`binacle-net-demo` has never been deployed.** The workflow was renamed from `deploy-web-site.yml` before it
was ever dispatched, so neither the environment nor the Worker exists on either side yet.

Both deploy workflows also push a marker tag after deploying, so a deployed site maps back to a commit. The
workflow table above names each one.

## What CI does not cover

Stated plainly, because the gaps are not obvious from a green check.

- **The image is never built on a pull request.** It is built only by the release pipeline, on a tag. A break
  in the `Dockerfile` or the publish chain is found at release time.
- **The integration suites run core modules only.** Every module combination the image actually ships is
  untested end to end.
- **Neither analysis lands on the pull request that caused the finding.** CodeQL runs on merge and Sonar when
  somebody presses the button, so a finding is read after the fact — in the Security tab or in SonarCloud — and
  nothing goes red when one appears. That is what keeps `gate` the only required check. Sonar's Automatic
  Analysis is off on top of that: it only reads source, and it fights a CI run that uploads coverage.
- **Coverage sees one storage backend.** `shared-test-suite.yml` runs the ServiceModule suite against all three, but
  `sonar-analysis.yml` runs it against SQLite only, so coverage never reaches the Postgres or Azure provider
  code.
- **One architecture.** The image is `linux/amd64` only. It does ship an SPDX SBOM and SLSA provenance, and is
  cosign-signed — see `$ci-cd/release-pipeline`.
- **Nothing verifies a release after it is published.** The `cosign verify` invocation, with the certificate
  identity and OIDC issuer to match, is documented for users in `SECURITY.md` and pointed at from the
  `README`, and `just image verify <version>` runs the checks by hand. No workflow runs them after a tag.
