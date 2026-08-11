# CI/CD - rebuild the release pipeline on GHCR, and put the notes in CHANGELOG.md

**Status:** Not started. **Do this after v3.0.0 ships, not before.** Designed 2026-08-11 with the maintainer;
every decision below is settled unless it says otherwise. One session can do the whole thing, delegating the
changelog harvest.

**Do not pull this into the v3.0.0 release.** The pipeline in the working tree today is what beta 2 exists to
test, and swapping it mid-cycle means testing two things and knowing which broke neither. That pipeline stays
until v3.0.0 is out; this replaces it afterwards.

**What it replaces.** The current release workflow builds, pushes the immutable tag straight to Docker Hub,
smokes it there, then promotes the moving tags by digest. It works. Two things are wrong with it: an unsmoked
artifact is briefly public on the registry users pull from, and the release body is read from
`.agents/release-notes-<tag>.md` - a folder whose own contract says the file is deleted once the version ships.

---

## The target pipeline

Six jobs, cheapest check first, so nothing that costs money runs until the things that cannot be undone have
passed.

```
push tag v3.0.0
  |
  1. notes    the CHANGELOG section for this tag exists (seconds)
  2. test     the whole suite, reusing run-tests.yml (minutes)
  3. build    just build publish, push to GHCR, capture the digest
  4. smoke    pull that digest back from GHCR, structure + 5 profiles
  5. publish  crane copy to Docker Hub - SKIPPED for a prerelease
  6. release  gh release create, body from CHANGELOG.md
```

### Decisions already made - do not re-litigate these

- **GHCR is the staging registry, and the package is PUBLIC.** Private was never the goal; keeping Docker Hub
  free of anything unsmoked or unreleased was. A public package means no credential on the OVH server, nothing
  to rotate, and no dependency on classic personal access tokens - which do not have a fine-grained equivalent
  for packages and are on a deprecation path.
- **A prerelease never reaches Docker Hub.** Job 5 is skipped for any tag containing a hyphen. This is
  deliberately stricter than metadata-action's own guard, which would still publish the immutable tag.
- **A prerelease still gets a GitHub release**, marked prerelease. It is honest because the image is pullable
  from GHCR.
- **The rule, stated once:** GHCR carries everything including betas; Docker Hub carries releases only.
- **Copy, never rebuild.** `crane copy` by digest. A second build would ship an image nothing tested.
- **Notes live in `CHANGELOG.md` at the repo root**, newest version at the top, Keep a Changelog shape.
- **One section accumulates per cycle.** Betas publish `## [Unreleased]`; the real release publishes
  `## <version>`. The heading is renamed once, as the last edit before the real tag.
- **A real release with no matching section fails the build.** Never fall back to generated notes for a real
  release - that would silently publish a commit list as the release body.

### Still open - ask the maintainer, do not decide alone

- Whether `samples/` and `README.md` should point at GHCR for betas. Today they pin a Docker Hub beta tag,
  which this design makes impossible. Someone has to decide what the beta instructions say.
- Whether the docs site release-notes page is generated from `CHANGELOG.md` or stays hand-copied. It is a docs
  decision and repo-root `docs/` is off limits here - write down what the page must say and leave it.

---

## Phase 1 - build CHANGELOG.md

**This is the bulk of the work and the part to delegate.** It is mechanical, high-volume and easy to verify,
so hand it to a Sonnet agent with the brief below and check the result. Everything else in this plan is
judgement work and should stay with the executing session.

There are **24 releases**, v0.7.0 (2024-04-28) through v3.0.0-beta.1, plus whatever has shipped since. Bodies
range from 20 to 11,347 characters.

### The brief for the delegated agent

> Build `CHANGELOG.md` at the repo root from the project's published GitHub releases.
>
> Fetch them from the public API - `gh` is not installed on this machine:
>
> ```
> curl -sS "https://api.github.com/repos/ChrisMavrommatis/Binacle.Net/releases?per_page=100"
> ```
>
> Each item has `tag_name`, `published_at`, `prerelease` and `body`.
>
> Rules:
>
> 1. **Newest first.** The file opens with `# Changelog`, then an empty `## [Unreleased]`, then one section per
>    release in descending version order.
> 2. **Skip prereleases.** Any release with `"prerelease": true` is excluded. A beta's body was the in-progress
>    notes at that moment; it is not a version of its own. The releases stay on GitHub as the record.
> 3. **Heading shape:** `## [2.1.1] - 2026-01-12`, taking the date from `published_at` (the date part only).
>    Strip the leading `v` from the tag.
> 4. **Reproduce each body VERBATIM.** Do not reword, reformat, re-wrap, fix typos, or normalise the emoji
>    section headings. These are published text and the changelog is a record of what was said.
> 5. **Do not change heading levels inside a body.** The bodies contain 76 `## ` headings such as
>    `## 🔎 Overview` and `## 🛠️ Migration Guide`. Leave them exactly as they are - the extractor is written
>    to stop only at a heading that parses as a version, so they are safe.
> 6. Some bodies are a single line. Some are nearly empty. Reproduce them anyway; a short section is accurate.
> 7. Do not commit. Leave the file in the working tree.
>
> When done, report: the number of sections written, the earliest and latest version, and any release whose
> body was empty.

### Verify the result yourself before moving on

- `just changelog extract 2.1.1` returns that release's body and stops before `## [2.1.0]`.
- `just changelog extract 3.0.0` (or whatever shipped) returns a body containing its emoji subheadings intact
  and does **not** stop at the first one.
- Section count matches the number of non-prerelease releases.
- `## [Unreleased]` is present, at the top, and empty.

### Then delete the old home

`.agents/release-notes-v3.0.0.md` and any sibling notes file go once their content is in `CHANGELOG.md`. The
release plan's own companions are deleted by the release process; do not leave a second copy behind.

---

## Phase 2 - the changelog just module

New file `config/changelog.just`, registered as a module in the root `justfile` alongside the others. Two
recipes, so CI and a laptop parse the file the same way and the body can be previewed before a tag is pushed.

The parsing lives here rather than in the workflow for the same reason every other recipe does: the workflow
calls `just`, and `config/` is the only place that knows how a thing is done.

```just
# CHANGELOG.md sections, loaded by the root justfile as the `changelog` module.
#
#   just changelog extract 3.0.0      print that version's section
#   just changelog extract Unreleased print the in-progress section
#   just changelog check 3.0.0        exit 1 if the section is missing or empty
#
# The release workflow calls both: `check` as its first gate, `extract` to build the release body. Keeping the
# parsing here means CI and a laptop read the file the same way, and you can preview the exact body you are
# about to publish before you push the tag.

set working-directory := '..'
set no-exit-message := true

changelog := "CHANGELOG.md"

# List the recipes
default:
    @just --list changelog

# Print one version's section [version|Unreleased]
extract version:
    #!/usr/bin/env bash
    set -euo pipefail
    # A section ends at the next VERSION heading, not at the next `## ` - the release bodies are full of
    # `## 🔎 Overview` and `## 🛠️ Migration Guide`, and stopping at those would truncate every section at its
    # first subheading. So a terminator is a `## ` whose first token parses as `Unreleased` or as semver.
    #
    # Accepts `## 3.0.0`, `## [3.0.0]`, and either with a trailing ` - <date>`, so the file can follow Keep a
    # Changelog exactly without the matcher caring which shape you used.
    awk -v want='{{ version }}' '
      function version_token(line,   h, a) {
        h = line; sub(/^## +/, "", h); gsub(/[][]/, "", h)
        split(h, a, / +/)
        if (a[1] == "Unreleased" || a[1] ~ /^[0-9]+\.[0-9]+\.[0-9]+/) return a[1]
        return ""
      }
      /^## / {
        tok = version_token($0)
        if (tok != "") {
          if (f) exit
          if (tok == want) { f = 1; next }
        }
      }
      f { print }
    ' {{ changelog }}

# Fail if a version's section is missing or empty [version|Unreleased]
check version:
    #!/usr/bin/env bash
    set -euo pipefail
    notes="$(just changelog::extract '{{ version }}')"
    if [ -z "${notes//[[:space:]]/}" ]; then
        echo "No '## {{ version }}' section in {{ changelog }}." >&2
        echo "Rename [Unreleased] to {{ version }} before tagging." >&2
        exit 1
    fi
    lines=$(printf '%s\n' "$notes" | grep -c '' || true)
    echo "'## {{ version }}' found - ${lines} lines."
```

Both recipes were tested during design against a file carrying emoji subheadings and both bracket and bare
version headings. Re-test after wiring; do not assume.

---

## Phase 3 - the workflow

Replace `.github/workflows/release-docker-image.yml` wholesale with the draft below. It parses and its job
graph was verified during design. **Read the gaps under it before running anything.**

```yaml
# Six jobs. The image is built once, lands on GHCR, and only reaches Docker Hub after it has been smoked
# there:
#
#   push tag v3.0.0 -> notes gate -> test -> build+push GHCR -> smoke -> copy to Docker Hub -> release
#
# The order is cheapest-check-first: the notes gate is seconds, the suite is minutes, the build is longer
# still. Nothing that costs money runs until the things that cannot be fixed after the fact have passed.
#
# A PRERELEASE STOPS AFTER SMOKE. `publish` is skipped for any tag with a hyphen, so a beta exists only on
# GHCR and Docker Hub never sees an unreleased artifact. Deploying a beta therefore pulls from GHCR.
#
# The copy preserves the digest - a manifest copy is content-addressed - so what Docker Hub serves is bit for
# bit what the smoke job passed. Never rebuild between the two: that would ship an image nothing tested.
#
# Do NOT add `release: published` as a second trigger - the release created in the last job would re-enter
# this workflow and build everything twice.

name: Build and Release Docker Image

on:
  push:
    tags:
      - 'v*'

permissions:
  contents: read

env:
  # GHCR rejects an uppercase path and the owner here is mixed case, so it is written out lowercase rather
  # than derived from github.repository_owner at runtime.
  STAGING_IMAGE: ghcr.io/chrismavrommatis/binacle-net

jobs:
  # FIRST, and everything waits on it. The release body has to exist before anything is built, because the
  # alternative is discovering it at the end - with the image already on Docker Hub and `latest` moved, and
  # no release to describe it. A grep costs seconds; that costs a bad release.
  notes:
    runs-on: ubuntu-latest
    permissions:
      contents: read

    outputs:
      section: ${{ steps.section.outputs.name }}

    steps:
      - uses: actions/checkout@v4

      - name: Setup just
        uses: extractions/setup-just@53165ef7e734c5c07cb06b3c8e7b647c5aa16db3 # v4.0.0
        with:
          just-version: '^1.45'

      - name: Which section this tag publishes
        id: section
        run: |
          TAG='${{ github.ref_name }}'
          case "$TAG" in
            *-*) echo "name=Unreleased" >> "$GITHUB_OUTPUT" ;;
            *)   echo "name=${TAG#v}"   >> "$GITHUB_OUTPUT" ;;
          esac

      - name: Check the section exists
        run: just changelog check '${{ steps.section.outputs.name }}'

  # Nothing guarantees the tag sits on a commit that passed CI - you can tag anything. This is that guarantee,
  # and it reuses the PR workflow rather than restating the leaves.
  #
  # After the gate, not beside it: the gate is seconds and this is minutes, so a missing changelog section
  # should not cost a full suite run before it is reported.
  test:
    needs: notes
    uses: ./.github/workflows/run-tests.yml
    permissions:
      contents: read

  build:
    needs: [ notes, test ]
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    outputs:
      staging: ${{ steps.ref.outputs.staging }}
      version: ${{ steps.meta.outputs.version }}
      digest: ${{ steps.push.outputs.digest }}

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ vars.DONET_VERSION }}

      # 1.x range: ubuntu's apt ships a just too old to parse config/build.just.
      - name: Setup just
        uses: extractions/setup-just@53165ef7e734c5c07cb06b3c8e7b647c5aa16db3 # v4.0.0
        with:
          just-version: '^1.45'

      # config/build.just owns the project, the output folder and --no-self-contained, so CI and a laptop build
      # the same thing. Do not inline restore/publish here again.
      - name: Publish
        run: just build publish

      # One metadata step now, not two: only the immutable tag is pushed here. The moving tags are computed in
      # `publish`, which is the only job that creates them.
      - name: Docker metadata
        id: meta
        uses: docker/metadata-action@c299e40c65443455700f0fdfc63efafe5b349051 # v5.10.0
        with:
          images: ${{ env.STAGING_IMAGE }}
          tags: |
            type=semver,pattern={{version}}
          # The only two labels metadata-action gets wrong: licenses auto-detects to NOASSERTION on a
          # dual-licensed repo, and url auto-fills the repo instead of the landing site.
          labels: |
            org.opencontainers.image.licenses=GPL-3.0-only AND CC-BY-SA-4.0
            org.opencontainers.image.url=https://www.binacle.net

      - name: Assemble the staging reference
        id: ref
        run: echo "staging=${STAGING_IMAGE}:${{ steps.meta.outputs.version }}" >> "$GITHUB_OUTPUT"

      # GITHUB_TOKEN is minted for this run and expires with it, so staging needs no stored credential.
      - name: Login to GHCR
        uses: docker/login-action@c94ce9fb468520275223c153574b00df6fe4bcc9 # v3.7.0
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@8d2750c68a42422c14e847fe6c8ac0403b4cbd6f # v3.12.0

      # file: is the literal Dockerfile at the repo root - there is exactly one, so a repo variable could only
      # drift from it.
      - name: Build and push to staging
        id: push
        uses: docker/build-push-action@ca052bb54ab0790a636c9b5f226502c73d547a25 # v5.4.0
        with:
          context: .
          file: Dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          # metadata-action's version, never github.ref_name: the latter would put "v3.0.0" in BINACLE_VERSION
          # while the image tag said "3.0.0".
          build-args: |
            VERSION=${{ steps.meta.outputs.version }}

  # The gate. Calls smoke-image.yml rather than copying its steps, so the release path runs exactly what a
  # maintainer runs by hand. Red here means nothing reaches Docker Hub and no release is created.
  smoke:
    needs: build
    uses: ./.github/workflows/smoke-image.yml
    permissions:
      contents: read
      packages: read
    with:
      image: ${{ needs.build.outputs.staging }}

  # The only job that touches Docker Hub, and the only place the stored credential is used.
  #
  # SKIPPED FOR A PRERELEASE. A hyphen in a semver tag is the prerelease marker, so a beta ends at `smoke` and
  # lives only on GHCR. This is stricter than metadata-action's own guard, which would still have published
  # the immutable tag publicly.
  publish:
    needs: [ build, smoke ]
    if: ${{ !contains(github.ref_name, '-') }}
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: read

    steps:
      # Computes the Docker Hub tag set. This job only runs for a real release, so all three apply - but
      # metadata-action still owns the semver parsing rather than this file hand-rolling it.
      - name: Docker metadata - public tags
        id: meta
        uses: docker/metadata-action@c299e40c65443455700f0fdfc63efafe5b349051 # v5.10.0
        with:
          images: ${{ vars.DOCKERHUB_ORGNAME }}/${{ vars.DOCKERHUB_REPO }}
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
          flavor: |
            latest=auto

      # PIN BY SHA BEFORE THIS LANDS - the version tag below is a placeholder, not a looked-up commit.
      - name: Setup crane
        uses: imjasonh/setup-crane@v0.4

      - name: Login to GHCR
        run: crane auth login ghcr.io -u ${{ github.actor }} -p ${{ secrets.GITHUB_TOKEN }}

      - name: Login to Docker Hub
        run: crane auth login docker.io -u ${{ secrets.DOCKERHUB_USERNAME }} -p ${{ secrets.DOCKERHUB_TOKEN }}

      # `crane copy` moves the manifest, not a rebuild - the digest is preserved, so Docker Hub serves exactly
      # what the smoke job passed. Copying by DIGEST rather than by tag is what makes that guarantee hold even
      # if something re-tagged staging in between.
      #
      # Every tag after the first is `crane tag`, a registry-side alias with no data transfer.
      - name: Copy staging to Docker Hub
        env:
          SOURCE: ${{ needs.build.outputs.staging }}@${{ needs.build.outputs.digest }}
          TAGS: ${{ steps.meta.outputs.tags }}
        run: |
          set -euo pipefail
          first=""
          while IFS= read -r tag; do
            [ -n "$tag" ] || continue
            if [ -z "$first" ]; then
              echo "Copying ${SOURCE} -> ${tag}"
              crane copy "$SOURCE" "$tag"
              first="$tag"
            else
              echo "Tagging ${first} as ${tag##*:}"
              crane tag "$first" "${tag##*:}"
            fi
          done <<< "$TAGS"
          echo "Published: $(echo "$TAGS" | tr '\n' ' ')"

  # Last, so the release never describes an image that failed to build, smoke or publish.
  #
  # The `if:` is load-bearing: `publish` is skipped for a prerelease, and a skipped dependency skips everything
  # downstream by default. Without this a beta would get no GitHub release at all.
  release:
    needs: [ notes, test, build, smoke, publish ]
    if: ${{ !failure() && !cancelled() }}
    runs-on: ubuntu-latest
    permissions:
      contents: write

    steps:
      - uses: actions/checkout@v4

      - name: Setup just
        uses: extractions/setup-just@53165ef7e734c5c07cb06b3c8e7b647c5aa16db3 # v4.0.0
        with:
          just-version: '^1.45'

      # The section was already proven to exist by the `notes` job, so this only formats and publishes it.
      - name: Create the GitHub release
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          TAG: ${{ github.ref_name }}
          SECTION: ${{ needs.notes.outputs.section }}
        run: |
          set -euo pipefail
          case "$TAG" in
            *-*) prerelease=(--prerelease) ;;
            *)   prerelease=() ;;
          esac
          just changelog extract "$SECTION" > "$RUNNER_TEMP/notes.md"
          gh release create "$TAG" --title "$TAG" --notes-file "$RUNNER_TEMP/notes.md" "${prerelease[@]}"
```

### Gaps in the draft - close these before running it

1. **`run-tests.yml` has no `workflow_call:` trigger.** Add one beside `pull_request` and `workflow_dispatch`.
   It takes no inputs. `smoke-image.yml` already has the equivalent; copy its shape.
2. **`smoke-image.yml` needs no change if the GHCR package is public.** Confirm that first. If the package is
   left private, it needs a `docker/login-action` step for `ghcr.io`, guarded on
   `if: ${{ startsWith(inputs.image, 'ghcr.io/') }}` so the manual Docker Hub dispatch keeps working.
3. **`imjasonh/setup-crane@v0.4` is not SHA-pinned.** Look up the real commit and pin it with the version in a
   trailing comment, matching every other third-party action here.
4. **The `release` job's `needs:` includes `notes`** because it reads that job's output. Keep it.

---

## Phase 4 - GHCR setup, by the maintainer

Not something a session can do. Record here when done.

- [ ] Push once so the package is created and linked to the repo. A package that exists in the namespace but
      is not linked makes `GITHUB_TOKEN` fail with `permission_denied` and no useful message.
- [ ] Set the package visibility to **public**. GHCR defaults every new package to private regardless of repo
      visibility, so this is a deliberate step, and the OVH deployment depends on it.
- [ ] Confirm `docker pull ghcr.io/chrismavrommatis/binacle-net:<tag>` works from the OVH server with no
      `docker login` at all.

---

## Phase 5 - housekeeping the same session should do

These are small, related, and were found while designing the above.

- [ ] **Add `.github/dependabot.yml`** for `github-actions`, weekly. SHA pins without automation rot silently:
      `docker/build-push-action` is pinned at v5.4.0 and is several majors behind.
- [ ] **Finish the SHA pinning.** `actions/checkout@v4`, `setup-dotnet@v4`, `setup-node@v4`, `setup-java@v4`
      and `cache@v4` are mutable tags while every third-party action is SHA-pinned. Pick one rule and apply it
      to all six workflows. Dependabot makes it maintainable.
- [ ] **Add `timeout-minutes` to every job** across all six workflows. There is none anywhere, so a hung smoke
      profile burns the default six hours.
- [ ] **The `publish` job needs no `contents` permission** - it never checks out. Trim it.

---

## Phase 6 - update the reference docs

The CI/CD docs and design ledger were written on 2026-08-11 against the pipeline this plan replaces. Both are
wrong the moment the workflow changes.

- [ ] **`.agents/docs/ci-cd/release-pipeline.md`** - rewrite for six jobs, GHCR staging, the prerelease
      split, and the CHANGELOG body. The current page describes four jobs and Docker-Hub-first.
- [ ] **`.agents/docs/ci-cd/README.md`** - the workflow table, the vars/secrets tables (GHCR needs no new
      secret, which is worth stating), and the "what CI does not cover" list, which loses the "image is never
      built on a PR" line only if that changed, and gains nothing otherwise.
- [ ] **`.agents/design/ci-cd/decisions.md`** - D2 and D3 are superseded, not deleted: the promote-by-digest
      reasoning still holds but now runs across registries, and D3's "empty tag list is the guard" is replaced
      by an explicit job-level skip. Record the change and why, and add decisions for the GHCR choice, the
      public-package call, and CHANGELOG.md as the single notes source. O1 (a prerelease cannot test promotion)
      is still open and gets worse - a prerelease now skips the entire publish job, so plan the throwaway-tag
      check accordingly.
- [ ] **`.agents/docs/config/README.md`** - add the `changelog` module to the scripts table.
- [ ] Regenerate the indexes with `just agents all`.

---

## Done when

- A beta tag builds, smokes on GHCR, creates a prerelease GitHub release from `[Unreleased]`, and puts
  **nothing** on Docker Hub.
- A release tag does all of that plus copies to Docker Hub as `x.y.z`, `x.y` and `latest`, all three on the
  digest the smoke job passed.
- A tag whose changelog section is missing fails in under a minute, before anything is built or pushed.
- `just changelog extract <version>` prints the same body the release shows.
- The OVH server pulls a beta with no credentials.
- The CI/CD docs and design ledger describe this pipeline, not the old one.

## Do not

- Rebuild the image in the publish job. Copy by digest or the smoke proves nothing.
- Reword any historical release body while building the changelog.
- Let a real release fall back to generated notes.
- Add `release: published` as a second trigger.
- Touch repo-root `docs/` or `web/`.
