# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## Docs site

- Confirm **CodeQL alert #7** (`js/xss-through-dom`, `docs/_js/main.js`) closes on the next scan after the fix
  is committed. Fixed 2026-08-06 with a `URL`-based same-origin check; the plan file was deleted with it. The
  alert closing is automatic, so this is only worth a glance - if it is still open after the next scan, the fix
  did not reach the scanned branch.

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
