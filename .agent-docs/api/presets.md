---
description: What presets are, where they're configured, how route params map to bins, and how to add one for tests
---

# Presets

A preset is a named collection of bins defined in config.
Preset endpoints let callers pick a bin by name instead of sending dimensions in the request body.

## Config

Presets live in `src/Binacle.Net/Config_Files/Presets.json`, loaded into `BinPresetOptions`
(`src/Binacle.Net/Configuration/BinPresetOptions.cs`).

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

Preset endpoints use `{preset}/{bin}` in the URL path.

- `{preset}` — the top-level key in `Presets` (e.g., `rectangular-cuboids`)
- `{bin}` — the `ID` of a bin within that preset (e.g., `Small`)

Example: `POST /api/v4/fit/bin/rectangular-cuboids/Small`

The endpoint looks this up via `BinPresetOptions.TryGetPresetBin(preset, bin, out binOption)`.
Returns `404` if the preset or bin name doesn't exist.

## Lookup is cached

`BinPresetOptions` caches resolved `{preset}:{bin}` lookups in a `ConcurrentDictionary` after the first access.
Presets.json reloads on file change (`ReloadOnChange: true`), but the cache is not automatically cleared — a restart
is needed for cache to reflect file changes in long-running processes.

## Adding a preset for tests

Integration tests use a separate `Presets.json` at `test/Binacle.Net.IntegrationTests/Config_Files/Presets.json`
(same format). Preset name constants live in `test/Binacle.Net.IntegrationTests/PresetKeys.cs`.

To add a preset for testing:
1. Add the entry to the test `Presets.json`
2. Add a constant to `PresetKeys.cs`
3. Reference it in the test via `PresetKeys.YourPreset`
