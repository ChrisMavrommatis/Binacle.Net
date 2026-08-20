# Cloudflare

One wrangler config per site - `docs.wrangler.jsonc` and `web.wrangler.jsonc`. They are the whole deployment
configuration for [`sites/docs`](../../sites/docs) and [`sites/web`](../../sites/web).

**Nothing here is run by hand.** The `Deploy Docs Site` and `Deploy Web Site` workflows call
`wrangler deploy --config` against them; both are manual, and both tag the commit they published.

## ⚙️ What each one sets

| Key | Why |
|---|---|
| `assets.directory` | The built site to upload - `artifacts/docs` or `artifacts/web`. Relative to **this file**, and it has to match the `destination` in that site's `_config.yml` |
| `assets.not_found_handling` | `404-page`, which wants `404.html` in the site root. That is why each site's `pages/404.html` carries `permalink: /404.html` |
| `preview_urls` | Off. A deploy is meant to be the site, not a second copy of it on another URL |
| `observability` | On. Request metadata, 404s and exceptions - the only way to see a dead inbound link, because the link check runs offline against the built folder and cannot know what anyone typed |

## ⚠️ Change the path in two places

`assets.directory` here and `destination` in the site's `_config.yml` name the same folder. Change one alone
and the deploy uploads a folder the build never wrote - or worse, a stale one from an earlier build.
