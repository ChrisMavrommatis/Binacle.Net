# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.

## Repo hygiene

- **Stale workspace entry in `package-lock.json`.** Line 7164 still holds a `vipaq/binacle-vipaq` entry, the
  path from before the package moved to `vipaq/packages/binacle-vipaq`. It is inert today - the live link entry
  resolves correctly - but a stale lockfile in this exact spot is what broke `npm run watch` for a whole
  session, and it survives because nobody hand-edits a lockfile. Clear it deliberately: regenerate and accept
  the diff, or remove the one entry.
