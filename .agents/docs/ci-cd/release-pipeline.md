---
id: "ci-cd/release-pipeline"
description: "The release pipeline in release-docker-image.yml — six jobs from a pushed tag to a published GitHub release, GHCR as the staging registry, the copy-to-Docker-Hub step every tag reaches with a prerelease narrowed to its immutable tag, and the CHANGELOG.md release body"
verified: "2026-08-13"
check: "The six jobs, their needs: edges and job outputs match release-docker-image.yml; no job carries a prerelease condition and the release job's !failure() note is still accurate; run-tests.yml and smoke-image.yml still expose workflow_call; `just changelog check` and `extract` still take a bare version or Unreleased"
also_update:
  - ci-cd
  - tooling
paths:
  - ".github/workflows/**"
---

# The release pipeline

`release-docker-image.yml`. One pushed tag produces a smoked image on GHCR, a copy of that exact image on
Docker Hub, and a GitHub release. The order is cheapest check first, so nothing that cannot be undone happens
until the things that can be checked cheaply have passed.

## The flow

```
git push origin v3.0.0
  |
  v  on: push: tags: 'v[0-9]*'
notes     the CHANGELOG.md section this tag publishes exists and is not empty   (seconds)
test      the whole suite, by calling run-tests.yml                             (minutes)
build     just build publish, push the immutable tag to GHCR, capture the digest
smoke     pull that digest back from GHCR, structure check + all five profiles
publish   imagetools copy to Docker Hub - a prerelease gets its immutable tag only
release   gh release create, body from CHANGELOG.md
```

Each job `needs:` the ones before it, so a red anywhere leaves Docker Hub untouched and creates no release.

## The rule the shape exists to enforce

**Nothing unsmoked reaches Docker Hub.**

That comes from job ordering, not from the registry split: `smoke` runs against the staging copy and only a
digest that passed is ever copied across. GHCR is staging; Docker Hub is what users pull, and it carries every
tag the pipeline publishes — betas included, with the immutable tag only.

## The six jobs

**`notes`** — checkout, `just`, then work out which section this tag publishes and check it exists. A tag
containing a hyphen publishes `Unreleased`; any other tag publishes its own version with the leading `v`
stripped. `just changelog check <section>` fails if the section is missing or empty. Job output: `section`.

This runs first, and everything waits on it. The alternative is finding out at the end, with the image already
on Docker Hub and `latest` already moved.

**`test`** — `uses: ./.github/workflows/run-tests.yml`, no inputs. Nothing guarantees a tag sits on a commit
that passed CI, because a tag can be pushed at anything; this is that guarantee. It runs after the notes gate
rather than beside it, so a missing section is reported in seconds instead of after a full suite.

**`build`** — checkout, .NET, `just`, then `just build publish`. One `docker/metadata-action` step, a GHCR
login with `GITHUB_TOKEN`, buildx, and one `docker/build-push-action` that pushes the immutable tag to GHCR
with `provenance: mode=max` and `sbom: true`. `VERSION` is passed as a build arg from the metadata step's
stripped `version` output, not from `github.ref_name`, so `BINACLE_VERSION` inside the container never carries
the leading `v`. It ends by signing the pushed digest with cosign.

Job outputs: `staging` (the full `ghcr.io/...:tag` the smoke job pulls), `version` and `digest`.

**`smoke`** — `uses: ./.github/workflows/smoke-image.yml` with the `staging` output. It calls the same workflow
a maintainer runs by hand, rather than copying its steps, so the release path and a manual check are the same
thing. See `$ci-cd` for that workflow's runner pin.

**`publish`** — the only job that touches Docker Hub and the only place the stored Docker Hub credential is
used. A `metadata-action` step computes the public tag set, then one `docker buildx imagetools create` moves
the manifest **by digest** from GHCR under all three public tags at once, and cosign signs the result. It never
checks out, so it holds no `contents` permission.

**`release`** — checkout, `just`, then the release body from `just changelog extract <section>`. It `needs:`
the `notes` job because it reads that job's `section` output, and the prerelease flag is set explicitly either
way from whether the tag contains a hyphen.

**It creates the release, or edits one that already exists.** GitHub's web UI cannot create a bare tag — the
only way to tag from the site is to publish a release, which makes both at once — so by the time this job runs
the release may be there already. A plain `gh release create` would fail on that after every other job had
succeeded, leaving the image published and one red job. Editing instead means the body comes from
`CHANGELOG.md` whichever way the tag was made, and a release marked prerelease by hand is corrected for a real
version tag.

## Copy, never rebuild

`imagetools create` transfers a manifest, and a manifest is content-addressed, so the digest is preserved:
what Docker Hub serves is bit for bit what the smoke job passed. The copy source is the digest rather than the
tag, so the guarantee holds even if something re-tagged staging in between. All three tags go in one command,
because they are aliases of one manifest and the blobs need moving only once.

A second build in the publish job would ship an image nothing tested, however identical the inputs looked.

Smoking the registry copy rather than a locally loaded image is the point of the shape: compression, manifest
shape and attestation handling are exactly what a registry round trip changes.

## What ships alongside the image

The pushed artifact is an OCI **image index**, not a single manifest: the `linux/amd64` manifest plus an
`unknown/unknown` attestation manifest carrying two in-toto documents.

| | Predicate | Produced by |
|---|---|---|
| SBOM | `https://spdx.dev/Document` | `sbom: true` on `build-push-action` |
| Provenance | `https://slsa.dev/provenance/v1` | `provenance: mode=max` on the same step |

Both are manifests inside the index, so they travel with the copy to Docker Hub.

**The cosign signature does not.** It is a separate manifest rather than a child of the index, so the image is
signed twice — once on GHCR in `build`, once on Docker Hub in `publish`.

Specifically, cosign attaches it as an **OCI 1.1 referrer**: a manifest whose `subject` points at the index
digest, carrying one layer of `artifactType`
`application/vnd.dev.sigstore.bundle.v0.3+json`, discoverable through the registry's referrers API and
addressable by the fallback tag `sha256-<digest>` — **no `.sig` suffix**. Observed on
`v3.0.0-beta.2`, 2026-08-11. The older cosign scheme put signatures in a `sha256-<digest>.sig` tag instead;
this repo does not use it, so do not go looking for one.

**The two registries expose it differently, and one of them looks broken.** Docker Hub serves the signature
through the referrers API; **GHCR answers `/v2/.../referrers/<digest>` with a 404**, so a referrers query there
returns nothing at all. The signature is present either way — on GHCR it is visible in the tag list as
`sha256-<digest>`, and `cosign verify` passes against both, checked on the published `3.0.0-beta.2`
(2026-08-13). An empty referrers response from GHCR is not evidence of a missing signature; only a failed
`cosign verify` is.

Either way the point stands: a referrer is not inside the index, so `imagetools create` does not carry it, and
the published image must be signed where it lands.

Signing is keyless — cosign exchanges the job's OIDC token for a short-lived certificate, which is why both
jobs declare `id-token: write` and why no signing key exists to store. The signature is made against the
**digest**, so one signature covers `x.y.z`, `x.y` and `latest` alike.

## How a prerelease differs

A hyphen in a semver tag is the prerelease marker. Every job runs either way; what changes is the tag set.

| | `v3.0.0` | `v3.0.0-beta.2` |
|---|---|---|
| Section the `notes` job checks | `3.0.0` | `Unreleased` |
| Pushed to GHCR | `3.0.0` | `3.0.0-beta.2` |
| `publish` job | runs | runs |
| Docker Hub tags | `3.0.0`, `3.0`, `latest` | `3.0.0-beta.2` only |
| GitHub release | normal | marked `--prerelease` |

**No job is conditional.** The narrowing is entirely `metadata-action`'s: it withholds `{{major}}.{{minor}}`
and `latest` for a prerelease, so a beta can never move a tag anyone is following.

**The consequence for testing:** a prerelease now exercises every job, `publish` included. What it still does
not cover is the *moving-tag* half — creating `3.0` and `latest` — since a beta produces neither. That is one
extra argument to the same `imagetools create` call, so the residual gap is much smaller than it was, but it
is not nothing: `latest=auto` firing correctly is first proven on a real release.

## Where the release body comes from

`CHANGELOG.md` at the repo root, newest version first, Keep a Changelog shape. One section accumulates per
cycle: betas publish `## [Unreleased]`, and renaming that heading to the version is the last edit before the
real tag.

The parsing lives in `tooling/changelog.just`, not in the workflow, so CI and a laptop read the file the same
way and the exact body can be previewed before the tag is pushed. See `$tooling` for the module.

Inside the file a release is `##` and its own sections are `###`, nesting under the single `# Changelog`.
`just changelog extract` shifts each section so its shallowest heading returns to `##`, because a release body
has no such parent. Relative depth inside the body is preserved and nothing has to be recorded anywhere.

A real release whose section is missing fails the `notes` job. There is no fallback to generated notes — that
would silently publish a commit list as the release body.

## Labels

Three sources, and they do not collide.

- **Constant labels are `LABEL` lines in the `Dockerfile`** — title, description, source, url, documentation,
  vendor, licenses, base.name.
- **Per-build labels are applied at build time**, never as `LABEL` fed by `ARG`: version, revision and created
  change every build, so as Dockerfile layers they would bust the cache from that point down. `--label` sets
  image-config metadata with no layer.
- **The `build` job's metadata step overrides two** that metadata-action gets wrong on its own: `licenses`,
  because auto-detection returns `NOASSERTION` for a dual-licensed repo, and `url`, which should be the landing
  site rather than the repo.

`tooling/build.just` does the same three per-build labels for a local `just build image`, so a locally built
image carries the same metadata shape a pushed one does.

## What still happens by hand

- **Deciding the tag and creating it.** The pipeline has no other entry point. Two ways, and both work:
  `git tag v3.0.0 && git push origin v3.0.0`, or *Releases → Draft a new release → Choose a tag → Create new
  tag on publish* on github.com. The web route also creates the release, which is why the last job edits rather
  than insists on creating.
- **Writing the `[Unreleased]` section of `CHANGELOG.md`** as the work lands, and renaming that heading to the
  version before the real tag.
- **The moving-tag check on a throwaway tag.** A prerelease reaches the `publish` job but produces only its
  immutable tag, so `3.0` and `latest` are first created on a real release.
- **Deploying the docs site**, which is its own `workflow_dispatch` workflow and is not chained to a release.
