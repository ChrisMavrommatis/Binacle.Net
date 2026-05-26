# UIModule — Port from Blazor Reactivity to Alpine.js

**Status:** Not started  
**Goal:** Replace Blazor Interactive Server reactivity with Alpine.js so `packages/binacle-net-ui`
is the single source for the packing demo UI — shared between `web/` and `UIModule`.

## Why

Currently `web/` uses Alpine.js components from `packages/binacle-net-ui`, while `UIModule` has
its own parallel Blazor implementation. Any change to the demo UI requires updating both.
The port makes `packages/binacle-net-ui` the only place the demo logic lives.

## Current State (as of 2026-05-26)

UIModule uses:
- Blazor Interactive Server rendering for all reactivity (forms, state, API calls)
- `BinacleVisualizerService` — wraps `IJSRuntime`, calls `window.binacle.*`
- `MessagingService` — scoped pub/sub bus coordinating components
- `PackingDemo.razor.cs` / `ProtocolDecoder.razor.cs` — full C# logic in code-behind

`packages/binacle-net-ui` has:
- `packingDemo.ts` — Alpine component, takes `base_url: string`, calls `/api/v3/pack/by-custom`
- `protocolDecoder.ts` — Alpine component, decodes ViPaq locally
- `packingVisualizer.ts` — Alpine component, wraps `window.binacle.*` calls
- `packingDemoPlugin.ts` / `protocolDecoderPlugin.ts` — Alpine plugin registrations

The `base_url` for `packingDemoApp` is already a parameter — not hardcoded.
In the web site it comes from `site.api_url` (Jekyll config). In UIModule it will be `""` (relative).

## What Changes

### Delete
- `BinacleVisualizerService.cs` — JS interop bridge, no longer needed
- `MessagingService.cs` — pub/sub coordination, no longer needed
- `PackingDemo.razor.cs` — all C# logic moves to Alpine
- `ProtocolDecoder.razor.cs` — all C# logic moves to Alpine
- Interactive Server render mode registration (if no other components need it)

### Add / Modify
- **webpack: second output target** — build `packages/binacle-net-ui` into a UIModule IIFE bundle.
  Current web target produces `web/js/binacle-net-ui.js` (CommonJS).
  New UIModule target produces an IIFE (attaches to `window`) for use via plain `<script src>`.
  The IIFE format is required because UIModule loads scripts with `<script src>`, not ES module imports.
- **MSBuild hook (optional)** — add a `BeforeBuild` target in `UIModule.csproj` that runs the npm build,
  so `dotnet build` keeps the JS up to date automatically.
- **`App.razor`** — add Alpine.js `<script src>` and the UIModule IIFE bundle after `blazor.web.js`.
  Alpine must load before the bundle so plugins can register.
- **`PackingDemo.razor`** — replace code-behind logic with `<div x-data="packing_demo_app('')">`.
  The `""` base_url means relative URLs — hits the same-host API automatically.
- **`ProtocolDecoder.razor`** — same pattern, `<div x-data="protocol_decoder_app">`.
- **`PackingVisualizer.razor`** — becomes a plain HTML container `<div>` with an id/ref.
  Alpine's `packingVisualizer` component handles `window.binacle.*` calls directly.

### Stays the Same
- `wwwroot/js/PackingVisualizer.js` — Three.js scene, ES module, no Alpine dependency
- `wwwroot/js/addons/PackingVisualizer.utils.js` — helpers for PackingVisualizer.js
- `wwwroot/js/cookies.js` and `themeswitcher.js` — plain scripts, unchanged
- The importmap in `App.razor` for Three.js — unchanged
- `LocalStorageService`, `SampleDataService`, `AppletsService`, `ThemeService` — unchanged
- All Blazor pages that don't involve the packing demo — unchanged

## Build Pipeline Detail

The key decision: **how does the UIModule bundle get built and land in `wwwroot/js/`?**

Recommended approach — **dual webpack output target**:

In `packages/binacle-net-ui/webpack.config.js` (or the root webpack config), add a second entry/output:

```js
// Target 1 — existing web/ bundle (CommonJS, for webpack consumption in web/)
{
  entry: './src/index.ts',
  output: { path: '../../web/js', filename: 'binacle-net-ui.js', libraryTarget: 'commonjs2' }
}

// Target 2 — UIModule IIFE bundle
{
  entry: './src/index.ts',
  output: {
    path: '../../api/src/Binacle.Net.UIModule/wwwroot/js',
    filename: 'binacle-net-ui.js',
    library: 'BinacleNetUI',
    libraryTarget: 'umd'   // or 'iife' — check webpack version support
  }
}
```

Alternative — MSBuild `<Exec>` target in `UIModule.csproj`:

```xml
<Target Name="BuildAlpineBundle" BeforeTargets="Build">
  <Exec Command="npm run build:uimodule" WorkingDirectory="$(RepoRoot)packages/binacle-net-ui" />
</Target>
```

Both can be combined: npm script triggers webpack with the dual config, MSBuild calls the npm script.

## Open Questions Before Starting

1. **Does `packingVisualizer.ts` (Alpine) already call `window.binacle.*` directly?**
   If yes, it's a drop-in. If not, it may still delegate to Blazor — check the component source.

2. **Does the ProtocolDecoder Alpine component handle localStorage itself,**
   or does it expect `LocalStorageService` via some bridge? If the former, `LocalStorageService` can go too.

3. **Does anything else in UIModule use Interactive Server rendering?**
   If yes, keep `AddInteractiveServerComponents()` but limit it to those components only.

4. **Alpine version** — confirm `docs/lib/alpine.js` and `web/lib/alpine.js` are the same version.
   UIModule should load from `wwwroot/vendor/` — copy from one of those.

## Execution Order

1. Answer open questions above (read source if unsure).
2. Add second webpack target, verify the IIFE bundle builds and loads correctly in a browser.
3. Wire up Alpine and the bundle in `App.razor`.
4. Replace `PackingDemo.razor` with the Alpine shell. Test the full demo flow.
5. Replace `ProtocolDecoder.razor` with the Alpine shell. Test decode flow.
6. Delete `BinacleVisualizerService`, `MessagingService`, and code-behind files.
7. Remove Interactive Server render mode if nothing else needs it.
8. Add MSBuild hook so `dotnet build` keeps the bundle fresh.
9. Update `.agent-docs/api/modules/ui.md` to reflect the new stack.
