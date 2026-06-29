---
description: UIModule — optional Blazor Web App interactive packing demo. Pages, JS stack, API connection, config, and services.
verified: 2026-05-26
check: Pages, JS imports, services, and window.binacle API match api/src/Binacle.Net.UIModule/
also_update:
  - packages/README.md
---

# UIModule

`api/src/Binacle.Net.UIModule`

Optional interactive packing demo. Enabled by the `UI_MODULE` feature flag.

Not relevant to core API or Lib work. Skip this doc unless you are working on the demo UI.

## Technology

Blazor Web App with Interactive Server rendering (not classic Blazor Server).
Uses `AddInteractiveServerComponents()` / `AddInteractiveServerRenderMode()`.

**No Alpine.js.** All reactivity is Blazor (`@onclick`, `@onchange`, component bindings).
BeerCSS handles Material Design animations and interactions.

## Pages

| Route | Page | What it does |
|---|---|---|
| `/` | Home | Landing page |
| `/PackingDemo` | Packing Demo | Form to enter bins/items, calls the pack API, shows result in 3D viewer |
| `/ProtocolDecoder` | Protocol Decoder | Paste a ViPaq-encoded result, decodes and renders it without calling the API |
| `/Error` | Error | Generic error page |
| `/Error/{ErrorCode}` | Error | Error page with specific HTTP status code |

## JS Stack

`App.razor` is the root layout (equivalent to `_Host.cshtml`). It loads scripts in this order:

1. `blazor.web.js` — Blazor runtime
2. `cookies.js` — adds `window.Cookies` globally (plain script, no modules)
3. importmap — tells the browser how to resolve ES module imports
4. `PackingVisualizer.js` — loads as `type="module"`, imports Three.js via the importmap
5. `beer.min.js` — Material Design interactions
6. `themeswitcher.js` — plain script for light/dark toggle

### The `@Assets` Helper

All static file paths go through Blazor's `@Assets["_content/Binacle.Net.UIModule/..."]` helper.
This resolves paths to the module's `wwwroot/` at `/_content/Binacle.Net.UIModule/`.
You'll see this pattern everywhere in `App.razor` — it's not magic, just Blazor's static web asset system.

### Importmap

Defined inline in `App.razor`:

```json
{
  "imports": {
    "three": "https://cdn.jsdelivr.net/npm/three@0.176.0/build/three.module.js",
    "three/addons/": "https://cdn.jsdelivr.net/npm/three@0.176.0/examples/jsm/",
    "binacle/addons/": "/_content/Binacle.Net.UIModule/js/addons/"
  }
}
```

`binacle/addons/` maps to `wwwroot/js/addons/` (local ES module helpers).

### wwwroot/js/ Files

| File | Type | What it does |
|---|---|---|
| `PackingVisualizer.js` | ES module | Creates and manages the Three.js scene; exposes `window.binacle` |
| `addons/PackingVisualizer.utils.js` | ES module | Three.js mesh/camera helpers, imported by `PackingVisualizer.js` |
| `cookies.js` | Plain script | Adds `window.Cookies` — used for theme persistence |
| `themeswitcher.js` | Plain script | Adds theme switching logic |

### window.binacle API

`PackingVisualizer.js` creates a `window.binacle` object that C# calls via JS interop:

| Method | What it does |
|---|---|
| `binacle.initialize(bin)` | Creates the Three.js scene for the given bin |
| `binacle.redrawScene(bin, packedItems)` | Replaces the scene contents |
| `binacle.addItemToScene(bin, packedItem, index)` | Animates adding one item |
| `binacle.removeItemFromScene(index)` | Animates removing one item |

Communication is **one-way: C# → JS only**. There are no `[JSInvokable]` methods — JS never calls back into C#.

## JS Interop Bridge

`BinacleVisualizerService` wraps `IJSRuntime` and calls the `window.binacle.*` methods above.
It's the only service that touches `IJSRuntime`. Everything else is pure Blazor or C#.

## Component Coordination

`MessagingService` is a scoped in-process pub/sub bus.
`PackingVisualizer.razor` subscribes to `"UpdateScene"` messages.
`PackingDemo` and `ProtocolDecoder` publish them when the user triggers a visualization update.
The handler invokes `BinacleVisualizerService` which calls into the JS visualizer.

## API Connection

Uses a named `HttpClient` registered as `"BinacleApi"`.
By default posts to the same host (no config needed for local dev).

Override the base URL via `Config_Files/UiModule/ConnectionStrings.json` (optional):

```json
{
  "ConnectionStrings": {
    "BinacleApi": "https://your-api-host"
  }
}
```

## Services

All scoped (per connection / browser tab) unless noted:

| Service | Lifetime | What it does |
|---|---|---|
| `ThemeService` | Scoped | Manages light/dark theme state |
| `MessagingService` | Scoped | In-component pub/sub for cross-component communication |
| `BinacleVisualizerService` | Scoped | Drives the Three.js 3D visualizer via JS interop |
| `LocalStorageService` | Scoped | Read/write browser localStorage |
| `SampleDataService` | Scoped | Provides sample bin/item data for the demo form |
| `AppletsService` | Singleton | Manages UI applets / widget state |

## Status Code Pages

Status code pages are disabled for `/api`, `/swagger`, and `/scalar` paths.
This prevents Blazor error middleware from intercepting API or OpenAPI error responses.
