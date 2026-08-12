# CI - publish the image for arm64 as well as amd64

**Status:** Not started, and **not scheduled** - the demand question below has never been answered. Trimmed out
of `ci-release-workflow-build` on 2026-08-11, which was superseded when the release pipeline was rebuilt on
GHCR. This is the only part of that plan that had not landed.

**What changed under it, and it matters for the shapes below.** The pipeline now stages on GHCR and copies the
manifest to Docker Hub by digest; the publish is `--no-self-contained --runtime linux-x64`; and the image
carries an SBOM, provenance and a cosign signature, so the pushed artifact is already an OCI index rather than
a single manifest. Two consequences:

- **The copy step already moves an index.** Adding a second platform makes that index bigger; it does not
  change the copy, which was verified across registries carrying an amd64 manifest plus its attestation.
- **Signing needs re-checking.** `cosign sign` is run against the digest of the index. Confirm that covers
  every platform manifest under it rather than only the top-level entry before claiming a multi-arch image is
  signed.

Everything below was written on 2026-08-10 against the old pipeline. The reasoning holds - the naming trap, the
`.dockerignore` allowlist, and the testing ladder are all independent of where the image is staged.

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
COPY ["artifacts/binacle-net-${TARGETARCH}", "."]
```

publishing to `artifacts/binacle-net-amd64` and `artifacts/binacle-net-arm64`.

**The trap is the naming.** Docker's `TARGETARCH` is `amd64` / `arm64`; .NET's runtime identifiers are
`linux-x64` / `linux-arm64`. They do not match, and the mapping has to live in exactly one place or the image
gets the wrong binaries and still builds.

Also note `.dockerignore` allowlists `artifacts/binacle-net`. Renaming the publish directories means updating it,
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
