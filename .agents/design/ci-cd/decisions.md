---
id: ci-cd/decisions
description: CI/CD decisions ledger — why the release pipeline is tag-triggered, stages on GHCR and copies to Docker Hub by digest, why the prerelease guard is metadata-action's rather than a job-level skip, why the notes come from CHANGELOG.md, the pinning rules, why lychee is a pinned binary rather than its own action, and the open questions about the PR gate and supply-chain attestation.
verified: 2026-08-19
check: Decisions still match .github/workflows/*.yml and tooling/build.just; D2/D3/D14 against release-docker-image.yml's publish job, which must carry no prerelease condition, D7 against tooling/changelog.just, D6 against shared-smoke-image.yml's runs-on, D11 against .github/dependabot.yml, D12 against build.just's publish recipe, D14's STAGING_IMAGE against release-docker-image.yml, D15's identity regexp against SECURITY.md and tooling/image.just, D16 against .github/actions/install-lychee and the deploy workflows' build job, D17 against both deploy workflows' triggers, which must stay workflow_dispatch only
paths:
  - ".github/workflows/**"
---

# CI/CD — decisions ledger

Why the workflows are shaped the way they are. What they *do* is `$ci-cd` and `$ci-cd/release-pipeline`; this
file is the reasoning, so a later session does not re-litigate a deliberate choice or "fix" one.

Several of these were argued out at length while the release pipeline was being rebuilt, in a working file that
does not outlive the work. This is where that reasoning lives now.

## Locked

### D1 — the release pipeline triggers on the tag, not on `release: published`

`on: push: tags: 'v[0-9]*'`, and creating the GitHub release is the pipeline's **last** job rather than its
trigger.

**Why:** with `release: published`, the release is already public when the workflow starts, so a failure leaves
an announced release whose image never arrived. Building from the tag and creating the release at the end
inverts that — a failure leaves a tag you delete, and nothing a user ever saw.

**This is a replacement, not an addition.** Leaving both triggers on would make the release created in the last
job re-trigger the whole workflow and build everything a second time.

**The pattern is `v[0-9]*` rather than `v*` because this repo has three tag-pushing workflows.** The two site
deploys create `docs-release-<run>` and `web-release-<run>`, and a release build must never fire on one. Neither
matches either pattern, so this is not a fix for a live bug — it is refusing to depend on a coincidence of
naming, and on the second coincidence underneath it: those workflows push with `GITHUB_TOKEN`, which GitHub
does not let trigger further workflows. That protection disappears the day either switches to a PAT.

**The residual risk this does not close: re-pushing an old version tag.** `v1.0.0` matches, and now that
`CHANGELOG.md` carries a `## [1.0.0]` section the notes gate would pass rather than stop it. `latest=auto`
marks any non-prerelease semver as latest, so a re-pushed old tag would move `latest` backwards onto it.
Deliberate action is needed to get there, so it is recorded rather than guarded — but do not delete and
re-push a released tag.

### D2 — build once, smoke the registry copy, then copy by digest

Order: push the immutable tag to GHCR alone, pull it back from there and smoke it, then copy that digest to
Docker Hub with `docker buildx imagetools create`, under all three public tags at once.

**Why not the tempting alternative.** Building with `load: true`, smoking the local image, then pushing sounds
equivalent and is not — it tests a local copy that *ought to be* identical to what lands in the registry.
Compression, manifest shape and attestation handling are precisely what a registry round trip changes, so the
only honest smoke target is what the registry actually serves.

**Promotion is a transfer, not a rebuild.** A manifest is content-addressed, so `imagetools create` preserves
the digest: Docker Hub serves the exact bytes that passed, not a rebuild that ought to match. The copy source
is the digest rather than the tag, so that holds even if something re-tagged staging in between. All three tags
go in one command - they are aliases of one manifest, so the blobs move once.

**Verified across registries on 2026-08-11, not assumed.** The published `v3.0.0-beta.1` index - an amd64
manifest plus its attestation manifest - was copied by digest from Docker Hub into a scratch registry. It came
out on `sha256:c458644...`, the digest it went in with, and all three tags resolved to it. The attestation
entry survived the copy.

**Superseded 2026-08-11 — what changed and what did not.** This decision originally ran entirely on Docker Hub:
the immutable tag was pushed there, smoked there, and `docker buildx imagetools create` re-pointed `3.0` and
`latest`. The reasoning above survived intact; only the registry topology changed. What forced it was the one
cost the old shape accepted and should not have — an unsmoked artifact was briefly public on the registry users
pull from. It was argued as acceptable because nobody follows an exact pin on release day, and that is true, but
"true for the tag nobody watches" is a weaker claim than "never happens", and D14 makes it never happen for
free. The copy command did not change - `imagetools create` handles a cross-registry source as readily as a
local one, which is what kept a third-party tool out of the job that moves the artifact users pull.

### D3 — the prerelease guard is metadata-action's, not an explicit skip

`publish` runs for every tag. A prerelease reaches Docker Hub with its **immutable tag only**, because
metadata-action skips `{{major}}.{{minor}}` for one and `latest=auto` withholds `latest` for the same reason.
So a beta can never move `3.0` or `latest`, and the guard is the action's rather than this workflow's.

**Observed, not assumed.** Checked on Docker Hub on 2026-08-06 after `v3.0.0-beta.1`: it published
`3.0.0-beta.1` and moved neither `latest` nor `3.0`, and no `3.0` tag existed at all.

**Reversed 2026-08-11, same day it was introduced.** For part of that day `publish` carried
`if: ${{ !contains(github.ref_name, '-') }}`, so a prerelease stopped after `smoke` and lived only on GHCR —
"Docker Hub carries releases only". Two things killed it:

- **It was never a safety rule.** The property that matters is *nothing unsmoked reaches Docker Hub*, and that
  comes from `smoke` running before `publish`. It holds identically with or without the skip. What the skip
  actually bought was a tidy tag list.
- **It cost real deployability.** A beta could then be pulled only from GHCR, and a host that cannot route to
  GitHub's AS36459 — which is not hypothetical — could not deploy the beta at all. Paying that for tidiness is
  the wrong trade.

**What the reversal takes back with it.** `release` no longer needs `if: ${{ !failure() && !cancelled() }}`;
with nothing conditional above it, plain `needs` says the same thing. Restore that condition if any job in the
chain ever becomes conditional again, or a beta will silently get no GitHub release.

No `{{major}}` tag is emitted on purpose — a bare `3` would cross minor lines.

### D14 — GHCR is staging, and only the release workflow touches it

Everything built lands on `ghcr.io/binacle-labs/binacle-net` first. Docker Hub receives only what has been
smoked there.

**Why a second registry at all.** It buys the property D2 used to trade away: nothing unsmoked is ever visible
where users pull from - `smoke` runs against the staging copy, and only a smoked digest is ever copied across. **GHCR is staging; Docker Hub is what users pull, and it carries every tag the
pipeline publishes, betas included.**

**Only the release workflow touches GHCR - decided 2026-08-15, and it is the strong form of the rule.** The
staging registry exists so the workflow can push an image, smoke it and copy the smoked digest to Docker Hub.
That is its whole job. **Nothing else reads it**: no public surface names it, no local recipe queries it, and
no deployment pulls from it. **One image, one place anyone gets it from.**

**What that changed, on the day it was decided.**

- `SECURITY.md` and `CHANGELOG.md` stopped naming it. The docs-site verification page is written the same way
  at the next deploy.
- **`just image verify` lost its `digest` check and is Docker Hub only.** That check compared the tag across
  the two registries to say Docker Hub serves what the smoke job passed.

  **That property is now the workflow's to keep, not a reader's to re-derive**, and it is not lost. `publish`
  copies by digest instead of rebuilding, so the copy cannot be a different artifact; the run log shows the
  digest at each step. From Docker Hub alone, the SLSA provenance names the run that built the image and
  `cosign verify` proves it came from this repository's release workflow - which is the question a reader
  actually has. What no longer has an outside witness is "this digest is the one `smoke` pulled", and that was
  only ever checkable by reading staging.
- The Docker Hub page must not name it - already the plan's own rule, but for a weaker reason.
- The deployment host is repointed at `binacle/binacle-net`.

**Why the rule is worth the check it cost.** A staging registry anyone reads is a second published registry
wearing a different word. It grows instructions, support questions and surfaces to keep true, all for bytes
identical to what Docker Hub already serves. The moment something outside the workflow depends on it, it is
not staging.

**The consumer-side argument for a public package is spent.** It was that a deployment host could pull a beta
from GHCR with no `docker login`. That held only while a prerelease stopped at staging, and it stopped being
true on 2026-08-11 when the prerelease skip was reversed - every beta now reaches Docker Hub under its
immutable tag. Nothing exists on GHCR that Docker Hub does not have.

**Why GHCR specifically.** `GITHUB_TOKEN` is minted per run and expires with it, so staging needs no stored
credential and nothing to rotate. Keeping Docker Hub free of anything unsmoked or unreleased is what the second
registry buys, and it does that without adding a secret anywhere.

**The package is private, and nothing in the pipeline minds.** Both jobs that reach GHCR log in with
`GITHUB_TOKEN` - `build` to push and `publish` to read the manifest it copies - and `shared-smoke-image.yml` logs in
the same way before pulling the staging tag. Public was only ever load-bearing for readers outside the
workflow, and there are none left. Private also removes the last public pointer at GHCR that this repo does
not control: the package's own page, which advertises a `docker pull ghcr.io/...` line.

**It arrived by the move rather than by a flip.** GHCR defaults a new package to private, and the package at
`ghcr.io/binacle-labs/binacle-net` was created fresh when the repository moved - see `$decisions#D1`. The old
`ghcr.io/chrismavrommatis/binacle-net`, which had been set public after its first run, was deleted on
2026-08-16. `3.0.0-beta.3` then ran `build`, `smoke` and `publish` green against the private package, so all
three jobs are proven against it.

**Nothing deletes the staging copy, and that is deliberate.** It is the rollback source if a Docker Hub tag is
ever found bad — the exact bits that were smoked, still addressable by digest. The second reason is failure
mode: a cleanup step inside the release path can fail, and a release that goes red *after* the image is
published is the worst outcome the ordering exists to avoid. If the package ever needs pruning, it happens on
its own schedule, not in this workflow.

**The workflow creates the package on its own** — `packages: write` is enough to create one in the repo's
namespace, and the `Dockerfile`'s `org.opencontainers.image.source` label is what links it back. An earlier
version of this decision claimed a manual first push was required; it is not. The `permission_denied` failure
that claim came from is real but narrower — it happens when a package already exists in the namespace
*unlinked*, from a personal token or a recreated repo.

### D4 — a workflow step calls a `just` recipe, it does not inline the command

**Why:** the release workflow used to inline `dotnet restore` + `dotnet publish` while `tooling/build.just`
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
layout change and broke the publish. The project and output folder are decided in `tooling/build.just`, and
there is exactly one `Dockerfile`, at the repo root.

What legitimately stays a variable is a value with **no** home in the repo: the SDK version, the Docker Hub
coordinates, the Sonar project key.

### D6 — `shared-smoke-image.yml` pins `ubuntu-24.04`, everything else takes `ubuntu-latest`

**Why:** hurl links `libxml2.so.2`. Ubuntu 26.04 ships only `libxml2.so.16` and carries no compat package, so
hurl dies there with a missing-library error that reads like a hurl bug rather than a distro change.
`ubuntu-latest` will move to 26.04 eventually, and this workflow runs rarely enough that it would break on the
day it is needed most.

### D7 — `CHANGELOG.md` is the single source of release notes, and a missing section is fatal

`gh release create --notes-file`, with the body produced by `just changelog extract <section>`. The `notes` job
proves the section exists before anything is built. There is **no fallback to generated notes.**

**Why a changelog and not a per-release file.** The body used to come from `.agents/release-notes-<tag>.md`,
which was body-only so it could be published whole — that part was right and is kept. What was wrong is where
it lived: `.agents/` deletes a release's companions once the version ships, so the notes source was a file whose
own contract guaranteed it would disappear. A published release body is a permanent record, and it belongs in a
permanent file at the repo root where users read it.

**Why one section accumulates per cycle.** Betas publish `## [Unreleased]`; renaming that heading to the
version is the last edit before the real tag. A beta's notes are the in-progress notes at that moment, not a
version of their own, which is also why prereleases are excluded from the file — the GitHub releases stay as
the record of what each beta said.

**Why no fallback.** The old `--generate-notes` fallback existed because a prerelease normally had no written
body. Under the current shape a prerelease publishes `[Unreleased]`, which always exists mid-cycle, so the
fallback's only remaining effect would be to let a *real* release silently publish a commit list as its body.
Failing the build in seconds is the better outcome, and it is checked first for exactly that reason.

**Why the parsing is a `just` recipe and not inline YAML.** Same reason as D4 — CI and a laptop must read the
file the same way, and the exact body has to be previewable before the tag is pushed. A section terminates at
the next heading that *parses as a version*, not at the next `## `, because bodies carry their own subheadings
and stopping at those would truncate every section at its first one.

**Heading depth is normalised in the file and restored on the way out.** A release is `##` and its own sections
are `###`, so the file nests under a single `# Changelog`. `extract` shifts each section so its shallowest
heading returns to `##`, since a release body has no parent heading. Deriving the shift from the section's own
minimum keeps relative depth intact and means nothing has to be recorded anywhere.

### D8 — Sonar analysis is a CI run, and Automatic Analysis stays off

**Why:** coverage requires a build. Automatic Analysis only reads source, so it can never report coverage, and
the two fight if both are on. The build and the coverage run must sit between `Sonar begin` and `Sonar end` —
the scanner only sees projects compiled inside that pair — and a failing suite skips `Sonar end`, so a broken
run publishes nothing. That last part is deliberate, not a bug.

Two mechanical consequences: the checkout needs `fetch-depth: 0`, because a shallow clone makes all code look
new to the new-code comparison; and the scanner is a Java program whatever language it analyses, so the job sets
up a JDK.

**Scope and coverage paths live in `tooling/sonar-analysis.xml`, not in the workflow.** The Scanner for .NET
ignores `sonar-project.properties`, so that XML is the file form it reads, and `/s:` needs an absolute path.
Only the key, org, token and host stay in the YAML.

### D9 — the Postgres service in `shared-test-suite.yml` carries no password

`POSTGRES_HOST_AUTH_METHOD: trust`, and no `POSTGRES_PASSWORD`.

**Why:** the container lives for one job, is reachable only from that job, and is thrown away after. There is
nothing for a password to protect. Under `trust` it accepts any password, so the integration tests connect
unchanged using the shared local-dev connection string.

**This removed the credential from that file, not from the repo.** The same local-dev password is still in
`tooling/serve.services.yml`, `tooling/image.full.yml`, the test default in `BinacleApi.cs` and
`Config_Files/ServiceModule/ConnectionStrings.Development.json`, where it is load-bearing. It is the same value
everywhere on purpose. Change it in all of them or none.

### D10 — `npm ci --ignore-scripts`

**Why:** an install-time lifecycle hook is arbitrary code execution from a dependency. Nothing here needs one —
no workspace declares `prepare` or `postinstall`, and the only dependency with an install script is `fsevents`,
which is darwin-only and never installed on a Linux runner. The flag costs nothing and closes the hole.

### D11 — every action is pinned by commit SHA, and Dependabot keeps the pins moving

With the version in a trailing comment, so the pin is readable. `.github/dependabot.yml` raises a weekly PR per
action, rewriting the SHA and the comment together.

**Why:** a mutable tag is a supply-chain hole — it can be re-pointed at any commit, including after review. The
trailing comment is what keeps the pin maintainable; a bare SHA tells a reader nothing about how far behind it
is.

**Why first-party actions too, as of 2026-08-11.** `actions/*` and `docker/setup-*` were left on major tags on
the grounds that the publisher is trusted. That is a weaker rule than it looks: the risk a SHA pin addresses is
the tag being re-pointed, and `actions/checkout@v4` is exactly as re-pointable as any other tag. Two rules also
meant every reader had to know which action fell under which. One rule, applied to all six workflows.

**The pin and the automation are one decision, not two.** A pinned action with nothing watching it stays on
whatever commit it was set to and stops receiving security fixes, which is worse than a floating tag because
nothing reports it. `docker/build-push-action` sat at v5.4.0, several majors behind, which is what made the
point concrete.

**The pins stayed in the composite actions; the config grew instead, on 2026-08-19.** Four outside SHAs moved
into `.github/actions/` with the workflow restructure, and Dependabot does not reach that folder from
`directory: /` — it covers `.github/workflows` and a root-level `action.yml`, and nothing else. The open
question was whether to answer that by pulling the four pins back out into a workflow file. **No:** the point
of the composite actions is that a setup step is written once, and undoing that to satisfy a config format is
the tail wagging the dog. `.github/dependabot.yml` carries one entry per action folder instead. The cost is
that adding an outside pin to a new action means remembering to add an entry — which is why the rule is
written down in `$ci-cd` beside the actions themselves, not only here.

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

### D15 — the image carries an SBOM and provenance, and is signed keyless

`build-push-action` gets `provenance: mode=max` and `sbom: true`; `cosign sign` runs against the digest with
no key, using the job's OIDC token.

**Provenance was already being produced, and the ledger said the opposite.** Inspecting the published
`v3.0.0-beta.1` on 2026-08-11 showed an OCI **image index**: the amd64 manifest plus an `unknown/unknown`
manifest annotated `vnd.docker.reference.type: attestation-manifest`, carrying an in-toto document with
predicate type `https://slsa.dev/provenance/v1`. That is buildx's default in Actions. Nothing in this repo
asked for it, which is exactly how it came to be written down as absent. Stating it in the workflow makes it a
choice rather than a default that can change underneath us.

**`mode=max` over the default `min`** records the full build definition rather than just the materials. The
only build arg is `VERSION`, so nothing secret is captured — adding a secret-bearing build arg means revisiting
this.

**Why signing is separate from attestation, and why it happens twice.** The SBOM and provenance say how the
image was built; without a signature they do not prove the record itself was not altered. cosign closes that.
But a cosign signature is **not** a manifest inside the index — so unlike the attestations it does not travel
with the copy in D2. The staging image is signed on GHCR and the published image is signed again on Docker Hub,
so the copy users pull verifies. **The Docker Hub signature is the load-bearing one; since D14 nothing outside
the release workflow reads the staging signature at all.** Removing the `build` job's cosign step is a live
question and deliberately not release work - it deletes a step from the path a tag runs, for no gain beyond
tidiness. (The parenthetical here used to say GHCR was the only
place a beta ever exists; that stopped being true on 2026-08-11 when the prerelease skip was reversed, and
every beta now reaches Docker Hub under its immutable tag.) Signing the **digest** rather than a tag means one signature covers
`x.y.z`, `x.y` and `latest`, since all three are aliases of it.

**Corrected 2026-08-11, against the real artifact.** This said the signature lands in a `sha256-<digest>.sig`
tag. That is the older cosign scheme and it is not what happens here. `sigstore/cosign-installer` v4.1.2
installs a cosign that attaches the signature as an **OCI 1.1 referrer**: a manifest whose `subject` is the
index digest, one layer of `artifactType` `application/vnd.dev.sigstore.bundle.v0.3+json`, reachable through
the referrers API and by the fallback tag `sha256-<digest>` with no suffix. Verified by walking the GHCR
manifests for `v3.0.0-beta.2`.

The correction does not move the decision — a referrer is still outside the index and still does not survive
`imagetools create`, so signing twice is still required. It matters because anyone auditing the registry for a
`.sig` tag will not find one and may conclude the image is unsigned.

**A second way to reach that wrong conclusion, found 2026-08-13 on the published beta 2.** Docker Hub answers
the referrers API for the signature; **GHCR answers it with a 404**, so the same query returns nothing there.
The signature is present - it is in the GHCR tag list as `sha256-<digest>` and `cosign verify` passes against
both registries. Only a failed verify is evidence of an unsigned image; an empty referrers response is not.

**The verify invocation is copied to several surfaces on purpose, and one thing changes it.** The same
`cosign verify` - identity regexp plus issuer - now lives in `CHANGELOG.md`, `SECURITY.md`, the `image.just`
recipe and the docs site, and is headed for the Docker Hub page. That repetition is deliberate: each audience
arrives somewhere different, and a link instead of the command defeats the point. **The only things that
change it are renaming `.github/workflows/release-docker-image.yml` or moving the repository** - both rare,
both visible in a diff. If either happens every copy changes together, and the certificate-identity regexp is
the part that breaks; the issuer flag never moves. `SECURITY.md` is the wording the others follow.

**The second of those happened on 2026-08-16**, when the repository moved to the `binacle-labs` organization,
and it played out as written: every copy of the regexp changed together, the issuer flag did not move, and
`SECURITY.md` led. It is worth reading as evidence rather than as prediction - the cost of the move was five
edits and one beta to prove them, because the copies were listed here before anyone needed the list.

**Signing starts at `3.0.0-beta.2`**, along with the SBOM and the GHCR staging copy. Everything earlier
answers `no signatures found`, and that is history rather than a broken check. **Which images verify under
which identity is `$decisions#D3`** - the move split the signed images into two bands, and every example on
every surface has to name one that passes today.

**Keyless, so there is no key.** cosign exchanges the job's OIDC token for a short-lived certificate, which is
why both jobs need `id-token: write` and why this adds no secret to the repo. `sigstore/cosign-installer` comes
from the sigstore org itself rather than an individual, which is the standard this is adhering to in the first
place.

**Verified on 2026-08-11, not assumed.** A throwaway image built with both flags produced a single attestation
manifest carrying two in-toto layers — `https://spdx.dev/Document` and `https://slsa.dev/provenance/v1` — and a
cross-registry `imagetools create` of that index came out on the digest it went in with, attestations intact.

**What this obliges.** Users have no way to verify what they are not told about. Publishing signed images
without a documented `cosign verify` invocation, including the certificate identity and OIDC issuer to match
against, is decoration. That page is owed and is not written yet.

### D16 — lychee is installed as a pinned binary, not through its own action

`.github/actions/install-lychee` curls the release and checks its SHA-256, and the workflow step is
`just check links <site>`. **`lycheeverse/lychee-action` exists and is maintained, and was still not used.**

**Why:** the action runs lychee itself from `args:` in YAML. The flags that decide what the check *is* —
`--offline`, `--root-dir`, `--config` — would then live in the workflow as well as in `tooling/check.just`, and
the two would drift. `just check links docs` on a laptop and the CI step would stop being the same check while
continuing to look like it. **This is D-nothing-new: it is the first convention in `$ci-cd`** — a step calls a
recipe — applied where the obvious answer pointed the other way.

**What it costs:** about twenty lines of curl and checksum, copied from `install-hurl`, and lychee's version
now lives in two places (that action and `DEVELOPMENT.md`) rather than being bumped by Dependabot. That is the
same trade already accepted for `hurl` and `container-structure-test`, and the same watch item applies.

**Where the action would win, if it is ever wanted:** a scheduled external run. `just check links-external` is
deliberately not a gate — it reports on other people's servers — and the useful shape for it is a monthly run
that opens an issue, which the action supports directly and a `run:` step does not. Different job, different
tool; that would be an addition, not a reversal of this.

**The check is `--offline` in CI, and that is not a preference either.** Every page carries a `canonical` and
an `og:url` pointing at where it *will* live. Run externally from the build job, they 404 on every page the
deploy is about to create — 35 of 36 failures on the first real run, all of them self-references. A gate red
for that reason before anyone writes a line is a gate people learn to ignore.

### D17 — the two site deploys are published by hand, and never on a push

**Decided by the maintainer, 2026-08-19.** `deploy-docs-site.yml` and `deploy-web-site.yml` are
`workflow_dispatch` and stay that way. No `push` trigger on `docs/**` or `web/**`, and no scheduled run.

**Why:** publishing to the internet is a deliberate act, not a side effect of a commit. Those two folders are
written in their own session, and pressing the button is part of how that session ends — a merge that happens
to touch a page is not a decision to put it live.

**Two mechanical consequences that make the same point.** The marker tag is numbered by `github.run_number`, so
a push trigger would produce a tag per commit and the tag would stop meaning "this is live". And the
concurrency group is never cancelled — `cancel-in-progress: false`, because a stopped deploy leaves App
Platform mid-rollout — so a busy branch would queue rollouts behind each other rather than skip to the last.

**This closes the question `$plans/ci-cd/workflow-restructure` left open.** It was not CI's to answer.

## Open

### O1 — a prerelease cannot test the publish step, and this got worse

**Mostly closed by the D3 reversal on 2026-08-11.** For part of that day `publish` was skipped entirely for a
prerelease, which meant the Docker Hub login, the copy and the release-side signature were all first exercised
by a real release. That is no longer true: `publish` now runs for every tag, so a beta proves the job's
credentials, its wiring, the cross-registry copy and the signature.

**What is still untested is narrower: the moving tags.** A prerelease produces only its immutable tag, so
`{{major}}.{{minor}}` and `latest=auto` firing correctly — and `imagetools create` being handed three
references instead of one — are first proven on the release itself. That is one extra argument to a command
that will have run several times by then.

Whether that residual deserves a throwaway-tag run is a judgement call rather than an obvious yes. If it is
done, note the two traps: a tag containing a hyphen is treated as a prerelease and proves nothing, and a clean
`v0.0.1` against the real repo **would move `latest`**, because metadata-action never queries the registry and
`latest=auto` marks any non-prerelease semver as latest. Point `DOCKERHUB_REPO` at a scratch repo instead.

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

### O3 — multi-arch is still absent

`linux/amd64` only. No second architecture is built, and nothing asks for one yet.

It stays out because it **changes the artifact** and roughly doubles build time, and because there is no
evidence of demand. Attestation and signing, which used to share this entry, are now done — see D15.
