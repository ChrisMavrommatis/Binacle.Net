---
id: ci-cd/decisions
description: CI/CD decisions ledger — why the release pipeline is tag-triggered and promotes by digest, why workflows call just recipes, the pinning rules, and the open questions about the PR gate and supply-chain attestation.
verified: 2026-08-11
check: Decisions still match .github/workflows/*.yml and config/build.just; D2/D3 against release-docker-image.yml, D6 against smoke-image.yml's runs-on, D12 against build.just's publish recipe
---

# CI/CD — decisions ledger

Why the workflows are shaped the way they are. What they *do* is `$ci-cd` and `$ci-cd/release-pipeline`; this
file is the reasoning, so a later session does not re-litigate a deliberate choice or "fix" one.

Several of these were argued out at length while the release pipeline was being rebuilt, in a working file that
does not outlive the work. This is where that reasoning lives now.

## Locked

### D1 — the release pipeline triggers on the tag, not on `release: published`

`on: push: tags: 'v*'`, and creating the GitHub release is the pipeline's **last** job rather than its trigger.

**Why:** with `release: published`, the release is already public when the workflow starts, so a failure leaves
an announced release whose image never arrived. Building from the tag and creating the release at the end
inverts that — a failure leaves a tag you delete, and nothing a user ever saw.

**This is a replacement, not an addition.** Leaving both triggers on would make the release created in the last
job re-trigger the whole workflow and build everything a second time.

### D2 — push the immutable tag, smoke the registry copy, then promote by digest

Order: push `3.0.0` alone, pull it back and smoke it, then move `3.0` and `latest` onto that same digest with
`docker buildx imagetools create`.

**Why not the tempting alternative.** Building with `load: true`, smoking the local image, then pushing sounds
equivalent and is not — it tests a local copy that *ought to be* identical to what lands in the registry.
Compression, manifest shape and attestation handling are precisely what a registry round trip changes, so the
only honest smoke target is what the registry actually serves.

**Why the exposure is acceptable.** The immutable tag is briefly public and unsmoked. Nobody is following
`3.0.0` on release day — it did not exist a minute earlier. The moving tags are the ones the samples, the
README and the quick start point at, and they never point at anything unsmoked. The alternative buys a smaller
exposure window on a tag nobody watches, in exchange for never testing the registry copy at all.

**Promotion is a re-point, not a rebuild.** `imagetools create` aims new tags at an existing manifest. It
uploads nothing, so the moving tags land on the exact bytes that passed, not on a rebuild that ought to match.

### D3 — `metadata-action` runs twice, and an empty tag list is the prerelease guard

One step for the immutable tag (`{{version}}`), one for the moving tags (`{{major}}.{{minor}}` plus
`latest=auto`).

**Why:** hand-rolling "is this a prerelease" is the thing most likely to be subtly wrong, and metadata-action
already gets it right — it skips `{{major}}.{{minor}}` for a prerelease, and `latest=auto` withholds `latest`
for the same reason. Splitting the step turns that into the guard: the moving list comes out empty, so the
promote step is a natural no-op instead of an `if:` condition somebody has to maintain.

**Observed, not assumed.** Checked on Docker Hub on 2026-08-06 after `v3.0.0-beta.1`: it published
`3.0.0-beta.1` and moved neither `latest` nor `3.0`, and no `3.0` tag existed at all.

No `{{major}}` tag is emitted on purpose — a bare `3` would cross minor lines.

### D4 — a workflow step calls a `just` recipe, it does not inline the command

**Why:** the release workflow used to inline `dotnet restore` + `dotnet publish` while `config/build.just`
published the same project to the same place. They matched by coincidence, and a coincidence is not a
guarantee — the project path, the output folder and the runtime identifier each had two homes that could drift.
Calling the recipe makes CI and a laptop build the same thing by construction.

It also makes a red step reproducible: the `run:` line is what you paste into a terminal.

The corollary is that recipes must stay callable from CI as they stand — nothing interactive, no `sudo`, no
local-only paths. Directory preparation that needs `sudo` is a precondition of *running* a compose stack, not
of building anything, and is deliberately kept out of the build recipes.

### D5 — a repo variable may not duplicate a fact that lives in the repo

`API_PROJECT_PATH`, `BUILD_OUTPUT` and `BUILD_DOCKERFILE` were removed and replaced by the literal values.

**Why:** a repo setting is invisible to a reader of the repo, it is not versioned with the code, and it can only
drift from the fact it duplicates. One did — `API_PROJECT_PATH` still named the pre-move `src/` path after the
layout change and broke the publish. The project and output folder are decided in `config/build.just`, and
there is exactly one `Dockerfile`, at the repo root.

What legitimately stays a variable is a value with **no** home in the repo: the SDK version, the Docker Hub
coordinates, the Sonar project key.

### D6 — `smoke-image.yml` pins `ubuntu-24.04`, everything else takes `ubuntu-latest`

**Why:** hurl links `libxml2.so.2`. Ubuntu 26.04 ships only `libxml2.so.16` and carries no compat package, so
hurl dies there with a missing-library error that reads like a hurl bug rather than a distro change.
`ubuntu-latest` will move to 26.04 eventually, and this workflow runs rarely enough that it would break on the
day it is needed most.

### D7 — the release body is a file, published whole

`gh release create --notes-file .agents/release-notes-<tag>.md`, falling back to `--generate-notes` when there
is no such file.

**Why:** the file is body only — no title line, no preamble, no instructions — precisely so it can be published
whole. A file you paste whole cannot be pasted wrongly, and a file the workflow reads cannot be forgotten. The
fallback matters because a prerelease normally has no written body, and publishing notes written for a different
version is worse than generating them.

### D8 — Sonar analysis is a CI run, and Automatic Analysis stays off

**Why:** coverage requires a build. Automatic Analysis only reads source, so it can never report coverage, and
the two fight if both are on. The build and the coverage run must sit between `Sonar begin` and `Sonar end` —
the scanner only sees projects compiled inside that pair — and a failing suite skips `Sonar end`, so a broken
run publishes nothing. That last part is deliberate, not a bug.

Two mechanical consequences: the checkout needs `fetch-depth: 0`, because a shallow clone makes all code look
new to the new-code comparison; and the scanner is a Java program whatever language it analyses, so the job sets
up a JDK.

**Scope and coverage paths live in `config/sonar-analysis.xml`, not in the workflow.** The Scanner for .NET
ignores `sonar-project.properties`, so that XML is the file form it reads, and `/s:` needs an absolute path.
Only the key, org, token and host stay in the YAML.

### D9 — the Postgres service in `run-tests.yml` carries no password

`POSTGRES_HOST_AUTH_METHOD: trust`, and no `POSTGRES_PASSWORD`.

**Why:** the container lives for one job, is reachable only from that job, and is thrown away after. There is
nothing for a password to protect. Under `trust` it accepts any password, so the integration tests connect
unchanged using the shared local-dev connection string.

**This removed the credential from that file, not from the repo.** The same local-dev password is still in
`config/docker-compose.yml`, `config/docker-compose.build.yml`, the test default in `BinacleApi.cs` and
`Config_Files/ServiceModule/ConnectionStrings.Development.json`, where it is load-bearing. It is the same value
everywhere on purpose. Change it in all of them or none.

### D10 — `npm ci --ignore-scripts`

**Why:** an install-time lifecycle hook is arbitrary code execution from a dependency. Nothing here needs one —
no workspace declares `prepare` or `postinstall`, and the only dependency with an install script is `fsevents`,
which is darwin-only and never installed on a Linux runner. The flag costs nothing and closes the hole.

### D11 — third-party actions are pinned by commit SHA

With the version in a trailing comment, so the pin is readable. First-party `actions/*` and `docker/setup-*`
stay on a major tag.

**Why:** a mutable tag on a third-party action is a supply-chain hole — the tag can be re-pointed at any commit,
including after review. The trailing comment is what keeps the pin maintainable; a bare SHA tells a reader
nothing about how far behind it is.

### D12 — the image is framework-dependent, and the publish flag is spelled out

`--no-self-contained --runtime linux-x64`, written explicitly rather than left to the default of a bare
`--runtime`.

**Why:** the runtime comes from the `aspnet` base image, which is the whole point, and that has to be readable
on the line. The image was self-contained until 2026-08-10 while basing on `aspnet:10.0`, so it carried two
copies of .NET — the bundled one the app ran on, and the base image's, which nothing loaded.

**Measured before the change was kept:** image 150.2 MB to 103.2 MB, publish output 123 MB to 18 MB,
`System.*.dll` count 172 to 4. All structure assertions, all five smoke profiles and every test leaf green on
the rebuilt image. The entrypoint did not change — `dotnet Binacle.Net.dll` was always the framework-dependent
idiom, which is what made the old pairing wrong in the first place.

The second reason is durability: framework-dependent means a .NET security fix reaches users by rebasing on a
newer `aspnet` tag rather than by republishing the app, which matters for a project that ships months apart.

### D13 — per-build OCI labels are applied at build time, never as `LABEL` fed by `ARG`

Version, revision and created are set with `--label` (locally) or by metadata-action (in CI). Constant labels
stay as `LABEL` lines in the `Dockerfile`.

**Why:** those three change on every build. As Dockerfile `LABEL`s fed by `ARG` they would invalidate the layer
cache from that point down, for metadata nothing executes. `--label` writes image-config metadata with no layer
and no cache cost.

metadata-action overrides two of the Dockerfile's constant labels on purpose — `licenses`, because
auto-detection returns `NOASSERTION` for a dual-licensed repo, and `url`, which should be the landing site
rather than the repo.

## Open

### O1 — a prerelease cannot test the promotion step

By design: D3 makes the moving tag list empty for a prerelease, so `imagetools create` never runs. That is the
newest and least exercised command in the pipeline, and the first tag that reaches it would otherwise be a real
release — the one that cannot be taken back.

The answer is a separate check against a throwaway tag, run once, before a release depends on it. Not yet done.

### O2 — how much the pull-request gate should prove

Today a PR runs the test leaves and nothing else: the image is never built, the integration suites cover core
modules only, and Sonar runs when somebody presses a button. Each is a known gap rather than an oversight, and
the shape of the fix is not settled — one folded job or three workflows, and what the runtime budget allows.

One part **is** settled: **coverage must not be made blocking yet.** The project runs the read-only "Sonar way"
gate, which asks 80% on new code, and custom gates need a paid plan. The gap is almost entirely four areas at
exactly 0% — the Blazor UI module and three TypeScript packages, 1571 lines, 22.5% of the coverage denominator.
Excluding them was considered and rejected: it moves the number without changing anything true. A condition
that is red before anyone writes a line blocks every PR for a reason none of them caused, and gets waived within
a week. It goes green when the UI gets a test harness, not by configuration.

### O3 — supply-chain attestation and multi-arch are deliberately absent

No SBOM, no provenance, no signature, `linux/amd64` only.

They are out because they **change the artifact**, and the pipeline rebuild was scoped to what a prerelease tag
could prove without altering what ships. That reasoning expires once the pipeline itself is trusted; nothing
about these needs a beta to justify them. Not scheduled.
