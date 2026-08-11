---
name: version-only-when-published
description: A component gets its own version number only once it is published independently; until then the docker image's BINACLE_VERSION is the only version.
type: decision
---

Nothing in this repo carries a version of its own. No `<VersionPrefix>` or `<Version>` in
`Directory.Build.props` or any `.csproj`, and every TS workspace package is `private` with an inert number.
The single version that exists is `BINACLE_VERSION`, set from `ARG VERSION` in the `Dockerfile` and fed by the
release tag. `Binacle.Net.Metadata.Version` is the one place that reads it.

**Why:** a version is only worth having when someone can obtain the component *independently of the product*.
Nothing here can be. `Binacle.Lib`, `Binacle.ViPaq`, `Binacle.Geometry` and `Binacle.CompactNotation` are
consumed only by `api/src/Binacle.Net` and by tests and tools inside this repo; nothing is packable, and no TS
package is published. Every project ships inside one artifact - the docker image - so a per-component number
could only ever repeat the image's, or lie about it. Stamping assemblies would add a second number that means
nothing, and "which build is this" is already answered by the image tag.

The ViPaq case looks like an exception and is not. Its wire format versions itself **in-band**: a 2-bit
`Version` field in byte 0 of every token, currently `0`, with `1`-`3` reserved, and a decoder rejects a
`Version` it does not know (`vipaq/PROTOCOL.md`). So token compatibility is answered by the token, not by a
package number. An assembly version on `Binacle.ViPaq` would only say which *implementation* of spec `Version 0`
you have, and it would not move between releases at all.

**How to apply:** do not add a version property to a `.csproj`, to `Directory.Build.props`, or to a
`package.json` in order to make the numbers look tidy. Leave the stale TS numbers alone - they are decoration
and bumping them communicates nothing.

The trigger that reverses this is **publishing something independently**: a `PackageId` going to NuGet, or an npm
package losing `private`. That component then earns a real version line, and two things become live problems
that are harmless today:

- A release build passes the version as a **global** MSBuild property, so `-p:Version=` on the API project
  flows into `Binacle.Lib`, `Binacle.ViPaq` and `Binacle.Geometry` too. Verified: all four assemblies took
  `9.9.9`. Per-component versions need a differently named property that only the owning directory consumes.
- A nested `Directory.Build.props` does **not** inherit the root one - MSBuild imports only the nearest. A
  `vipaq/Directory.Build.props` added carelessly silently drops `TargetFramework`, `Nullable`, `ImplicitUsings`
  and the `AD0001` `NoWarn`. It has to `<Import>` the root explicitly.
