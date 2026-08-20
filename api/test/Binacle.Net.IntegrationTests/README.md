# Integration tests

HTTP tests for the v3 and v4 endpoints. They start the API in-process with `WebApplicationFactory` and send
real requests through it - no mocks, no running server, and no ports.

```bash
just test api-core-integration   # from the repo root
```

## 📂 One folder per endpoint

`Tests/` mirrors the endpoint layout of the API, so a test is found the same way the endpoint is:

```
api/src/Binacle.Net/v4/Endpoints/Fit/CustomBin.cs  ->  Tests/v4/Endpoints/Fit/CustomBin/
```

Each of those folders holds the same **two** files, and the split is the point:

| File | What it covers | Marked with |
|---|---|---|
| `…Behavior.cs` | How the endpoint answers - status codes, validation, bad input | `[Trait("Behavioral Tests", …)]` |
| `…Scenario.cs` | What the packer actually returns, run against the shared fixture corpus | `[Trait("Scenario Tests", …)]` |

Behaviour tests build their request inline and assert the response code. Scenario tests take their cases from
the shared test kernel in [`shared/test/Binacle.TestsKernel`](../../../shared/test/Binacle.TestsKernel) as
xUnit `[MemberData]`, so the API and the lib are graded against the same problems.

The shared behaviour assertions - `Request_Returns_200Ok`, `Request_Returns_422UnprocessableContent` and the
rest - live in `Tests/<version>/Abstractions/BehaviourTestsBase.cs`, one per version.

## 🧩 Two fixtures

Both are assembly fixtures, so the app starts once for the whole run.

| Fixture | Why |
|---|---|
| `BinacleApi` | The normal one. Presets loaded, used by almost everything |
| `BinacleApiWithoutPresets` | The same app with no presets, to prove the endpoints answer sensibly when there are none |

`PresetKeys.cs` holds the preset names the tests ask for, so a renamed preset breaks in one place.

## ⚠️ v3 is frozen

`Tests/v3/` covers a version that does not change. If a v3 test starts failing, something in shared code moved
under it - that is the bug, not the test.
