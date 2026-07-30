# Docker release - prove a prerelease tag is safe

**Status:** Down to one item. The `API_PROJECT_PATH`/`BUILD_OUTPUT` block is resolved - the workflow's publish
step hardcodes `api/src/Binacle.Net/Binacle.Net.csproj` and `build/binacle-net` (2026-07-30), matching the
Dockerfile `COPY` source and `build.just`, so no repo-settings variable can break it. The image was also built
once against current code (`just build image`, green). What remains gates the v3.0.0 beta: proving the
prerelease tag moves neither `latest` nor the minor tag.

## Prove a prerelease tag moves neither `latest` nor the minor tag

The workflow fires on `release: published` and publishes two semver tags plus `latest`: `{{version}}` and
`{{major}}.{{minor}}` (so v3.0.0 gives `3.0.0` and `3.0`, and a sample pinned to `3.0` inherits every later
patch). Neither moving tag should ever land on a prerelease:

- `latest=auto` is the default `flavor:`, which applies `latest` only to a non-prerelease semver tag.
- metadata-action is documented to skip `{{major}}.{{minor}}` for a prerelease, the same guard.

**Neither has been observed in this repo.** If `v3.0.0-beta.1` moves `latest`, everyone on the docs quick-start
gets a prerelease with breaking changes; if it publishes `3.0`, so does everyone pinned to the minor line. One
beta tag answers both. Either dry-run the metadata step (`workflow_dispatch` a copy, or run
`docker/metadata-action` locally against the tag name), or publish the beta and check Docker Hub immediately.

Fixes, if either fires: `flavor: latest=false` plus an explicit `latest` rule for final releases only, and
`enable=` on the `{{major}}.{{minor}}` pattern to exclude prereleases. Delete the bad tag from Docker Hub.

There is deliberately no `{{major}}` rule - a `3` tag hands a reader the next minor line on an ordinary pull, and
nothing in the repo or docs pins it. Tags are also created at build time, so none of this backfills: `2.1` will
never exist, and everything already released keeps its exact pin.

## Done when

The prerelease tagging behaviour is known rather than assumed - for both moving tags. Record the answers here
before deleting the file.
