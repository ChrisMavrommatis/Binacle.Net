# TODOs

Found across `lib/`, `api/`, `vipaq/`, `shared/`. Grouped by area.

---

## OpenAPI / Schema

- `api/src/Binacle.Net.Kernel/OpenApi/EnumStringsSchemaTransformer.cs:35`
  If the property is required, remove nullable and strip `?` from the name.
  Currently applies `JsonStringNullableEnumConverter` even when not needed.

---

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.

---

## Integration Tests — General

- `api/test/Binacle.Net.IntegrationTests/BinacleApi.cs:34`
  Run integration tests with all modules enabled (currently only core is active).

- `api/test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
  Same — run with all modules enabled.

- `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`
  ServiceModule integration tests should also run with all modules enabled.
