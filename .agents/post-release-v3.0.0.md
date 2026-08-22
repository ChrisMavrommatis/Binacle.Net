---
description: Post-release - the checks to run once Binacle.Net v3.0.0 is out
---

# Post-release - the checks to run once v3.0.0 is out

**Status:** Do these once v3.0.0 is tagged and the pipeline has run. **None of them gate anything** - the
release is already out by the time this file opens.

**This file is self-contained and it is checks only.** Every item is something you look at, run or read back.
No tooling to build, no decision to take, nothing to figure out, and **nothing here links to a plan** - if an
item needed a plan, it was not a post-release check. The one exception is the last item, which is a decision
the release deliberately deferred.

**That is the test for anything proposed for this file.** If working it needs a decision, a credential, a new
file or a workflow, it belongs in the release plan while there is still time, or on the board if there is not.

**Delete this file once the list is clear.** The tag does not delete it; working through it does.

Rewritten 2026-08-14, when the release scope was reset. Pruned 2026-08-20.

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

      **Docker Hub only.** Only the release workflow touches GHCR, so the staging copy's signature is read by
      nothing.

      All of this has run green against `3.0.0-beta.3` and `3.0.0-beta.4`, under the identity v3.0.0 uses.
      What is new at v3.0.0 is only that the copy writes three tags instead of one.

- [ ] **Read the repo landing page by eye.** `README.md` is the most read file in the repo, and its pin warning
      names `binacle/binacle-net:3.0` - a tag that only starts resolving with this release. **A wrong pin there
      outlives every other miss**, and a stale one would not fail loudly, because every tag this project has
      ever published is still pullable. You have to look.

- [ ] **Check the six sample pins and the two sample READMEs moved.** `samples/docker/*/docker-compose.yml`,
      `samples/kubernetes/minimal/binacle-deployment.yaml`, `samples/README.md` and `samples/docker/README.md`
      sat on a beta pin through the whole sequence by design. They were the last commit before the tag; confirm
      none was missed and that the "beta patch for now" comment went with them, because a copied sample carries
      the pin forward forever.

- [ ] **Confirm nothing froze.** Read `immutable_tags_settings` back from the repository API. The switch should
      be off and the rule should be whatever the release left it at - the publish should have written `3.0.0`,
      `3.0` and `latest` with no interference.

## Read what the release published

- [ ] **Read the Docker Hub page - the release run published it, and that step had never run before.** The
      page was never dispatched by hand, so Docker Hub went from the old hand-written 2.x page straight to this
      one in a single write. Check the rendered page: the description names 3.x rather than `2.1.1`, the version
      placeholders were substituted with the real numbers, the hand-maintained tag list is gone, the
      verification section is there, and the logo and categories took. **Read it properly rather than glancing
      at it.**

      **Run the quick start off the page itself** - the `docker run` and the `curl` - and check the response
      matches what the page prints. It is the first thing most readers do.

      **This is an eyeball, not a rewrite.** If it turns into a rewrite, the pre-tag half did not happen.

- [ ] **Run the verification checks against the real `3.0.0`, from a clean shell.** They were proven against
      `3.0.0-beta.3` and `3.0.0-beta.4`, both single-tag copies. This is the first time the copy writes three
      tags. **Confirm the invocation printed on the Docker Hub page and in `SECURITY.md` is the one that
      actually works** - a
      published command that fails reads as our bug.

- [ ] **Check the docs site is on v3.0.x.** Confirm `/version/latest/` lands on `v3.0.x` and the version picker
      shows four versions. **This is the item most likely to have been silently skipped**, because nothing
      fails when it is - the site just keeps presenting v2.1.x.

- [ ] **Confirm no public surface still names a beta.** Betas 1 and 2 are deleted from Docker Hub, so anything
      left pointing at one is a 404 rather than an old number. `grep -rn "3\.0\.0-beta" --exclude-dir=.agents`
      over the repo, and read the docs site's verifying-a-release page.

## The one decision the release deferred

- [ ] **Decide the immutability switch, now that a real release is behind it.** The switch is off and the rule
      is whatever the release left it at. Turning it on means testing it on a scratch repo first - there is no
      undo, and an immutable tag cannot be deleted either, so a release tag pushed by mistake is permanent.
      The plan on the board holds the test procedure. **If you decide not to turn it on, write that down as a
      decision and drop the plan.** Leaving it as a permanently open question is the one outcome with no value.

## Tidy up

- [ ] **Delete `release-v3.0.0.md`** once the release is out and the docs are deployed.
- [ ] **Move anything left in it to the board** rather than carrying it forward. If it was not done for the
      release, it is standing work now.
- [ ] **Delete this file** when its own list is clear.

## Then what

**The board.** It holds every plan and idea not tied to this release, grouped by area with blockers named, and
it carries a recommended order.

**Three things were held back from v3.0.0 and are waiting there:** the heavy architecture tools (ArchUnitNET,
dependency-cruiser, lychee), CI gates 2 and 3, and the last of the UI test harness the coverage gate hangs on.
**Each is waiting on something specific**, and the board row names it.

**The first thing to do is not a build.** The ServiceModule direction decision places five ideas, two one-liners
and the Azure Storage removal question in one sitting. **Pick from the board once this list is clear.**
