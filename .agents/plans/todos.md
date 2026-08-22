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

## Kernel

- `api/src/Binacle.Net.Kernel/ReservedPathOptions.cs`, `AddPrefix`. A prefix without a leading slash is
  accepted and held, then throws `ArgumentException` on the first request, because `Covers` matches it as a
  `PathString`. **A module that declares `api` instead of `/api` therefore fails every request rather than the
  one it got wrong.** Reject it or add the slash, in the same guard that already drops null and whitespace.
  Found 2026-08-22 while writing the tests, which pin the current behaviour.

## Ruby gems

- **Neither gem under `ruby/` has a `Gemfile`**, so `bundle exec rspec` in `ruby/jekyll-filters` or
  `ruby/jekyll-gtm` fails with "Could not locate Gemfile or .bundle/ directory". Both declare `rspec` as a
  development dependency in their gemspec, and both have a full `spec/` suite that **nothing has ever run** -
  no workflow, no `just` recipe. Found 2026-08-19 while verifying the ruby doc.

  The mechanical half is a two-line `Gemfile` in each directory (`gemspec` plus the rspec group). **Whether
  the suites then go on the PR gate is a separate call** - if it grows past adding the file, it needs its own
  plan.

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
