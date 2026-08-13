---
description: Text that reaches a user stays plain ASCII - no em dashes, curly quotes, ellipsis characters or arrows.
load: on-trigger
when: writing text a user will see - validation and exception messages, log lines, OpenAPI descriptions, UI strings
paths:
  - "api/src/**"
  - "packages/**"
  - "**/Config_Files/**"
---

# User-facing text is plain ASCII

Validation and exception messages, log lines, OpenAPI descriptions, UI strings: no em or en dashes, no curly
quotes, no ellipsis character, no arrows or symbols. Write `-` and `...`, and say "0-100", never "0–100".

Prose under `.agents/` and code comments are read by us, so they are free.

**Why:** these land in consoles, log files, JSON and terminals where the encoding is not ours to control. A
mangled character in a startup error is one more thing to debug.
