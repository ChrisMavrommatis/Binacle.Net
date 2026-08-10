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

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
