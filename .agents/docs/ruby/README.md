---
id: ruby
description: Ruby gems under ruby/ — Jekyll plugins used by docs/ and web/ sites.
verified: 2026-07-15
check: Gem list, filter names, and tag names match ruby/ source files
---

# Gems

Jekyll plugins used by both the `docs/` and `web/` sites. Both sites load them via `plugins:` in `_config.yml`.

| Gem | What it adds |
|---|---|
| `jekyll-filters` | Two Liquid filters: `clean_content`, `capitalize_all` |
| `jekyll-gtm` | Two Liquid tags: `{% gtm_head %}`, `{% gtm_body %}` |

## jekyll-filters

**`clean_content(input, length = 160)`** — strips HTML tags, collapses whitespace, truncates to `length`.
Used to generate clean meta description strings from page content.

**`capitalize_all(input)`** — title-cases every word in a string.

Source: `ruby/jekyll-filters/lib/`

## jekyll-gtm

**`{% gtm_head %}`** — injects the Google Tag Manager `<script>` snippet in `<head>`.

**`{% gtm_body %}`** — injects the GTM `<noscript>` fallback at the top of `<body>`.

Both tags use the `gtm:` value from `_config.yml`. If `gtm` is empty, nothing is injected.

Source: `ruby/jekyll-gtm/lib/`

## Running Tests

Each gem has its own RSpec suite under `spec/`:

```bash
cd ruby/jekyll-filters && bundle exec rspec
cd ruby/jekyll-gtm    && bundle exec rspec
```
