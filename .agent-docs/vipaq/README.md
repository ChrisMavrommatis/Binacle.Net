---
description: Binacle.ViPaq — compact binary format for encoding packing results. High-level wire layout, encoding techniques, and C# / TypeScript API surface.
---

# ViPaq

> **Warning: ViPaq is experimental and may change.**

`Binacle.ViPaq` is a compact binary format for encoding packing results (bin dimensions + item positions).
The TypeScript mirror lives at `vipaq/binacle-vipaq`.

Used in v3 and v4 API responses when `includeViPaqData: true` is set — returns a base64 payload for 3D visualization.

## Wire Format (high level)

<!-- sourced from docs site; verify against current code if behaviour changes -->

```
[Header][NumberOfItems][Bin: L,W,H][Item1: L,W,H,X,Y,Z]...[ItemN: L,W,H,X,Y,Z]
```

- **Header** — decoding metadata
- **NumberOfItems** — count of encoded items
- **Bin** — Length, Width, Height
- **Items** — each item has its dimensions (L, W, H) and position coordinates (X, Y, Z)

Exact byte offsets and version negotiation are not yet documented — see "Gap" below.

## Encoding

<!-- sourced from docs site; verify against current code if behaviour changes -->

Three techniques are applied to the binary payload:

- **Base64** — converts binary data to a transfer-friendly string
- **Variable Length Encoding (VLE)** — reduces size by compressing redundant numeric values
- **Gzip** — applied for larger payloads to further reduce size

## Related Tests

| Project | Alias | What it covers |
|---|---|---|
| `vipaq/test/Binacle.ViPaq.UnitTests` | `vipaq` | Encoding, decoding, and roundtrip behaviour |
| `vipaq/binacle-vipaq` | — | TypeScript mirror — run with `npm test` in that directory |

## Gap

Still missing from these docs: exact byte offsets, version negotiation, and the full C# and TypeScript decoder API surface.
