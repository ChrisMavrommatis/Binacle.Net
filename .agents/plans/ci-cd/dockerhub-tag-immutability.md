---
description: Turn on Docker Hub tag immutability, for release tags only
paths:
  - ".github/workflows/**"
---

# Turn on Docker Hub tag immutability, for release tags only

**Status:** not started. Not a release gate. **Nothing here is urgent, and one option in it is actively unsafe
today** - see section 3.

**Terminology first, because this repo already uses the word.** Elsewhere "immutable tag" means the exact-version
tag (`3.0.0`, `3.0.0-beta.2`) that the pipeline never moves, as opposed to `3.0` and `latest`, which it does.
That is a convention. **This plan is about the Docker Hub repository setting of the same name**, which enforces
the convention at the registry by rejecting a re-push. The two are easy to confuse in a sentence, so this file
says "the setting" whenever it means the feature.

## 1. What is true today

Read from `https://hub.docker.com/v2/repositories/binacle/binacle-net/` on 2026-08-13:

```json
"immutable_tags_settings": { "enabled": false, "rules": [".*"] }
```

Off, with a rule left at the default. The setting appears in the repository's own API response, so plan
availability is probably not the blocker - **confirm it is offered in the repository's settings UI before doing
any of this**, since the sponsored org's entitlements are what decide it.

## 2. The trap in the rule

**A rule marks the tags it matches as immutable. It does not exempt them.** Docker's docs are explicit: matching
tags cannot be overwritten or deleted, and a new image needs a new tag name. The rules are Go `regexp` (RE2).

So the current `".*"` is the worst possible value to switch on. It would freeze **every** tag, `latest` and `3.0`
included - and those two are *designed* to move. The next real release would run `imagetools create` against a
frozen `latest`, and the publish job would fail after the image had already been built, smoked and copied under
its version tag. A red at the last step of an otherwise good release, with the moving tags half done.

**Change the rule before touching the switch.** Not after, and not in the same sitting without checking the value
took.

## 3. What to actually match - and why not full semver

The obvious rule is "every exact version", prereleases included:

```
^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?$
```

**Do not use that one yet.** `3.0.0-beta.2` was re-cut on 2026-08-13 - the tag was deleted and pushed again at a
new commit, after the prerelease-skips-publish rule was reversed. That is a normal thing to do to a beta, it will
happen again before 3.0.0 stabilises, and the setting would have blocked it with the release half shipped.

**Match released versions only:**

```
^\d+\.\d+\.\d+$
```

That gives the property worth having - a shipped release can never be quietly replaced - while leaving betas
re-cuttable and `3.0` and `latest` free to move. Revisit widening it to prereleases once the 3.x line is stable
and a re-cut is no longer routine.

## 4. Why bother at all

The release pipeline is built around one rule: never rebuild between `build` and `publish`, because the copy is
by digest and that is what makes the published image bit-for-bit the one the smoke job passed. Everything in the
workflow protects that property **inside a single run**. Nothing protects it afterwards.

The setting is the part that holds across runs. It makes "`3.0.0` is the artifact that was smoked on the day it
shipped" a fact the registry enforces, rather than a promise the page makes. Given the page is about to promise
exactly that - "`3.0.0`, moves: never" - the two should be true together.

**The cost, stated plainly:** an immutable tag cannot be deleted either. A release tag pushed by mistake is
permanent, and the only remedy is a new version. That is the trade, and it is the reason section 3 keeps
prereleases out of it.

## 5. How to check it without gambling the repo

There is no undo, so do not learn the behaviour on the live repository.

1. Create a scratch public repo under the org.
2. Set the setting there with the section 3 rule.
3. Push `9.9.9`, then push `9.9.9` again - it must be rejected. Push `latest` twice - it must succeed both times.
   Push `9.9.9-beta.1` twice - it must succeed, which is the half of the rule that is easy to get backwards.
4. Try deleting `9.9.9` and confirm what the error says, so the failure mode is known before it matters.
5. Only then set it on `binacle/binacle-net`, and read the value back from the API rather than trusting the form.
6. Delete the scratch repo.

## 6. Done when

- The setting is enabled on `binacle/binacle-net` with a rule that matches released versions only.
- The value has been read back from `https://hub.docker.com/v2/repositories/binacle/binacle-net/` and matches.
- A release has shipped since, moving `3.0` and `latest` without incident.
- The ci-cd docs say the setting is on and what the rule matches, because the next person to hit a rejected push
  needs to find that in one search.

## 7. Do not

- Enable it while the rule is `.*`. That breaks the next release's publish job.
- Include prereleases in the rule while 3.x is still stabilising.
- Test it on the live repository.
- Treat it as a substitute for the digest-preserving copy. It catches an overwrite after the fact; the pipeline's
  shape is what prevents one in the first place.
