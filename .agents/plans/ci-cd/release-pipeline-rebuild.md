---
description: CI/CD - finish the GHCR release pipeline
paths:
  - ".github/workflows/**"
---

# CI/CD - finish the GHCR release pipeline

**Status: the pipeline is built and proven.** `v3.0.0-beta.2` ran the whole thing on 2026-08-11 and every job
did what it was meant to. What it built - the changelog, the changelog recipes, the six-job workflow, the
supply-chain flags and the signing - is described in the ci-cd docs and the decisions ledger now, so it is not
repeated here. **This file is down to the four things that have not happened.**

**Timing note.** The original plan said to do this after v3.0.0 shipped. The maintainer decided on 2026-08-11 to
do it before, so beta 2 became its first run rather than v3.0.0.

## 1. Confirm the credential-free pull from the deployment host

- [ ] Proven from one machine with no `ghcr.io` entry in its docker config. Not yet from the deployment host,
      which is the one that matters.

## 2. One open question - ask the maintainer, do not decide alone

- **The docs site release-notes page.** Whether it is generated from `CHANGELOG.md` or stays hand-copied. It is
  a docs decision and repo-root `docs/` is off limits here - write down what the page must say and leave it.

## 3. Owed: tell users how to verify the signature

The images are now signed and carry an SBOM and provenance, and **nothing tells anyone how to check that**. An
unverifiable signature is decoration. What the page has to give is the exact `cosign verify` invocation with
the certificate identity and the OIDC issuer to match against, and the `docker buildx imagetools inspect`
command that shows the attestations.

This is user-facing writing, so it belongs on the docs site, which is off limits from a coding session. Whoever
writes it needs the real values from the first signed run - the certificate identity is the workflow's own
ref, so it is not knowable until a tag has actually been pushed.

## 4. Before a real release - the moving-tag gap

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

- A release tag builds, smokes on GHCR, copies to Docker Hub as `x.y.z`, `x.y` and `latest` - all three on the
  digest the smoke job passed - and creates a GitHub release from the version's `CHANGELOG.md` section.
- The deployment host pulls an image with no credentials.
- Users have a page telling them how to verify the signature.

## Do not

- Rebuild the image in the publish job. Copy by digest or the smoke proves nothing.
- Reword any historical release body in the changelog.
- Let a real release fall back to generated notes.
- Add `release: published` as a second trigger.
- Touch repo-root `docs/` or `web/`.
