# Post-release - the days right after Binacle.Net v3.0.0

**Status:** Do these once v3.0.0 is out. None gate the release. Like the release plan, this coordinates other
files and nothing points back at it. Delete it once the list is clear - the tag does not delete it, working
through it does.

**The cap, and it is tighter than it was.** Only things that must happen *because this release just shipped*
belong here. Not "soon", not "in 3.0.x" - **now, because of the tag.** Everything else is standing work and
lives on the board.

Rewritten 2026-08-07. This file had become a second backlog, which its own cap warned against: it was carrying
the CI plans, the UI client migration, the benchmark ledger, the TestsKernel fixtures and the v4 flip. None of
those are consequences of shipping v3.0.0 - they are just the next things to do. They moved to `board.md`,
which is permanent and exists to hold exactly that.

**Same index rule as the release plan.** When a plan lands its file is deleted - tick the row and drop the link
in the same change, leaving the text.

---

## Do these because the tag just happened

- [ ] **Smoke the published image.** `just smoke all binacle/binacle-net:3.0.0`. About a minute, nothing to
      bring up.

      **This is a confirmation now, not the safety net it used to be.** The old workflow pushed to Docker Hub
      without ever running the image, so this manual run was the only thing between a broken image and the
      people who pull it. Since 2026-08-11 the pipeline smokes the GHCR copy before anything is copied across.
      What it still buys is the one thing the pipeline cannot check: that the *copy* landed something runnable,
      not just something with the right digest.

- [ ] **Confirm `3.0` resolves on Docker Hub, and that `latest` moved.** Nine files were bumped from
      `3.0.0-beta.1` to `3.0` in the last change before the tag - six pins plus `README.md`, `samples/README.md`
      and `samples/docker/README.md`. Until the release image is published that tag does not exist, so this is
      the check that `main` is not pointing at nothing. A2 verified a prerelease moved neither `latest` nor the
      minor tag; this is the same check for the real release, where both are expected to move.

      **Check the signature and the attestations while you are here** - `cosign verify` against the digest, and
      `docker buildx imagetools inspect` for the SBOM and provenance entries. `publish` runs for every tag now,
      but no tag has been pushed since that became true - beta 2 ran during the few hours the prerelease skip
      existed. So the copy that produced these tags and the signature over them have run at most once before
      this, on the throwaway pipeline-test tag if it was done at all.

      **Check the repo landing page by eye while you are here.** `README.md` is the one that carried a
      beta-conditional sentence ("Until then, pin `binacle/binacle-net:3.0.0-beta.1`") and it is the most read
      file in the repo. A stale beta pin there outlives every other miss. Note the tag it names: `3.0.0-beta.1`
      is the only beta on Docker Hub, because beta 2 was tagged during the few hours the prerelease skip
      existed. Betas do reach Docker Hub again, with their immutable tag only.

- [ ] **Delete the release set.** `release-v3.0.0.md` goes once the release is out and verified. This file goes
      when its own list is clear. `release-notes-v3.0.0.md` is already gone - deleted 2026-08-11 when the
      release body moved to `CHANGELOG.md`, which is permanent and stays.

- [ ] **Check the docs site is actually on v3.0.x.** B8 flipped `current` forward as part of releasing the
      docs. Confirm `/version/latest/` lands on `v3.0.x` and the version picker shows four versions. This is
      the item most likely to have been silently skipped, because nothing fails when it is - the site just
      keeps presenting v2.1.x.

## Everything else

On the board. The CI work, the UI clients, the v4 flip, the `Parallel*` decision, the benchmark
ledger and the TestsKernel fixtures are all there, grouped by area with their blockers named. Pick from there
once this list is clear.
