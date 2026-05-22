---
description: What presets are, where they're configured, how route params map to bins, and how to add one for tests
verified: 2026-05-23
---

# Presets

A preset is a named collection of bins defined in config.
Preset endpoints let callers pick a bin by name instead of sending dimensions in the request body.

See [configuration.md](configuration.md) for where config files live and how to override them.

## Config

Presets live in `api/src/Binacle.Net/Config_Files/Presets.json`, loaded into `BinPresetOptions`
(`api/src/Binacle.Net/Configuration/BinPresetOptions.cs`).
This file is **required** (`Optional: false`) — the app fails to start if it is missing.

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

**v4** preset endpoints use `{preset}/{bin}` — pick one specific bin by name:

- `{preset}` — the top-level key in `Presets` (e.g., `rectangular-cuboids`)
- `{bin}` — the `ID` of a bin within that preset (e.g., `Small`)

Example: `POST /api/v4/fit/bin/rectangular-cuboids/Small`

The endpoint looks this up via `BinPresetOptions.TryGetPresetBin(preset, bin, out binOption)`.
Returns `404` if the preset or bin name doesn't exist.

**v3** preset endpoints use only `{preset}` — no `{bin}`. They run all bins in the preset and return
one result per bin. Example: `POST /api/v3/fit/by-preset/rectangular-cuboids`

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
