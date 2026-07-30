---
id: packages/dependencies
description: TypeScript packages dependency tree — the npm workspaces and which package imports (and declares) which.
verified: 2026-07-14
check: package.json deps plus actual `from "<pkg>"` imports across packages/*/src and vipaq/packages/binacle-vipaq/src match the graph; every workspace import is declared
---

# Packages — TypeScript dependencies

The npm workspaces. Root `package.json` globs `packages/*` and `vipaq/packages/binacle-vipaq`, so every package
resolves the others from the workspace. Every workspace import is also declared in its package's `package.json`
(all as `"*"`), so the graph is honest on its own, not only by hoisting.

## The graph

Arrows point at what a package imports (each is declared in the importer's `package.json`).

```
binacle-compact-notation      leaf (no deps)
   ▲
binacle-vipaq  ───────────────┘  — imported only in tools/ + tests/, not runtime src
   ▲                              mirror of the C# ViPaq
   │
binacle-net-ui ───────────────┘  + external: alpinejs, three
   Alpine components + Three.js visualizer

cookies                       leaf (no deps)
   ▲
theme-switcher ───────────────┘
```

## Packages at a glance

| Package | Location | Imports | External deps |
|---|---|---|---|
| `binacle-compact-notation` | `packages/` | — | — |
| `cookies` | `packages/` | — | — |
| `theme-switcher` | `packages/` | `cookies` | — |
| `binacle-vipaq` | `vipaq/packages/` | `binacle-compact-notation` | — |
| `binacle-net-ui` | `packages/` | `binacle-vipaq` | `alpinejs`, `three` |

## Notes

1. **Every workspace import is declared** as `"*"` in the importer's `package.json` (`binacle-net-ui` →
   `binacle-vipaq`, `theme-switcher` → `cookies`, `binacle-vipaq` → `binacle-compact-notation`), so resolution
   never relies on workspace hoisting.

2. **`binacle-vipaq` touches `binacle-compact-notation` only in `tools/` and `tests/`**, not in runtime `src/`.
   The vipaq mirror parses the shared compact-geometry notation when generating interop artifacts and reading
   vectors; the format itself pulls in nothing. See `$vipaq/typescript`.

3. **`binacle-compact-notation` and `cookies` are leaves** — no workspace or runtime deps.
