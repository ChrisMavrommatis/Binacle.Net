---
description: A test harness for the UI
---

# A test harness for the UI

**The harness is built.** Four leaves run it, each with a step in `shared-test-suite.yml`:

| Suite | Leaf |
|---|---|
| `packages/binacle-net-ui` | `just test packages-net-ui-unit` |
| `packages/cookies` | `just test packages-cookies-unit` |
| `packages/theme-switcher` | `just test packages-theme-switcher-unit` |
| `api/test/Binacle.Net.UIModule.UnitTests` | `just test api-ui-unit` |

`api/test/Binacle.Net.UIModule.IntegrationTests` covers what needs a booted host, and
`api/test/Binacle.Net.Kernel.UnitTests/Paths/` covers the reserved-path matching underneath it.

## What is left

**One item, and it is a verification.** The leaves write lcov through `_jest_test` and Visual Studio xml
through `_dotnet_test`, the same way every other suite does - but no Sonar run has happened since they
landed, so **nobody has seen the numbers arrive there**. Until someone dispatches `sonar-analysis.yml` and
reads the result, "coverage reaches Sonar" is an assumption.

Two things to check when that run happens, because both would look like success:

- **Four new flat files, one per leaf.** A leaf whose report never got imported shows up as the same red
  condition as a leaf with no tests.
- **The UIModule assembly reads far lower than its hand-written code.** Two generated namespaces land inside
  it at 0%. That is expected; it is written down in the CI/CD design record with the rest of the numbers.

## Why this was a plan and not a config line

The obvious shortcut is `sonar.coverage.exclusions` over the untested paths. That was considered on
2026-08-09 and **rejected**: it moves the number without changing anything true, and it hides the gap from
the one place anybody looks at it. The same reasoning governs Sonar issue findings in this repo - answer them
where a reader can see the answer, never in a config file nobody opens.

**The same rule applies one layer down.** A jest config was written with the WebGL half of `binacle-net-ui`
excluded, and removed for exactly this reason. The only exclusion anywhere is `.d.ts`, which carries no
runtime code, and it lives in the root jest config because per-project copies are ignored in multi-project
mode.

## What will bite whoever picks this up

- **The coverage numbers are recorded in the CI/CD design record, not here.** They were being re-derived in
  four files and three had gone stale. Do not copy them back into this file.
- **`binacle-net-ui`'s uncovered third is the Three.js half** and is meant to stay in the denominator. If the
  Sonar run makes that look like a problem to solve, it is not.
- **Do not write bUnit tests. There is no Blazor.** The module is Razor Pages, Alpine and one options bag.
- **The C# unit suite reaches internal types**, because Razor generates internal page classes and the module
  follows them. It works through `InternalsVisibleTo`, the same as every other module in `api/src/`.

## Done when

- A Sonar run has happened with the four leaves in it, and the coverage condition is failing for a number
  somebody chose to look at rather than because the UI was never tested.
