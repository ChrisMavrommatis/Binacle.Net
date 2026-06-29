---
description: packages/binacle-net-ui — Alpine.js components + Three.js visualizer for the packing demo. Components, plugins, model layers, and the window.binacle global.
verified: 2026-06-29
check: Component x-data names, plugin exports, and model layers match packages/binacle-net-ui/src/
also_update:
  - packages/README.md
---

# binacle-net-ui

TypeScript package implementing the interactive packing UI as Alpine.js components plus a Three.js 3D visualizer.
Private npm workspace package (`"private": true`, name `binacle-net-ui`).

## Build & consumers — important

This package has **no build step and no bundle of its own** — `"main": "index.ts"` points at raw TypeScript, and
there are no `build`/`test` scripts. It is consumed as **TS source** by the `web/` Jekyll site, which compiles it
with its own webpack + ts-loader (`web/_js/packing_demo.js`, `web/_js/protocol_decoder.js`; split chunk in
`web/webpack.config.js`). `three` is bundled from web's `node_modules`, not a CDN.

The `Binacle.Net.UIModule` does **not** use this package — it has its own legacy raw-JS visualizer in
`wwwroot/js`. There is an open plan to converge them (`.agents/plans/uimodule-alpine-port.md`).

## Public entry points (`index.ts`)

Two aggregate Alpine plugins — these are the only public surface:

| Plugin | Registers |
|---|---|
| `packingDemoPlugin` | `fieldPlugin`, `loggerPlugin`, `packingDemoAppPlugin`, `packingVisualizerPlugin`, `errorsDialogPlugin` |
| `protocolDecoderPlugin` | `loggerPlugin`, `packingVisualizerPlugin`, `protocolDecoderAppPlugin`, `errorsDialogPlugin` |

A host page imports a plugin, calls `Alpine.plugin(...)`, then `Alpine.start()`, and uses the `x-data` names in HTML.

## Components (`src/core/`)

| `x-data` name | Factory | Params | What it does |
|---|---|---|---|
| `packing_demo_app` | `packingDemoApp` | `(base_url)` | Form model (bins/items/algorithm), validation, randomizers. On submit POSTs to **`${base_url}/api/v3/pack/by-custom`**; dispatches `update-scene` / `error-occurred`. Algorithms: FFD/BFD/WFD |
| `protocol_decoder_app` | `protocolDecoderApp` | none | Decodes base64 ViPaq via `binacle-vipaq`'s `ViPaqSerializer.deserialize`; saves to `localStorage` key `ProtocolDecoderSavedResults` |
| `packing_visualizer` | `packingVisualizer` | none | The Three.js scene. Listens for `update-scene`; sets up the scene in `init()` and stores it on `window.binacle`. Playback controls drive items in/out |
| `errors_dialog` | `errorsDialog` | `(default_title)` | Error dialog; `onErrorOccurred(detail)` handles a `string[]` or an `Error` view-model |

Supporting (not `x-data`): `field` (Alpine directive `x-field-prefix` + magics `$fieldId`/`$fieldName` for
hierarchical field names), `logger` (magic `$logger`), `Binacle` (the interface shape of `window.binacle`),
`ControlsManager` (visualizer button state). All re-exported from `core/index.ts`.

Cross-component messaging is via Alpine window events: **`update-scene`** (payload is a `() => Promise<{bin,items}>`
or a `DecodedPackingResult`) and **`error-occurred`** (payload `string[]` or `Error`). `packing_visualizer` and
`errors_dialog` are the listeners.

## window.binacle (two different ones — don't confuse them)

This package's `packingVisualizer.init()` sets `window.binacle = { rendererContainer, visualizerContainer,
visualizerState }` (typed in `src/types/global.d.ts`). It is **event-driven** — there are no public
`initialize`/`redrawScene` methods.

The UIModule's `wwwroot/js/PackingVisualizer.js` defines a **different** `window.binacle` with an imperative API
(`initialize`, `redrawScene`, `addItemToScene`, `removeItemFromScene`, …) called from Blazor JS interop. Same name,
separate implementation.

## Model layers (`src/`)

| Folder | Nature | Key types |
|---|---|---|
| `apiModels/` | API request/response DTOs (match the v3 pack contract) | `PackingRequest`, `PackingParameters`, `PackingResponse`/`PackedData`, `Bin`, `Item`, `PackedItem`, `UnpackedItem` |
| `viewModels/` | UI-side stateful classes with validation | `Box` (dimension validation, min 1 / max 65535), `Bin`, `Item`, `Control`, `DecodedPackingResult`, `Error`, `ErrorCollection` |
| `models/` | Shared structural interfaces + Three.js scene state | `Coordinates`, `Dimensions`, `Dictionary<T>`, `SceneData`, `VisualizerState` |
| `types/` | Ambient declarations only | `alpine.d.ts` (adds `$logger`, `_x_fieldPrefix`), `global.d.ts` (`Window.binacle`) |

`onSubmit` maps `viewModels` (classes) into `apiModels` (plain objects) before POST. Three.js scene helpers live
in `src/utils/` (`redrawScene`, `createBin`/`createItem`, `addItemToScene`/`removeItemFromScene`, camera helpers,
`getThemeColors`, …) alongside non-scene utils (`defineComponent`, `getRandom*`, `findClosestElement`).

## Conventions for adding / modifying a component

- Factory is camelCase (`packingDemoApp`); the `x-data` string is snake_case (`packing_demo_app`); wire them with
  `Alpine.data('snake_name', factory)` in a `*Plugin`. Wrap the factory body in `defineComponent(...)` for typing.
- To add a component to a page, add its `*Plugin` to `src/packingDemoPlugin.ts` or `src/protocolDecoderPlugin.ts`,
  and export the factory + plugin from `src/core/index.ts`.
- The only API call is hardcoded at `packingDemo.ts` → `POST {base_url}/api/v3/pack/by-custom`. To point the demo
  at v4, that's the single line (and the request shape in `apiModels/PackingRequest`) to change.
- No tests, no compile here — `web/`'s webpack picks up changes via the workspace symlink.
