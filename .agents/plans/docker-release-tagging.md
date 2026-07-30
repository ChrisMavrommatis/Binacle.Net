# Docker release - build the image once, and prove a prerelease tag is safe

**Status:** Not started. Gates the v3.0.0 beta. Both items are about `.github/workflows/release-docker-image.yml`,
which nobody has run since the `Binacle.Geometry` extraction and which has never been fired by a prerelease tag.

## 1. Build the image once before tagging

The test suites are covered - `run-tests.yml` builds the solution and runs every C# suite plus the TS suites on
each PR, including ServiceModule against both SQLite and Postgres. It does **not** build the docker image, and it
triggers only on `pull_request` / `workflow_dispatch`, never on a tag.

So the image build has not run since the repo was restructured and `Binacle.Geometry` was extracted. Build it
once by hand before the beta tag:

```
just build image         # publishes, then tags binacle-net:local
```

Also make sure the PR that lands the release went green - nothing gates that the tagged commit is the tested
commit.

**Blocked by the `API_PROJECT_PATH` Actions variable.** The workflow publishes
`${{ vars.API_PROJECT_PATH }}`, which still points at the pre-move `src/Binacle.Net/Binacle.Net.csproj`. Until
it says `api/src/Binacle.Net/Binacle.Net.csproj`, the publish step fails. That is a repo settings change, tracked
as an action on the release file.

## 2. Prove a prerelease tag moves neither `latest` nor the minor tag

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

The image has been built once against current code, and the prerelease tagging behaviour is known rather than
assumed - for both moving tags. Record the answers here before deleting the file.
