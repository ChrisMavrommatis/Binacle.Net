# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## CI

- Lint the OpenAPI documents on every PR. One step: `just openapi lint`. It generates the documents itself and
  needs nothing brought up. Moved out of `ci-gates` on 2026-08-07 - it shares none of that plan's checkout,
  ordering or runtime constraints.

  **Add the `servers` block first.** The lint reports two `oas3-api-servers` warnings and no errors, and the
  only open question is whether the gate fails on warnings. A `servers` entry clears both, after which the gate
  can fail on warnings from day one with nothing to argue about. Turn the gate on first and you choose between
  a gate that ignores warnings - which stops being read - and one that is red on arrival.

  The shape was decided 2026-08-07: **a single relative `/`**, in both documents. Binacle is self-hosted, so
  the API is at the root of wherever the reader deployed it. Do not name `api.binacle.net` - it is real and it
  serves our own demo, and putting it in the spec makes it the baked-in default of every generated client.

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
