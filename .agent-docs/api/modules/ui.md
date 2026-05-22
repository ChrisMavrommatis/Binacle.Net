---
description: UIModule — optional Blazor Web App interactive packing demo. Pages, JS stack, API connection, config, and services.
verified: 2026-05-23
---

# UIModule

`api/src/Binacle.Net.UIModule`

Optional interactive packing demo. Enabled by the `UI_MODULE` feature flag.

Not relevant to core API or Lib work. Skip this doc unless you are working on the demo UI.

## Technology

Blazor Web App with Interactive Server rendering (not classic Blazor Server).
Uses `AddInteractiveServerComponents()` / `AddInteractiveServerRenderMode()`.

## Pages

| Route | Page | What it does |
|---|---|---|
| `/` | Home | Landing page |
| `/PackingDemo` | Packing Demo | Form to enter bins/items, calls the pack API, shows result in 3D viewer |
| `/ProtocolDecoder` | Protocol Decoder | Paste a ViPaq-encoded result, decodes and renders it without calling the API |
| `/Error` | Error | Generic error page |
| `/Error/{ErrorCode}` | Error | Error page with specific HTTP status code |

## JS Stack

Three.js is loaded from CDN via an importmap in `App.razor`:

```json
{
  "three": "https://cdn.jsdelivr.net/npm/three@0.176.0/build/three.module.js",
  "binacle/addons/": "/js/addons/"
}
```

`binacle/addons/` maps to local files in `wwwroot/js/addons/`.

Custom JS files (in `wwwroot/js/`):
- `PackingVisualizer.js` — Three.js 3D scene for rendering packing results
- `PackingVisualizer.utils.js` — Three.js helper utilities
- `cookies.js` — cookie read/write helpers
- `themeswitcher.js` — light/dark theme toggle

Vendor bundles (in `wwwroot/vendor/`):
- BeerCSS — CSS framework
- `material-dynamic-colors` — dynamic Material You color theming

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
