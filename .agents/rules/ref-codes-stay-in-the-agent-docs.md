---
description: A label like D16 is an agent cross-reference. Never put a bare code in anything a human reads.
load: on-trigger
when: writing anything a human reads outside .agents/ - a comment, release notes, PR text, a chat reply
---

# Ref codes stay inside the agent docs

Decision and finding headings carry short labels - `D1`, `O1`, `F1`. They are anchors for a `$` reference.

A human reading "D16" has no idea what it means. Never put a bare code in anything human-facing: a code
comment, release notes, PR text, or a message to the maintainer. Spell the thing out instead.
