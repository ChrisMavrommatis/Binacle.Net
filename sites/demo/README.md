# Binacle.Net Demo Site

The demo site, built with [Jekyll](https://jekyllrb.com/). It covers the product home, the apps listing, and
the two interactive apps. Its host is <https://demo.binacle.net>.

## 📂 What is in it

| Path | What it is |
|---|---|
| `pages/` | The site itself - `index.html`, the apps listing, `404.html`, `robots.txt` |
| `collections/_apps/` | The two interactive apps - the packing demo and the ViPaq protocol decoder |
| `collections/_sitemaps/` | The sitemap sources, one per collection |
| `_data/` | Header, footer and includes configuration |
| `_layouts/`, `_includes/`, `_sass/`, `css/` | Templates, partials and styles |
| `_js/` | The three webpack entry points - `main.js`, `packing_demo.js`, `protocol_decoder.js` |

`js/`, `lib/`, `media/` and the favicons at the root are **generated or copied in** and gitignored - webpack
writes the first, `just assets` writes the rest. Nothing there is edited by hand.

## 🚀 Develop

From the repo root:

```bash
just serve demo
```

That runs `jekyll serve` and the webpack watch together in one terminal, and one Ctrl-C stops both. **Use it
rather than `jekyll serve` on its own** - Jekyll alone does not rebuild the TypeScript or SCSS under `_js/`, so
a script or style change appears to do nothing.

Run `just install` once on a fresh clone, and `just assets` after changing anything under the repo-root
[`assets/`](../../assets) folder - the site serves its own copy, so a new logo does not show up until that runs.

## 🧩 The two apps

Both live in `collections/_apps/` as a page, with their behaviour in a `_js/` entry point that imports the
shared packages from [`packages/`](../../packages) - `binacle-net-ui` for the packing demo and the decoder,
`theme-switcher` for the site chrome. They are resolved through the npm workspace, not copied, so editing a
package changes the site on the next webpack pass.

The packing demo calls a running API. `api_url` in `_config.yml` points it at the local one; `_config.prod.yml`
overrides that for a deploy.

## ⚙️ One thing that bites

`pages/404.html` carries `permalink: /404.html`. Cloudflare is configured with `not_found_handling:
"404-page"`, which looks for that exact name in the site root.

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../../ruby),
wired up through this site's `Gemfile`.
