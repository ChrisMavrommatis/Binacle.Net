---
id: packages/binacle-net-ui
description: packages/binacle-net-ui — Alpine.js components + Three.js visualizer for the packing demo. Components, plugins, model layers, and the window.binacle global.
verified: 2026-08-22
check: Every Alpine.data name in src/core/ appears in the component table and vice versa; the two plugins register exactly what is listed; the model-layer folders and the utils split match src/; the hardcoded endpoint in core/packingDemo.ts is still the one named here; the coverage figure below still matches a `just coverage` run
also_update:
  - packages
paths:
  - "packages/**"
---

# binacle-net-ui

TypeScript package implementing the interactive packing UI as Alpine.js components plus a Three.js 3D visualizer.
Private npm workspace package (`"private": true`, name `binacle-net-ui`).

## Build & consumers — important

This package has **no build step and no bundle of its own** — `"main": "index.ts"` points at raw TypeScript, and
its only script is `test`. Each host compiles it from source with its own webpack + ts-loader.

| Host | Entries | Config |
|---|---|---|
| `sites/demo/` (Jekyll) | `sites/demo/_js/packing_demo.js`, `protocol_decoder.js` | `sites/demo/webpack.config.js` |
| `api/src/Binacle.Net.UIModule` (Razor Pages) | `_js/packing_demo.js`, `_js/protocol_decoder.js` | the module's `webpack.config.js` |

**One implementation, two hosts. A change here lands on both** — that is the point, and it is the rule to test
any proposed feature against: pass data in, never fork the component.

Both configs give this package its own split chunk with the same name and priority, so the chunk set cannot
drift. `three` is bundled from `node_modules` in both, never a CDN.

**Every host must resolve exactly one copy of `three`.** This package imports it and so does each host; if the
two resolve to different directories, webpack bundles both and a mesh built by one fails `instanceof` against
the other. Resolving through the root workspace is what prevents it.

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
| `packing_demo_app` | `packingDemoApp` | `({ baseUrl })` | Form model (bins/items/algorithm), validation, randomizers. On submit POSTs to **`${baseUrl}/api/v3/pack/by-custom`**; dispatches `update-scene` / `error-occurred`. Algorithms: FFD/BFD/WFD |
| `protocol_decoder_app` | `protocolDecoderApp` | none | Decodes base64 ViPaq via `binacle-vipaq`'s `ViPaqSerializer.deserialize`; saves to `localStorage` key `ProtocolDecoderSavedResults` |
| `packing_visualizer` | `packingVisualizer` | none | The Three.js scene. Listens for `update-scene`; sets up the scene in `init()` and stores it on `window.binacle`. Playback controls drive items in/out |
| `errors_dialog` | `errorsDialog` | `(default_title)` | Error dialog; `onErrorOccurred(detail)` handles a `string[]` or an `Error` view-model |

Supporting (not `x-data`): `field` (Alpine directive `x-field-prefix` + magics `$fieldId`/`$fieldName` for
hierarchical field names), `logger` (magic `$logger`), `Binacle` (the interface shape of `window.binacle`),
`ControlsManager` (visualizer button state). All re-exported from `core/index.ts`.

Cross-component messaging is via Alpine window events: **`update-scene`** (payload is a `() => Promise<{bin,items}>`
or a `DecodedPackingResult`) and **`error-occurred`** (payload `string[]` or `Error`). `packing_visualizer` and
`errors_dialog` are the listeners.

## window.binacle

This package's `packingVisualizer.init()` sets `window.binacle = { rendererContainer, visualizerContainer,
visualizerState }` — exactly the three members of the `Binacle` interface in `src/core/binacle.ts`, all
nullable, declared onto `Window` in `src/types/global.d.ts`. It is **event-driven** — there are no public
`initialize`/`redrawScene` methods.

**There is only one now.** The UIModule used to define a second, imperative `window.binacle` in a hand-written
`wwwroot/js/PackingVisualizer.js`, driven from Blazor JS interop. That file and the stack around it are gone;
both hosts run the event-driven one above.

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
- The only API call is hardcoded at `packingDemo.ts` → `POST {baseUrl}/api/v3/pack/by-custom`. To point the demo
  at v4, that's the single line (and the request shape in `apiModels/PackingRequest`) to change.
- **`packing_demo_app` takes an options object** — `PackingDemoOptions`, `baseUrl` optional. Options rather than
  positional so a second value later is not a signature break.
- **A signature change here lands on both hosts**, and neither can be updated without the other. The way through
  is to widen first, move each host, then narrow: that is how the base URL went from positional to an object on
  2026-08-22 without either page breaking in between.
- No compile here — each host's webpack picks up changes via the workspace symlink.

## Tests

`just test packages-net-ui-unit`. jsdom, because the components read `document` and `window` even where the
logic under test does not. **20 suites, 273 tests, 69.73% of lines** as of 2026-08-22.

`tests/model/` is the pure half — the randomizer, the view models, `ControlsManager`. `tests/components/` is
the Alpine half: each component factory is a plain object, so a test calls it directly with a stub `$dispatch`
and `$logger` rather than starting Alpine.

**What is uncovered is the Three.js half, and that is the intended answer** — `core/packingVisualizer.ts` and
the scene helpers in `utils/` need a WebGL context, so a test there could only assert that a call happened.
They stay in the coverage denominator rather than being excluded from it: the number is meant to show the gap,
not hide it. Only `.d.ts` files are excluded, in the root config, because they carry no runtime code.

**`three` ships `OrbitControls` as ESM only**, which the commonjs transform cannot load. Importing either
plugin barrel pulls the visualizer in and hits it, so `jest.config.js` maps it to `tests/stubs/orbitControls.ts`.
Nothing under test constructs one.
