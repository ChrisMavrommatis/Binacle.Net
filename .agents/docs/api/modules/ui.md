---
id: api/modules/ui
description: UIModule — optional Razor Pages demo host. Routes, the webpack and sass build, the applet list, and how error pages are decided.
verified: 2026-08-22
check: Routes match the @page directives under Pages/; the DI registrations match ModuleDefinition.cs; the script and stylesheet paths in Pages/Shared/_Layout.cshtml and _AppletScripts.cshtml match the webpack entries and cacheGroups in webpack.config.js; the applet list matches Services/AppletsService.cs; the switch list in Models/FeatureSwitch.cs matches the feature flag table in api/configuration; a grep for Blazor, IJSRuntime or .razor in the module returns nothing
also_update:
  - packages
  - api/configuration
paths:
  - "api/src/Binacle.Net.UIModule/**"
---

# UIModule

`api/src/Binacle.Net.UIModule`

The demo UI that ships inside the API. Enabled by the `UI_MODULE` feature flag.

Not relevant to core API or Lib work. Skip this doc unless you are working on the demo UI.

## Technology

**Razor Pages. No Blazor, no SignalR circuit, no server-side component state.** `AddRazorPages()` and
`MapRazorPages().WithStaticAssets()` are the whole registration.

**All interactivity is Alpine.js**, from TypeScript compiled out of `packages/binacle-net-ui`. The module
holds no demo logic of its own — see `$packages/binacle-net-ui`.

`<AddRazorSupportForMvc>true</AddRazorSupportForMvc>` is required in the csproj. Without it the `.cshtml`
files are never discovered from the host app and every route 404s.

**Razor generates `internal sealed` page and partial classes here**, so `@model` and `@inject` both work with
internal types. `Applet`, `AppletsService`, `UIModuleOptions` and every PageModel are internal.

## Pages

| Route | Page | What it does |
|---|---|---|
| `/` | `Index` | Three cards, one per applet. The whole card is the link; the page has no button |
| `/packing` | `Packing` | The packing demo. Calls the pack API from the browser |
| `/vipaq` | `Vipaq` | Pastes a ViPaq-encoded result and renders it. Calls nothing |
| `/instance` | `Instance` | Version, the switch list, and the presets this instance loaded |
| `/error/{errorCode?}` | `Error` | The error page, and the `UseStatusCodePagesWithReExecute` target |

`RouteOptions.LowercaseUrls` is true globally. **Every internal link uses the `asp-page` tag helper, never a
literal path**, so a route change is one edit in the `@page` directive.

`Pages/Shared/` holds the chrome: `_Layout`, `_Header`, `_Navbar`, `_Footer`, plus `_PackingVisualizer`,
`_ErrorsDialog` and `_AppletScripts`. `_AppletScripts` is the one copy of the library load order, so the two
demo pages cannot drift.

**Alpine's `@click` and `@submit` are `@@click` and `@@submit` in a `.cshtml` file.** `x-on:` forms need no
escaping.

## The build

`wwwroot/` is **generated in full and gitignored**. Nothing in it is hand-maintained. Three producers fill it:

| Producer | Fills | From |
|---|---|---|
| `just assets` (gulp) | `lib/`, `media/`, the root icons | repo-root `assets/` |
| `npm run build:css` (dart-sass) | `css/main.css` | `_sass/main.scss` |
| `npm run build:js` (webpack) | `js/` | `_js/` — three entries |

`just build publish` runs all three before `dotnet publish`, because static web assets are collected at
publish time. **A missing bundle fails nothing** — the image ships pages that return 200 and do nothing —
which is why `full.hurl` and `quickstart.hurl` assert the bundle and stylesheet directly.

The webpack entries are `main`, `instance`, `packing_demo` and `protocol_decoder`. The chunk names and
priorities match `sites/demo/webpack.config.js`; both compile the same package source, so there is one
implementation and only the config is duplicated.

`instance` imports nothing, so it is its own 1 KB file and pulls in no shared chunk. The instance page loads
`runtime` + `main` + `instance` and none of `vendors`, `three` or the two package chunks.

**The module is a root npm workspace member.** `binacle-net-ui`, `binacle-vipaq`, `cookies` and
`theme-switcher` resolve to symlinks in the root `node_modules`, and `three` resolves to one copy — which is
why this config needs no `three` alias. Two copies in one bundle would make a mesh from one fail `instanceof`
against the other. One root `npm ci` covers the module; it has no lock file of its own.

**Nothing on a page is fetched from the internet.** Three.js and Alpine are bundled, beercss and the fonts are
copied in, and the footer carries no remote badges. The only external URLs in the rendered HTML are anchors a
person clicks. **An air-gapped install is a normal way to run this**, so a new remote asset is a defect.

## Configuration

**None.** The module reads no config file. `UIModuleOptions.ApiBaseUrl` is the seam for pointing the demo at
another API host; `AddUIModule` sets it to empty and nothing else writes it. Empty means the demo fetches
relative, from the API it ships in.

`Pages/Packing.cshtml.cs` renders it into the demo's `x-data` attribute. The demo site does the same thing
from a build-time value — same mechanism, different producer.

## It makes no server-side HTTP calls

Everything that needs the API runs in the browser and fetches relative. There is no `HttpClient`, no
`IHttpContextAccessor`, and the csproj has one project reference, `Binacle.Net.Kernel`.

**That is why the instance page reads presets over HTTP.** `BinPresetOptions` lives in `Binacle.Net`, the entry
project, which references this module — so a project reference would be a cycle. `_js/instance.js` calls
`GET /api/v4/presets` instead, always relative, because that page describes the instance serving it and never
whichever API `ApiBaseUrl` points at.

## Services

| Service | Lifetime | What it does |
|---|---|---|
| `AppletsService` | Singleton | The applet list — title, icon, copy and Razor Page name for each demo |

`Applet.Page` is a **page name for `asp-page`, not a path**. `AppletPageModel` takes a page's title and copy
from the same list, so the index cards and the page cannot disagree.

**`Models/FeatureSwitch.cs` is a second list, and it has to be.** `FeatureOptions` only records what is
switched **on**, so the instance page cannot show a feature as off without knowing it exists. `FeatureSwitch.All`
names four — Swagger UI, Scalar UI, the health check and the debug endpoint — with a display name and the
setting that turns each one on. A new switch needs a row here or the page never mentions it.

**Two are deliberately missing.** The service module is not advertised and the documentation site has no page
for it. The demo UI is the page you are reading it on.

**Paths are not in that list.** `FeatureOptions.PathFor` carries them, set by whoever switched the feature on —
the health path is configurable, so the module that owns it is the only thing that can know where it ended up.

## Error pages

`UseStatusCodePagesWithReExecute("/error/{0}")` turns a bare status into the error page, and a plain
`try`/`catch` middleware turns an unhandled exception into a bare 500 so the same re-execute renders it.

**Both must sit on `app`, not inside a `UseWhen` branch** — a re-execute inside a branch selects no endpoint
and returns 404 with zero bytes. Measured, not assumed.

**`UseExceptionHandler` cannot do the 500 job.** When its handler writes no body it falls back to
`IProblemDetailsService`, and the browser gets JSON over the page.

**Who gets a page is decided by `ReservedPathOptions`** (`Binacle.Net.Kernel`), not by anything in this module.
Every module declares the paths it serves that must never answer with a web page; the module reads the set per
request and switches `IStatusCodePagesFeature.Enabled` off for those. See `$api/modules` for who declares what.
