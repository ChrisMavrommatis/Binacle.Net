---
description: TODOs
---

# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## CI

**Nothing here.** The OpenAPI lint moved out on 2026-08-17 and into the v3.0.0 release plan, which owns it
whole along with the `--fail-severity=warn` flag it needs. It had been written in both files and the two copies
had already started to differ.

## Comments

Found in a 2026-08-12 sweep of every comment outside `.agents`. The layer is in good shape overall - the
`just` modules, the workflows and the sample compose files carry "why" at the point of use, which is where it
has to stay. These two are the exceptions.

- `Dockerfile`, the line above `COPY ["artifacts/binacle-net", "."]`, reads "from the 'build' stage". **There is no
  build stage** - the publish happens outside the file, in `just build publish`. Say that instead, and that
  the path is hardcoded here and allowlisted in `.dockerignore`, so publishing elsewhere builds an empty image.

- `tooling/tmux.sh` carries ~40 comment lines that restate the line below them ("# Select the first pane" over
  `tmux select-pane`), and two banners are wrong: window 5's block closes with a `WINDOW 6` banner and window
  6's opens with `WINDOW 5`. Nothing here folds into `.agents` - it is deletable noise. **Do this with the
  keep-or-convert decision in the scripts-to-just-recipes plan**, not before: if the script moves into a
  shebang recipe body whole, the noise moves with it.

## Ruby gems

- **Neither gem under `ruby/` has a `Gemfile`**, so `bundle exec rspec` in `ruby/jekyll-filters` or
  `ruby/jekyll-gtm` fails with "Could not locate Gemfile or .bundle/ directory". Both declare `rspec` as a
  development dependency in their gemspec, and both have a full `spec/` suite that **nothing has ever run** -
  no workflow, no `just` recipe. Found 2026-08-19 while verifying the ruby doc.

  The mechanical half is a two-line `Gemfile` in each directory (`gemspec` plus the rspec group). **Whether
  the suites then go on the PR gate is a separate call** - if it grows past adding the file, it needs its own
  plan.

## Web site

- **`web/lib/alpine/` and `web/lib/swagger-ui/` are referenced by nothing** under `web/` — not by
  `_data/includes.yml`, not by any include, layout or page. They are published to the marketing site and never
  loaded. Found 2026-08-19 while verifying the web-site doc. Deleting them is a `web/` change, so it belongs to
  the site session, not a coding one; record it there rather than doing it here.

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
