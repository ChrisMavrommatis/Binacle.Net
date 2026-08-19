---
id: api/presets
description: What presets are, where they're configured, how route params map to bins, and how to add one for tests
verified: 2026-08-19
check: Every {preset} and {preset}/{bin} route in v3/Endpoints and v4/Endpoints is accounted for; the flags, lookup methods and validator rules match BinPresetOptions.cs; the shipped preset tables match Presets.json
also_update:
  - api/configuration
paths:
  - "api/src/Binacle.Net/Configuration/BinPresetOptions.cs"
  - "api/src/Binacle.Net/Config_Files/Presets.json"

---

# Presets

A preset is a named collection of bins defined in config.
Preset endpoints let callers pick a bin by name instead of sending dimensions in the request body.

See `$api/configuration` for where config files live and how to override them.

## Config

Presets live in `api/src/Binacle.Net/Config_Files/Presets.json`, loaded into `BinPresetOptions`
(`api/src/Binacle.Net/Configuration/BinPresetOptions.cs`).
This file is **required** (`Optional: false`) — the app fails to start if it is missing. It is the one config
file with **no environment override**: `GetEnvironmentFilePath` returns `null`, so a `Presets.Development.json`
is never read.

Structure:

```json
{
  "PresetOptions": {
    "Presets": {
      "<preset-name>": {
        "Bins": [
          { "ID": "<bin-id>", "Length": 60, "Width": 40, "Height": 10 }
        ]
      }
    }
  }
}
```

Built-in presets: `rectangular-cuboids`, `perfect-cubes`, `sample`. Each has `Small`, `Medium`, `Large` bins.

## Route Params

Two parameters, and which of them a route takes says which lookup it does:

- `{preset}` — the top-level key in `Presets` (e.g. `rectangular-cuboids`)
- `{bin}` — the `ID` of a bin within that preset (e.g. `Small`)

| Route shape | Lookup | Which routes |
|---|---|---|
| `{preset}/{bin}` | `TryGetPresetBin(preset, bin, out binOption)` — one named bin | v4 `fit/bin`, v4 `pack/bin` |
| `{preset}` | `TryGetPreset(preset, out presetOption)` — every bin in the preset | v4 `compare-bins`, `smallest-bin`, `pack/best-bin`, `GET presets/{preset}`; all v3 `by-preset` routes |

Examples: `POST /api/v4/fit/bin/rectangular-cuboids/Small`, `POST /api/v4/fit/smallest-bin/rectangular-cuboids`,
`POST /api/v3/fit/by-preset/rectangular-cuboids`. The full route list is in `$api/v4` and `$api/v3`.

Either lookup returns `404` when the name doesn't exist. **Both names are matched case-sensitively** — the
preset through a plain `Dictionary` lookup, the bin through `b.ID == bin` — so `.../rectangular-cuboids/small`
is a `404` while `.../Small` is not.

## Presets.json is validated at startup

`BinPresetOptionsOptionsValidator` refuses to boot on any of these, naming the preset and the bin in the
message:

- a preset with no bins,
- two bins in one preset sharing an `ID` — the route `{preset}/{bin}` would reach only the first, and the
  second would be silently dead,
- a bin with an empty `ID`, or a `Length`, `Width` or `Height` that is not greater than 0.

## Lookup is cached

`BinPresetOptions` caches resolved `{preset}:{bin}` lookups in a `ConcurrentDictionary` after the first access.
Presets.json reloads on file change (`ReloadOnChange: true`), but the cache is not automatically cleared — a restart
is needed for cache to reflect file changes in long-running processes.

## Default Presets

Three presets ship with the default `Presets.json`. All dimensions are integers (centimetres assumed,
but any consistent unit works).

**`rectangular-cuboids`**

| ID | Length | Width | Height |
|---|---|---|---|
| Small | 60 | 40 | 10 |
| Medium | 60 | 40 | 20 |
| Large | 60 | 40 | 30 |

**`perfect-cubes`**

| ID | Length | Width | Height |
|---|---|---|---|
| Small | 10 | 10 | 10 |
| Medium | 20 | 20 | 20 |
| Large | 30 | 30 | 30 |

**`sample`** (approximates common parcel sizes)

| ID | Length | Width | Height |
|---|---|---|---|
| Small | 62 | 45 | 8 |
| Medium | 62 | 45 | 17 |
| Large | 62 | 45 | 36 |

## Adding a preset for tests

Integration tests do **not** use a separate `Presets.json` file. Presets are configured in code inside
`BinacleApi.ConfigureWebHost` — the defaults are cleared and test presets are added programmatically.

To add a preset for testing:
1. Add a constant to `api/test/Binacle.Net.IntegrationTests/PresetKeys.cs`
2. In `BinacleApi.ConfigureWebHost`, add an entry via `options.Presets.Add(PresetKeys.YourKey, ...)`
3. Reference it in tests via `PresetKeys.YourKey`

`BinacleApiWithoutPresets` is a second fixture in the same project that clears the presets and adds none — it
is how the empty-preset responses are tested.
