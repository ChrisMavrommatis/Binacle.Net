---
id: docs
description: Jekyll documentation site at docs/ — versioned API docs with Swagger UI embed.
verified: 2026-07-06
check: Collections, versions, and plugin list match docs/_config.yml and docs/_plugins/
---

# Docs Site

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

Versioned docs are served at `/version/:path/`. Currently two versions: `v1.3.0` and `latest`.

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
