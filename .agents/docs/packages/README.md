---
id: packages
description: TypeScript packages under packages/ (npm workspaces) — UI components, compact-notation mirror, cookie utilities, and theme switching.
verified: 2026-07-15
check: Package list and descriptions match packages/ directory and their package.json files
also_update:
  - packages/binacle-net-ui
  - web-site
  - api/modules/ui
---

# Packages

npm workspaces at the repo root. All packages are private (not published to npm).

| Package | Description |
|---|---|
| `binacle-net-ui` | Alpine.js + Three.js frontend for the packing demo and protocol decoder — see `$packages/binacle-net-ui` |
| `binacle-compact-notation` | Compact text notation for Binacle geometry — TS mirror of C# `Binacle.CompactNotation`; used by `binacle-vipaq` (tools/tests) |
| `cookies` | Cookie read/write utility (based on js-cookie v3.0.5, MIT) |
| `theme-switcher` | Custom web element for toggling light/dark themes |

The ViPaq TypeScript mirror lives at `vipaq/packages/binacle-vipaq/` — see `$vipaq`.

## binacle-net-ui

Alpine.js components + Three.js visualizer for the packing demo and protocol decoder. Full reference —
components, plugins, model layers, the `window.binacle` global, and how to add a component — is in
`$packages/binacle-net-ui`. Consumed as TS source by `web/`'s webpack; the UIModule has its own
legacy JS copy (see `$api/modules/ui`).

## binacle-compact-notation

TypeScript mirror of the C# `Binacle.CompactNotation` — the shared compact text notation for Binacle geometry
(`"10x10x10 (0,0,0)"` style). A leaf with no dependencies. Used by `binacle-vipaq` in its `tools/` and `tests/`
(not runtime `src/`) to parse geometry when generating interop artifacts and reading shared vectors.

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

Which package imports which — every workspace import declared in its `package.json` — is in
`$packages/dependencies`.
