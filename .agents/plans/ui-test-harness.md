---
description: A test harness for the UI
---

# A test harness for the UI

**Status:** Built and remeasured 2026-08-22. One item left, and it is a verification: nobody has seen the
numbers arrive in Sonar. Decided 2026-08-09, while treating
Sonar findings: the coverage gate stays red until this exists, rather than being configured away.

The UI used to be the only part of the codebase with **no test harness at all**. Four areas sat at exactly 0%
coverage, every line uncovered - **1571 lines, 22.5% of the whole denominator**, contributing nothing. Overall
coverage was 53%.

**Remeasured 2026-08-22**, from `just coverage all` and the merged report rather than from counting files:

| Area | Lines | Covered | |
|---|---|---|---|
| `api/src/Binacle.Net.UIModule` (C#) | 232 | 216 | **93.1%** |
| `packages/binacle-net-ui/src` | 631 | 440 | **69.7%** |
| `packages/cookies/src` | 48 | 47 | **97.9%** |
| `packages/theme-switcher/src` | 41 | 40 | **97.6%** |

**Overall is 56.6%** - 9513 of 16787 coverable lines, 20 assemblies. The old first row counted 959 lines of
Blazor that no longer exist; what replaced it is a tenth of the size and nearly covered.

**Two caveats on the first row, and both matter to whoever sets a floor.** The figure is hand-written code
only: the assembly reports **35.4%** because two generated namespaces land inside it,
`Microsoft.AspNetCore.OpenApi.Generated` and `System.Runtime.CompilerServices`, both at 0% and neither written
by anyone here. And 216 of those 232 lines are covered by the two new suites together - the unit one alone
does not reach `ModuleDefinition`.

**These are local cobertura numbers, not Sonar's.** Sonar counts coverable lines its own way and has not run
since. Expect the shape to hold and the digits to move.

## Why this is a plan and not a config line

The obvious shortcut is `sonar.coverage.exclusions` over the four paths. That was considered on 2026-08-09 and
**rejected**: it would move the number without changing anything true, and it would hide the gap from the one
place anybody looks at it. The same reasoning already governs Sonar issue findings in this repo - answer them
where a reader can see the answer, never in a config file nobody opens.

So the Sonar quality gate fails its coverage condition on purpose, and will keep failing until there are tests.
Anyone reading a red gate should find this file, not a mystery.

## The rebuild landed, and it dissolved the C# half

**There is almost no C# left to unit-test.** The module is **282 physical lines across ten files**, and 99 of
those are `ModuleDefinition.cs`, which is pipeline wiring - integration territory, not unit. What remains is
`AppletsService` (a hardcoded list), `AppletPageModel`, three small PageModels, and two pure switch
expressions: `IndexModel.SummaryFor` and `ErrorModel.MessageFor`.

**Do not write bUnit tests. There is no Blazor.** Razor Pages, Alpine and one options bag.

**So the shape changed: the work is one TypeScript harness, plus integration cover for behaviour.**

| Part | State |
|---|---|
| The TypeScript packages | **ready.** Unchanged by the rebuild, and the bulk of what is left |
| `Binacle.Net.UIModule` C# | **done 2026-08-22.** `api/test/Binacle.Net.UIModule.UnitTests`, 30 cases |
| The module's behaviour | **integration, and partly already covered** - see below |

**The line above used to read "not worth a unit harness - two switch expressions and a list", and it was
wrong.** What is there is the applet list that every card and page heading is looked up in, four page models,
and the error page. A demo page whose applet name has drifted out of the list throws on construction and the
route 500s; the error page's trace id is meant to appear only in Development. Neither is a switch expression,
and both fail silently. The reason the claim held for a while is that everything in the module is `internal` -
which is one `InternalsVisibleTo` line, and every other module in `api/src/` already had one.

**The behaviour worth protecting is routing, not logic**, and the existing tools already reach it:

- `tooling/smoke/full.hurl` and `quickstart.hurl` assert the four routes, the bundle, the stylesheet, and both
  sides of the error-page rule.
- What had no cover: the `ReservedPathOptions` contract. **A module that maps a path and forgets to declare it
  silently starts rendering HTML for its 404s**, and only the smoke files would catch it, only for `/api`.

  **The unit half landed on 2026-08-22** - `api/test/Binacle.Net.Kernel.UnitTests/Paths/`, 24 cases over
  `Covers` and `AddPrefix`: segment matching so `/apidocs` does not match `/api`, case-insensitivity, an empty
  prefix ignored rather than reserving every path, and the seven prefixes the shipped image declares. It found
  one defect, which is a one-liner in `todos.md`.

  **The other half is still open and it is integration, not unit:** one test asserting every mapped endpoint's
  first segment is either a UIModule page or a declared prefix. It needs a host booted with the modules on,
  which is `api/integration-test-additions`, so it is written there rather than twice.

**The coverage gate still does not move on the TypeScript half alone.** That is expected, not a failure of this
plan - the gate is honest about the UI being untested and stays red until the UI is tested.

## The TypeScript half - done 2026-08-22

**All three packages have suites.** jsdom was the answer to the DOM question, and every suite uses it.

| Package | Leaf | Suites / tests |
|---|---|---|
| `binacle-net-ui` | `packages-net-ui-unit` | 20 / 273, 69.73% of lines |
| `cookies` | `packages-cookies-unit` | 3 / 31 |
| `theme-switcher` | `packages-theme-switcher-unit` | 1 / 21 |

`cookies` and `theme-switcher` were ported from JavaScript to TypeScript in the same pass - the plan used to
call them "the TypeScript packages" and they were not.

**`binacle-net-ui`'s uncovered third is the Three.js half**, `core/packingVisualizer.ts` and the scene helpers
in `utils/`. They need a WebGL context, so a test there can only assert that a call happened. **They stay in
the denominator.** Excluding them would be the same act as the `sonar.coverage.exclusions` this plan rejected,
one layer down - a jest config was written with exactly that exclusion and removed for that reason. The only
exclusion is `.d.ts`, which carries no runtime code.

## Decisions, both answered on 2026-08-22

- **Is `binacle-net-ui` worth unit-testing, or is it integration-only?** Worth it, and it was not mostly
  wiring. The randomizer has a contract that can be stated and checked - a roll can never produce a box the
  items do not fit - and `packingDemo.ts` reached 100% of lines without a browser, because each component
  factory is a plain object a test can call directly. The thin-wrapper failure this bullet feared did not
  happen; four real defects came out of the pass instead, all in `todos.md`.
- **What coverage number is honest to aim for?** Still not a target, deliberately. What the pass costs is now
  known - 69.73% of `binacle-net-ui`, and the remaining third is the WebGL half. Anyone setting a floor should
  set it from a settled run, which is the open item below.

**One decision the rebuild removed.** "Where does the demo page get tested - bUnit or a browser?" only existed
because the demo spanned two stacks, a Blazor page driving a TS visualizer through JS interop. **There is one
stack now and no seam**, so the question is closed: the demo is tested where its code lives, in TypeScript.

## Done when

- ~~The TypeScript harness exists and runs as a test leaf, like every other suite.~~ Four leaves, and each has
  a step in `shared-test-suite.yml`.
- Its coverage reaches Sonar the same way the existing suites' does, one flat file per suite. **Unverified** -
  the leaves write lcov through `_jest_test`, but no Sonar run has happened since, so nobody has seen the
  numbers arrive.
- ~~The denominator has been remeasured against the rebuilt module, so the table at the top is true again.~~
  Done - the table at the top is measured, and the deleted Blazor row is gone.
- ~~The three TypeScript areas are no longer at zero~~, and the coverage condition on the quality gate is
  failing for a reason somebody chose rather than because the UI was never tested.
