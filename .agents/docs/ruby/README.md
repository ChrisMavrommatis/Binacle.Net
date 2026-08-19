---
id: ruby
description: Ruby gems under ruby/ — Jekyll plugins used by docs/ and web/ sites.
verified: 2026-08-19
check: Gem list, filter names and tag names match ruby/ source; the gtm tags still take the id as an argument; both sites still load the gems through their Gemfile :jekyll_plugins group and list them under plugins: in _config.yml
paths:
  - "ruby/**"
---

# Gems

Jekyll plugins used by both the `docs/` and `web/` sites.

| Gem | What it adds |
|---|---|
| `jekyll-filters` | Two Liquid filters: `clean_content`, `capitalize_all` |
| `jekyll-gtm` | Two Liquid tags: `{% gtm_head %}`, `{% gtm_body %}` |

**Both sites load them twice over, and both halves are needed.** Each site's `Gemfile` names the gem inside its
`group :jekyll_plugins` block with `path: "../ruby/<gem>"` — that is what resolves the local directory, since
neither gem is published. Each site's `_config.yml` then lists the gem under `plugins:`. Dropping either half
stops the plugin loading.

## jekyll-filters

**`clean_content(input, length = 160)`** — strips HTML tags, collapses newlines and runs of spaces, trims, then
truncates to `length`. Used to generate meta description strings from page content.

**`capitalize_all(input)`** — capitalises every space-separated word.

Source: `ruby/jekyll-filters/lib/` — `sanitization_filters.rb` and `capitalization_filters.rb`.

## jekyll-gtm

**Both tags take the Google Tag Manager id as an argument.** They are not configuration readers: the tag resolves
whatever it is handed as a Liquid variable and falls back to treating it as a literal id. The sites call them as
`{% gtm_head site.gtm %}` and `{% gtm_body site.gtm %}`, so the `gtm:` key in `_config.yml` is the *caller's*
convention, not something the gem knows about. A bare `{% gtm_head %}` resolves to an empty id and injects
nothing.

- **`{% gtm_head <id> %}`** — the GTM `<script>` snippet, for `<head>`.
- **`{% gtm_body <id> %}`** — the `<noscript>` fallback, for the top of `<body>`.

**An empty or missing id renders an empty string** rather than a broken snippet. Both sites currently set
`gtm: ''`, so GTM is off on both.

Source: `ruby/jekyll-gtm/lib/` — `gtm_head_tag.rb`, `gtm_body_tag.rb`, registered in `jekyll-gtm.rb`.

## The specs do not run

Each gem has an RSpec suite under `spec/` and declares `rspec` as a development dependency in its gemspec.
**Neither gem directory has a `Gemfile`**, so `bundle exec rspec` there fails with "Could not locate Gemfile or
.bundle/ directory", and nothing in CI or in the `just` recipes runs them either — the only workflow that reads
`ruby/` is CodeQL, which analyses rather than tests. Treat these suites as unrun until that is fixed.
