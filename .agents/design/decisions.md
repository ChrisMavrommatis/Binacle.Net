---
id: decisions
description: General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, the rule that a version is named only where the version is the fact, and how the agent reference layer is kept honest against the code.
verified: 2026-08-19
check: D1 against the copyright lines in NOTICE, README.md, LICENSE.CC-BY-SA-4.0, the root package.json author, Footer.razor and the two gemspecs, and against org.opencontainers.image.vendor in Dockerfile; every repository.url stays on binacle-labs; D3 against the certificate-identity-regexp in SECURITY.md, CHANGELOG.md and tooling/image.just, which must all name binacle-labs
paths:
  - "NOTICE"
  - "README.md"
  - "SECURITY.md"
  - "CHANGELOG.md"
  - "Dockerfile"
  - "sites/docs/**"
---

# General — decisions ledger

Decisions that belong to no single slice. What each area *is* lives in its own doc; this file is the reasoning,
so a later session does not undo a deliberate choice.

## Locked

### D1 — the repository moved to an organization, and copyright did not move with it

The repo has lived at `binacle-labs/Binacle.Net` since 2026-08-16.

**Copyright and authorship stay on the person, everywhere they appear** — `NOTICE` and `README.md`
("Copyright (c) 2023-2026 Chris Mavrommatis"), `LICENSE.CC-BY-SA-4.0` ("© 2026"), the root `package.json`
`author`, `Footer.razor`'s rendered `CopyrightNotice`, and the `authors` in both `.gemspec` files. Moving a
repository into a GitHub organization does not move copyright, and `binacle-labs` is a namespace rather than a
legal entity — there is nothing for it to hold. Writing the org name into a copyright line would make that line
less true.

**A `repository.url` is the opposite case and does carry the org**: both `package.json` files point at
`github.com/binacle-labs/Binacle.Net`, which is where the repository actually is. `packages/binacle-net-ui/package.json`
has a `repository` and a `license` but **no author field at all**, so there is nothing on it to protect — do not
add one to make the set look symmetrical.

**The vendor label does move.** `org.opencontainers.image.vendor` in the `Dockerfile` reads `Binacle Labs`.
It is descriptive and makes no legal claim, which is the whole reason it can differ from the copyright line.
In CI, metadata-action overrides title, description, source, url and licenses — **`vendor` is not in that
list**, so the `Dockerfile` value is what reaches published images.

**This is written down because a sweep that replaces one string tends to replace the other.** The copyright
lines are correct as they are. Do not tidy them.

### D2 — a version's published page must match what that version's image serves

Each folder under `sites/docs/collections/_versions/` describes the image that shipped under that minor version.
`2.1.1` really does serve `https://github.com/ChrisMavrommatis/Binacle.Net` in its OpenAPI documents and its
UI, so rewriting v1.3.x, v2.0.x or v2.1.x to say `binacle-labs` would make the page disagree with the running
artifact. **Only v3.0.x changed**, because `3.0.0` is built after `Metadata.cs` moved and serves the new owner.
How the site is versioned is `$sites/docs`.

The same reason covers every other survivor of the move: the `v1.3.0...v2.0.0` compare link in `CHANGELOG.md`,
the `ChrisMavrommatis.*` NuGet package names listed there, the 2024 records under `results/lib/benchmarks/`,
and the links to workflow runs that happened under the old owner. They are records of what was true then.
GitHub redirects them forever, and rewriting them makes them false.

**The swagger json under each version folder is generated output.** Regenerate it, never hand-edit it — the
rule and the generator are in `$sites/docs`.

### D3 — the signing identity moved with the repository, and there are three bands

cosign keyless writes the repository's full path into the certificate, so the published verify command names
the owner. **GitHub redirects web links; it does not redirect a signing identity.** A stale one fails the
check rather than warning, and a `cosign verify` failure reads as tampering rather than as a moved repo.

| Band | Images | Verifies with |
|---|---|---|
| unsigned | up to `3.0.0-beta.1`, and `2.1.1` and earlier | nothing — `no signatures found` |
| old identity | `3.0.0-beta.2` alone | `ChrisMavrommatis` in the identity regexp |
| new identity | `3.0.0-beta.3` onward | `binacle-labs`, the string every surface now carries |

Signing, the SBOM and the GHCR staging copy all start at beta 2. Beta 3 is the first tag pushed after the
move, and `3.0.0-beta.4` followed on 2026-08-19 into the same band.

**`3.0.0-beta.3` is the only image a verify run has passed against under the current identity**, so it is the
tag to name wherever a doc needs a real one and the tag to re-run any verification against. Beta 2 is signed
but the published command rejects it. Beta 4 is in the band and untried.

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

### D5 — the reference layer is checked by a dated query, and the query is the fragile part

Every file under `.agents/docs/` and `.agents/design/` carries `verified:` (when someone last confirmed it
against the code) and `paths:` (the code it describes). A file whose `paths:` have been committed to since its
`verified:` date has not been proven wrong — it has been proven *unconfirmed*, which is the only signal there
is, because **a stale doc reads exactly like a current one**.

This is what derives that list. It runs from the repo root and prints every file it could not check, which is
the load-bearing half:

```bash
for f in $(find .agents/docs .agents/design -name "*.md" ! -name "_index.md" | sort); do
  fm=$(awk '/^---[ \t]*$/{n++; next} n==1' "$f")
  v=$(printf '%s\n' "$fm" | grep -m1 "^verified:" | sed 's/verified:[ \t]*//;s/"//g')
  pl=$(printf '%s\n' "$fm" | sed -n '/^paths:/,$p' \
        | grep -oE '^[ \t]*-[ \t]*"[^"]+"' | grep -oE '"[^"]+"' | tr -d '"')
  if [ -z "$v" ]; then echo "NO-VERIFIED  $f"; continue; fi
  if [ -z "$pl" ]; then echo "NO-PATHS     $f  (verified $v)"; continue; fi
  last=$(git log -1 --format=%ad --date=short -- $pl 2>/dev/null)
  if [ -z "$last" ]; then echo "NO-COMMITS   $f  paths=[$(echo $pl | tr '\n' ' ')]"; continue; fi
  [[ "$last" > "$v" ]] && printf "BEHIND       %-42s verified %s  code moved %s\n" "$f" "$v" "$last"
done
```

A clean run prints exactly two lines, both deliberate: `design/README.md` (navigation, claims nothing about
code) and `design/vipaq/history.md` (frozen at the date it was measured, and path-less on purpose so a live
session is never handed superseded numbers). **Anything else in the skip list is a hole, not a result.**

**This decision is not watched by its own query.** Its subject is the reference layer, and a `paths:` broad
enough to cover that would fire on every edit to it. Some claims are not expressible as a pathspec; saying so
beats a glob that matches everything.

#### What the 2026-08-19 sweep established

Thirty-two files across eight slices were re-verified in one pass. Five things generalise, and they are the
reason this is a decision rather than a closed task:

1. **Widen the `check:` while verifying.** Every slice's real errors sat just *outside* what its check asked
   for. A check naming three types is satisfied by three types while the fourth paragraph rots. The check is
   part of the deliverable, not a label on it.
2. **Count anything the doc counts.** Wrong counts turned up in five separate files — frozen packs (716 vs
   721), tests (334 vs 380), solution projects, `.dcproj` files, `just` modules. A number is the cheapest claim
   to verify and the one nobody re-reads.
3. **Confirm a check is runnable, not just written.** `lib/findings` pointed its check at
   `BenchmarkDotNet.Artifacts/`, which `.gitignore` excludes — so the one instruction for confirming those
   numbers could never be followed by anyone but the machine that produced them. When a check names an
   artifact, confirm the artifact is committed.
4. **Distrust the tool before the data.** The query above was wrong twice, in three ways, and every failure
   presented as a clean result: a `sed` range that ran past the front matter and swept prose into the pathspec
   (hiding three files, overstating a fourth), and a `[[:space:]]` class that in this environment does not match
   `^[[:space:]]*-[[:space:]]*"` against `  - "api/**"` — so `pl` came back empty for every file and the query
   checked nothing while printing nothing.
5. **A checker must report what it skipped.** Two files — `docs/concepts.md` and `docs/README.md` — carried a
   `verified:` date and a `check:` naming code with **no `paths:` at all**, so nothing could ever have flagged
   them; `concepts.md` sat unwatched at 2026-07-15. They surfaced only once the skips were printed. "No output"
   and "nothing to report" are the same line on a terminal.

**Measured evidence a doc quotes is not renumbered when the world moves.** Both findings records quote dataset
sizes that were correct when measured and have since grown; they now say so and name the live count, rather
than restating splits that would need a re-run to be true. Re-dating a measurement is falsifying it.

## Open

### O1 — what happens to `3.0.0-beta.1` and `3.0.0-beta.2` on Docker Hub

Both are still pullable. Under D4 no public surface names either, and under D3 neither passes the published
command — beta 1 was never signed, and beta 2 needs a string no page carries any more. So anyone who pulls one
gets a failure with nothing anywhere to explain it.

Deleting both tags once v3.0.0 is out is the clean end of it, and **it has a deadline**: Docker Hub tag
immutability is off today, and an immutable tag cannot be deleted. Not decided.
