# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## CI

- Lint the OpenAPI documents on every PR. One step: `just openapi lint`. It generates the documents itself and
  needs nothing brought up. Moved out of `ci-gates` on 2026-08-07 - it shares none of that plan's checkout,
  ordering or runtime constraints.

  **Unblocked 2026-08-10.** The prerequisite was the `servers` block, because the lint reported two
  `oas3-api-servers` warnings and turning the gate on first would have forced a choice between a gate that
  ignores warnings - which stops being read - and one that is red on arrival. The block landed, both documents
  carry a single relative `/`, and the lint is now clean: 0 errors, 0 warnings. **So set the gate to fail on
  warnings** - there is nothing left to argue about, and that is the whole reason the ordering mattered.

## Comments

Found in a 2026-08-12 sweep of every comment outside `.agents`. The layer is in good shape overall - the
`just` modules, the workflows and the sample compose files carry "why" at the point of use, which is where it
has to stay. These two are the exceptions.

- `Dockerfile`, the line above `COPY ["build/binacle-net", "."]`, reads "from the 'build' stage". **There is no
  build stage** - the publish happens outside the file, in `just build publish`. Say that instead, and that
  the path is hardcoded here and allowlisted in `.dockerignore`, so publishing elsewhere builds an empty image.

- `tooling/tmux.sh` carries ~40 comment lines that restate the line below them ("# Select the first pane" over
  `tmux select-pane`), and two banners are wrong: window 5's block closes with a `WINDOW 6` banner and window
  6's opens with `WINDOW 5`. Nothing here folds into `.agents` - it is deletable noise. **Do this with the
  keep-or-convert decision in the scripts-to-just-recipes plan**, not before: if the script moves into a
  shebang recipe body whole, the noise moves with it.

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
