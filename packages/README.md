# Packages

Shared JavaScript/TypeScript packages used across the repo. They are npm workspaces — install once
from the repo root with `npm install`.

## 📦 Packages

| Package | What it is |
|---|---|
| [`binacle-net-ui`](binacle-net-ui) | Alpine.js components and a Three.js visualizer for the interactive packing demo |
| `binacle-compact-notation` | Parses and formats the compact strings (`"60x40x30"`, `"108x76x30 [40]"`). TypeScript mirror of C# `Binacle.CompactNotation` |
| `cookies` | Small cookie read/write helpers |
| `theme-switcher` | Light/dark theme toggle |

`binacle-compact-notation` is the only one with tests - `just test shared-ts-unit`. It has to agree with
[`shared/src/Binacle.CompactNotation`](../shared/src/Binacle.CompactNotation) by hand; there is no codegen
between them.

## 🌐 Who imports them

The two sites pull them in by package name and webpack bundles them - `sites/web` uses `binacle-net-ui` for
the packing demo and the protocol decoder, and both sites use `theme-switcher`. Nothing is copied: the import
resolves through the workspace. (`just assets` is a different job - it copies the static files in
[`assets/`](../assets), not these.)

**The UI module does not import them.** `api/src/Binacle.Net.UIModule/wwwroot/js/` keeps its own hand-written
`cookies.js`, `themeswitcher.js` and `PackingVisualizer.js` doing the same three jobs. Nothing keeps the two
sides in step, so a fix here is not a fix there.
