---
description: v3/v4 HTTP integration test conventions — response shape, naming, hardcoded IDs, test data
verified: 2026-06-09
check: validBinId const pattern, preset keys, special bins, and base-class asserts match the integration test source
---

# Integration Test Conventions (v3 / v4)

Cheat sheet for `Binacle.Net.IntegrationTests`. Just the non-obvious rules an agent would
otherwise spend many tool calls rediscovering. For test data format and providers, see
[scenarios.md](scenarios.md).

## The big gotcha: response shape differs by version

- **v3** returns `FitResponse` / `PackResponse` with a `.Data` array — many bin results.
- **v4 fit/pack** returns a single `FitBinResponse` / `PackBinResponse` — no array, no `.Data`.

So in v4: never use `FirstOrDefault` or index into a response array. Assert on the single result
directly (`result.Bin.ID`, `result.Status`, ...). This is the most common source of confusion.

## Hardcoded bin IDs — use a const, never a dynamic lookup

v4 preset routes target one bin by ID (`/api/v4/fit/bin/{preset}/{bin}`), so the test must pin
the exact ID:

```csharp
private const string validBinId = "60x40x10";
```

Do **not** do `CustomProblemsScenarioProvider.GetScenarios().First().Bin.ID`. Dynamic lookup
couples the test to provider ordering and hides which bin is actually under test.

## Test data (hardcode these — don't look them up)

Preset keys — `PresetKeys.cs`:

| Const | Value |
|---|---|
| `PresetKeys.CustomProblems` | `custom-problems` |
| `PresetKeys.BiscoffSuite` | `biscoff-suite` |
| `PresetKeys.SpecialSet` | `special` |

CustomProblems bins (`LxWxH` naming): `60x40x10` (vol 24000), `60x40x20` (48000),
`60x40x30` (72000). Default `validBinId` = `60x40x10`.

SpecialSet bins (configured in `BinacleApi.cs`): `special_bin_1` 60x40x10, `special_bin_2`
60x40x11, `special_bin_3` 60x40x12.

Two different "special" things — don't mix them up:
- **Preset** early-exit tests use the `SpecialSet` preset (`special_bin_1/2/3`).
- **Custom-bin** early-exit tests build an inline bin via `CreateSpecialRequest` (id `special_bin`,
  10x40x60) — not the preset.

Don't use `BiscoffSuite` in behavior tests; it's bulk algorithm data. Both v3 and v4 scenario
tests only run CustomProblems — equal coverage, so missing Bischoff is not a gap.

## Naming: DisplayName must match what the test asserts

- Method: `Post_With<Condition>_Returns_<Status>` (status tests) or
  `Post_With<Condition>_Returns_<Result>` (data tests).
- DisplayName: `$"POST {routePath}. With <Condition>, <Outcome>"`.
- The words must match the real assertion. Don't say "Returns 400" when you assert 422, or
  "With Large Volume" when the test mutates a dimension. Keep fit wording in fit tests and pack
  wording in pack tests. Response property names (`PackedItems`, etc.) are contract names — exempt.

## Don't re-assert what the base class already checks

`FitRequest_Validate` / `PackRequest_Validate` (v4) and `*_ValidateBasedOnParameters` (v3) already
assert: result not null, `Bin` not null, `AlgorithmUsed` not empty, and packed/unpacked presence by
status. Your `additionalValidation` callback should add *new* checks (a specific bin ID, a status,
coordinates) — not repeat the base ones. Read the base before adding asserts:
- v4: `Tests/v4/Abstractions/BehaviourTestsBase.cs`
- v3: `Tests/v3/Abstractions/BehaviourTestsBase.cs`
