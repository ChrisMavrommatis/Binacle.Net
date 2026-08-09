---
name: v3-frozen
description: v3 API is frozen — never modify it; all new endpoints and contract work go in v4 only
type: convention
---

The v3 API is stable and frozen. Do not modify v3 code — no new endpoints, no behavioural changes,
no contract edits. All new API work goes in v4 only.

**What "frozen" covers.** Clarified by the maintainer on 2026-08-09, when a Sonar sweep wanted to add
`static` to the four v3 endpoint handlers: *"as long as it works it's fine, it doesn't change the contract
for an outsider."* The freeze protects what a **client can observe** — routes, request and response shapes,
status codes, behaviour. It is not a ban on touching the files. An internal change no caller can see (a
method modifier, a dead local removed, a rename of something private) is allowed, and the sweep landed.

The test is the outsider, not the file path: if a client could tell, it is frozen; if only the compiler can,
it is not. When in doubt, ask — the ruling above is the only one on record.

**Why:** v3 is a published, stable contract; changing it breaks existing clients.

**How to apply:** target v4 (`api/src/Binacle.Net/v4/`) for any new or changed endpoints and contracts.
Touch v3 only for a fix the maintainer explicitly asks for. See `.agents/docs/api/v3/README.md` (v3
reference, marked "do not modify") and `.agents/docs/api/v4/README.md`.
