---
id: packages
description: TypeScript packages under packages/ (npm workspaces) — UI components, compact-notation mirror, cookie utilities, and theme switching.
verified: 2026-08-22
check: The package list, their descriptions and the private flag match each packages/*/package.json; the Related Tests table names every package under packages/ that has a suite, with the alias tooling/tests.just gives it
also_update:
  - packages/binacle-net-ui
  - sites/demo
  - api/modules/ui
paths:
  - "packages/**"
---

# Packages

npm workspaces at the repo root. All four are `private: true` — none is published to npm, and all four are
TypeScript with no build step of their own: `main` points at a `.ts` entry and each host compiles the source
with its own webpack + ts-loader. `binacle-compact-notation` puts that entry at `src/index.ts`; the other
three keep an `index.ts` barrel at the package root.

| Package | Description |
|---|---|
| `binacle-net-ui` | Alpine.js + Three.js frontend for the packing demo and ViPaq decoder — see `$packages/binacle-net-ui` |
| `binacle-compact-notation` | Compact text notation for Binacle geometry — TS mirror of C# `Binacle.CompactNotation`; used by `binacle-vipaq` (tools/tests) |
| `cookies` | Cookie read/write utility (based on js-cookie v3.0.5, MIT) |
| `theme-switcher` | Custom web element for toggling light/dark themes |

The ViPaq TypeScript mirror lives at `vipaq/packages/binacle-vipaq/` — see `$vipaq`.

## binacle-net-ui

Alpine.js components + Three.js visualizer for the packing demo and ViPaq decoder. Full reference —
components, plugins, model layers, the `window.binacle` global, and how to add a component — is in
`$packages/binacle-net-ui`. **Consumed as TS source by two hosts**, each with its own webpack config:
`sites/demo/` and the UIModule (`$api/modules/ui`). One implementation, two pages — a change lands on both.

## binacle-compact-notation

TypeScript mirror of the C# `Binacle.CompactNotation` — the shared compact text notation for Binacle geometry
(`"10x10x10 (0,0,0)"` style). A leaf with no dependencies. Used by `binacle-vipaq` in its `tools/` and `tests/`
(not runtime `src/`) to parse geometry when generating interop artifacts and reading shared vectors.

## cookies

Vendored fork of js-cookie v3.0.5, MIT, kept close to upstream so a re-sync stays cheap. Reached through
`theme-switcher` by both sites and the UIModule. No dependencies.

`Cookies` is a static class, not the upstream factory: there is no `withConverter` or `withAttributes`.
Defaults are `path=/`, `expires` 90 days, `sameSite=Lax`, `secure` — so a page served over plain http cannot
read back what it writes.

## theme-switcher

Custom HTML element (`<theme-switcher>`) for switching light/dark themes. Used by both sites under `sites/`
and by the UIModule. Depends on the `cookies` workspace package; no external dependencies.

**The disconnect hook is spelled `disconectedCallback`** — one `n`. The browser never calls it, so the click
listener outlives the element. Correcting the spelling changes runtime behaviour, so it is pinned by two
tests rather than fixed. The `removeEventListener` inside it would not have worked either: it binds a fresh
function, which never matches the one `connectedCallback` added.

## Related Tests

| Project | What it covers | Run |
|---|---|---|
| `packages/binacle-compact-notation` | the notation parser/formatter, `tests/compactNotation.test.ts` | `just test shared-ts-unit` |
| `packages/cookies` | the converter round trip, get/set/remove, attribute stringifying | `just test packages-cookies-unit` |
| `packages/theme-switcher` | connect, click, icon and the pinned disconnect behaviour | `just test packages-theme-switcher-unit` |
| `vipaq/packages/binacle-vipaq` | the ViPaq TS mirror, including the shared cross-language vectors | `just test vipaq-ts-unit` |

`binacle-net-ui` has no suite. The compact-notation alias is filed under **shared**, not packages, because
that package mirrors a `shared/src` C# project; the other two are named after the folder they live in.

Both new suites run on jsdom, so their configs add `jest-environment-jsdom` (jest 29 does not bundle it) and
point jsdom at an `https` URL — the cookies defaults include `secure`, and jsdom hides a secure cookie from a
document on an insecure origin.

## Dependencies

Which package imports which — every workspace import declared in its `package.json` — is in
`$packages/dependencies`.
