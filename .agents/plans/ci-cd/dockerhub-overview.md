---
description: The Docker Hub repository page
paths:
  - ".github/workflows/**"
---

# The Docker Hub repository page

**Status:** not started. **The page is published by the release workflow**, in the same run that posts the
release notes - so the file can land on `main` at any time and nothing goes live until a release writes the tags
it describes. Section 4 says why, and records that an earlier draft of this plan got the trigger wrong.

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

## 5. What the page should say

The current page's biggest structural mistake is the hand-maintained version list. Docker Hub already has a Tags
tab that is always right, and GitHub already has a releases page. Fifteen links that must be edited on every
release, to duplicate two pages that maintain themselves, is the reason the page rots.

**Replace the list with the tagging policy.** That is what a reader actually needs and what neither tab explains:
which tags move, which never do, and which one to put in a compose file. It changes about once a minor - only the
pinning example carries a number - so the file stops needing an edit per release.

### The constraint the release trigger already satisfies

**Every tag this page names must exist and be signed at the moment the page goes live.** Today `latest` still
resolves to 2.1.1 and there is no `3.0` row at all; signing started at beta 2, so both moving tags currently
point at an unsigned digest. A reader running the page today would get `manifest unknown` on the quick start and
`no signatures found` on the verify - the second reads as our bug rather than as history.

**Publishing from the release job satisfies this by construction**, because the job runs after the copy that
writes those tags. There is no separate gate to remember and no version of the file that has to be held back.

The one case still worth checking by hand is the **first** run, where the page goes from describing 2.x to
describing 3.x.

### Who owns the "Verifying what you pulled" section

**The draft below carries that section, but this plan does not own its wording.** The same invocation is already
live in `CHANGELOG.md` and is headed for `SECURITY.md` and the docs site, and the separate image-verification
work owns what all of them say - what is signed, what a pass proves, and the sentence that a pass is not a
clean bill of health. **Take the wording from there rather than editing it here**, or there will be two versions
of one paragraph, drifting.

What this plan does own is that the section **exists on this page and carries the commands rather than a link**.
This is where the pull happens - a reader is here because they are about to run `docker run` - so it is the last
page that should send someone elsewhere to learn the image is signed.

**Do not swap the commands for `just image verify`.** That recipe is for someone with a clone; this reader has a
registry and a shell.

### The draft

It is public writing, so it is drafted here rather than committed as a page: someone should read it as writing
before it goes live, the same way the two Jekyll sites get their own session. What follows is content, not
layout - trim it freely.

---

```markdown
# Binacle.Net

3D bin packing over HTTP, in real time. Give it bins and items; it answers whether they fit, and how to
pack them.

Built for e-commerce checkout - parcel lockers, box selection, shipping quotes - where the answer has to come
back inside a page load.

## Quick start

    docker run -d --name binacle-net -p 8080:8080 \
      -e SWAGGER_UI=True -e SCALAR_UI=True -e UI_MODULE=True \
      binacle/binacle-net:3.0

Then ask it something:

    curl -X POST http://localhost:8080/api/v3/fit/by-custom \
      -H 'Content-Type: application/json' \
      -d '{
        "parameters": { "algorithm": "FFD" },
        "bins":  [ { "id": "locker-M", "length": 40, "width": 30, "height": 20 } ],
        "items": [ { "id": "box-a", "quantity": 2, "length": 10, "width": 10, "height": 10 } ]
      }'

    {"result":"Success","data":[{"result":"AllItemsFit","bin":{...},
     "fittedItems":[...],"unfittedItems":[],
     "fittedBinVolumePercentage":8.33,"fittedItemsVolumePercentage":100}]}

With the flags above you also get:

- <http://localhost:8080/> - interactive packing demo
- <http://localhost:8080/swagger/> - Swagger UI
- <http://localhost:8080/scalar/> - Scalar

The API itself is under `/api/v3` and `/api/v4`, and needs none of them.

## Which tag to use

| Tag | Moves | Use it for |
|---|---|---|
| `3.0.0` | never | pinning to an exact build |
| `3.0` | on each patch in the 3.0 line | production - fixes, no behaviour changes |
| `latest` | on every release, major ones included | trying Binacle.Net out |

`latest` will cross a major version and can break your integration. **Pin `3.0` for anything you keep.**

Prereleases publish their exact version only (`3.0.0-beta.2`) - they never move `3.0` or `latest`.

Every tag is on the Tags tab. What changed in each is in the
[changelog](https://github.com/ChrisMavrommatis/Binacle.Net/blob/main/CHANGELOG.md).

## Configuration

| Variable | Default | What it turns on |
|---|---|---|
| `SWAGGER_UI` | off | Swagger UI at `/swagger/` |
| `SCALAR_UI` | off | Scalar at `/scalar/` |
| `UI_MODULE` | off | The interactive packing demo at `/` |
| `SERVICE_MODULE` | off | JWT auth, rate limiting and accounts. Needs a database |

The API works with all four off, which is the right setup for a service nobody browses to.

Logs and the SQLite database are written to `/app/data` - mount a volume there if you enable the service
module. Full configuration, including the PostgreSQL and Azure Tables backends, is at
<https://docs.binacle.net>.

## Verifying what you pulled

Every published image is signed with cosign - keyless, against the digest, so one signature covers `3.0.0`,
`3.0` and `latest` alike - and carries an SPDX SBOM and SLSA provenance.

    cosign verify binacle/binacle-net:3.0 \
      --certificate-identity-regexp '^https://github\.com/ChrisMavrommatis/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
      --certificate-oidc-issuer https://token.actions.githubusercontent.com

    docker buildx imagetools inspect binacle/binacle-net:3.0

A pass proves the image came from this repository's release workflow. It does not claim the image is free of
vulnerabilities.

## Deploying

Docker Compose and Kubernetes samples, from a one-line quickstart to a production setup:
<https://github.com/ChrisMavrommatis/Binacle.Net/tree/main/samples>

## About the image

- Base: `mcr.microsoft.com/dotnet/aspnet:10.0`
- Runs as a non-root user, listens on 8080
- `linux/amd64`

## Links

- Website: <https://www.binacle.net>
- Documentation: <https://docs.binacle.net>
- Source: <https://github.com/ChrisMavrommatis/Binacle.Net>
- Releases: <https://github.com/ChrisMavrommatis/Binacle.Net/releases>

Dual-licensed GPL-3.0-only (code) and CC-BY-SA-4.0 (content).
```

---

### Notes on the draft

- **Short description** is a separate field, 100 bytes, also settable by the action. Use
  `Real-time 3D bin packing API. Fit and pack over HTTP, with a browser demo.` (74 bytes.) Note that the action
  truncates rather than fails, so a longer one degrades silently.
- **Full description** is capped at 25,000 bytes. The draft is nowhere near it.
- **Everything executable in the draft was run, on 2026-08-13, against the published
  `binacle/binacle-net:3.0.0-beta.2`.** The `fit/by-custom` body is accepted and the response shown is the real
  one, elided at the `{...}`; with the three flags set, `/` and `/scalar/` return 200 and `/swagger/` returns the
  301 that lands a browser on the UI. Nothing here is a plausible-looking guess, which matters because a broken
  first command on this page is the whole first impression. **They were run against the beta tag - see the
  ordering gate above for why the draft says `3.0` anyway.**
- **The verify commands are exact too.** Both were run against the same beta and pass verbatim against Docker
  Hub. Only the tag changes.
- **The "does not claim it is free of vulnerabilities" sentence stays.** Without it a signature turns into a
  marketing badge.
- **No health endpoint is mentioned on purpose.** It is off by default and its path is configurable, so a line
  about it would be wrong for most readers. That belongs on the docs site.
- **`linux/amd64` only** is stated because it is currently true. Multi-arch is a separate plan; when it lands
  this line changes with it.
- The emoji headings from the old page are dropped. Keep them if the house style wants them - the point of the
  rewrite is the content, not the decoration.

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

1. Set the logo and the categories (section 6). Independent of everything else, and they improve the page today.
2. Run the credential test in section 2, backing the page up first. Record the result and the date here. If it
   403s at the widest scope with Admin confirmed, and the only fallback is a password, stop - the rest is not
   worth it.
3. Get the draft in section 5 read as writing, and settle the short description.
4. Add `.github/dockerhub-overview.md`, with placeholders where a version appears. **It can land on `main`
   whenever** - nothing publishes it.
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
