---
description: Post-release - the checks to run once Binacle.Net v3.0.0 is out
---

# Post-release - the checks to run once v3.0.0 is out

**Status:** Do these once v3.0.0 is tagged and the pipeline has run. **None of them gate anything** - the
release is already out by the time this file opens.

**This file is self-contained and it is checks only.** Every item is something you look at, run or read back.
No tooling to build, no decision to take, nothing to figure out, and **nothing here links to a plan** - if an
item needed a plan, it was not a post-release check.

**That is the test for anything proposed for this file.** If working it needs a decision, a credential, a new
file or a workflow, it belongs in the release plan while there is still time, or on the board if there is not.

**Delete this file once the list is clear.** The tag does not delete it; working through it does.

Rewritten 2026-08-14, when the release scope was reset.

---

## Confirm the release landed

- [ ] **Smoke the published image.** `just smoke all binacle/binacle-net:3.0.0`. About a minute, nothing to
      bring up.

      **This is a confirmation, not a safety net.** The pipeline smokes the GHCR copy before anything is copied
      across, so a broken image cannot reach Docker Hub. What this still buys is the one thing the pipeline
      cannot check: that the **copy** landed something runnable, not just something with the right digest.

- [ ] **Confirm `3.0` resolves on Docker Hub, and that `latest` moved.** Both are written for the first time by
      this release - every beta withheld them. Until now `latest` resolved to `2.1.1`.

- [ ] **Confirm `3.0.0` resolves, and that all three tags share one digest.**
      `docker buildx imagetools inspect binacle/binacle-net:3.0.0 --format '{{ .Manifest.Digest }}'`, then the
      same for `3.0` and `latest`. Three names, one hash, or the copy did not do what it claims.

- [ ] **Verify the signature and the attestations against the real `3.0.0`.**
      `cosign verify binacle/binacle-net:3.0.0` with **both** the certificate-identity regexp and the OIDC
      issuer - the identity flag is the entire value, since anyone with a GitHub account can sign anything.
      Then `docker buildx imagetools inspect` for the SBOM and provenance entries.

      **Docker Hub only.** An earlier version of this said to check both registries. It is the staging copy's
      signature that is now read by nothing - only the release workflow touches GHCR, decided 2026-08-15.

      All of this has run green against `3.0.0-beta.2` already. What is new at v3.0.0 is only that the copy
      writes three tags instead of one.

- [ ] **Read the repo landing page by eye.** `README.md` carried a beta-conditional sentence - *"Until then,
      pin `binacle/binacle-net:3.0.0-beta.1`"* - and it is the most read file in the repo. **A stale beta pin
      there outlives every other miss.** Note that both betas are still on Docker Hub, so a stale pin resolves
      to a real image and **will not fail loudly.** You have to look.

- [ ] **Confirm nothing froze.** The immutability rule was corrected before the tag and the switch was left
      off, so the publish should have written `3.0.0`, `3.0` and `latest` with no interference. Read
      `immutable_tags_settings` back from the repository API and check the corrected rule is still the value
      stored.

## Read what the release published

- [ ] **Read the Docker Hub page - the release already published it.** The page update runs inside the release
      workflow now, so there is nothing to trigger by hand. Look at the rendered page: the description names 3.x
      rather than `2.1.1`, the version placeholders were substituted with the real numbers, the hand-maintained
      tag list is gone, the verification section is there, and the logo and categories took.

      **This is an eyeball, not a rewrite.** If it turns into a rewrite, the pre-tag half did not happen.

      **This is the first run of that step**, and the first time the page goes from describing 2.x to describing
      3.x - so it is worth reading properly rather than glancing at.

- [ ] **Run the verification checks against the real `3.0.0`, from a clean shell.** They were proven against
      `3.0.0-beta.2` and against a two-tag copy. This is the first time the copy writes three tags. **Confirm
      the invocation printed on the Docker Hub page and in `SECURITY.md` is the one that actually works** - a
      published command that fails reads as our bug.

- [ ] **Check the docs site is on v3.0.x.** Confirm `/version/latest/` lands on `v3.0.x` and the version picker
      shows four versions. **This is the item most likely to have been silently skipped**, because nothing
      fails when it is - the site just keeps presenting v2.1.x.

## Loose ends the release could not close

- [x] **Move the test server onto Docker Hub. Closed 2026-08-17 - no host pulls from GHCR.** Confirmed by
      the maintainer. The GHCR package was deleted on 2026-08-16 and nothing broke, which is the proof by
      removal this item wanted. **Docker Hub is now the only registry anything pulls from, ours included.**
      **Decided 2026-08-15: Docker Hub is the only registry anyone is pointed at** - `$ci-cd/decisions#D14` -
      and that includes our own hosts, or the decision is a wording change nobody follows. Repoint the
      deployment at `binacle/binacle-net`, then `docker logout ghcr.io` on that host so the next pull proves it
      needs no GHCR credential. This also closes the older question of whether that host was pulling GHCR with a
      stored credential: after the move it has no reason to hold one.

- [x] **Make the GHCR package private. Done 2026-08-16, by the org move rather than by a flip.** The repo
      moved to `binacle-labs`, and a package created under an organization starts private even when the repo
      is public. `3.0.0-beta.3` ran the whole pipeline against it - `build`, `smoke` and `publish` all green -
      so the three jobs that touch GHCR are proven against a private package. The old public
      `ghcr.io/chrismavrommatis/binacle-net` was deleted the same day.

- [ ] **Move the verification floor from `3.0.0-beta.2` to `3.0.0`.** Signing, the SBOM and the GHCR staging
      copy all started at beta 2, so every surface currently says that is the first verifiable image - which
      is true and reads like a pre-release footnote. **Once v3.0.0 is published, `3.0.0` is the first
      verifiable *release*, and the examples can finally name a tag users actually pull.** Two things change:
      the "from `3.0.0-beta.2` onward" wording becomes `3.0.0`, and every worked example that had to say
      `3.0.0-beta.2` - because `3.0`, `latest` and `3.0.0` were unsigned or absent - can use a real tag.

      **The surfaces that say it:** `SECURITY.md`, `README.md`, the docs-site verification page, the
      `image.just` header, `tooling/README.md`, and the Docker Hub page when it goes up. Grep for
      `3.0.0-beta.2` and you have the list. Keep beta 2 named only where the *history* is the point - it is
      still the first signed image, and someone verifying an old pull needs to know their `2.1.1` never can be.

      **`SECURITY.md` is already done**, as part of the org move on 2026-08-16 - its floor reads `3.0.0`. Beta
      2 is still named there on purpose, for a second reason this item did not anticipate: **it is the only
      image signed under the old `ChrisMavrommatis` identity**, so it needs the old string to verify at all.
      That sentence is history, not a floor, and it stays.

- [ ] **Decide the immutability switch, now that a real release is behind it.** The rule is corrected and the
      switch is off. Turning it on means testing it on a scratch repo first - there is no undo, and an
      immutable tag cannot be deleted either, so a release tag pushed by mistake is permanent. **If you decide
      not to turn it on, write that down as a decision and drop the plan.** Leaving it as a permanently open
      question is the one outcome with no value.

## Tidy up

- [ ] **Delete `release-v3.0.0.md`** once the release is out and verified.
- [ ] **Move anything left in it to the board** rather than carrying it forward. If it was not done for the
      release, it is standing work now.
- [ ] **Delete this file** when its own list is clear.

`release-notes-v3.0.0.md` is already gone - deleted 2026-08-11 when the release body moved to `CHANGELOG.md`,
which is permanent and stays.

## Then what

**The board.** It holds every plan and idea not tied to this release, grouped by area with blockers named, and
it carries a recommended order.

**Three things were held back from v3.0.0 and are waiting there:** the heavy architecture tools (ArchUnitNET,
dependency-cruiser, lychee), CI gates 2 and 3, and the Blazor half of the UI test harness that the coverage gate
hangs on. **Each is waiting on something specific**, and the board row names it.

**The first thing to do is not a build.** The ServiceModule direction decision places five ideas, two one-liners
and the Azure Storage removal question in one sitting. **Pick from the board once this list is clear.**
