---
id: sites
description: Every published site lives under sites/, one directory each. What the two share, and what is per-site.
verified: 2026-08-21
check: The directory list matches sites/; both sites still build through `just build <site>` into artifacts/<site>; the shared list below still matches each site's Gemfile, package.json and webpack.config.js
paths:
  - "sites/**"
---

# Sites

Every site this repo publishes, one directory each.

| Directory | Site | Doc |
|---|---|---|
| `sites/docs/` | the documentation site | `$sites/docs` |
| `sites/web/` | the marketing site | `$sites/web` |

**All of it is off limits from a coding session.** Each site is written in its own session; see
`.agents/README.md` for the rule and its one carve-out.

## What every site here has in common

Read this once, then the per-site doc for what differs.

- **Jekyll + webpack + TypeScript.** Its own `Gemfile`, its own `package.json`, its own `webpack.config.js`.
  **Both are root npm workspace members**, so one `npm ci` at the root covers them and neither has a lock file
  of its own. Ruby is still per site: `bundle install` runs in each.
- **Two configs.** `_config.yml` holds everything; `_config.prod.yml` overrides the few values that differ off
  localhost. A build passes both, in that order.
- **Output goes to `artifacts/<site>`**, set as `destination` in `_config.yml` — two levels up from the site.
  `artifacts/` does not sit under `sites/` and does not move with it.
- **`just build <site>` and `just serve <site>`** are the pair: built once, or served with a webpack watch
  beside it. Three steps in a fixed order — asset copy, webpack, jekyll — and skipping one builds a site that
  looks finished. See `$commands`.
- **Assets and gems come from outside.** `assets/` is copied in by gulp; `ruby/jekyll-gtm` and
  `ruby/jekyll-filters` are loaded from each `Gemfile` by path. Both are relative paths that count levels, so
  they are what a move breaks first.
- **Deployed by hand, one workflow each.** `workflow_dispatch` only, and the workflow builds the site, link
  checks what it built, and hands that same directory to the host — see `$ci-cd`.
