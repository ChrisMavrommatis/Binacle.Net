---
id: api/tests
description: api/test integration tests — v3/v4 HTTP conventions, validBinId, preset keys, special bins, base-class asserts, and test host config
verified: 2026-07-15
check: validBinId, PresetKeys, special bins, and base-class asserts match api/test/ source
also_update:
  - shared
---

# API Tests

Two projects under `api/test/`:

| Project | Covers | Run |
|---|---|---|
| `Binacle.Net.IntegrationTests` | v3 + v4 HTTP endpoints (fit, pack, presets) | `./config/tests.api.sh core` |
| `Binacle.Net.ServiceModule.IntegrationTests` | auth token, admin account/subscription (ServiceModule on) | `./config/tests.api.sh service` |

Both are xUnit over `WebApplicationFactory<IApiMarker>`, registered with `[assembly: AssemblyFixture(...)]`.
The core fixture uses `UseEnvironment("Test")`, a camelCase `JsonSerializerOptions` with `JsonStringEnumConverter`,
and a null logger factory.

## Constants

- `validBinId = "60x40x10"` — declared in **v4 only**: `Tests/v4/FitPresetBinBehavior.cs` and
  `PackPresetBinBehavior.cs`. v3 preset tests don't substitute a bin id into the route.
- `PresetKeys` (`PresetKeys.cs`): `BiscoffSuite = "biscoff-suite"` (note the misspelling, in both the C# symbol
  and the value), `CustomProblems = "custom-problems"`, `SpecialSet = "special"`.

## Special bins

The registered `special` preset has three bins, 60×40 footprint, heights 10/11/12:
`special_bin_1` 60×40×10, `special_bin_2` 60×40×11, `special_bin_3` 60×40×12.
`ListPresets` tests assert the preset contains these three.

`CreateSpecialRequest` request bodies differ by version (because the request shapes differ):

- **v3** sends a `Bins` array of three: `special_bin_1` 10×40×60, `_2` 11×40×60, `_3` 12×40×60.
- **v4** sends a single `Bin`: `{ ID = "special_bin", 10×40×60 }`.
- v4 `pack/smallest-bin` instead takes a `Bins` array (`bin_small` 10×40×60 / `bin_medium` 20×40×60 /
  `bin_large` 30×40×60) — the multi-bin "pick smallest" path.

Items in both: `special_box_1` 8×40×60 + `box_1` 5×5×5.

## Base-class asserts (v3 vs v4)

Response shapes differ: **v3** wraps results in a `.Data` collection (multi-bin); **v4** returns one bin result
directly. Both base classes (`Tests/v{3,4}/Abstractions/BehaviourTestsBase.cs`) provide
`Request_Returns_200Ok / _422UnprocessableContent / _404NotFound`.

| | v3 (`*_ValidateBasedOnParameters`) | v4 (`*_Validate`) |
|---|---|---|
| Fit | 200; `FitResponse` not null; `.Data` not empty; each `Bin` not null | 200; `Bin` not null; `AlgorithmUsed` not empty; `PackedItems` and `UnpackedItems` only `ShouldNotBeNull()` |
| Pack | 200; `.Data` not empty; per bin: if `FullyPacked` → packed not empty + unpacked empty, else unpacked not empty | 200; `Bin` not null; `AlgorithmUsed` not empty; if `FullyPacked` → packed not empty + unpacked empty, else unpacked not empty |

**Asymmetry:** pack verifies the packed/unpacked split keyed on `FullyPacked`; fit only checks presence, not the
emptiness split.

## Test host config

- `BinacleApi.cs` — `ConfigureTestServices` clears `BinPresetOptions.Presets`, then registers `custom-problems`
  (bins from `CustomProblemsScenarioProvider`), `biscoff-suite` (from `BischoffSuiteScenarioProvider`), and
  `special` (the three special bins). Presets come from the shared kernel — see shared (`$shared`).
  Runs with default modules (ServiceModule off). `// TODO: Run the tests with all modules enabled` (line 34).
- `BinacleApiWithoutPresets.cs` — same shape but only clears presets (no registration); tests the no-presets path.
- `Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs` — `IAsyncLifetime`; enables ServiceModule via
  in-memory config (`SERVICE_MODULE=true`, an `AuthToken=NoLimiter::0` rate-limit rule, an Azurite
  `AzureStorage` connection string, JWT issuer/audience/secret "ForTestsOnly"). `InitializeAsync` seeds an admin
  (`DefaultAdminAccount`) and a known user; `NonExistentId = EF81C267-A003-44B8-AD89-4B48661C4AA5` is hard-coded.
  Same all-modules TODO (line 44).
