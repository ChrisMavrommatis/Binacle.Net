---
name: v3-frozen
description: v3 API is frozen — never modify it; all new endpoints and contract work go in v4 only
type: convention
---

The v3 API is stable and frozen. Do not modify v3 code — no new endpoints, no behavioural changes,
no contract edits. All new API work goes in v4 only.

**Why:** v3 is a published, stable contract; changing it breaks existing clients.

**How to apply:** target v4 (`api/src/Binacle.Net/v4/`) for any new or changed endpoints and contracts.
Touch v3 only for a fix the maintainer explicitly asks for. See `.agents/docs/api/v3/README.md` (v3
reference, marked "do not modify") and `.agents/docs/api/v4/README.md`.
