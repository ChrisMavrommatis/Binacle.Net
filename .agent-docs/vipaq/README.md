---
description: Binacle.ViPaq — compact binary format for encoding packing results
---

# ViPaq

`Binacle.ViPaq` is a compact binary format for encoding packing results (bin dimensions + item positions).
The TypeScript mirror lives at `packages/binacle-vipaq`.

Used in v4 API responses when `IncludeViPaqData: true` is set — returns a base64 payload for 3D visualization.

> **Gap** — expand with: wire format spec, encoding/decoding rules, versioning, C# and TypeScript API surface.
