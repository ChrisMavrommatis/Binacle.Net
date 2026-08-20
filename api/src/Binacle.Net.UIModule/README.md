# UI module

The interactive demo that ships inside the API - a Blazor Server app serving the packing demo and the ViPaq
protocol decoder at `/`. Optional: it only loads when the `UI_MODULE` feature is on.

```bash
just serve api U                 # from the repo root - core plus this module
just serve api All               # everything
```

`ModuleDefinition.cs` is the whole switch. `Program.cs` calls `AddUIModule()` and `UseUIModule()` behind the
feature check, so with the flag off none of this is registered and nothing here runs.

## 📂 What is in it

| Folder | What it is |
|---|---|
| `Components/Pages/` | The pages - `Home`, `PackingDemo`, `ProtocolDecoder`, `Error` |
| `Components/Features/` | The two big pieces - `PackingVisualizer` and `ErrorsDialog` |
| `Components/Layout/`, `Components/Shared/` | The chrome and the shared form controls |
| `ViewModels/` | What a page binds to - one per page, plus bin, item, algorithm, errors |
| `Models/` | Internal types - applet, bin, item, coordinates, theme, packing result |
| `ApiModels/` | The request and response shapes for the API call this app makes |
| `Services/` | Applets, visualizer, sample data, theme, local storage, messaging |
| `wwwroot/` | Everything the browser gets - see below |

## 🌐 It calls the API over HTTP

The demo is a client of the same API it ships in: it configures a named `BinacleApi` `HttpClient` and posts a
pack request like any other caller. It does not reach into the packing engine directly.

`wwwroot/data/sample_data.json` is what the demo loads when you press the sample button.

## ⚠️ `wwwroot/` is hand-maintained, and it is a second copy

Nothing builds it, and no repo tooling touches it:

- **`js/`** - `PackingVisualizer.js`, `cookies.js` and `themeswitcher.js` are hand-written here. The website
  does the same three jobs from the shared packages in [`packages/`](../../../packages), and the two are not
  connected. A fix in one is not a fix in the other.
- **`vendor/`** - its own copy of beercss and material-dynamic-colors, currently beercss `3.10.8` while
  repo-root [`assets/`](../../../assets) is on `3.11.11`. `just assets` copies into the two sites and **never**
  here.

The module ships self-contained inside the image, which is why the copies exist. Treat them as forks and
expect to change things twice.
