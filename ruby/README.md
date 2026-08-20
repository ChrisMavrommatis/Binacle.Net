# Ruby Gems

Custom Jekyll plugins used by the `sites/docs/` and `sites/web/` sites.

Both gems are loaded as local path dependencies via each site's `Gemfile`.

## Gems

### `jekyll-filters`

Custom Liquid filters:
- `capitalize_all` — capitalizes every word in a string
- `clean_content` — strips HTML tags, collapses whitespace, truncates to a given length (default 160 chars)

### `jekyll-gtm`

Liquid tags for Google Tag Manager:
- `{% gtm_head <id> %}` — outputs the GTM `<script>` block; goes in `<head>`
- `{% gtm_body <id> %}` — outputs the GTM `<noscript>` block; goes at the start of `<body>`

Both tags return an empty string if the ID is blank, so GTM can be disabled by setting an empty `gtm` value in `_config.yml`.