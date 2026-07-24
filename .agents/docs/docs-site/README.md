---
id: docs-site
description: The published Jekyll documentation site at repo-root docs/ — versioned API docs with Swagger UI embed. `$docs-site` always means repo-root docs/, never .agents/docs/.
verified: 2026-07-21
check: Collections, versions, plugin list, and version folders match docs/_config.yml and docs/collections/_versions/
---

# Docs Site

**`$docs-site` is the repo-root `docs/` folder** — the published site, not `.agents/docs/` (the agent docs you
are reading). It is off limits from a coding session; see `.agents/README.md`.

Jekyll site at `docs/`. The public API documentation for Binacle.Net users.
Built with Jekyll + webpack + TypeScript. Output goes to `../build/docs`.

Run locally:

```bash
cd docs && bundle exec jekyll serve   # port 7195
```

## Content Structure

| Path | What it is |
|---|---|
| `collections/_versions/` | Versioned docs — each subfolder is a version |
| `collections/_common_pages/` | Pages shared across all versions |
| `collections/_sitemaps/` | Sitemap XML files |
| `pages/` | Top-level pages (index, 404, robots.txt) |

Versioned docs are served at `/version/:path/`. See "Versioning model" below.

## Versioning model

**Every folder is a version; there is no moving folder.** Folders are `vMAJOR.MINOR.x` — one per minor line
(`v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x`). The current line is edited in place; when a new line opens, its folder
is copied and the old one is never touched again. `/version/latest/` survives only as a **redirect** to
`current_version`, holding no content.

**The one knob:** `current_version` in `docs/_data/sidebar.yml` says which folder is current and where the
`latest` redirect points. One edit per new line.

**Why per-minor, not per-major.** A folder answers "what does my image do", and the API set is what changes:
versions are **added at minors** (v1.2.0 added API v3) and **removed at majors** (v2.0.0 removed v1, v3.0.0
removes v2). Per-major would show a v3 to a v1.1.4 image that never had it. Per-minor also caught the swagger UI,
which landed at v2.1.1 — so `v2.0.x` has no `swagger/` and `v2.1.x` does. Patches never move the docs (every
patch pair in history is byte-identical across `docs/`). This makes the freeze **structural** — an old folder is
frozen because nothing edits it, not because someone remembered to snapshot it. That discipline is exactly what
failed before: four releases (v2.0.0 → v2.1.1) shipped with no snapshot, and only one folder was ever authored.

**Never derive a folder from an API tag.** The tree at a tag is whatever was in the repo that day — maybe
mid-edit. Copy the current folder the moment a new line opens; that is the only sound source.

### When a new line opens (standing rule)

A line opens on every new **minor** (`v3.0.x` → `v3.1.x`, or `v3.1.x` → `v4.0.x`):

1. `cp -r _versions/v3.0.x _versions/v3.1.x` — copy the folder the new line grows out of.
2. Rewrite every `permalink`/`menu_title`:
   `grep -rl "/version/v3\.0\.x/" v3.1.x/ | xargs sed -i 's|/version/v3\.0\.x/|/version/v3.1.x/|g'`
3. Add the folder's `defaults` block in `docs/_config.yml`, or it is invisible in the selector.
4. Point `current_version` in `_data/sidebar.yml` at it (also moves the `latest` redirect).
5. `bundle exec jekyll build` to confirm.
6. Edit only the new folder. **Never touch an old one** — that is what keeps it true.

**Watch out:**
- `vlink` (`docs/_plugins/VLink.rb`) **raises and fails the build** on a missing target. Removing a page without
  removing its `vlink` references breaks the build — grep the page name before deleting.
- Selector order is alphabetical by path. Fine through v9; `v10.0.x` would sort before `v2.1.x`.

## Plugins

| Plugin | Source |
|---|---|
| `jekyll-gtm` | `ruby/jekyll-gtm` |
| `jekyll-filters` | `ruby/jekyll-filters` |
| `jekyll-tidy` | gem |
| `VLink` | `docs/_plugins/VLink.rb` (local) |

**VLink** (`{% vlink path %}`) — resolves a relative path to the correct versioned URL based on the
current page's `version` front matter. Use it instead of plain links inside `_versions/` pages
so links stay correct across versions.

## JS and Vendor Libs

Webpack bundles `docs/_js/main.js` → `docs/js/main.js`.

Vendor libs the docs site loads:
- BeerCSS — theming (`/lib/beercss/`, via `docs/_data/includes.yml`)
- Swagger UI — embedded OpenAPI explorer, loaded in the `versions/swagger.html` layout

Note: docs does **not** use Alpine.js or material-dynamic-colors (neither is referenced anywhere under
`docs/`). Don't assume they're available here.
