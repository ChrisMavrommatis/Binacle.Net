---
description: TypeScript packages under packages/ (npm workspaces) — UI components, cookie utilities, and theme switching.
verified: 2026-05-23
check: Package list and descriptions match packages/ directory and their package.json files
also_update:
  - web/README.md
  - api/modules/ui.md
---

# Packages

npm workspaces at the repo root. All packages are private (not published to npm).

| Package | Description |
|---|---|
| `binacle-net-ui` | TypeScript frontend for the packing demo and protocol decoder |
| `cookies` | Cookie read/write utility (based on js-cookie v3.0.5, MIT) |
| `theme-switcher` | Custom web element for toggling light/dark themes |

The ViPaq TypeScript mirror lives at `vipaq/binacle-vipaq/` — see [vipaq/README.md](../vipaq/README.md).

## binacle-net-ui

TypeScript port of the UIModule frontend. Depends on Alpine.js and Three.js.

Organised into:
- `src/apiModels/` — request/response shapes matching the C# API contracts
- `src/viewModels/` — UI-facing models (bin, box, item, packing result, error)
- `src/core/` — Alpine.js components: `packingDemo.ts`, `protocolDecoder.ts`, `packingVisualizer.ts`
- `src/utils/` — Three.js scene helpers (create bin/item meshes, camera positioning, scene redraw)
- `src/packingDemoPlugin.ts` — Alpine plugin that registers the PackingDemo component
- `src/protocolDecoderPlugin.ts` — Alpine plugin that registers the ProtocolDecoder component

Consumed by:
- `web/` — bundled into `web/js/binacle-net-ui.js` via webpack
- `api/src/Binacle.Net.UIModule` — the Blazor app uses its own JS copy (see [api/modules/ui.md](../api/modules/ui.md))

## cookies

Thin wrapper over js-cookie v3.0.5. Used by both `docs/` and `web/` sites for cookie read/write.
No dependencies.

## theme-switcher

Custom HTML element (`<theme-switcher>`) for switching light/dark themes on the Binacle.Net websites.
Used by both `docs/` and `web/`. No external dependencies.

## Related Tests

| Project | What it covers |
|---|---|
| `vipaq/binacle-vipaq` | Has its own test suite — run with `npm test` inside the package directory |
