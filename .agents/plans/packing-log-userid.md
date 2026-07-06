# Plan: per-user packing logs (a ServiceModule concern)

## Decision
Per-user attribution of packing logs belongs in the **ServiceModule**, not the always-on DiagnosticsModule.
"Which user made this request" is an auth/account concept, and auth lives entirely in the ServiceModule. The
DiagnosticsModule stays auth-agnostic.

Accordingly, the premature `UserId` hook was **removed** from the DiagnosticsModule
(`AlgorithmOperationLogChannelRequest` and `PackingLogEntry` no longer carry it). Diagnostics logs what it
computed — bins, items, parameters, results, timestamp — and nothing about identity.

## When per-user logging is actually needed
Build it in the ServiceModule. Do NOT reintroduce a `UserId` field into the diagnostics log just so the
ServiceModule can fill it — that couples the always-on module to an optional one. Prefer one of:

- **ServiceModule owns its own per-user log / audit.** It has `HttpContext.User` and the account/subscription
  context; it can record "user X ran a pack" independently of the diagnostics packing log. This keeps the two
  concerns cleanly separated.
- **Enrich at the ServiceModule boundary.** If we genuinely want the user id *inside* the packing log line, the
  ServiceModule (which owns auth) is the place to add it — e.g. its own enrichment/log-processor variant, or a
  ServiceModule-registered accessor that a ServiceModule-owned log type reads. The diagnostics types stay clean.

## Key constraint (still true whenever this is built)
The user id must be captured on the **request thread** — the background `LogsProcessor` has no `HttpContext`.
Reading one claim off `HttpContext.User` is cheap (a field read, not morphing), so it doesn't violate the
"keep the request thread light" rule.

## Open questions (for when it's built)
- Which claim is the user id (NameIdentifier / `sub` / account id)? Confirm against ServiceModule's JWT setup.
- Anonymous / ServiceModule-off requests → no user (fine).
- Do we want more than an id (account, subscription)? Model it then, not now (YAGNI).
- Is a separate ServiceModule audit log a better fit than enriching the diagnostics packing log? Likely yes.

## Non-goals
- No `UserId` on the DiagnosticsModule log types.
- Not AOT-related.
