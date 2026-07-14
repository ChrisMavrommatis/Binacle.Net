---
description: TypeScript packages under packages/ (npm workspaces) — UI components, cookie utilities, and theme switching.
verified: 2026-07-05
check: Package list and descriptions match packages/ directory and their package.json files
also_update:
  - packages/binacle-net-ui.md
  - web/README.md
  - api/modules/ui.md
---

# Packages

npm workspaces at the repo root. All packages are private (not published to npm).

| Package | Description |
|---|---|
| `binacle-net-ui` | Alpine.js + Three.js frontend for the packing demo and protocol decoder — see [binacle-net-ui.md](binacle-net-ui.md) |
| `cookies` | Cookie read/write utility (based on js-cookie v3.0.5, MIT) |
| `theme-switcher` | Custom web element for toggling light/dark themes |

The ViPaq TypeScript mirror lives at `vipaq/packages/binacle-vipaq/` — see [vipaq/README.md](../vipaq/README.md).

## binacle-net-ui

Alpine.js components + Three.js visualizer for the packing demo and protocol decoder. Full reference —
components, plugins, model layers, the `window.binacle` global, and how to add a component — is in
[binacle-net-ui.md](binacle-net-ui.md). Consumed as TS source by `web/`'s webpack; the UIModule has its own
legacy JS copy (see [api/modules/ui.md](../api/modules/ui.md)).

## cookies

Thin wrapper over js-cookie v3.0.5. Used by both `docs/` and `web/` sites for cookie read/write.
No dependencies.

## theme-switcher

Custom HTML element (`<theme-switcher>`) for switching light/dark themes on the Binacle.Net websites.
Used by both `docs/` and `web/`. Depends on the `cookies` workspace package; no external dependencies.

## Related Tests

| Project | What it covers |
|---|---|
| `vipaq/packages/binacle-vipaq` | Has its own test suite — run with `npm test` inside the package directory |

## Dependencies

Which package imports which — and two undeclared workspace deps that resolve only by hoisting — is in
[dependencies.md](dependencies.md).
