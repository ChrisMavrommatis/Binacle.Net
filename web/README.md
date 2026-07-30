# Binacle.Net Website

The marketing and landing site for Binacle.Net, built with [Jekyll](https://jekyllrb.com/). It covers
the product home, the apps listing, and an interactive packing demo.

## Develop

```bash
npm run copy-assets-to-web    # from the repo root — copies shared assets in
cd web && bundle exec jekyll serve
```

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../ruby),
wired up through this site's `Gemfile`.
