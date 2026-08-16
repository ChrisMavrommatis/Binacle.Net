---
description: CI/CD - finish the GHCR release pipeline
paths:
  - ".github/workflows/**"
---

# CI/CD - finish the GHCR release pipeline

**Status: the pipeline is built and proven.** `v3.0.0-beta.2` first ran it on 2026-08-11, under the rule that
skipped `publish` for a prerelease; the tag was **re-cut on 2026-08-13** at `d317cd2b` and that run exercised
all six jobs, the Docker Hub copy and the release-side signature included. What it built - the changelog, the
changelog recipes, the six-job workflow, the supply-chain flags and the signing - is described in the ci-cd
docs and the decisions ledger now, so it is not repeated here. **This file is down to what has not happened:**
moving the deployment host onto Docker Hub, one open question, the signature-verification page (now owned
elsewhere), and the moving-tag gap.

**Why a beta ran it first.** A prerelease tag is the only free test this pipeline gets - a failure costs a
deleted tag instead of a bad release. Beta 2 was that run.

## 1. Move the deployment host onto Docker Hub

**Rewritten 2026-08-15, and the question it used to ask no longer matters.** The decisions ledger now says
Docker Hub is the only registry anyone is pointed at, and GHCR is staging that nothing public names. Our own
test server is included - a deployment pulling from staging is the same thing the decision rules out, and it
would keep the second registry alive as a real dependency.

**Closed 2026-08-17 - no host pulls from GHCR.** Confirmed by the maintainer. The GHCR package was deleted on
2026-08-16 and nothing broke, which is the proof by removal this section was after.

- [x] **The host pulled `3.0.0-beta.2` from GHCR on 2026-08-14.** That was the one time, and it is history.
- [x] **Repointed at `binacle/binacle-net`.** Every beta reaches Docker Hub under its immutable tag, so there
      was never anything the host could only get from staging.
- [x] **No `ghcr.io` credential left on that host.** It closed by removal rather than by measurement -
      the staging package no longer exists to pull from.

## 2. The docs release-notes page - answered 2026-08-14

**It stays hand-copied, and it gets updated by hand as part of each docs deploy.** Not generated from
`CHANGELOG.md`.

That closes the last open question in this file. **The cost is known and accepted:** the same release notes live
in two places and they drift - v3.0.0's docs deploy carries three edits that exist purely because the release
body gained content the page never got. **So the deploy checklist is the control**, and every release's docs
handover has to list what changed in the changelog since the page was written.

## 3. Owed: tell users how to verify the signature - moved out on 2026-08-14

The images are signed and carry an SBOM and provenance, and **nothing tells anyone how to check that**. An
unverifiable signature is decoration. That work is real and still owed, but **it is no longer specified here.**

It grew a second half this file has no business holding - a `just` recipe for running the checks, and five
user-facing surfaces rather than the one docs-site page this section assumed. **The image-verification work owns
all of it now**, including the verified `cosign verify` invocation this section used to carry, the three points
the docs-site page has to make, and the rule that the tag in any example must name a signed image.

**Nothing was lost in the move** - the invocation, the digest-covers-every-tag point, the attestations-survive-
the-copy point and the GHCR-referrers-404 point are all carried over verbatim. This section is left as a stub
rather than deleted because the pipeline work is what created the debt, and a reader arriving here should be
told where it went rather than concluding it was dropped.

## 4. Before a real release - the moving-tag gap

**Much smaller than it was, after the 2026-08-11 reversal, and beta 2 has since proved the rest.** `publish`
runs for every tag, and the 2026-08-13 run did the Docker Hub login, the credential, the cross-registry copy
(digest preserved: `sha256:ccce2a44` on both registries) and the release-side signature, all verified from
outside.

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
- The deployment host pulls from Docker Hub, with no credentials and no `ghcr.io` entry left in its docker
  config.
- Users have a page telling them how to verify the signature.

## Do not

- Rebuild the image in the publish job. Copy by digest or the smoke proves nothing.
- Reword any historical release body in the changelog.
- Let a real release fall back to generated notes.
- Add `release: published` as a second trigger.
- Touch repo-root `docs/` or `web/`.
