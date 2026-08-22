---
id: sites/demo
description: The published Jekyll demo site at sites/demo/ — product home, apps listing, and interactive packing demo. `$sites/demo` always means sites/demo/.
verified: 2026-08-22
check: Collections, JS bundles and plugin list match sites/demo/_config.yml and sites/demo/js/; the demo/prefetch script split still matches sites/demo/_data/includes.yml; artifacts/demo/lib/ after `just build demo` holds exactly the vendor folders listed, and gulpfile.js's IGNORE map still explains what is missing
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

`sites/demo/lib/` holds two vendor folders — `beercss` and `material-dynamic-colors` — and **only BeerCSS is
loaded**, as a stylesheet and a module in `sites/demo/_data/includes.yml`.

- **material-dynamic-colors** is present and commented out there; uncomment it only when the site needs runtime
  theme switching.
- **`swagger-ui` is in `assets/` but never copied here.** `gulpfile.js` carries a per-target `IGNORE` map: the
  docs site's swagger layout is the only thing that loads it, so it reaches `sites/docs/` alone. That map is
  also what keeps it out of the image.

Alpine.js arrives as an npm dependency bundled by webpack, which is the copy the demo code uses — there is no
vendored copy any more.

**Three.js is not in `vendors.js`** — it is its own bundle, for the reason in the table above.

Built size is **3.0 MB**, of which 1.14 MB is three Material Symbols `woff2` files. `beer.min.css` declares
four `@font-face` families and a browser downloads only the one a page uses, so that weight is in the deploy
and not in the page load.
