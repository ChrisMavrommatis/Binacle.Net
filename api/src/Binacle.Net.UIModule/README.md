# UI module

The demo UI that ships inside the API - a packing demo and a ViPaq decoder, served as Razor Pages at `/`.
Optional: it only loads when the `UI_MODULE` feature is on.

```bash
just serve api U                 # from the repo root - core plus this module
just serve api All               # everything
```

`ModuleDefinition.cs` is the whole switch. `Program.cs` calls `AddUIModule()` and `UseUIModule()` behind the
feature check, so with the flag off none of this is registered and nothing here runs.

## 📂 What is in it

| Folder | What it is |
|---|---|
| `Pages/` | Every page - `Index`, `Packing`, `Vipaq`, `Instance`, `Error` - and the chrome under `Pages/Shared/` |
| `Models/`, `Services/` | The applet list the cards and the navigation are built from, and the instance page's switch list |
| `_sass/` | The stylesheet source, compiled to `wwwroot/css/main.css` |
| `_js/` | The four webpack entries, bundled into `wwwroot/js/` |
| `wwwroot/` | Generated. Never edit anything in here |

| Route | What it serves |
|---|---|
| `/` | Three cards, one per applet. The whole card is the link |
| `/packing` | The packing demo |
| `/vipaq` | The ViPaq decoder |
| `/instance` | Version, what is switched on, and the presets this instance loaded |
| `/error/{errorCode?}` | The error page |

## 🛠️ Both demos come from the shared package

The packing form, the 3D visualizer and the decoder are TypeScript, in
[`packages/binacle-net-ui`](../../../packages/binacle-net-ui). The marketing site's demos are built from the
same source, so a fix lands on both. Everything around them - the pages, the navigation, the copy - is this
module's alone and is free to differ.

They run in the browser and call the API over relative URLs, so this module makes no server-side HTTP calls
and needs no configuration file. The instance page reads its preset list the same way, from
`GET /api/v4/presets` - the preset options live in the entry project, which references this one, so there is no
project reference to take.

## ⚠️ `wwwroot/` is generated, and nothing in it is yours

It is rebuilt from three places and gitignored whole:

- `just assets` copies [`assets/`](../../../assets) into `lib/`, `media/` and the icons at the root.
- `npm run build:css` compiles `_sass/main.scss` into `css/main.css`.
- `npm run build:js` bundles `_js/` into `js/`.

`just build publish` runs all three before `dotnet publish`, because static web assets are collected at
publish time. **Nothing fails when they are missing** - the image just ships pages with no styling and demos
that do nothing. `just serve api U` runs the two watches beside `dotnet run`.

## ⚙️ Who gets the error page

**Everything that is not a reserved path.** Every module declares the paths it serves that must not answer
with a web page - `/api`, `/openapi`, `/swagger` and `/scalar` from the API, `/_debug` and the health path
from the diagnostics module, `/_content` from here. Those come back as a bare status or problem-details JSON,
whether or not the feature serving them is switched on. Everything else gets the error page, for a 404 and
for an unhandled exception alike.

**Map a path, declare it.** `ReservedPathOptions` in the kernel is the one place, and a running instance
lists what it reserved under `ReservedPaths` in `/_health` and `[reservedPaths]` in `/_debug`. A fixed
list inside this module could never be right: the health path is configurable.
