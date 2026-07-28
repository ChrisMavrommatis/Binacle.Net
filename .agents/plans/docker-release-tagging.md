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

## 2. Prove a prerelease tag does not move `latest`

The workflow fires on `release: published` and builds its tags with:

```yaml
uses: docker/metadata-action@v5
tags: |
  type=semver,pattern={{version}}
```

There is no explicit `flavor:`, so the default `latest=auto` applies - `latest` is generated for a semver tag
that is **not** a prerelease. On that reading, `v3.0.0-beta.1` produces only `binacle/binacle-net:3.0.0-beta.1`
and leaves `latest` on `2.1.1`.

That reading has never been tested, and it matters: every sample in the repo, the docs quick-start, and the
sample compose files inside the older docs version folders all pull `binacle/binacle-net:latest`. If `latest`
moves to the beta, every one of those users silently gets a prerelease with breaking changes.

Confirm before publishing the beta release. Either:
- dry-run the metadata step (`workflow_dispatch` a copy, or run `docker/metadata-action` locally against the tag
  name), or
- publish the beta and check Docker Hub immediately, with `latest` ready to be re-pointed.

If it does move, set `flavor: latest=false` and add an explicit `latest` tag rule for final releases only.

## Done when

The image has been built once against current code, and the prerelease tagging behaviour is known rather than
assumed. Record the answer here before deleting the file.
