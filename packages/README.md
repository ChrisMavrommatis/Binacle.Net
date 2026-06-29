# Packages

Shared JavaScript/TypeScript packages used across the repo. They are npm workspaces — install once
from the repo root with `npm install`.

## Packages

| Package | What it is |
|---|---|
| `binacle-net-ui` | Alpine.js components and a Three.js visualizer for the interactive packing demo |
| `cookies` | Small cookie read/write helpers |
| `theme-switcher` | Light/dark theme toggle |

The sites in [`docs/`](../docs) and [`web/`](../web) consume the built assets, copied in by the gulp
tasks (`npm run copy-assets-to-docs`, `npm run copy-assets-to-web`).
