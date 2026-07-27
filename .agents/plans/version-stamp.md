# Give the build a version of its own

**Status:** Not started. After v3.0.0 - it does not block the release, but the release is what exposes it.

## Why

Nothing in the build carries a version. There is no `<Version>`, `<VersionPrefix>`, `<AssemblyVersion>` or
`<InformationalVersion>` in any `.csproj` or in `Directory.Build.props`. The only version the app ever learns is
`ARG VERSION` in the `Dockerfile`, turned into `ENV BINACLE_VERSION`, and that is fed by
`${{ github.ref_name }}` in the release workflow.

Consequences:

- An image built locally or in CI has no version at all - `config/build.sh` passes whatever `$VER` happens to be.
- Assemblies all report the SDK default. Nothing in a crash dump, a log line, or a support question says which
  build it came from.
- Root `package.json` still says `2.0.1`, which was true two majors ago.

## What

- Decide where the number lives. One `<VersionPrefix>` in `Directory.Build.props` is the smallest thing that
  works, with CI overriding it from the tag on a release build.
- Decide whether the API surfaces it - a version in the OpenAPI document info block, or on a health endpoint, is
  the cheap way to answer "which build is this" without shell access to the container.
- Decide what to do with root `package.json`. It is `private`, so its version is decoration - either track the
  product version or set it to something that admits it is not tracking anything.

## Watch out

Do not put the version in twenty `.csproj` files. Anything per-project drifts.

## Done when

A built artifact can say which version it is without being asked what tag produced it.
