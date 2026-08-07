# CI - fix the release workflow: build through the recipe, then smoke before pushing

**Status:** Not started. Split out of `ci-shared-scripts` on 2026-08-07, then absorbed the smoke gate from
`ci-gates` the same day. **One plan owns `release-docker-image.yml` end to end** - the two changes touch the
same file, and the second is blocked by the first, so splitting them only creates an ordering to remember.

Two things are wrong with that workflow. It builds the image a different way from everything else, and it
pushes it without ever running it.

`release-docker-image.yml` still inlines its own build:

- line 26 - `dotnet restore --runtime linux-x64`
- line 32 - `dotnet publish api/src/Binacle.Net/Binacle.Net.csproj -c Release -o build/binacle-net --no-restore
  --self-contained --runtime linux-x64`

`config/build.just` publishes the same project to the same place. They match today by coincidence and are one
edit away from not matching, so the image CI ships and the image a maintainer builds come from two command
lines that can drift.

## Part 1 - build through the recipe

### Why this part comes first

The PR image gate waits on it too. Its step is already `just build image`, but until the release workflow calls
the same recipe, a green gate proves the recipe builds - not that the release path does. And the smoke in part
2 is worth little against an image built a different way from the shipped one.

### The shapes are not identical - that is the point

The workflow splits restore and publish; the recipe is one `dotnet publish` that restores on its own.
`build.just` explains why it collapsed them: nothing caches the restore between the two steps, so the split
only adds a second place for the runtime identifier to drift. Expect the workflow to lose a step, not to gain a
wrapper around the one it has.

### What to change

- Replace the `Restore` and `Publish` steps with one `just build publish`.
- **`vars.BUILD_DOCKERFILE` stays.** `docker/build-push-action` needs `file:`, and the push, the semver metadata
  and `latest=auto` are genuinely CI's concerns, not build ones.
- `vars.API_PROJECT_PATH` and `vars.BUILD_OUTPUT` are already gone - hardcoded in the workflow on 2026-07-30 to
  unblock the beta, because `BUILD_OUTPUT` had to equal the path the Dockerfile hardcodes and was therefore a
  repo setting that could silently break the image. That removed the repo-setting risk but not the duplication,
  which is what is left here.
- The runner needs `just` on it. Check how the other workflows get it before assuming it is there.

### Prove it by diffing the output, not by the exit code

The publish was proved that way once already, when the recipe was written: same file list and the same sizes as
the old restore plus `publish --no-restore`. Do it again after the workflow changes. A green workflow only says
the command ran, not that it produced the same thing.

## Part 2 - smoke the image before it is pushed

Moved here from `ci-gates` on 2026-08-07. **The suite is built, green and proven** - `just smoke all
binacle/binacle-net:3.0.0-beta.1` passed 31 structure assertions and all five profiles against the published
beta. Nothing about it is unfinished. What is left is wiring, and it belongs in this file because both blockers
are in this workflow.

This is the **release** workflow, not the PR gate. Do it by hand locally first; a gate nobody trusts gets
disabled. The suite is already CI-ready: pinned binaries, non-zero exit on failure, JUnit output.

### The one blocker part 1 does not cover

**`release-docker-image.yml` uses `build-push-action` with `push: true` and no `load:`,** so the image never
lands in the runner's daemon and **there is nothing to smoke.** Needs `load: true` first - cheap, one platform.

### The path once both are fixed

Install the two binaries the way `DEVELOPMENT.md` documents, build with `load: true`, run
`just smoke test-structure <image>`, then `just smoke test <profile> <image>` for each of `minimal`,
`quickstart`, `prod`, `service` and `full`. Push only if green.

**All five profiles are one container each** - checked 2026-08-07 with `docker compose config --services`.
There is no reason to stage them: `service` and `full` use SQLite in a named volume, so no profile brings up a
database, and the whole suite is under ten seconds of container time. Every recipe takes the image as its last
argument, so CI passes the tag it just built and never touches `just smoke all`, which would rebuild.

**On a runner newer than noble, check hurl first.** It links `libxml2.so.2`, which Ubuntu 26.04 dropped -
`DEVELOPMENT.md` has the detail and the workaround. A runner that moves off noble breaks this in a way that
reads as a hurl bug rather than a distro change.

## Done when

- `release-docker-image.yml` decides nothing about *what* is built. Every step that does is a call a maintainer
  can run the same way.
- A release that would ship a broken image fails before it is pushed, without anyone running the smoke by hand.
