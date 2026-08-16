---
id: decisions
description: General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, and the rule that a version is named only where the version is the fact.
verified: 2026-08-17
check: D1 against the copyright lines in NOTICE, README.md, LICENSE.CC-BY-SA-4.0, package.json, packages/binacle-net-ui/package.json and Footer.razor, and against org.opencontainers.image.vendor in Dockerfile; D3 against the certificate-identity-regexp in SECURITY.md, CHANGELOG.md and tooling/image.just, which must all name binacle-labs
paths:
  - "NOTICE"
  - "README.md"
  - "SECURITY.md"
  - "CHANGELOG.md"
  - "Dockerfile"
  - "docs/**"
---

# General — decisions ledger

Decisions that belong to no single slice. What each area *is* lives in its own doc; this file is the reasoning,
so a later session does not undo a deliberate choice.

## Locked

### D1 — the repository moved to an organization, and copyright did not move with it

The repo has lived at `binacle-labs/Binacle.Net` since 2026-08-16.

**Copyright and authorship stay on the person, everywhere** — `NOTICE`, `README.md`,
`LICENSE.CC-BY-SA-4.0`, `package.json`, `packages/binacle-net-ui/package.json`, `Footer.razor` and the two
`.gemspec` files. Moving a repository into a GitHub organization does not move copyright, and `binacle-labs`
is a namespace rather than a legal entity — there is nothing for it to hold. Writing the org name into a
copyright line would make that line less true.

**The vendor label does move.** `org.opencontainers.image.vendor` in the `Dockerfile` reads `Binacle Labs`.
It is descriptive and makes no legal claim, which is the whole reason it can differ from the copyright line.
In CI, metadata-action overrides title, description, source, url and licenses — **`vendor` is not in that
list**, so the `Dockerfile` value is what reaches published images.

**This is written down because a sweep that replaces one string tends to replace the other.** The copyright
lines are correct as they are. Do not tidy them.

### D2 — a version's published page must match what that version's image serves

Each folder under `docs/collections/_versions/` describes the image that shipped under that minor version.
`2.1.1` really does serve `https://github.com/ChrisMavrommatis/Binacle.Net` in its OpenAPI documents and its
UI, so rewriting v1.3.x, v2.0.x or v2.1.x to say `binacle-labs` would make the page disagree with the running
artifact. **Only v3.0.x changed**, because `3.0.0` is built after `Metadata.cs` moved and serves the new owner.
How the site is versioned is `$docs-site`.

The same reason covers every other survivor of the move: the `v1.3.0...v2.0.0` compare link in `CHANGELOG.md`,
the `ChrisMavrommatis.*` NuGet package names listed there, the 2024 records under `results/lib/benchmarks/`,
and the links to workflow runs that happened under the old owner. They are records of what was true then.
GitHub redirects them forever, and rewriting them makes them false.

**The swagger json under each version folder is generated output.** Regenerate it, never hand-edit it — the
rule and the generator are in `$docs-site`.

### D3 — the signing identity moved with the repository, and there are three bands

cosign keyless writes the repository's full path into the certificate, so the published verify command names
the owner. **GitHub redirects web links; it does not redirect a signing identity.** A stale one fails the
check rather than warning, and a `cosign verify` failure reads as tampering rather than as a moved repo.

| Band | Images | Verifies with |
|---|---|---|
| unsigned | up to `3.0.0-beta.1`, and `2.1.1` and earlier | nothing — `no signatures found` |
| old identity | `3.0.0-beta.2` alone | `ChrisMavrommatis` in the identity regexp |
| new identity | `3.0.0-beta.3` onward | `binacle-labs`, the string every surface now carries |

Signing, the SBOM and the GHCR staging copy all start at beta 2. Beta 3 is the first tag pushed after the move.

**`3.0.0-beta.3` is the only image that verifies under the current identity**, so it is the tag to name
wherever a doc needs a real one and the tag to re-run any verification against. Beta 2 is signed but the
published command rejects it.

**Proven end to end on 2026-08-17.** `just image verify 3.0.0-beta.3` passed all four checks; the command
printed in `SECURITY.md` passed verbatim from a clean shell; and the SLSA provenance names
`github.com/binacle-labs/Binacle.Net/actions/runs/31970609518` — Fulcio's record of which workflow signed it,
not a string this repo controls.

Which surfaces carry the invocation, and what else would change it, is `$ci-cd/decisions#D15`.

### D4 — name a version where the version is the fact, never as a floor or an example

A floor ("signed from `X` onward") and a sample tag both go stale on their own. A record of what was signed
does not. So a floor names the current released version, an example uses a placeholder the reader
substitutes, and a concrete version survives only where the point is what happened to that version.

**No public surface names a beta image at all** — decided 2026-08-17. A beta stays pullable long after it
stops being the right thing to pull, and a published command that fails against it reads as our bug rather
than as history. Agent docs under `.agents/` may name one, and have to: the bands in D3 mean nothing without
the numbers.

## Open

### O1 — what happens to `3.0.0-beta.1` and `3.0.0-beta.2` on Docker Hub

Both are still pullable. Under D4 no public surface names either, and under D3 neither passes the published
command — beta 1 was never signed, and beta 2 needs a string no page carries any more. So anyone who pulls one
gets a failure with nothing anywhere to explain it.

Deleting both tags once v3.0.0 is out is the clean end of it, and **it has a deadline**: Docker Hub tag
immutability is off today, and an immutable tag cannot be deleted. Not decided.
