# Idea: UIModule — port from Blazor reactivity to Alpine.js

**Status:** Unvetted idea — no committed timeline or priority. Detailed below because the ground was already
scouted, not because it's scheduled.

**Goal:** Replace Blazor Interactive Server reactivity with Alpine.js so `packages/binacle-net-ui`
is the single source for the packing demo UI — shared between `web/` and `UIModule`.

**Accuracy.** The build section was written 2026-05-26 and got four things wrong; it was re-verified against the
code on 2026-07-08 and the corrections are recorded inline where they belong — the blockquote under "Build
Pipeline Detail", and open question 4. Spot-checked again 2026-08-07: no webpack config in the package, no build
scripts, no `src/index.ts`, `packingDemo.ts` still calls v3 at line 127, and the Three.js skew is still real.
One drift found and fixed below: the package now also depends on `binacle-vipaq`. Treat the paths here as good,
and re-check anything the build touches — it is the part that moved before.

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

`packages/binacle-net-ui` has (verified paths):
- `src/core/packingDemo.ts` — Alpine component `packing_demo_app`, takes `base_url: string`,
  calls `` `${base_url}/api/v3/pack/by-custom` `` (line 127)
- `src/core/protocolDecoder.ts` — Alpine component, decodes ViPaq locally
- `src/core/packingVisualizer.ts` — Alpine component, wraps `window.binacle.*` calls
- `src/packingDemoPlugin.ts` / `src/protocolDecoderPlugin.ts` — Alpine plugin registrations, and the real
  entry points. **There is no `src/index.ts`.**

The `base_url` for `packingDemoApp` is already a parameter — not hardcoded.
In the web site it comes from `site.api_url` (Jekyll config). In UIModule it will be `""` (relative).

**Note:** the demo calls **v3**, on a branch whose point is v4. Porting as-is carries v3 into UIModule. Fine if
deliberate; decide it rather than inherit it.

## What Changes

### Delete
- `Services/BinacleVisualizerService.cs` — JS interop bridge, no longer needed
- `Services/MessagingService.cs` — pub/sub coordination, no longer needed
- `Components/Pages/PackingDemo.razor.cs` — all C# logic moves to Alpine
- `Components/Pages/ProtocolDecoder.razor.cs` — all C# logic moves to Alpine
- `Components/Features/PackingVisualizer.razor.cs` — **exists, and the original plan forgot it.** If
  `PackingVisualizer.razor` becomes a plain container, its code-behind goes too. Check what it holds first.
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

> **The original plan for this section was wrong on four counts.** Corrected 2026-07-08 against the code. Read this
> before writing any webpack.
>
> 1. **There is no `packages/binacle-net-ui/webpack.config.js`.** The only webpack configs in the repo are
>    `web/webpack.config.js` and `docs/webpack.config.js`. The package is source-only.
> 2. **`web/js/binacle-net-ui.js` is not a webpack output target.** It is a **`splitChunks` cache group** in
>    `web/webpack.config.js` (`test: /[\\/]packages[\\/]binacle-net-ui[\\/]/`, priority 20). The real entries are
>    `web/_js/{main,packing_demo,protocol_decoder}.js`. It has no `libraryTarget`, so it is a code-split chunk,
>    not a consumable library — you cannot "add a second output" beside it.
> 3. **The package has no `scripts` and no build deps.** `package.json` lists `alpinejs`, `three` and
>    `binacle-vipaq` as dependencies and the two `@types` as devDependencies. No webpack, no ts-loader. So
>    `npm run build:uimodule` has nothing to run and the MSBuild `<Exec>` below has no target.
> 4. **There is no `src/index.ts`.** The entry points are `src/packingDemoPlugin.ts` and
>    `src/protocolDecoderPlugin.ts`.

**So the real work is: create a build for the package that does not exist today.** Two viable shapes:

- **A new `packages/binacle-net-ui/webpack.config.js`** with its own `webpack` + `ts-loader` devDependencies and a
  `build:uimodule` script, emitting one IIFE bundle straight into
  `api/src/Binacle.Net.UIModule/wwwroot/js/`. Entry is the two plugin files (or a new `src/index.ts` that
  re-exports them). `web/` keeps consuming the package by source, unchanged.
- **A second config inside `web/`** that reuses its already-installed webpack toolchain and emits the IIFE to
  UIModule's `wwwroot`. Cheaper to stand up, but makes UIModule's assets a by-product of the website build —
  a coupling worth avoiding.

Prefer the first. The IIFE format is required because UIModule loads scripts with `<script src>`, not ES imports.

Then, optionally, the MSBuild hook in `UIModule.csproj` — which only works **after** the npm script exists:

```xml
<Target Name="BuildAlpineBundle" BeforeTargets="Build">
  <Exec Command="npm run build:uimodule" WorkingDirectory="$(RepoRoot)packages/binacle-net-ui" />
</Target>
```

## Open Questions Before Starting

1. **Does `packingVisualizer.ts` (Alpine) already call `window.binacle.*` directly?**
   If yes, it's a drop-in. If not, it may still delegate to Blazor — check the component source.

2. **Does the ProtocolDecoder Alpine component handle localStorage itself,**
   or does it expect `LocalStorageService` via some bridge? If the former, `LocalStorageService` can go too.

3. **Does anything else in UIModule use Interactive Server rendering?**
   If yes, keep `AddInteractiveServerComponents()` but limit it to those components only.

4. ~~**Alpine version** — confirm `docs/lib/alpine.js` and `web/lib/alpine.js` are the same version.~~
   **Malformed — neither file exists.** Alpine is an npm dependency (`alpinejs ^3.15.2`) that `web/_js/*.js`
   pulls in with `import Alpine from 'alpinejs'`, and webpack bundles it. There is nothing to copy to
   `wwwroot/vendor/`. The real question: does UIModule's IIFE bundle **include** Alpine, or load it from a CDN /
   vendored copy and only register plugins against `window.Alpine`? Pick one — it decides script order in
   `App.razor`.

5. **Three.js version skew.** `App.razor`'s importmap pins `three@0.176.0` from jsDelivr; the package depends on
   `three@^0.182.0`. The plan says the importmap is unchanged, which keeps two versions in play once the bundle
   lands. Confirm `wwwroot/js/PackingVisualizer.js` still works, or align them.

## Execution Order

1. Answer open questions above (read source if unsure).
2. Add second webpack target, verify the IIFE bundle builds and loads correctly in a browser.
3. Wire up Alpine and the bundle in `App.razor`.
4. Replace `PackingDemo.razor` with the Alpine shell. Test the full demo flow.
5. Replace `ProtocolDecoder.razor` with the Alpine shell. Test decode flow.
6. Delete `BinacleVisualizerService`, `MessagingService`, and code-behind files.
7. Remove Interactive Server render mode if nothing else needs it.
8. Add MSBuild hook so `dotnet build` keeps the bundle fresh.
9. Update the UIModule doc to reflect the new stack.
