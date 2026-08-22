---
id: sites/demo
description: The published Jekyll demo site at sites/demo/ — product home, apps listing, and interactive packing demo. `$sites/demo` always means sites/demo/.
verified: 2026-08-20
check: Collections, JS bundles and plugin list match sites/demo/_config.yml and sites/demo/js/; the demo/prefetch script split still matches sites/demo/_data/includes.yml; sites/demo/lib/ still holds exactly the vendor folders listed
also_update:
  - packages
paths:
  - "sites/demo/**"
---

# Demo Site

**`$sites/demo` is the `sites/demo/` folder** — the published demo site. It is off limits from a coding
session; see `.agents/README.md`.

Jekyll site at `sites/demo/`. The public demo site for Binacle.Net.
Built with Jekyll + webpack + TypeScript. Output goes to `../../artifacts/demo`.

Run locally, or build it once:

```bash
just serve demo   # jekyll serve (port 7196) + webpack watch, one Ctrl-C stops both
just build demo   # the same site built once, into artifacts/demo
```

## Pages

| Page | Route | What it is |
|---|---|---|
| `pages/index.html` | `/` | Landing page |
| `pages/apps.html` | `/apps/` | List of apps using Binacle.Net |
| `collections/_apps/` | `/apps/:name/` | Individual app pages |
| `collections/_sitemaps/` | `/sitemap/:name:output_ext` | Generated sitemaps |
| `pages/404.html` | `/404` | Error page |

## JS Bundles

Webpack bundles from `sites/demo/_js/` and npm packages into `sites/demo/js/`:

| Bundle | What it is |
|---|---|
| `main.js` | Site-wide JS (theme, navigation) |
| `packing_demo.js` | Interactive packing demo — calls the Binacle.Net API |
| `protocol_decoder.js` | ViPaq protocol decoder — decodes pack results without calling the API |
| `binacle-net-ui.js` | Built from `packages/binacle-net-ui` — UI components and 3D visualizer |
| `binacle-vipaq.js` | Built from `vipaq/packages/binacle-vipaq` — TypeScript ViPaq decoder |
| `runtime.js` | Webpack runtime, loaded on every page |
| `vendors.js` | Shared npm dependencies, loaded on every page |
| `three.js` | Three.js on its own, **524 KiB** — demo pages only |

Only `main`, `packing_demo` and `protocol_decoder` are webpack entry points; the rest are split chunks or
package builds.

**The demo bundles are split out on purpose, and `sites/demo/_data/includes.yml` is the one list that decides
it.** `runtime.js`, `main.js` and `vendors.js` load on every page. `three.js`, `binacle-net-ui.js` and
`binacle-vipaq.js` load only where the front matter says `demo: true`; every other page **prefetches** that same
list, so arriving at a demo costs no download. Both halves read the one list, so they cannot drift apart.

## Plugins

Same as the docs site: `jekyll-gtm`, `jekyll-filters`, `jekyll-tidy` (no `VLink` — no versioned docs here).

## Vendor Libs

`sites/demo/lib/` holds four vendor folders — `alpine`, `beercss`, `material-dynamic-colors`, `swagger-ui` —
but **only BeerCSS is loaded**, as a stylesheet and a module in `sites/demo/_data/includes.yml`.

- **material-dynamic-colors** is present and commented out there; uncomment it only when the site needs runtime
  theme switching.
- **`alpine` and `swagger-ui` are referenced by nothing** under `sites/demo/`. They ship without being loaded.

Alpine.js also arrives as an npm dependency bundled by webpack, which is the copy the demo code actually uses.
**Three.js is not in `vendors.js`** — it is its own bundle, for the reason in the table above.
