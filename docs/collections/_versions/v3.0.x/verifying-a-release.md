---
title: Verifying a Release
nav:
  order: 8
  icon: 🔏
---

Every image published from `3.0.0-beta.2` onward is signed with [Sigstore](https://www.sigstore.dev/) cosign,
and carries an SPDX software bill of materials and SLSA build provenance. Signing is keyless and happens inside
the GitHub Actions release workflow, so there is no private key anywhere - the signature is tied to the workflow
that built the image.

Two commands cover it. Replace `<version>` with the release you pulled.

```bash
cosign verify binacle/binacle-net:<version> \
  --certificate-identity-regexp '^https://github\.com/ChrisMavrommatis/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com

docker buildx imagetools inspect binacle/binacle-net:<version>
```

The rest of this page is what those two commands mean and how to read what they print.

> **Releases before `3.0.0-beta.2` cannot be verified.** `3.0.0-beta.1`, `2.1.1` and everything earlier were
> published before the signing pipeline existed, so `cosign verify` answers `no signatures found` against them.
> That is history, not a failed check. It applies to a moving tag like `latest` too, for as long as it still
> points at one of those releases.
{: .block-warning}

## 🛠️ Install cosign

`cosign` is a single binary. Install it from the [Sigstore docs](https://docs.sigstore.dev/) or download it
from the [cosign releases page](https://github.com/sigstore/cosign/releases). `docker buildx` already ships
with Docker.

## 🔐 Why both cosign flags matter

Drop `--certificate-identity-regexp` and you are only asking whether *anyone* signed the image. Anyone can:
Sigstore is open to every GitHub account, and a signature on its own says nothing about who made it. The two
flags together are the whole check - the issuer says the identity came from GitHub Actions, and the identity
pattern says it was this repository's release workflow.

The signature covers the **image digest**, not the tag. So it holds for the `3.0` and `latest` tags as well as
the exact version tag: whichever one you verify, you are verifying the same artifact.

## 🧾 Reading the attestations

`docker buildx imagetools inspect` lists what is in the image index: the platform manifests, plus one
attestation manifest per platform holding the SPDX bill of materials and the SLSA provenance.

The bill of materials is the package list - every OS package and .NET assembly in the image, with versions. It
is what you feed to a scanner or check a CVE against. The provenance records how the image was built: the
workflow, the run, and the source commit it came from.

## 🔍 A worked example

Against `3.0.0-beta.2`, the verify passes and the tag resolves to this digest on Docker Hub:

```text
sha256:ccce2a441e9c7d8b301d7f3f57777d9fa25b295d1a5bd3c07b5e738fc54b3397
```

The same release inspects to a bill of materials of **167 packages**, and provenance naming the build that
produced it:

```text
https://github.com/ChrisMavrommatis/Binacle.Net/actions/runs/31738643520/attempts/1
```

The image config in the same output shows the container runs as `app (1654)` rather than root, with `/app/data`
owned `app:app 755` - the writable folder for a mounted database or key ring.

## 🚦 What a pass means, and what it does not

A passing verify proves the image came from this repository's release workflow and has not been altered since.
That is a strong claim about **origin**.

It is not a claim about **safety**. A signature says nothing about the vulnerabilities in what was signed - a
genuine image with a known CVE in it verifies perfectly. For that question, read the bill of materials and scan
it. Do not treat a green check as a clean bill of health.

> Contributors with a clone can run `just image verify <version>`, which runs four checks against a published
> image in one go.
{: .block-tip}
