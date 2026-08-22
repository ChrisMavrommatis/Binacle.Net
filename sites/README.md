# Sites

Every site Binacle.Net publishes, one directory each. Both are [Jekyll](https://jekyllrb.com/) sites built
with webpack and TypeScript beside them.

## 📂 The sites

| Directory | What it is |
|---|---|
| [`docs/`](docs) | The documentation site — versioned API reference and guides |
| [`demo/`](demo) | The demo site — the packing demo, the ViPaq decoder, and the pages around them |

Each has its own `README.md`, `Gemfile` and `package.json`. Neither is a root npm workspace, so a fresh clone
installs into the site as well as the root — `just install` from the repo root does both.

## 🛠️ Building and serving

From the repo root, one pair of recipes per site:

```bash
just serve docs                  # jekyll serve + webpack watch, one Ctrl-C stops both
just build docs                  # the same site built once, into artifacts/docs
```

`serve demo` and `build demo` are the same for the other one. A build is three steps in a fixed order — copy
the shared assets, run webpack over `_js/`, then `jekyll build` — and **skipping any of them still produces a
site**, just one with no scripts and no logo. Use the recipes rather than calling `jekyll` yourself.

Output goes to `artifacts/<site>` at the repo root, which is what gets deployed.

## ☁️ Deploying

Both go to Cloudflare, each from its own workflow - `Deploy Docs Site` and `Deploy Demo Site`. Both are
**manual** (`workflow_dispatch`), both build the site fresh, check its links offline, upload
`artifacts/<site>`, and then tag the commit they published so a live site maps back to a commit.

The wrangler config for each lives in [`tooling/cloudflare/`](../tooling/cloudflare). The `directory` it
uploads has to match the `destination` in that site's `_config.yml`.

Shared static assets live in [`assets/`](../assets) and are copied in by gulp; the custom Liquid filters and
Google Tag Manager tags come from the local gems in [`ruby/`](../ruby).
