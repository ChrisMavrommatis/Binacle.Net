# Packages

Shared TypeScript packages used across the repo. They are npm workspaces — install once
from the repo root with `npm install`.

## 📦 Packages

| Package | What it is |
|---|---|
| [`binacle-net-ui`](binacle-net-ui) | Alpine.js components and a Three.js visualizer for the interactive packing demo |
| `binacle-compact-notation` | Parses and formats the compact strings (`"60x40x30"`, `"108x76x30 [40]"`). TypeScript mirror of C# `Binacle.CompactNotation` |
| `cookies` | Cookie read/write helpers. A vendored fork of js-cookie v3.0.5, MIT, kept close to upstream |
| `theme-switcher` | Light/dark theme toggle, as a `<theme-switcher>` custom element |

Three have tests. From the repo root:

```
just test shared-ts-unit                # binacle-compact-notation
just test packages-cookies-unit         # cookies
just test packages-theme-switcher-unit  # theme-switcher
```

`binacle-compact-notation` has to agree with
[`shared/src/Binacle.CompactNotation`](../shared/src/Binacle.CompactNotation) by hand; there is no codegen
between them. Its leaf is named after `shared` for that reason, not after this folder.

## 🌐 Who imports them

The two sites and the API's UI module pull them in by package name and webpack bundles them —
[`sites/demo`](../sites/demo) and [the UI module](../api/src/Binacle.Net.UIModule) use `binacle-net-ui` for
the packing demo and the ViPaq decoder, and all three use `theme-switcher`. Nothing is copied: the import
resolves through the workspace. (`just assets` is a different job - it copies the static files in
[`assets/`](../assets), not these.)

**None of them has a build step.** Each host compiles the TypeScript from source with its own webpack and
ts-loader, so a change here lands on every host at once and none of them can be updated on its own. That is
also why an edit here is only proven by building the hosts, not by building the package.
