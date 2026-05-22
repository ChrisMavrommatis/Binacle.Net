---
description: Shared test infrastructure — scenario data, compact format, providers, and fixtures
verified: 2026-05-23
---

# Test Scenarios

## Shared Library — `Binacle.TestsKernel`

Not an executable. Used by four projects:
- `Binacle.Lib.UnitTests`
- `Binacle.Net.IntegrationTests`
- `Binacle.Lib.Benchmarks`
- `Binacle.Lib.PerformanceTests`

## Scenario Suites

Scenario data is embedded JSON. Two suites:

- **Bischoff Suite** — from the OR-Library benchmark (`orlib_thpack1–5`); tests algorithms against known problems
- **Custom Problems** — hand-crafted scenarios (`simple.json`, `baseline.json`, `complex.json`)

## Compact String Format

Used in JSON scenario files to keep data concise:

| Pattern | Example |
|---|---|
| Dimensions: `"LxWxH"` | `"60x40x10"` |
| Item with quantity: `"LxWxH-Qty"` | `"25x25x10-2"` |
| Metrics: `"itemsVolume binVolume itemCount volumePct"` | `"12500 24000 2 52.1"` |
| Result: `"FittingResult PackingResult"` | `"FullyPacked FullyPacked"` |

## Providers

Give scenario names to xUnit via `[MemberData]`:

- `BischoffSuiteScenarioProvider`
- `CustomProblemsScenarioProvider`
- `AllScenariosProvider`

## AlgorithmFactories

Factories for creating every algorithm version (FFD v1/v2, WFD v1/v2, BFD v1/v2).
`CommonTestingFixture` runs tests against all of them.

## Result Selection Data

Lives under `ResultSelection/Data/`.
Has providers for `BestAlgorithm`, `BestBin`, and `SmallestBin` scenarios.
