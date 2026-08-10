# CI - rebuild how the image is released

**Status:** Not started. Split out of `ci-shared-scripts` on 2026-08-07, absorbed the smoke gate the same day,
and **rewritten 2026-08-10** after a design pass. One plan owns `release-docker-image.yml` end to end - every
change below touches that one file, and splitting them only creates an ordering to remember.

**Scheduled against the beta cycle - decided 2026-08-10.** An earlier draft of this file deferred everything
past v3.0.0, on the grounds that you should not change the publish path in the window you depend on it. The
maintainer's call reverses that, and the reasoning is better: **a prerelease tag is the only free test this
pipeline will ever get.** It exercises the whole path, and by design it moves neither `latest` nor the minor
tag, so a mistake costs a deleted tag rather than a bad release. Waiting until after v3.0.0 means the first
real run of a rebuilt pipeline is a release nobody can take back.

So steps 1 and 2 of the sequence land **before a beta tag** and are proven by it. Steps 3 and 4 stay after
v3.0.0. **Read "Testing this during the beta cycle" below before tagging anything** - there is one part a
prerelease cannot test, and it is the newest part.

**What already exists and is not repeated here:** `smoke-image.yml` is a `workflow_dispatch` workflow that takes
an image tag, installs the two smoke tools pinned, and runs the structure check plus all five profiles as
separate steps against a published image. It was proven on 2026-08-10 against `binacle/binacle-net:3.0.0-beta.1`
- pull plus six steps, all green, no containers or volumes leaked. Everything below reuses it rather than
reinventing it.

---

## What is wrong today

`release-docker-image.yml` is `on: release: published` and does: restore, publish, metadata-action, login,
buildx, build-push with `push: true`. Four problems, in order of how much they matter:

1. **It pushes without ever running the image.** Nothing between a broken image and the people who pull it.
2. **The release is already public when the workflow starts.** A failure leaves an announced release whose
   image never arrived.
3. **It inlines its own build** - `dotnet restore` + `dotnet publish` - while `config/build.just` publishes the
   same project to the same place. They match today by coincidence.
4. **`vars.BUILD_DOCKERFILE` is still a repo variable.** The same argument that removed the other two
   variables applies: `docker/build-push-action` needs a `file:` input, but that input can be the literal
   `Dockerfile`. There is one, at the repo root, and the build recipe does not parameterise it either.

---

## The target flow, end to end

This is the shape the rest of the file builds up. Read it once, then the sections explain each step.

```
git push origin v3.0.0
  |
  v  on: push: tags: 'v*'
1. checkout
2. just build publish                     the app into build/binacle-net
3. build + push the IMMUTABLE tag only    3.0.0        -> capture the digest
4. smoke what is now in the registry      pull 3.0.0, structure + 5 profiles
5. promote, only if step 4 is green       3.0 and latest -> point at that digest
6. create the GitHub release              from the notes file
```

The order is the whole point. **Nothing a user follows moves until the artifact has been tested**, and the
release does not exist until the image does.

---

## Step 3 and 5 - promote by digest, do not rebuild

This is the part that is easy to get subtly wrong, and there is a tempting wrong version of it.

**The wrong version:** build with `load: true`, smoke the local image, then push. It sounds equivalent and is
not. It tests a local copy that *ought to be* identical to what lands in the registry. Registry round trips are
exactly where compression, manifest shape and attestation handling differ.

**The right version:** push the immutable tag first, smoke **what the registry now serves**, then move the
other tags onto that same digest. No rebuild, no second push - the moving tags are re-pointed at a manifest
that has already passed.

### Why the moving tags are the ones that matter

`3.0` and `latest` are what the samples, the README and the quick start tell people to follow. Those are the
tags that hurt when they are wrong. `3.0.0` is an exact pin that nobody is following on release day - it did
not exist a minute earlier. So the honest trade is: **the immutable tag is briefly public and unsmoked, and the
moving tags never are.** That is worth it. The alternative buys a smaller exposure window on a tag nobody
watches, in exchange for never testing the registry copy at all.

### Keep metadata-action, but run it twice

Do **not** hand-roll "is this a prerelease". The current workflow gets that right and it was verified on Docker
Hub on 2026-08-06: the beta published `3.0.0-beta.1` and moved neither `latest` nor `3.0`. Keep those guards by
splitting the one metadata step into two:

- **meta-immutable** - `tags: type=semver,pattern={{version}}`. Always produces a tag.
- **meta-moving** - `tags: type=semver,pattern={{major}}.{{minor}}` plus `flavor: latest=auto`.

For a prerelease tag, metadata-action skips `{{major}}.{{minor}}` and `latest=auto` withholds `latest`, so
**meta-moving comes out empty and step 5 is a natural no-op.** Prerelease handling stays where it already works
instead of becoming an `if:` condition somebody has to maintain.

Guard step 5 on the tag list being non-empty so it does not run `imagetools` with no targets.

### The mechanics

`docker/build-push-action` exposes the pushed digest as `steps.<id>.outputs.digest`. Promotion is one command,
and it uploads nothing:

```bash
docker buildx imagetools create \
  -t binacle/binacle-net:3.0 \
  -t binacle/binacle-net:latest \
  binacle/binacle-net@sha256:<digest>
```

`imagetools create` points new tags at an existing manifest. It is not a pull, not a rebuild, and not a re-push.

### Step 4 reuses the smoke workflow, it does not copy it

The smoke steps are already written and proven in `smoke-image.yml`. Do not paste them into this workflow.
Either call it with `workflow_call` (add that trigger to it, keep `workflow_dispatch`), or extract the tool
install plus the six calls into a composite action once there are genuinely two callers. **Do not build the
composite action first** - wait until the duplication is real and you can see its shape.

---

## Step 6 - creating the release, and the one gotcha

`gh release create v3.0.0 --notes-file <body>` needs `permissions: contents: write` and the job's
`GITHUB_TOKEN`. Add `--prerelease` when the tag has a hyphen in it.

**This used to carry a gotcha, and it is already fixed.** `release-notes-v3.0.0.md` opened with a section for
whoever cut the release - style rules, a scope note, "before pasting" - so pointing `--notes-file` at it would
have published the internal notes. **Split on 2026-08-10**: that file is now body only, published verbatim, and
the guidance moved into the release plan under "The release notes file", which is where the rest of the
"how to cut this release" material already lives. Nothing to do here but point at the file.

Keep it that way. The rule for every future version is that `release-notes-v<version>.md` is **the body and
nothing else** - no preamble, no title line, because the release title is set separately. A file you publish
whole cannot be published wrongly, and that is worth more than the convenience of keeping notes at the top.

### Replace the trigger, do not add to it

`on: push: tags: 'v*'` **replaces** `on: release: published`. Leave both and creating the release in step 6
re-triggers the workflow, which builds and pushes everything a second time.

---

## Steps 2 and the build - through the recipe

Replace the `Restore` and `Publish` steps with one `just build publish`.

- The shapes are deliberately not identical. The workflow splits restore and publish; the recipe is one
  `dotnet publish` that restores on its own. `build.just` explains why: nothing caches the restore between the
  two steps, so the split only adds a second place for the runtime identifier to drift. **Expect the workflow
  to lose a step, not to gain a wrapper.**
- The runner needs `just`. `run-tests.yml` and `smoke-image.yml` both install it with
  `extractions/setup-just` pinned by SHA at `^1.45` - copy that.
- `vars.API_PROJECT_PATH` and `vars.BUILD_OUTPUT` are already gone, hardcoded on 2026-07-30 to unblock the
  beta. Remove `vars.BUILD_DOCKERFILE` in the same pass, for the same reason.

**Prove it by diffing the output, not by the exit code.** The recipe was proved that way once already when it
was written - same file list, same sizes as the old restore plus `publish --no-restore`. Do it again after the
change. A green workflow says the command ran, not that it produced the same thing.

---

## SBOM and provenance - small, do it while you are in the file

`docker/build-push-action` takes two inputs:

```yaml
sbom: true
provenance: mode=max
```

`sbom: true` attaches an SPDX document to the image as an attestation. `provenance: mode=max` records how the
image was built - source, workflow, materials. **You already publish provenance without having asked for it**:
the `unknown/unknown` entry in the published beta's manifest list is the default attestation manifest buildx
adds. Being deliberate about it costs one line.

Inspect what you shipped:

```bash
docker buildx imagetools inspect --format '{{ json .SBOM }}' binacle/binacle-net:3.0.0
docker buildx imagetools inspect --format '{{ json .Provenance }}' binacle/binacle-net:3.0.0
```

Two things to know before turning it on:

- **Attestations add those `unknown/unknown` manifest entries.** Modern tooling ignores them; some older
  registry UIs and scanners show them as mystery platforms. Harmless, but do not be surprised by it.
- **buildx's SBOM scans the image filesystem**, so it is good on OS packages and weak on .NET detail - a
  self-contained publish is a pile of DLLs to a filesystem scanner. If the goal is real dependency
  provenance rather than a checkbox, generate a CycloneDX SBOM from the projects with the `CycloneDX` dotnet
  tool and attach that as well. The `NOTICE` file already tracks direct dependencies by hand, so this is the
  automated version of something the repo already cares about.

Digest promotion and attestations get along: with attestations the build output digest is the manifest list
digest, and `imagetools create` from it carries the attestations across.

---

## Testing this during the beta cycle {#beta-testing}

A prerelease tag is the test vehicle. It runs the same code path as a real release, and metadata-action's
guards mean it cannot damage anything a user follows.

### What a beta run proves

Tag `v3.0.0-beta.N` and the rebuilt workflow exercises, in one go:

- the `on: push: tags` trigger firing at all,
- `just build publish` producing a working app on a runner,
- the immutable tag building and pushing, and the digest being captured,
- **the smoke gate running against the registry copy** - the whole point of the rebuild,
- the GitHub release being created from the notes file, with `--prerelease` set.

That is five of the six steps.

### The one thing a beta cannot test - and it is the newest part {#promotion-gap}

**Tag promotion never runs on a prerelease.** That is deliberate and correct: meta-moving comes out empty for a
prerelease, so step 5 no-ops. But it means the `docker buildx imagetools create` call - the least familiar
command in the whole workflow, and the one holding the registry credentials at the moment it matters - stays
completely unexercised until the real v3.0.0 tag.

Do not discover on release day that the token lacks a permission or the digest reference is malformed.
**Close it with a throwaway tag**, by hand, once, after a beta has published:

```bash
docker buildx imagetools create \
  -t binacle/binacle-net:promotion-check \
  binacle/binacle-net@sha256:<the beta's digest>

docker buildx imagetools inspect binacle/binacle-net:promotion-check   # same digest?
# then delete the tag from Docker Hub
```

That proves the command, the credentials and the digest plumbing without touching `latest` or `3.0`. It is five
minutes and it is the difference between the real release being the first run of that step or the second.

An alternative, if a throwaway tag on the public repo is unwelcome: temporarily add
`type=raw,value=promotion-check` to meta-moving so a beta exercises step 5 end to end inside the workflow, then
remove it. More faithful, more moving parts. The manual check is enough.

### Which beta, and keeping the two variables apart

Beta 2 exists to verify the Sonar refactor in a real deployment. Landing the pipeline change first means that
run is testing two things at once. That is acceptable, because the two fail at different stages and are easy to
tell apart - a pipeline fault means no image is published, a code fault means a deployed image misbehaves.

**One safeguard makes it clean:** diff the publish output before tagging, as step 1 already requires. If
`just build publish` produces the same file list and sizes as the old restore-plus-publish, the artifact beta 2
verifies is byte-identical to what the old path would have made, and the code verification is uncontaminated.

If that feels like too much at once, the conservative order is: beta 2 on the current workflow, land the
pipeline, then beta 3 as its test. It costs one extra prerelease and buys a clean separation. Either is
defensible - what is not defensible is v3.0.0 being the first run.

---

## Multi-arch - a real gap, but answer the demand question first

**The published image is `linux/amd64` only.** Confirmed 2026-08-10 by inspecting the manifest of
`3.0.0-beta.1`: one platform plus the attestation blob.

For self-hosted software that is a real limitation - Graviton, Ampere and every Apple Silicon dev machine
either emulate it or refuse it. But it is also **weeks-away work with no evidence anyone wants it**, so:

**Answer this first: does anyone run Binacle.Net on ARM?** If no, `linux/amd64` is a perfectly defensible
choice and the useful action is to *write it down as a decision* rather than leave it looking incidental. If
yes, the shape is below.

### Why it is not just a flag

`--self-contained --runtime linux-x64` bakes the architecture into the publish. Multi-arch means publishing
once per runtime identifier and having the Dockerfile pick the right one:

```dockerfile
ARG TARGETARCH
COPY ["build/binacle-net-${TARGETARCH}", "."]
```

publishing to `build/binacle-net-amd64` and `build/binacle-net-arm64`.

**The trap is the naming.** Docker's `TARGETARCH` is `amd64` / `arm64`; .NET's runtime identifiers are
`linux-x64` / `linux-arm64`. They do not match, and the mapping has to live in exactly one place or the image
gets the wrong binaries and still builds.

Also note `.dockerignore` allowlists `build/binacle-net`. Renaming the publish directories means updating it,
or nothing gets copied and the image builds empty - the same failure `build.just` already warns about.

### Two shapes

- **A - one job with QEMU.** `docker/setup-qemu-action`, then `platforms: linux/amd64,linux/arm64`. The
  Dockerfile's `apt-get install libgssapi-krb5-2` has to *run* per architecture, so the arm64 leg is emulated
  and slow - minutes, not seconds. Simplest YAML.
- **B - a runner matrix.** `ubuntu-24.04` and `ubuntu-24.04-arm`, each building and pushing by digest, then a
  merge job assembling the manifest list. Native speed, no emulation, more YAML. This is docker's documented
  multi-platform pattern.

**Start with A.** The release runs rarely and a few slow minutes on release day costs nothing. Move to B only
if that becomes annoying.

### How to actually test it runs on ARM - the important question

Building for a platform proves nothing about running on it. Three ways, best last:

1. **Locally, for a one-off check.** With QEMU installed:
   `docker run --rm --platform linux/arm64 binacle/binacle-net:3.0.0`. Enough to catch a wrong-RID binary that
   will not start at all.
2. **QEMU in CI.** Set `platform: linux/arm64` on the smoke compose stacks and run the suite emulated. It
   genuinely executes the arm64 binary, so it catches a broken publish or a missing native dependency. It does
   not tell you anything about performance, and emulation occasionally behaves differently from hardware.
3. **A native ARM runner - do this one.** `ubuntu-24.04-arm` is GitHub-hosted and free for public
   repositories. Real hardware, no emulation, and it needs almost no new work: **`smoke-image.yml` already does
   the whole job**, so turning its `runs-on` into a matrix over `[ubuntu-24.04, ubuntu-24.04-arm]` gives you
   the full structure-plus-five-profiles suite on both architectures. Docker pulls the matching variant from
   the manifest list automatically on each runner.

   **One thing to fix when you do it:** the tool install in that workflow hardcodes x86_64 URLs. Both tools
   publish arm64 builds - container-structure-test as `container-structure-test-linux-arm64`, hurl as an
   `aarch64-unknown-linux-gnu` tarball - so the install step needs to pick by `$(uname -m)`. That is the only
   change the workflow needs.

---

## Sequence

Each step is independently useful and independently revertible. Do not batch them.

**Before v3.0.0, proven by a beta tag:**

1. **Build through the recipe** plus drop `vars.BUILD_DOCKERFILE`. Diff the publish output - this is also the
   safeguard that keeps the beta's code verification clean. Smallest change, and everything else is easier to
   review once the workflow stops deciding what gets built.
2. **Restructure to tag-triggered with digest promotion**, reusing `smoke-image.yml`. The notes file this step
   depends on is already body-only, so there is nothing to split first.
3. **Tag a beta and let it run the whole path**, then close the promotion gap with the throwaway-tag check.
   See "Testing this during the beta cycle" - skipping that check leaves the newest command in the workflow
   untested until the release itself.

**After v3.0.0:**

4. **SBOM and provenance.** Two lines, once the file has stopped moving.
5. **Multi-arch** - only after the demand question has an answer.

The split is the point: everything that can be proven by a prerelease happens before v3.0.0, and everything
that changes the artifact waits until after it.

## Done when

- `release-docker-image.yml` decides nothing about *what* is built. Every step that does is a call a maintainer
  can run identically on a laptop.
- A release that would ship a broken image fails **before** anything a user follows points at it, without
  anyone running the smoke by hand.
- The GitHub release does not exist until the image it describes does.
