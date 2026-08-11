# CI/CD - finish the GHCR release pipeline

**Status: the pipeline is built and proven.** `v3.0.0-beta.2` ran the whole thing on 2026-08-11 and every job
did what it was meant to - `notes` 9s, `test` 2m08, `build` 1m23, `smoke` 26s, **`publish` skipped**, `release`
9s. Verified independently afterwards: Docker Hub untouched (`latest` still on the January digest, no
`3.0.0-beta.2`, no `3.0`), the GHCR package public and pullable with no credential, `BINACLE_VERSION` correct,
SBOM and provenance in the index, the signature attached as an OCI referrer, and the release body byte-identical
to `just changelog extract Unreleased`.

**What is left is not the pipeline.** One untested job, two open questions, and one owed doc. All below.

**Timing note.** The original plan said to do this after v3.0.0 shipped. The maintainer decided on 2026-08-11 to
do it before, so beta 2 became its first run rather than v3.0.0.

## What landed

- `CHANGELOG.md` at the repo root. 23 release sections, newest first, bodies copied word for word from the
  published GitHub releases. Prereleases excluded. `## [Unreleased]` holds the v3.0.0 notes.
- Headings are nested: a release is `##`, its own sections are `###`, under a single `# Changelog`.
  `just changelog extract` shifts each section back so its shallowest heading is `##` again, because a release
  body has no parent heading. Verified: 19 of the 23 sections round-trip byte-identical to what GitHub
  published, and the four that do not are the old ones that used `# Overview`, which now come out as `##` like
  every other release.
- `config/changelog.just`, registered in the root `justfile`. `extract` and `check`.
- `.github/workflows/release-docker-image.yml` rebuilt: six jobs, GHCR staging, `imagetools create` copy to Docker Hub,
  `publish` skipped for any tag with a hyphen.
- `workflow_call` added to `run-tests.yml`. A guarded GHCR login added to `smoke-image.yml`.
- `.github/dependabot.yml`, every action SHA-pinned, `timeout-minutes` on all nine jobs.
- **Supply-chain standards**: `provenance: mode=max` and `sbom: true` on the build, plus cosign keyless
  signing of the digest on both registries. Verified locally: the two flags produce one attestation manifest
  carrying an SPDX SBOM and SLSA provenance v1, and a cross-registry copy preserves the digest with both
  intact. Provenance turned out to have been on all along by buildx default - the ledger had recorded it as
  absent, which is now corrected.
- The ci-cd docs and the decisions ledger rewritten for the above.

## 1. ~~GHCR setup~~ Done 2026-08-11

The workflow created the package itself on its first push - `packages: write` is enough in the repo's own
namespace, and the `Dockerfile`'s `org.opencontainers.image.source` label is what links it. No manual push was
needed, contrary to what this section originally claimed.

Visibility is public: verified by pulling `3.0.0-beta.2` from a machine with no `ghcr.io` entry in its docker
config at all.

**Worth keeping:** the `permission_denied` failure this section used to warn about is real but narrow - it
happens when a package **already exists** in the namespace unlinked, from a personal token or a deleted and
recreated repo. It is not what a first push does.

- [ ] Confirm the credential-free pull from the deployment host specifically. Proven from one machine, not
      from that one.

## 2. One open question - ask the maintainer, do not decide alone

- **The docs site release-notes page.** Whether it is generated from `CHANGELOG.md` or stays hand-copied. It is
  a docs decision and repo-root `docs/` is off limits here - write down what the page must say and leave it.

~~Where the beta instructions point.~~ **Answered 2026-08-11 by removing the prerelease skip.** Betas now reach
Docker Hub with their immutable tag, so `samples/` and `README.md` can keep pinning a Docker Hub beta exactly as
they always have. Nothing to decide.

## 3. ~~Handoff - the old release-notes file~~ Done 2026-08-11

`.agents/release-notes-v3.0.0.md` was a byte-identical duplicate of the `[Unreleased]` section of
`CHANGELOG.md`. It has been **deleted**, and the release plan, the post-release list and the release-notes
section were rewritten to point at the changelog instead. There is one source again.

**Edit `CHANGELOG.md`.** Nothing else feeds the release body.

## 4. Owed: tell users how to verify the signature

The images are now signed and carry an SBOM and provenance, and **nothing tells anyone how to check that**. An
unverifiable signature is decoration. What the page has to give is the exact `cosign verify` invocation with
the certificate identity and the OIDC issuer to match against, and the `docker buildx imagetools inspect`
command that shows the attestations.

This is user-facing writing, so it belongs on the docs site, which is off limits from a coding session. Whoever
writes it needs the real values from the first signed run - the certificate identity is the workflow's own
ref, so it is not knowable until a tag has actually been pushed.

## 5. Before a real release - the moving-tag gap

**Much smaller than it was, after the 2026-08-11 reversal.** `publish` now runs for every tag, so a beta
already proves the Docker Hub login, the credential, the cross-registry copy and the release-side signature.

What a beta still cannot cover is the **moving tags**: it produces only its immutable tag, so `3.0` and
`latest` are first created on the release itself. That is one extra argument to an `imagetools create` call
that will have run several times by then, so this is now a judgement call rather than a must.

**If you do test it, two traps.** A tag containing a hyphen is a prerelease and produces no moving tags, so it
proves nothing. A clean `v0.0.1` against the real repo **would move `latest`** - metadata-action never queries
the registry, and `latest=auto` marks any non-prerelease semver as latest. Point `DOCKERHUB_REPO` at a scratch
repo, tag `v0.0.1`, check the three tags land on the smoked digest, then delete everything and **point
`DOCKERHUB_REPO` back**.

## Done when

- A beta tag builds, smokes on GHCR, creates a prerelease GitHub release from `[Unreleased]`, and puts
  **nothing** on Docker Hub.
- A release tag does all of that plus copies to Docker Hub as `x.y.z`, `x.y` and `latest`, all three on the
  digest the smoke job passed.
- The deployment host pulls a beta with no credentials.

## Do not

- Rebuild the image in the publish job. Copy by digest or the smoke proves nothing.
- Reword any historical release body in the changelog.
- Let a real release fall back to generated notes.
- Add `release: published` as a second trigger.
- Touch repo-root `docs/` or `web/`.
