# Binacle.Net Docs

The documentation site, built with [Jekyll](https://jekyllrb.com/). It hosts the versioned API reference
(with an embedded Swagger UI) and the guides. Published to <https://docs.binacle.net>.

## 📂 What is in it

| Path | What it is |
|---|---|
| `collections/_versions/` | The versioned documentation, one folder per minor line - `v1.3.x` … `v3.0.x`. A released version's folder is frozen |
| `collections/_common_pages/` | Pages shared by every version - quick start, core concepts, the configuration basics |
| `collections/_sitemaps/` | The sitemap sources, one per collection |
| `pages/` | The unversioned pages - the landing page, `404.html`, `robots.txt` |
| `_data/` | Site data. `versions.yml` is the version list and says which one is current |
| `_layouts/`, `_includes/`, `_sass/`, `css/` | Templates, partials and styles |
| `_plugins/VLink.rb` | The `{% vlink %}` tag - resolves a path inside *the page's own version* |
| `_js/` | TypeScript and JavaScript sources. Webpack bundles them into `js/` |

`js/`, `lib/`, `media/` and the favicons at the root are **generated or copied in** and gitignored - webpack
writes the first, `just assets` writes the rest. Nothing there is edited by hand.

## 🚀 Develop

From the repo root:

```bash
just serve docs
```

That runs `jekyll serve` and the webpack watch together in one terminal, and one Ctrl-C stops both. **Use it
rather than `jekyll serve` on its own** - Jekyll alone does not rebuild the TypeScript or SCSS under `_js/`, so
a script or style change appears to do nothing.

Run `just install` once on a fresh clone, and `just assets` after changing anything under the repo-root
[`assets/`](../../assets) folder - the site serves its own copy, so a new logo does not show up until that runs.

## 🔢 Adding a version

`_data/versions.yml` is the single source of truth for the list, newest first - Jekyll's own ordering sorts by
path and would put `v3.10.x` before `v3.2.x`. Opening a new minor line takes three steps, and the file says so
at the top: add it to `list`, point `current` at it, and add its `defaults` scope block in `_config.yml`.

**A version with no scope block is invisible** - it builds and renders nothing.

## ⚙️ Two things that bite

- **`{% vlink %}`, not `{% link %}`**, for anything inside a version. It joins the path onto the page's own
  version, so the same guide can link its neighbour without naming a version number.
- **`pages/404.html` carries `permalink: /404.html`.** Cloudflare is configured with `not_found_handling:
  "404-page"`, which looks for that exact name in the site root.

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../../ruby),
wired up through this site's `Gemfile`.
