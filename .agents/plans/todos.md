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

## binacle-net-ui

Both found 2026-08-22 while writing the first tests for the package. Both are pinned as they are, so a fix
changes a test that says why.

- `packages/binacle-net-ui/src/viewModels/errorCollection.ts`, `hasError`. Declared `boolean`, returns
  `undefined` for a field nothing ever pushed to. Every caller today negates it, so nothing is wrong on the
  page - but `=== false` anywhere would be silently wrong. Coerce it.

- `packages/binacle-net-ui/src/viewModels/decodedPackingResult.ts`,
  `packedBinVolumePercentage`. Divides by the bin volume unguarded, so a decoded result carrying a zero side
  renders `NaN%`. The input is a base64 token a visitor pastes in, so the zero is reachable.

## Webpack and tsconfig

Both found 2026-08-22, reviewing the cookies and theme-switcher port. Neither is that port's doing.

- ~~**Both host builds emit 32 ts-loader type errors.**~~ **Fixed 2026-08-22**, and measured on both hosts
  with the caches cleared: 32 to 0 in `api/src/Binacle.Net.UIModule` and in `sites/demo`. They were
  `Window.binacle`, `$logger` and `_x_fieldPrefix`, all declared in `packages/binacle-net-ui/src/types/`.
  Both files there are modules, and a module's `declare global` only applies when the file is in the program -
  nothing imported them, so no host added them. `index.ts` now carries a `/// <reference path>` to each.
  **Webpack emitted the bundle regardless**, which is why it survived this long.

- **`sites/demo` has no `tsconfig.json` while `sites/docs` does, and it does not matter.** Measured
  2026-08-22: ts-loader resolves the config by walking up from each `.ts` file it compiles, so
  `packages/binacle-net-ui/tsconfig.json` governs package code in **every** host. An es5 target in
  `sites/demo/tsconfig.json` built clean; an es5 target in the package's config broke the build immediately.
  **The es2016 target that keeps `class extends HTMLElement` working is the package's, and it is not luck.**
  A host tsconfig here would be inert and would read as protection, so there is nothing to add.

- **Both host webpack configs set `cache: {type: 'filesystem'}`, and it hides type errors.** A build can
  report success from cache while the same source fails a cold build - it cost most of a session on 2026-08-22
  and produced two confident wrong conclusions. **Measuring a type error means deleting
  `<host>/node_modules/.cache/webpack` first.** Worth a comment in both configs next to the cache line.

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
