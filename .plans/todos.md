# TODOs

Found across `lib/`, `api/`, `vipaq/`, `shared/`. Grouped by area.

---

## OpenAPI / Schema

- `api/src/Binacle.Net.Kernel/OpenApi/EnumStringsSchemaTransformer.cs:35`
  If the property is required, remove nullable and strip `?` from the name.
  Currently applies `JsonStringNullableEnumConverter` even when not needed.

---

## ServiceModule

- `src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.

---

## Integration Tests — General

- `test/Binacle.Net.IntegrationTests/BinacleApi.cs:34`
  Run integration tests with all modules enabled (currently only core is active).

- `test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
  Same — run with all modules enabled.

- `test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`
  ServiceModule integration tests should also run with all modules enabled.

---

## Integration Tests — v4 (needs review)

These test files are all marked `// TODO: Review` at the top.
They may have gaps in coverage, incorrect assertions, or need restructuring.

- `test/Binacle.Net.IntegrationTests/Tests/v4/FitCustomBinBehavior.cs:6`
- `test/Binacle.Net.IntegrationTests/Tests/v4/FitCustomBinScenario.cs:10`
- `test/Binacle.Net.IntegrationTests/Tests/v4/FitPresetBinBehavior.cs:10`
- `test/Binacle.Net.IntegrationTests/Tests/v4/ListPresetsBehavior.cs:6`
- `test/Binacle.Net.IntegrationTests/Tests/v4/PackCustomBinBehavior.cs:6`
- `test/Binacle.Net.IntegrationTests/Tests/v4/PackCustomBinScenario.cs:10`
- `test/Binacle.Net.IntegrationTests/Tests/v4/PackPresetBinBehavior.cs:10`
- `test/Binacle.Net.IntegrationTests/Tests/v4/PackSmallestBinBehavior.cs:6`
