---
description: Verify a published image - recipes, and telling users they can
paths:
  - "tooling/**"
---

# Verify a published image - recipes, and telling users they can

**Status:** Not started. **Nothing is implemented** - a working prototype was built on 2026-08-13, run green
against `binacle/binacle-net:3.0.0-beta.2`, and then reverted, so the tree is clean. This file is what it
learned. Written 2026-08-13, after beta 2 was published and verified by hand.

**Two halves, and they are not the same job.** The first is a `just` recipe so a check that currently takes
five commands and a 90-character flag takes one. The second is **telling anyone it exists** - which is the part
that decides whether the signing work was worth doing at all.

**The placement question is open and is the maintainer's call** - see "Where it goes". A session picking this
up decides that first, because it changes what gets written.

## Why bother

Since v3.0.0 every published image is signed, carries an SPDX SBOM and SLSA provenance, and is copied to Docker
Hub by digest from a smoke-tested GHCR staging copy. **None of that does anything for a user who does not check
it, and nobody checks what they have not been told about.** Right now the only place the `cosign verify`
invocation exists is the `CHANGELOG.md` release body and the plan on finishing the GHCR pipeline.

It is also a maintainer tool. Every release wants the same five answers, and the alternative to a recipe is
retyping a certificate-identity regexp from a previous terminal - which is exactly the kind of thing that gets
shortened until it no longer proves anything.

## The five checks

Each is one question, and the order matters - each answers something the next one assumes. All five were run
against `3.0.0-beta.2` on 2026-08-13 and all five passed; the outputs below are real.

**1. Digest, on both registries.** Resolve the tag at Docker Hub and at GHCR and compare.

```
docker buildx imagetools inspect binacle/binacle-net:<v> --format '{{ .Manifest.Digest }}'
docker buildx imagetools inspect ghcr.io/chrismavrommatis/binacle-net:<v> --format '{{ .Manifest.Digest }}'
```

Both gave `sha256:ccce2a44...`. **This is the check the whole pipeline shape exists to earn.** `publish` copies
by digest rather than rebuilding, and a copy preserves the hash - so equal digests mean Docker Hub is serving
the artifact the smoke job passed. Unequal means it is not, and nothing else in this list matters.

**2. The Docker Hub tag map.** `https://hub.docker.com/v2/repositories/binacle/binacle-net/tags?page_size=100`,
name + digest + size + date. Rows sharing a digest are the same image under two names, which is how you see
that `latest` still resolves to `2.1.1` (`f48edc911771`) and that no `3.0` row exists. Reading the *date* is
the trap: it moves for reasons that are not a retag.

**3. Signature.** `cosign verify` against **both** registries - a signature is a referrer, stored beside the
image rather than inside the index, so it does not survive `imagetools create` and the pipeline signs twice.
Checking one proves nothing about the other.

```
cosign verify <ref> \
  --certificate-identity-regexp '^https://github\.com/ChrisMavrommatis/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com
```

**Both flags are the entire value and a recipe must never make them optional.** Without the identity, this asks
"did anyone sign this" - and anyone can, Sigstore is open to every GitHub account alive. A useful extra: an
identity regexp that cannot match (`'^$'`) makes cosign print the real one in the error, which is the cheapest
way to read the signer off an image:

> `got "https://github.com/ChrisMavrommatis/Binacle.Net/.github/workflows/release-docker-image.yml@refs/tags/v3.0.0-beta.2"`

**4. Attestations.** `--format '{{ json .SBOM }}'` and `'{{ json .Provenance }}'` on `imagetools inspect`.
Beta 2: **167 SBOM packages**, built from `Dockerfile`, builder
`.../actions/runs/31738643520/attempts/1`. Both are manifests inside the index, so the index digest hashes them
and the one signature covers them - worth a comment in the recipe, because it is the reason there is nothing
extra to verify here.

**5. Metadata, and what the container says about itself.** Labels (`version`, `revision`, `base.name`), plus
`BINACLE_VERSION`, the uid, `/app/data`'s owner, and the `System.*.dll` count from inside a throwaway run.
Beta 2: revision `d317cd2b`, reports `3.0.0-beta.2`, runs as `app (1654)`, `/app/data` is `app:app 755`, and
**4** System dlls - the framework-dependent proof, where a self-contained build shows ~170.

## Where it goes - settled 2026-08-14

**It goes in `tooling/image.just`.** The prototype was a new `tooling/verify.just` module; a sixth module for
one job lost to the simpler grouping. The maintainer's call was that `image` means the image, local and
published, and that the backing services it stands up today belong to `serve` - which makes this exactly the
module a verification recipe belongs to. **Do not re-open this.**

The shape: one public recipe taking the version and an optional check name - `just image verify 3.0.0`,
`just image verify 3.0.0 signature` - with the five checks as private `_verify_*` helpers. That adds one line
to the module's help header rather than five, and keeps the file from becoming mostly verification.

**The module's charter sentence has to change either way.** It says everything in the module runs
`binacle-net:local`, and this is the first recipe that reads a registry.

**A separate piece of work rewrites that module's `up` and `down` recipes** and collapses its stacks. It does
not block this and this does not block it - the two touch different recipes - but whichever lands second reads
the other's header comment rather than reverting it.

Three things follow:

- The **version argument is required, never defaulted.** A default rots into a tag nobody meant to check, and
  green against last release is worse than no output.
- **No `docker login` anywhere.** These are the commands a user runs, and running them with a credential would
  not prove what they claim. The prototype worked anonymously against both registries.
- **cosign goes in `DEVELOPMENT.md`** beside `container-structure-test` and `hurl`, pinned to a version, with
  the same "single binary into `~/.local/bin`" shape. It is the only new dependency; `docker`, `curl` and `jq`
  are already assumed.

## Traps the prototype hit

Both cost time and neither is guessable, so they are the reason this file exists rather than a one-liner.

- **`{{` in a recipe is just's own interpolation.** A Go template for `docker --format` has to be written
  `{{{{ .Manifest.Digest }}` - **four braces open, two close.** The obvious `{{{{ ... }}}}` emits a literal
  `}}` on the end of the value, which silently corrupts the output: the digest comparison still passed, but
  every value carried a trailing `}}` and piping `{{{{ json .SBOM }}}}` into `jq` produced
  `parse error: Unmatched '}'` while still printing the right answer above it.
- **A multi-line Go template cannot be indented inside a recipe.** just reads the continuation lines as recipe
  lines and rejects the inconsistent indentation. Use `docker image inspect ... | jq -r` and build the lines in
  jq, or one-line the template.

Also worth keeping: **`set -e` is wrong for the aggregate recipe.** A failed check is the interesting output,
so run all five, OR their exit codes, and fail at the end - otherwise the first failure hides the four answers
that would explain it.

## Half two - how users find out

**Five surfaces. Only two can be written from a coding session**, and the split is the point of this section.

| Surface | Owner | What it says |
|---|---|---|
| `CHANGELOG.md` release body | **done** | The `cosign verify` invocation and an `imagetools inspect` line, in `⚙️ Core Changes` |
| `SECURITY.md` | a coding session - **free to edit** | The permanent short version: what is signed, the two commands, what a pass means |
| `README.md` | a coding session - **free to edit** | One line under the pin note pointing at `SECURITY.md`. No commands - the landing page is not the reference |
| The Docker Hub repository page | **the Docker Hub page work** - not written here | A "Verifying what you pulled" section, the same scope as `SECURITY.md`. That plan carries the draft; this one owns the wording it copies |
| The docs site | **off limits here** - the docs session | The long form. Specified below, because that spec has nowhere else to live |

**`SECURITY.md` is the right permanent home for the short version** and it is not off limits - only repo-root
`docs/` and `web/` are. The file already has a "Third-Party Dependencies" section pointing at `NOTICE` and the
dependency graph, so a "Verifying a release" section sits naturally beside it.

**The Docker Hub page is the surface with the strongest claim of the five and it is the one that was missing.**
It is where the pull actually happens - a reader is on it because they are about to run `docker run` - so it is
the last place that should send someone elsewhere to find out the image is signed. It gets the commands, not a
link.

### What the docs-site page must say

The docs session writes it. Both commands below were run against the published `3.0.0-beta.2` on 2026-08-13 and
pass verbatim, against Docker Hub and against GHCR - swap the tag for the version being documented:

```bash
cosign verify binacle/binacle-net:3.0.0 \
  --certificate-identity-regexp '^https://github\.com/ChrisMavrommatis/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com

docker buildx imagetools inspect binacle/binacle-net:3.0.0
```

Three things the page should say that a changelog bullet has no room for:

- **The signature covers the digest**, so it holds for `3.0` and `latest` as well as the exact-version tag, and a
  verify against any of them is a verify of the same artifact.
- **`imagetools inspect` shows the attestations**: an SPDX SBOM and SLSA provenance, both manifests inside the
  image index. That is why they survive the copy from GHCR to Docker Hub while the signature, a referrer stored
  beside the image, has to be made again.
- **GHCR's referrers API answers 404**, so a `referrers` call there looks empty. cosign finds the signature
  through its fallback tag and verifies fine. Only mention this if the page shows a GHCR pull at all.

### One invocation, four copies - keep them honest

After this lands the same `cosign verify` exists in `CHANGELOG.md`, `SECURITY.md`, the Docker Hub page and the
docs site. **That is accepted, not an oversight** - each audience arrives somewhere different and a link instead
of the command defeats the point. What is not accepted is finding out they disagree from a user.

So write it down once here: **the only things that change that invocation are renaming
`.github/workflows/release-docker-image.yml` or moving the repository.** Both are rare and both are visible in a
diff. If either happens, all four change together, and the certificate-identity regexp is the part that breaks -
the issuer flag never moves.

**Do not swap the commands for `just image verify` on any user-facing surface.** The recipe is for someone with
a clone; these readers have a registry and a shell. The recipe and the pages solve different problems and each
should keep its own form.

**One ordering constraint, and it is easy to get wrong.** Any example naming a tag has to name a **signed**
one. Signing started with beta 2, so:

- `3.0.0-beta.1` is **not signed** - an example using it fails with `no signatures found`, which reads as our
  bug rather than as history.
- `3.0` and `latest` do not point at a signed image until v3.0.0 is published.

So write the prose **version-neutral** with a `<version>` placeholder, and use `3.0.0-beta.2` if a worked
example is wanted before the tag. This is the one part of the notification work that cannot simply be done
early.

**It binds every surface, and it bites hardest on the Docker Hub page**, whose draft currently names `3.0`
throughout. There the constraint is doubled: a `cosign verify` against an unsigned tag reads as our bug, and the
quick start above it names the same tag - which does not resolve at all yet, since Docker Hub has no `3.0` row
and `latest` is still 2.1.1. That page must not go up before v3.0.0, or must say `3.0.0-beta.2` everywhere. The
plan that owns it carries the same gate; if one is ever relaxed, relax both.

**Say what a pass means, and say what it does not.** The sentence users need is that a verify proves the image
came from this repo's release workflow - not that it is free of vulnerabilities. Skipping that is how a
signature turns into a marketing badge.

## Done when

- One command runs all five checks against a published version, from a clone, with no `docker login`.
- It fails loudly and specifically when a check fails - and someone has watched it fail at least once, against
  an unsigned image such as `2.1.1`, which is the only proof the check is not a no-op.
- `cosign` is in `DEVELOPMENT.md` with a pinned version.
- The tooling docs describe the recipe, and the module's own header does too.
- `SECURITY.md` carries the short version and `README.md` points at it, both version-neutral.
- The docs-site page is written, or its content is recorded for the docs session (it is, above).
- The wording handed to the Docker Hub page work is the same wording `SECURITY.md` ends up with. That page is not
  this plan's to write, but a second version of the sentence is this plan's to prevent.

## Do not

- Ship a `cosign verify` anywhere - recipe, doc or page - without both the identity and the issuer flag.
- Default the version argument.
- Add a `docker login` to make a check work. If a check needs one, the thing it is checking is not public, and
  that is the finding.
- Restate the docs-site page's **prose** in `SECURITY.md`, or the reverse. The two commands are deliberately
  repeated across surfaces; the explanation around them is not, and that is the half that rots.
- Replace the commands with `just image verify` on a user-facing surface. Those readers have no clone.
- Promise that a verified image is a safe image.
