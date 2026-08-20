---
title: API
permalink: /version/v3.0.x/api/
nav:
  order: 5
  icon: 📡
---

Two API versions are available in {{ page.version }}.

**API v2 was removed in v3.0.0.** If you still call it, see the
[v2.1.x documentation]({{ '/version/v2.1.x/' | relative_url }}) for the old contract and the
[Release Notes]({% vlink release-notes.md %}) for how to move off it.

## Prerequisites

- [Core Concepts]({% link _common_pages/core-concepts.md %}) - the algorithms and functions Binacle.Net uses.

---

## ✅ Version 3

Version 3 provides fitting and packing over a set of bins, using either a preset or custom bin dimensions.
Every endpoint takes an algorithm and returns one result per bin.

V3 is **stable and recommended**. It is unchanged in this release apart from the ViPaq payload, which uses the
new format.

➡️ Learn more about [Version 3]({% vlink /api/v3.md %})

---

## 🧪 Version 4

Version 4 covers everything V3 does across 16 endpoints, and splits a request by the answer you want: one bin,
the smallest bin that works, the bin the items fill most, or a result for every bin. It also adds the `Best`
algorithm and single-preset lookups.

V4 is **experimental and can change at any time**, for the whole 3.0.x line. Use V3 for anything you keep.

➡️ Learn more about [Version 4]({% vlink /api/v4.md %})

---
