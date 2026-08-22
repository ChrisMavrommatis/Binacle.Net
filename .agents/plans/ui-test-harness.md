---
description: A test harness for the UI
---

# A test harness for the UI

**Status:** Not started. Decided 2026-08-09, while treating Sonar findings: the coverage gate stays red until
this exists, rather than being configured away.

The UI is the only part of the codebase with **no test harness at all**. Four areas sat at exactly 0% coverage,
every line uncovered:

| Area | Lines to cover | Covered |
|---|---|---|
| `api/src/Binacle.Net.UIModule` (Blazor components, since deleted) | 959 | 0 |
| `packages/binacle-net-ui/src` (TS) | 533 | 0 |
| `packages/cookies/src` | 40 | 0 |
| `packages/theme-switcher/src` | 39 | 0 |

That was **1571 lines, 22.5% of the whole coverage denominator**, contributing nothing. Overall coverage was
53%; without these four it would have been about 68%.

**The first row is stale and the denominator has to be remeasured.** The rebuild landed and deleted the Blazor
stack - see below. Those are Sonar coverable-line counts, not physical lines, so the new figure has to come
from running the analysis rather than from counting files.

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
expressions: `IndexModel.RouteFor` and `ErrorModel.MessageFor`.

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

## What the TypeScript half needs

jest already runs in this repo (`binacle-compact-notation` and `binacle-vipaq` have suites, and lcov from them
already reaches Sonar), so the runner is not the question. **The question is the DOM:** `binacle-net-ui`,
`cookies` and `theme-switcher` all touch browser APIs, so they need jsdom or a real browser. Decide which before
writing the first test.

**Start with `cookies` and `theme-switcher`.** They are 79 lines between them and they are stable. The rebuild
did not touch either, and both now have a third consumer - the UIModule loads `theme-switcher` from its `main`
bundle - so a test there protects three hosts instead of two.

`binacle-net-ui` is the 533 and it is now **shared by two hosts**, which raises what a test is worth: a
regression there breaks the demo site and the shipped image together.

## Decisions to make first

- **Is `binacle-net-ui` worth unit-testing, or is it integration-only?** It is largely wiring between the DOM
  and the API. A thin wrapper tested through jsdom can produce coverage without proving much - the failure mode
  this repo already calls out for gates that pass without proving anything.
- **What coverage number is honest to aim for?** UI code has a long tail that is not worth covering. Pick the
  target when the harness exists and a few real tests show what it costs, not now.

**One decision the rebuild removed.** "Where does the demo page get tested - bUnit or a browser?" only existed
because the demo spanned two stacks, a Blazor page driving a TS visualizer through JS interop. **There is one
stack now and no seam**, so the question is closed: the demo is tested where its code lives, in TypeScript.

## Done when

- The TypeScript harness exists and runs as a test leaf, like every other suite.
- Its coverage reaches Sonar the same way the existing suites' does, one flat file per suite.
- The denominator has been remeasured against the rebuilt module, so the table at the top is true again.
- The three TypeScript areas are no longer at zero, and the coverage condition on the quality gate is failing
  for a reason somebody chose rather than because the UI was never tested.
