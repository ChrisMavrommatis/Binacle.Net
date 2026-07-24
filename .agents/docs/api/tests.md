---
id: api/tests
description: api/test integration tests — layout, v3/v4 HTTP conventions, validBinId, preset keys, special bins, base-class asserts, and test host config
verified: 2026-07-24
check: Test folders mirror api/src/Binacle.Net/v{3,4}/Endpoints/; validBinId, PresetKeys, special bins, and base-class asserts match api/test/ source
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

> `Binacle.Net.ServiceModule.IntegrationTests` picks its database backend from **`BINACLE_TEST_INFRA`** —
> `AzureStorage`, `Postgres`, or `Sqlite`. Unset, it **falls back to SQLite**, so a bare `dotnet test` runs with
> no external service. Only the first two need something up (Azurite on `127.0.0.1:10002`, Postgres on `5432`);
> pick one with `config/tests.api.sh service [Sqlite|Postgres|AzureStorage]`.

## Layout — one folder per endpoint

`Tests/v{3,4}/Endpoints/` mirrors `api/src/Binacle.Net/v{3,4}/Endpoints/` exactly: a folder per endpoint class,
named after it, holding that endpoint's tests.

```
api/src/Binacle.Net/v4/Endpoints/Fit/CustomBin.cs
api/test/…/Tests/v4/Endpoints/Fit/CustomBin/FitCustomBinBehavior.cs
                                            /FitCustomBinScenario.cs
```

Each endpoint has up to two files. **Behavior** covers status codes, validation, and the endpoint's own rules.
**Scenario** replays the shared fixture cases (`$shared`) through the route and asserts the algorithm's answer.
The two `Presets` endpoints have a behavior file only — they run no algorithm, so there is nothing for a
scenario to assert.

**Namespaces track the folders**, with `Tests/` elided:
`Tests/v4/Endpoints/Fit/CustomBin/` → `Binacle.Net.IntegrationTests.v4.Endpoints.Fit.CustomBin`. `Abstractions/`
and `ExtensionMethods/` sit outside `Endpoints/`, at the version root, and follow the same rule.

Two consequences worth knowing:

- `BinacleApi` and `PresetKeys` live in the root namespace, which is an ancestor of every test namespace, so
  they still resolve with no `using`.
- `ScenarioResultExtensions` does **not** — it sits in `v{3,4}.ExtensionMethods`, a sibling. Every scenario
  test needs `using Binacle.Net.IntegrationTests.v{3,4}.ExtensionMethods;` for `EvaluateResult`.

Scenario tests for the multi-bin endpoints send the scenario's single bin as a one-element list, so selection
cannot change the answer and the result must match the single-bin endpoint's. The exceptions are the preset
selecting endpoints (`pack/smallest-bin/{preset}`, `pack/best-bin/{preset}`, `fit/smallest-bin/{preset}`), whose bins
come from config and cannot be reduced to one: they assert the selection invariant instead — if the scenario's
own bin packs fully, the endpoint must return a fully packed result, and for smallest, in a bin no larger.

## Constants

- `validBinId = "60x40x10"` — declared in **v4 only**: `Tests/v4/Endpoints/Fit/PresetBin/FitPresetBinBehavior.cs`
  and `Tests/v4/Endpoints/Pack/PresetBin/PackPresetBinBehavior.cs`. v3 preset tests don't substitute a bin id
  into the route.
- `PresetKeys` (`PresetKeys.cs`): `BiscoffSuite = "biscoff-suite"` (note the misspelling, in both the C# symbol
  and the value), `CustomProblems = "custom-problems"`, `SpecialSet = "special"`.

## Preset bins come from the scenario providers

The `custom-problems` and `biscoff-suite` presets are **not written down anywhere** — `BinacleApi` registers
whatever bins the scenarios use. That set is owned by the scenario data (`$shared`) and grows whenever a
scenario introduces a new bin, so the providers answer for it:

| Call | Gives |
|---|---|
| `CustomProblemsScenarioProvider.GetDistinctBins()` | The bins, one per ID, in the order scenarios introduce them |
| `CustomProblemsScenarioProvider.GetDistinctBinIds()` | Just the IDs — for asserting a preset's contents |
| `CustomProblemsScenarioProvider.GetSmallestBin()` | The least roomy bin; an item that fits it fits them all |
| `BischoffSuiteScenarioProvider.GetDistinctBins()` | Same, for `biscoff-suite` |

`BinacleApi` builds both presets from `GetDistinctBins()`, so a test asserting on a preset reads the same
source it was registered from and the two cannot drift.

**Never write the bin list into a test.** `custom-problems` holds more than the three `60x40x…` bins —
`600x400x300`, `50x50x50`, and others — which is easy to get wrong.

## Special bins

The registered `special` preset has three bins, 60×40 footprint, heights 10/11/12:
`special_bin_1` 60×40×10, `special_bin_2` 60×40×11, `special_bin_3` 60×40×12.
`ListPresets` tests assert the preset contains these three.

`CreateSpecialRequest` request bodies differ by version (because the request shapes differ):

- **v3** sends a `Bins` array of three: `special_bin_1` 10×40×60, `_2` 11×40×60, `_3` 12×40×60.
- **v4** sends a single `Bin`: `{ ID = "special_bin", 10×40×60 }`.
- v4's custom multi-bin endpoints instead take a `Bins` array (`bin_small` 10×40×60 / `bin_medium` 20×40×60 /
  `bin_large` 30×40×60).

Items in both: `special_box_1` 8×40×60 + `box_1` 5×5×5.

## Base-class asserts (v3 vs v4)

Response shapes differ: **v3** wraps results in a `.Data` collection (multi-bin); **v4** returns one bin result
directly, or a `Results` list from the compare endpoints. Both base classes
(`Tests/v{3,4}/Abstractions/BehaviourTestsBase.cs`) provide
`Request_Returns_200Ok / _422UnprocessableContent / _404NotFound`.

| | v3 (`*_ValidateBasedOnParameters`) | v4 (`*_Validate`) |
|---|---|---|
| Fit | 200; `FitResponse` not null; `.Data` not empty; each `Bin` not null | 200; `Bin` not null; `AlgorithmUsed` not empty; `PackedItems` and `UnpackedItems` only `ShouldNotBeNull()` |
| Pack | 200; `.Data` not empty; per bin: if `FullyPacked` → packed not empty + unpacked empty, else unpacked not empty | 200; `Bin` not null; `AlgorithmUsed` not empty; if `FullyPacked` → packed not empty + unpacked empty, else unpacked not empty |

**Asymmetry:** pack verifies the packed/unpacked split keyed on `FullyPacked`; fit only checks presence, not the
emptiness split.

v4 adds `FitCompareRequest_Validate` / `PackCompareRequest_Validate` for the compare endpoints: 200, `Results`
not empty, then the same per-entry asserts across every result.

## Test host config

- `BinacleApi.cs` — `ConfigureTestServices` clears `BinPresetOptions.Presets`, then registers `custom-problems`
  (bins from `CustomProblemsScenarioProvider`), `biscoff-suite` (from `BischoffSuiteScenarioProvider`), and
  `special` (the three special bins). Presets come from the shared kernel — see shared (`$shared`).
  Runs with default modules (ServiceModule off). `// TODO: Run the tests with all modules enabled` (line 34).
- `BinacleApiWithoutPresets.cs` — same shape but only clears presets (no registration); tests the no-presets path.
- `Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs` — `IAsyncLifetime`; enables ServiceModule via
  in-memory config (`SERVICE_MODULE=true`, an `AuthToken=NoLimiter::0` rate-limit rule, a connection string
  chosen by `ResolveTestInfrastructure()` from `BINACLE_TEST_INFRA`, JWT issuer and audience `"ForTestsOnly"`
  with a separate 70-plus-character `TokenSecret`). `InitializeAsync` seeds an admin
  (`DefaultAdminAccount`) and a known user; `NonExistentId = EF81C267-A003-44B8-AD89-4B48661C4AA5` is hard-coded.
  Same all-modules TODO (line 44).
