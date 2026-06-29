# Binacle.Net Docs

The documentation site for Binacle.Net, built with [Jekyll](https://jekyllrb.com/). It hosts the
versioned API reference (with an embedded Swagger UI) and guides.

## Develop

```bash
npm run copy-assets-to-docs    # from the repo root — copies shared assets in
cd docs && bundle exec jekyll serve
```

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../ruby),
wired up through this site's `Gemfile`.
