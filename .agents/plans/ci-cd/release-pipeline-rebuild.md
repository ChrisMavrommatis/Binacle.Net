# CI/CD - finish the GHCR release pipeline

**Status:** The pipeline itself landed 2026-08-11, in the working tree, unshipped. What is left is the
maintainer setup it depends on, two open questions, and one handoff. **Read the first section before pushing
any tag** - the pipeline cannot work until the GHCR package exists.

**Timing note.** The original plan said to do this after v3.0.0 shipped. The maintainer decided on 2026-08-11 to
do it before v3.0.0 and before beta 2, so **beta 2 is the first run of this pipeline** rather than of the old
one. That is what makes Phase 4 below blocking rather than housekeeping.

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

## 1. GHCR setup - one manual step, and it comes AFTER the first run

**Corrected 2026-08-11.** This section used to say the package had to be pushed by hand first and that the
pipeline would not work without it. That is wrong on both counts, and it would have sent the maintainer looking
for a setup screen that does nothing.

**The workflow creates the package itself.** The build job pushes with `packages: write`, which is enough to
create a package in the repo's own namespace, and the `Dockerfile` already carries
`org.opencontainers.image.source` pointing at this repo - the label GHCR uses to link a package to it. The smoke
job then pulls it back with the same run's token. So the whole pipeline works against a package nobody has
touched.

The `permission_denied` failure is real but narrower than stated: it happens when a package **already exists**
in the namespace unlinked - pushed earlier from a personal token, or left behind by a deleted and recreated
repo. It is not what a first push does.

- [ ] **Set the package visibility to public** - the only manual step, and it cannot happen until the first
      pipeline run has created the package. GHCR defaults every new package to private regardless of repo
      visibility.
- [ ] Confirm `docker pull ghcr.io/chrismavrommatis/binacle-net:<tag>` works from the OVH server with no
      `docker login` at all. This is what the public package buys, and the only way to know it worked.

## 2. Two open questions - ask the maintainer, do not decide alone

- **Where the beta instructions point.** `samples/` and `README.md` pin a Docker Hub beta tag today, which this
  design makes impossible - a beta only ever exists on GHCR. Someone has to decide what the beta instructions
  say. This is now live, not hypothetical: beta 2 will not be on Docker Hub.
- **The docs site release-notes page.** Whether it is generated from `CHANGELOG.md` or stays hand-copied. It is
  a docs decision and repo-root `docs/` is off limits here - write down what the page must say and leave it.

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

## 5. Before a real release - the throwaway-tag check

A prerelease skips the whole `publish` job, so the Docker Hub login and the copy are both first exercised by a
real release. Push a disposable tag to the Docker Hub repo, confirm the copy lands on the smoked
digest, then delete it. This is O1 in the decisions ledger, and it got wider with this rebuild.

## Done when

- A beta tag builds, smokes on GHCR, creates a prerelease GitHub release from `[Unreleased]`, and puts
  **nothing** on Docker Hub.
- A release tag does all of that plus copies to Docker Hub as `x.y.z`, `x.y` and `latest`, all three on the
  digest the smoke job passed.
- The OVH server pulls a beta with no credentials.

## Do not

- Rebuild the image in the publish job. Copy by digest or the smoke proves nothing.
- Reword any historical release body in the changelog.
- Let a real release fall back to generated notes.
- Add `release: published` as a second trigger.
- Touch repo-root `docs/` or `web/`.
