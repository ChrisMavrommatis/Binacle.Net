---
description: The Docker Hub repository page
paths:
  - ".github/workflows/**"
---

# The Docker Hub repository page

**Status:** the page is written; the wiring is not. `.github/dockerhub-overview.md` exists in the working
tree and section 5 now points at it rather than carrying a second copy. What is left is the credential test,
the workflow step, the short description, and the two web-form settings.

**The page is published by the release workflow**, in the same run that posts the release notes - so the file
can land on `main` at any time and nothing goes live until a release writes the tags it describes. Section 4
says why, and records that an earlier draft of this plan got the trigger wrong.

The page at `https://hub.docker.com/r/binacle/binacle-net` is typed into a web form and has been since v2.1.1.
Four things are wrong with it at once:

- the description advertises `2.1.1` as latest and hand-maintains a list of every tag ever published - fifteen
  entries, none of them 3.x
- it names neither Scalar nor the signing
- the repository has no logo, though the org is entitled to one
- its only category is "Integration & delivery", and three are allowed

For a lot of people this page is the first thing they read about the project.

Three pieces of work: put the description under version control and push it from CI, rewrite the text, and set
the two things that are pure web form.

---

## 1. What Docker-Sponsored Open Source already changes

**The `binacle` org carries the sponsored badge.** Confirmed 2026-08-13 - `https://hub.docker.com/v2/orgs/binacle/`
returns `"badge":"open_source"`. Two facts from that response drive decisions below: the namespace is an
**Organization**, not a user account, and `chrismavrommatis` is the member acting on it.

| Perk | What it means here |
|---|---|
| Unlimited pulls and egress on the namespace | **Nobody pulling this image is rate limited** - not the 100-per-6h anonymous cap, not the 200 authenticated |
| The sponsored badge | Renders on the page by itself. Nothing to do |
| A repository logo | Section 6. A real slot on the page, currently empty |
| Insights and analytics | Pulls by tag, digest and geography. The only way to answer "is anyone actually on the betas" |
| Docker Scout, up to 100 repos | Out of scope here, but it is paid for and unused |

**One decision this closes: do not name GHCR on this page.** The reason to would have been handing rate-limited
users an escape hatch, and there are no rate-limited users. GHCR is staging - the pipeline builds there, smokes
there, and copies the smoked digest to Docker Hub - and a staging registry named on a landing page becomes a
support surface nobody meant to own.

## 2. The credential - check this first, it may block the whole thing

**The endpoint matters more than the credential, and the earlier draft of this plan tested the wrong one.**

`peter-evans/dockerhub-description@v5` authenticates against **`POST https://hub.docker.com/v2/auth/token`** with
a body of `{"identifier": ..., "secret": ...}`, then sends `Authorization: Bearer <access_token>` on the PATCH.
Read off `src/dockerhub-helper.ts` at v5.0.0 (`1b9a80c0`) on 2026-08-13.

The long history of `403 access to the resource is forbidden with personal access token` reports is about
**`POST /v2/users/login/`**, whose JWT is used as `Authorization: JWT ...`. That is the legacy scheme and it is
where "a PAT cannot do this, only a password works" comes from. `/v2/auth/token` is the current one, and its
field names exist because it is built for tokens rather than passwords. **Test what the action actually calls** -
a 403 from the legacy endpoint would condemn this plan for no reason.

**Scope: the widest one.** Both tools surveyed in section 4 say the same thing in different vocabularies -
peter-evans documents "Personal Access Token with read/write/delete scope", `docker-pushrm` documents "sufficient
privileges (`admin` scope)". Docker Hub's UI offers Read / Read & Write / Read, Write & Delete; the API calls the
widest `repo:admin`. Same token.

**Second gate, easy to miss because the repo is under an org.** peter-evans documents that for an organization
repository the *account* must also hold **Admin** on that repository - token scope alone is not enough. Two
independent conditions, and a 403 does not say which one failed.

The test, in order. **Back the page up first** - this writes to a live public repo:

```bash
curl -s https://hub.docker.com/v2/repositories/binacle/binacle-net/ \
  | jq -r .full_description > overview-backup.md

ACCESS=$(curl -s -X POST https://hub.docker.com/v2/auth/token \
  -H 'Content-Type: application/json' \
  -d '{"identifier":"<user>","secret":"<the PAT>"}' | jq -r .access_token)

# Read first. Proves the token authenticates at all and changes nothing.
curl -s -o /dev/null -w '%{http_code}\n' \
  -H "Authorization: Bearer $ACCESS" \
  https://hub.docker.com/v2/repositories/binacle/binacle-net/

# Then the write, sending the CURRENT text straight back.
curl -i -X PATCH https://hub.docker.com/v2/repositories/binacle/binacle-net/ \
  -H "Authorization: Bearer $ACCESS" -H 'Content-Type: application/json' \
  --data "$(jq -Rs '{full_description: .}' < overview-backup.md)"
```

**Send the current text back, never a placeholder.** A PATCH of `{"full_description":"test"}` is a green result
and a defaced public page in the same second.

Try the existing `DOCKERHUB_TOKEN` first - it is a registry push credential and only needs `repo:write`, so it
may well 403. If it does, mint a **second** token at the widest scope and try again. **Do not widen the push
token:** the two jobs want different powers, and a description push has no business holding delete over the
registry.

A 200 means a second secret (`DOCKERHUB_DESC_TOKEN`) carries this. A 403 at the widest scope, with Admin on the
repo confirmed, means the only fallback is an account password - which cannot be scoped, cannot be rotated
without changing the login, and does not work with 2FA on. **If it comes to that, do not add it.** Leave the page
manual and record the result here; a repo that signs keylessly to avoid storing a key should not store an account
password to update a paragraph.

Whatever it returns, write the answer and the date in this file. Five minutes decides the whole item.

## 3. Where the file lives

A new file, **`.github/dockerhub-overview.md`**, next to the workflow that publishes it.

Not repo-root `README.md`. That file is written for someone standing in the repo - it has the directory tree, the
build-from-source section, the license breakdown - and its relative links (`DEVELOPMENT.md`, `LICENSE.GPL-3.0`)
do not resolve on Docker Hub. The action can rewrite relative links to absolute (`enable-url-completion`), but
that solves the smaller half of the problem; the content is still wrong for the audience.

Every link in the new file is absolute.

## 4. How it gets pushed

**`peter-evans/dockerhub-description@v5`**, pinned to `1b9a80c056b620d92cedb9d9b5a223409c68ddfa` with `# v5.0.0`
in a trailing comment, like every other action in this repo.

**The field was surveyed on 2026-08-13 and re-checked the same day. It is a one-horse race.** There is no
Docker-native feature for this - Docker's own "Update Docker Hub description with GitHub Actions" page links out
to the same third-party action. The alternatives are dormant:

| Tool | Stars | Last push | Last release | |
|---|---|---|---|---|
| `peter-evans/dockerhub-description` | 377 | 2026-08-08 | v5.0.0, Oct 2025 | Maintained. What Docker's docs point at |
| `christian-korneck/docker-pushrm` | 152 | 2024-06-10 | v1.9.0, Jul 2022 | CLI plugin. Also does Quay and Harbor |
| `christian-korneck/update-container-description-action` | 30 | 2022-11 | v1, Jul 2020 | Abandoned. Wraps pushrm |

`docker-pushrm` is worth remembering for exactly one reason: it is the only one that also writes to **Quay and
Harbor**. If Binacle.Net ever publishes to a second registry, revisit it. For Docker Hub alone, a tool with no
release since 2022 is the worse choice.

Two behaviours of the action to know before writing the workflow:

- It PATCHes **`full_description` always** and `description` (the short one) **only when that input is non-empty**.
  Leaving `short-description` unset does not clear the existing short description.
- It **silently truncates the short description to 100 bytes** with a warning in the log, rather than failing.

### It runs on a release, not on a push - decided 2026-08-14

**The page is updated by the release workflow, in the same run that publishes the release notes.** Not by a
push-triggered workflow of its own.

**An earlier draft of this plan said the opposite, and it was wrong.** It proposed `push` to `main` filtered to
the page's own path. That design makes **landing the file the same act as publishing it**, so the file could
never sit on `main` waiting for a tag - it had to be held out of the branch, or landed and immediately live with
text describing tags that did not exist yet. The whole "ordering gate" below existed to manage a hazard the
trigger created.

**Publishing on release fixes it at the root.** The page describes the tags a release writes, so the moment
those tags exist is the moment the page becomes true. One event, one source of truth.

Three things the old design was protecting, and how each is kept:

- **Prose should not need a tag.** **Keep `workflow_dispatch`.** A typo is fixed by a manual run, not by cutting
  a release.
- **Prereleases must not touch the page.** **Gate the job on a non-prerelease**, the same rule the moving tags
  already follow - a beta gets its immutable tag and nothing else, and it must not rewrite a page describing the
  stable line. The pipeline already makes this distinction inside metadata-action; this makes it a job condition.
- **A cosmetic failure must not redden a good release.** **Put it last**, after the Docker Hub copy and the
  signature have succeeded, with nothing depending on it. A page that failed to update is worth seeing in the
  run; it is not worth failing a release that shipped a correct image.

### Substitute the version at publish time

**This is what stops the page rotting, and it removes the ordering problem permanently rather than for one
release.** Keep version numbers out of the committed file - use placeholders and let the release job fill them
in from the version it just published.

Without it the file names `3.0` forever, and it is wrong the day 3.1.0 ships. With it the file is written once
and every release republishes it correctly, which is the same reason the tag list is being deleted from the page
in the first place.

Substitute at minimum the exact version and the minor tag. **Everything else on the page should read the same in
every release**, and anything that cannot is a sign it belongs in the Tags tab rather than in prose.

## 5. What the page says

**The page is written: `.github/dockerhub-overview.md`.** An earlier version of this section carried a full
draft. It does not any more - the file is the copy, and two copies of one page drift within a release.

Nine sections: the opener, a quick start with a real `curl` and its response, the interface flags, which tag to
use, configuration, verifying what you pulled, deploying, an ask to the people running it, and a quick
reference block.

### Two placeholders, substituted at publish time

`{{VERSION}}` becomes the exact version and appears twice. `{{MINOR}}` becomes the minor tag and appears eight
times. **Never commit a concrete version into that file** - without the substitution the page names one minor
forever and is wrong the day the next one ships.

### The short description is a separate field

100 bytes, and the action **truncates it silently** rather than failing. It is not part of the overview file;
it is an input to the action, so it lands independently of everything else here.

```
Will it fit in the box? Real-time 3D bin packing API - free, open source, self-hosted.
```

86 bytes. The action PATCHes `full_description` always, and `description` only when that input is non-empty -
so leaving the input unset does not clear what is already there.

### What the page leaves out on purpose

Each of these was considered and dropped. **Do not add them back without a reason that is new.**

- **The hand-maintained tag list.** Fifteen entries duplicating a Tags tab that is always right, and it is
  what rotted the page the first time. The three-row policy table replaces it and answers the one thing
  neither tab explains: which tag belongs in a compose file.
- **The service module.** Not advertised on this page, at the maintainer's call. The configuration table
  carries three variables, not four.
- **ViPaq.** It belongs on the docs site. On the page where someone is deciding whether to run one command, a
  second format name is a reason to hesitate.
- **GHCR.** It is staging, and a staging registry named on a landing page becomes a support surface nobody
  meant to own.
- **The health endpoint.** Off by default and its path is configurable, so a line about it is wrong for most
  readers.

### Two facts that bind the file

**The `cosign` block is copied from `SECURITY.md` verbatim, only the tag differs.** That file is the source.
There are now two copies of one command and they must match - a published verify command that fails reads as a
bug in the project rather than as drift.

**Every example tag must name a tag that passes today.** The org move re-keyed the certificate identity, so
anything signed under the old owner fails the published command. The same rule covers the `curl` example: it
was last run against a tag that has since been deleted from Docker Hub, so **re-run it against a tag that
exists before the page publishes.** A broken first command here is the whole first impression.

## 6. The logo and the categories

Both are web-form settings on the repository, both are one sitting, and neither needs the credential in
section 2. Do them whether or not the automation lands.

**Logo.** A sponsored-org perk, and the slot is empty today. `assets/media/logo/binacle-logo-512x512.png` is the
right source - Docker Hub wants a square image, and 512x512 clears its minimum with room. Check whether the
transparent variant (`-512x512a.png`) reads better against the page's background before picking; the page is
white in light mode and dark in dark mode, so a logo that assumes one of them will look broken in the other.

**Categories.** The repo has one, "Integration & delivery", and up to three are allowed. That category is about
CI/CD tooling, which this is not - it is the leftover of a single hurried pick. **"Developer Tools" is the closer
fit and should lead**, with "Integration & delivery" kept only if a third genuinely better one cannot be found in
the list Docker offers. Categories feed Hub search, so this is the only item on the page that changes who arrives
at it.

## 7. Order of work

1. Set the logo, the categories and the short description. None of them needs the credential or the workflow,
   and all three improve the page today.
2. Run the credential test in section 2, backing the page up first. Record the result and the date here. If it
   403s at the widest scope with Admin confirmed, and the only fallback is a password, stop - the rest is not
   worth it.
3. ~~Write the page.~~ Done - `.github/dockerhub-overview.md`. **It can land on `main` whenever**; nothing
   publishes it.
4. Re-run the `curl` example against a tag that exists and paste the real response back into the file.
5. Add the page step to the release workflow: last, gated on a non-prerelease, substituting the version, with
   nothing depending on it. Keep `workflow_dispatch` for prose fixes.
6. Run it once with `workflow_dispatch` and read the rendered page.
7. Add the step to the ci-cd docs table and its secret to the secrets table. Delete this file.

## 8. Do not

- PATCH a placeholder string into `full_description` to test write access. The page is live and public.
- Widen `DOCKERHUB_TOKEN` to the delete scope. Mint a second, narrower-purposed secret.
- Add a `DOCKERHUB_PASSWORD` secret if the PAT route fails. Leave the page manual instead.
- Name GHCR on the page. It is staging, and the rate-limit reason to mention it no longer exists.
- Publish a page whose quick start or verify command names a tag that is absent or unsigned.
- Put the tag list back. It is what rotted the page the first time.
- **Trigger the page update on a push to its own path.** That makes landing the file the same act as publishing
  it, which is the trap this plan was rewritten to remove.
- **Let a failed page update fail the release.** It runs last, after everything irreversible has already
  succeeded, and nothing depends on it.
- **Commit a concrete version into the page.** Use a placeholder and substitute it at publish, or the file is
  wrong the day the next minor ships.
