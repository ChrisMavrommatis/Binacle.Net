# Binacle.Net Website

The marketing and landing site for Binacle.Net, built with [Jekyll](https://jekyllrb.com/). It covers
the product home, the apps listing, and an interactive packing demo.

## Develop

From the repo root:

```bash
just serve web
```

That runs `jekyll serve` and the webpack watch together in one terminal, and one Ctrl-C stops both. **Use it
rather than `jekyll serve` on its own** — Jekyll alone does not rebuild the TypeScript or SCSS under `_js/`, so
a script or style change appears to do nothing.

Run `just install` once on a fresh clone, and `just assets` after changing anything under the repo-root
`assets/` folder — the site serves its own copy, so a new logo does not show up until that runs.

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../ruby),
wired up through this site's `Gemfile`.
