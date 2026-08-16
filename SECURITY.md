# Security Policy

## Reporting Security Issues

I take security bugs in Binacle.Net seriously. I appreciate your efforts to responsibly disclose your findings.

To report a security issue, please use GitHub's Security Advisory ["Report a Vulnerability"](https://github.com/binacle-labs/Binacle.Net/security/advisories/new) tab.

I will send a response indicating the next steps in handling your report. After the initial reply, I will keep you informed of the progress towards a fix and full announcement.

## Supported Versions

I release security patches for the latest version only. Please ensure you are using the most recent release.

| Version | Supported          |
| ------- | ------------------ |
| latest  | :white_check_mark: |
| < latest| :x:                |

## Verifying a Release

Images published from `3.0.0` onward are signed, and carry an SPDX software bill of materials and SLSA
build provenance. Replace `<version>` with the release you pulled.

```bash
cosign verify binacle/binacle-net:<version> \
  --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com

docker buildx imagetools inspect binacle/binacle-net:<version>
```

Both flags on `cosign verify` matter. Without the identity you are only asking whether *anyone* signed the
image, and anyone can - Sigstore is open to every GitHub account. The two flags together are what say it came
from this repository's release workflow.

The signature covers the image digest, so it holds for the `3.0` and `latest` tags as well as the exact
version - verifying any of them verifies the same artifact.

**Releases before `3.0.0-beta.2` cannot be verified.** `3.0.0-beta.1`, `2.1.1` and everything earlier were
published before the signing pipeline existed, so `cosign verify` answers `no signatures found` against them.
That is history rather than a failed check, and it applies to a moving tag like `latest` for as long as it
still points at one of those releases.

**`3.0.0-beta.2` verifies under a different identity.** It was signed before this repository moved to the
`binacle-labs` organization. To check that one release, put `ChrisMavrommatis` in place of `binacle-labs` in
the identity above.

**A passing verify means the image came from this repository's release workflow. It does not mean the image is
free of vulnerabilities.** For that, read the bill of materials.

## Third-Party Dependencies

For security issues in third-party dependencies, please refer to:
- [NOTICE](NOTICE) - Direct dependencies and their licenses
- [Dependencies](https://github.com/binacle-labs/Binacle.Net/network/dependencies) - Complete Software Bill of Materials

Report security bugs in third-party modules to the respective maintainers or through their security channels.
