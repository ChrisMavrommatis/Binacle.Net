---
description: Jekyll marketing/landing site at web/ — product home, apps listing, and interactive packing demo.
verified: 2026-07-06
check: Collections, JS bundles, and plugin list match web/_config.yml and web/js/
also_update:
  - packages/README.md
---

# Web Site

Jekyll site at `web/`. The public marketing and landing site for Binacle.Net.
Built with Jekyll + webpack + TypeScript. Output goes to `../build/web`.

Run locally:

```bash
cd web && bundle exec jekyll serve   # port 7196
```

## Pages

| Page | Route | What it is |
|---|---|---|
| `pages/index.html` | `/` | Landing page |
| `pages/apps.html` | `/apps/` | List of apps using Binacle.Net |
| `collections/_apps/` | `/apps/:name/` | Individual app pages |
| `pages/404.html` | `/404` | Error page |

## JS Bundles

Webpack bundles from `web/_js/` and npm packages into `web/js/`:

| Bundle | What it is |
|---|---|
| `main.js` | Site-wide JS (theme, navigation) |
| `packing_demo.js` | Interactive packing demo — calls the Binacle.Net API |
| `protocol_decoder.js` | ViPaq protocol decoder — decodes pack results without calling the API |
| `binacle-net-ui.js` | Built from `packages/binacle-net-ui` — UI components and 3D visualizer |
| `binacle-vipaq.js` | Built from `vipaq/packages/binacle-vipaq` — TypeScript ViPaq decoder |

## Plugins

Same as the docs site: `jekyll-gtm`, `jekyll-filters`, `jekyll-tidy` (no `VLink` — no versioned docs here).

## Vendor Libs

`web/lib/` ships only **BeerCSS** (`/lib/beercss/`). material-dynamic-colors is available but commented
out in `web/_data/includes.yml` (only load it when the app needs runtime theme switching).
The web site does **not** use Swagger UI.
Alpine.js and Three.js are npm dependencies bundled by webpack (into `vendors.js`), not `web/lib/` vendors.
