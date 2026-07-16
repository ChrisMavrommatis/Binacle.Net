# Docs Site — go version-only, drop `latest`

**Status (2026-07-16): the restructure is DONE.** Folders are `v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x`;
`latest` is gone as a folder and survives only as a redirect. The site builds clean. What remains is
**writing the v3.0.x docs** — see "What is left".
**Created:** 2026-07-16

## What is left

`v3.0.x` is a **stub**: `index.md` only, carrying the intro prose and a notice pointing at `v2.1.x`. Every
other page must be written for the release. Nothing was carried over from `v2.1.x` by choice — the v3.0 docs
are authored fresh, not edited down from a copy.

- [ ] Write the `v3.0.x` pages: `api/` (v3 + v4), `swagger/`, `configuration/`, `samples/`, `quick-start.md`,
      `release-notes.md`. **API v2 must not reappear** — it is removed in this version and lives on in
      `v2.1.x` / `v2.0.x`.
- [ ] Remove the notice block at the bottom of `v3.0.x/index.md` once the pages exist, and restore its section
      links (copy the shape from `v2.1.x/index.md`, minus V2).
- [ ] Generate `swagger/v4.json` — run the API and fetch `/openapi/v4.json` with `SWAGGER_UI` or `SCALAR_UI`
      on, on the **`Normal` profile (ServiceModule OFF)** so the spec matches the committed convention (the
      committed `v3.json` has no `/api/auth/token` path; a ServiceModule-on run adds it).
- [ ] Mark API v4 **experimental** — reuse the banner v3 carried in `v1.3.x/api/v3.md`.

**`vlink` raises and fails the build on a missing target.** That is why the stub has no section links: they
pointed at pages that do not exist yet. Add a link only when its target lands.

## What the site is for

The docs site is **informational for anyone pulling a previous docker image**. A reader on image `v2.1.1`
opens the `v2.1.x` folder and finds the docs for that image — config, samples, and the API versions it serves.
The folder is self-contained on purpose: it answers "what does my image do" without cross-referencing anything.

That purpose settles two things that look like problems but are not:

- **Copying the API pages into a version folder is correct**, not waste. The folder has to stand alone.
- **Content being dropped from a newer folder is correct.** ServiceModule config pages and `api/v1.md` were
  removed on purpose — ServiceModule is for internal consumption, and API v1 is gone. A folder describes its
  own image, so it sheds what that image no longer has. Older folders keep the older truth.

Only `collections/_versions/*` is per-version — roughly 300K of content. The site chrome (`_layouts`, `_sass`,
`_js`, `webpack.config.js`) sits outside it and is shared by every folder, so it is never copied.

## The decision: version-only; `latest` survives as a redirect only

**Every folder is a version. There is no moving folder.** The current line is edited in place in its own
folder; when a new line opens, that folder is copied to the new name and the old one is never touched again.

**`/version/latest/` stays as a redirect, holding no content**, pointing at whatever `current_version` names.
It is very likely bookmarked and indexed, so the URL keeps working — but it is a pointer, never a place docs
live. That is the distinction that matters: the old `latest` was a *folder* people edited, and its content had
to be rescued before each release. A redirect has nothing to rescue.

This is the whole point. Under the old model, `latest` was a moving pointer that had to be snapshotted
*before* anyone edited it — a discipline step. Discipline is exactly what failed: **four releases (v2.0.0,
v2.0.1, v2.1.0, v2.1.1) shipped with no snapshot at all**, and the only folder that exists was authored
retroactively. Version-only makes the freeze **structural** instead of procedural: an old folder is frozen
because nothing edits it, not because someone remembered to copy it in time.

### Folders are `vMAJOR.MINOR.x` — one per minor line

Folders track the **minor line**, so they map onto the tags people actually pull: `v1.3.x`, `v2.0.x`, `v2.1.x`,
`v3.0.x`. A reader on image `v2.0.1` looks for something named after their version and finds it.

Cut a new folder whenever a new minor line opens. At minimum this must happen when the **API version set
changes** — that is the question the folder exists to answer — but per-minor is the simpler rule and it
subsumes that case.

Measured history — the API surface, per tag:

| Tag | Date | API surface | Docs content |
|---|---|---|---|
| v1.0.0 – v1.1.4 | 2024-10 → 2025-01 | v1 + v2 | no site yet |
| **v1.2.0** | 2025-02-12 | **v3 added** (1 endpoint, experimental) | no site yet |
| v1.2.2, v1.3.0 | 2025-02 → 03 | unchanged | no site yet |
| **v2.0.0** | 2025-09-29 | **v1 removed** | `latest` appears |
| v2.0.1, v2.1.0 | 2025-10 → 12 | unchanged | **no change** |
| v2.1.1 | 2026-01-13 | unchanged | +6 files (swagger UI feature, additive) |
| HEAD | 2026-07 | unchanged | **untouched since v2.1.1** |
| **v3.0.0** (pending) | | **v2 removed, v4 added** | |

**API versions are added at minors and removed at majors** — both correct semver. Adding `/api/v3` alongside
v1 and v2 broke nothing, so v1.2.0 was right to be a minor; removing v1 broke callers, so v2.0.0 had to be
major. **This is why per-major would be wrong**: v1.2.0 added API v3 at a *minor*, so a single `v1.x.x` folder
would have shown a v3 to someone on v1.1.4 whose image had none. Per-minor cannot make that mistake.

**Per-minor also earned its keep immediately.** `v2.0.x` and `v2.1.x` are not duplicates: the swagger UI
landed at v2.1.1, so `v2.0.x` correctly has **no** `swagger/` at all while `v2.1.x` has four files. A single
`v2.x.x` folder would have shown a swagger UI to 2.0.x users who never had one.

**Patches, by contrast, never move the docs.** Every patch pair in the repo is byte-identical across `docs/`
(`v2.0.0→v2.0.1`, `v1.1.0→v1.1.1`, `v1.1.3→v1.1.4`, `v1.2.0→v1.2.1`). That is why the folder is `vMAJOR.MINOR.x`
and not one per release: the `.x` genuinely covers its patches.

**Cost, stated plainly:** a reader on `v2.1.0` gets `v2.1.x`, the end of that line — which includes the swagger
UI that landed in v2.1.1. The prose is identical, and the spec matches their image, since the API set was
constant across 2.1.

### Why an API tag can never drive this

Two independent tag streams and three deploy workflows:

| Tags | Releases | Workflow |
|---|---|---|
| `vMAJOR.MINOR.PATCH` (`v2.1.1`) | the API / docker image | `release-docker-image.yml` |
| `docs-release-N` (`docs-release-4`) | the docs site | `deploy-binacle-net-docs.yml` |
| `web-release-N` | the marketing site | `deploy-binacle-net-web.yml` |

The docs tree at an API tag is just whatever happened to be in the repo that day — possibly mid-edit, possibly
already carrying work for a future version. **Never derive a folder from an API tag, and never backfill from
one.** Copy the current folder at the moment a new line opens; that is the only sound method.

## The one knob

With no `latest`, one thing must say which version is current. Put it in `docs/_data/sidebar.yml` and drive
everything from it — **one edit per new line**:

```yaml
current_version: "v3.0.x"
latest_version_link_text: "Latest Version Docs "
```

## What was done (2026-07-16)

### The folders

| Folder | Source | API pages it serves |
|---|---|---|
| `v1.3.x` | renamed from `v1.3.0` — content untouched | v1, v2, v3 (experimental), users |
| `v2.0.x` | backfilled from the **v2.0.1 tag** | v2, v3 — **no swagger** (that feature landed at v2.1.1) |
| `v2.1.x` | the old `latest`, untouched since v2.1.1 | v2, v3 + swagger |
| `v3.0.x` | **stub — `index.md` only** | none yet |

The `v2.0.x` backfill is sound *because it was verified*, not assumed: `latest` was byte-identical across
v2.0.0 → v2.0.1 → v2.1.0, so the tree at the v2.0.1 tag provably is the 2.0 line's docs. **This is the only
case where a tag was a safe source.** Do not generalise it — see "Why an API tag can never drive this".

### The rewiring

- `docs/_config.yml` — one `defaults` scope block per folder (`v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x`). A
  folder without its block is **invisible** in the version selector, which is built from
  `site.versions | map: 'version' | uniq`.
- `docs/_data/sidebar.yml` — added `current_version: "v3.0.x"`. **This is the one knob.**
- `docs/_includes/sidebar.html` — the button used `{% link _versions/latest/index.md %}`, which **raises and
  fails the build** once `latest` is gone. Now built from `current_version`.
- `docs/_layouts/redirect.html` — hardcoded `/version/latest/` (it would have redirected to itself). Now
  driven by `current_version`. The layout was previously unused.
- `docs/collections/_common_pages/version-latest.html` — **new.** The `/version/latest/` redirect. It lives in
  `common_pages`, *not* `versions`, so `site.versions` never sees it and no phantom "latest" appears in the
  selector. Verified absent from the built output.
- `docs/_includes/versions/breadcrumbs.html` — dropped the dead `contains 'latest'` branch. Every folder is
  now `vX.Y.x`, which the existing `contains '.'` check already skips; `latest` needed its own branch only
  because it had no dot.

**Untouched, because they were already dynamic:** `_common_pages/version.html` and
`_includes/versions/sidebar.html` build their lists from `site.versions` and never assumed `latest`.

### Verified

`cd docs && bundle exec jekyll build` passes. Selector lists all four folders; `/version/latest/` redirects to
`/version/v3.0.x/`; the sidebar button points at `/version/v3.0.x/`; no phantom `latest` entry.

## Leave the `v1.3.x` folder's content alone

**Its content is accurate — do not "fix" it.** Verified: at v1.3.0 the v3 API had exactly one endpoint file
(`v3/Endpoints/Pack/ByCustom.cs`), matching the single path in its `swagger/v3.json`, and its "experimental,
can change at any time" banner was true. API v3 was introduced at **v1.2.0**, so v1.0.0–v1.1.4 images had no
v3 at all. It is the only record of the 1.x era and it was written carefully.

## When a new line opens (the standing rule)

A line opens on **every new minor** — `v3.0.x` → `v3.1.x`, or `v3.1.x` → `v4.0.x`. Patches stay in the folder
they were born in; they have never moved the docs.

1. `cp -r _versions/v3.0.x _versions/v3.1.x` (or `v4.0.x`) — copy the folder the new line grows out of.
2. Rewrite `menu_title` and every `permalink` in the new folder. They are all `/version/<version>/...`, so:
   `grep -rl "/version/v3\.0\.x/" v3.1.x/ | xargs sed -i 's|/version/v3\.0\.x/|/version/v3.1.x/|g'`
3. Add the new folder's `_config.yml` `defaults` block, or it will not appear in the selector.
4. Point `current_version` in `_data/sidebar.yml` at it. That also moves the `/version/latest/` redirect.
5. `bundle exec jekyll build` to confirm.
6. Edit only the new folder. **Never touch an old one** — that is what keeps it true.

## Watch out

- Version order in the selector follows Jekyll's collection order (alphabetical by path). Fine through v9;
  `v10.0.x` would sort before `v2.1.x`. Not a problem yet — worth knowing.
- The `vlink` tag (`docs/_plugins/VLink.rb`) resolves against the page's `version` front matter and **raises**
  if the target is missing. Removing a page without removing its `vlink` references breaks the build. Grep for
  the page name before deleting.
- **Never commit** — leave changes in the working tree.

Trim each item as it lands; delete this file when nothing pending remains.
