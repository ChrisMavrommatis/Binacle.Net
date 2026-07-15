# Idea: per-user packing logs

**Status:** Unvetted idea. The *decision* about where this belongs is already settled and shipped —
only the build is an idea.

## Settled already (do not re-litigate)

Per-user attribution belongs in the **ServiceModule**, not the always-on DiagnosticsModule. "Which user made
this request" is an auth concept, and auth lives entirely in the ServiceModule. The premature `UserId` hook was
removed from `AlgorithmOperationLogChannelRequest` and `PackingLogEntry`; diagnostics logs what it computed and
nothing about identity. Recorded in `$api/modules/diagnostics`.

**Never reintroduce a `UserId` field into the diagnostics log** just so the ServiceModule can fill it — that
couples the always-on module to an optional one.

## What

Two shapes, if we ever want per-user attribution:

- **ServiceModule owns its own per-user log / audit.** It has `HttpContext.User` and the account/subscription
  context, so it can record "user X ran a pack" independently of the diagnostics packing log. Cleanest split.
- **Enrich at the ServiceModule boundary.** If we genuinely want the user id *inside* the packing log line, the
  ServiceModule adds it — its own enrichment/log-processor variant, or a ServiceModule-registered accessor that a
  ServiceModule-owned log type reads. The diagnostics types stay clean.

## Constraint that survives either shape

The user id must be captured on the **request thread** — the background `LogsProcessor` has no `HttpContext`.
Reading one claim off `HttpContext.User` is cheap (a field read, not morphing), so it doesn't break the
"keep the request thread light" rule.

## Open questions

- Which claim is the user id (NameIdentifier / `sub` / account id)? Confirm against ServiceModule's JWT setup.
- Anonymous / ServiceModule-off requests → no user (fine).
- Do we want more than an id (account, subscription)? Model it then, not now.
- Is a separate ServiceModule audit log a better fit than enriching the diagnostics packing log? Likely yes.

## Related

- `$api/modules/diagnostics` (the settled rule), `$api/modules/service`
