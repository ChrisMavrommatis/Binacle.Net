---
id: ci-cd/release-pipeline
description: The release pipeline in release-docker-image.yml — four jobs from a pushed tag to a published GitHub release, the immutable-then-promote tag order, and how a prerelease differs
verified: 2026-08-11
check: The four jobs, their needs: edges and job outputs match release-docker-image.yml; the metadata-action tag patterns still produce the immutable/moving split; smoke-image.yml still accepts the same workflow_call input
also_update:
  - ci-cd
  - config
---

# The release pipeline

`release-docker-image.yml`. One pushed tag produces a smoked image on Docker Hub and a GitHub release, in an
order chosen so that nothing a user follows moves until the artifact has been tested.

## The flow

```
git push origin v3.0.0
  |
  v  on: push: tags: 'v*'
build     just build publish, then push the IMMUTABLE tag only (3.0.0) and capture its digest
smoke     pull that tag from the registry, structure check + all five profiles
promote   point 3.0 and latest at the same digest, by manifest, with no rebuild
release   gh release create, from the notes file
```

Each job `needs:` the ones before it, so a red smoke leaves the moving tags where they were and creates no
release.

## The four jobs

**`build`** — checkout, .NET, `just`, then `just build publish`. Two `docker/metadata-action` steps
(below), a Docker Hub login, buildx, and one `docker/build-push-action` that pushes the immutable tag alone.
`VERSION` is passed as a build arg from the metadata step's stripped `version` output, not from
`github.ref_name`, so `BINACLE_VERSION` inside the container never carries the leading `v`.

Job outputs: `image` (the full `repo:tag` the smoke job pulls), `repo`, `digest` (from the push step) and
`moving_tags`.

**`smoke`** — `uses: ./.github/workflows/smoke-image.yml` with the `image` output. It calls the same workflow a
maintainer runs by hand, rather than copying its steps, so the release path and a manual check are the same
thing. See `$ci-cd` for that workflow's runner pin.

**`promote`** — logs in, then one `docker buildx imagetools create` that points the moving tags at the digest
the smoke job just tested. `imagetools create` re-points an existing manifest: no pull, no rebuild, no second
push. Each moved tag is then read back with `imagetools inspect` and its digest printed, because this is the
least exercised command in the file. The step is guarded on `moving_tags` being non-empty; a second step prints
why there was nothing to do.

**`release`** — checks out, then `gh release create`. The body comes from `.agents/release-notes-<tag>.md` when
that file exists, and falls back to `--generate-notes` when it does not. A tag containing a hyphen gets
`--prerelease`.

## Immutable first, then promote

`3.0.0` is pushed and smoked before `3.0` and `latest` are moved onto it.

The trade is deliberate. The immutable tag is briefly public and unsmoked — but nobody follows an exact pin on
release day, since it did not exist a minute earlier. The moving tags are what the samples, the README and the
quick start tell people to follow, and those never point at anything unsmoked.

Smoking the registry copy rather than a locally loaded image is the point of the shape: compression, manifest
shape and attestation handling are exactly what a registry round trip changes.

## How a prerelease differs

Nothing in the workflow asks whether a tag is a prerelease. Two `metadata-action` steps do it by construction:

| Step | Tag pattern | `v3.0.0` | `v3.0.0-beta.2` |
|---|---|---|---|
| `meta-immutable` | `type=semver,pattern={{version}}` | `3.0.0` | `3.0.0-beta.2` |
| `meta-moving` | `type=semver,pattern={{major}}.{{minor}}` plus `flavor: latest=auto` | `3.0`, `latest` | *(empty)* |

metadata-action skips `{{major}}.{{minor}}` for a prerelease tag, and `latest=auto` withholds `latest` for the
same reason. So `moving_tags` comes out empty and the promote step is a natural no-op — the guard is the tag
pattern, not an `if:` somebody has to keep correct.

There is no `{{major}}` tag on purpose. A bare `3` would cross minor lines.

**The consequence for testing:** a prerelease exercises every step of this pipeline except promotion, which it
can never reach. That one command needs a separate check against a throwaway tag.

## Labels

Three sources, and they do not collide.

- **Constant labels are `LABEL` lines in the `Dockerfile`** — title, description, source, url, documentation,
  vendor, licenses, base.name.
- **Per-build labels are applied at build time**, never as `LABEL` fed by `ARG`: version, revision and created
  change every build, so as Dockerfile layers they would bust the cache from that point down. `--label` sets
  image-config metadata with no layer.
- **`meta-immutable` overrides two** that metadata-action gets wrong on its own: `licenses`, because
  auto-detection returns `NOASSERTION` for a dual-licensed repo, and `url`, which should be the landing site
  rather than the repo.

`config/build.just` does the same three per-build labels for a local `just build image`, so a locally built
image carries the same metadata shape a pushed one does.

## What still happens by hand

- **Deciding the tag and pushing it.** The pipeline has no other entry point.
- **Writing `.agents/release-notes-<tag>.md`** before the tag is pushed, if the release should have a written
  body. It is body only — no title line, no preamble — precisely so it can be published whole.
- **The promotion check on a throwaway tag**, since a prerelease cannot reach that step.
- **Deploying the docs site**, which is its own `workflow_dispatch` workflow and is not chained to a release.
