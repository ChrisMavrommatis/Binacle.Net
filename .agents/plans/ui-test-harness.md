---
description: A test harness for the UI
---

# A test harness for the UI

**Status:** Not started. Decided 2026-08-09, while treating Sonar findings: the coverage gate stays red until
this exists, rather than being configured away.

The UI is the only part of the codebase with **no test harness at all**. Four areas sit at exactly 0% coverage,
every line uncovered:

| Area | Lines to cover | Covered |
|---|---|---|
| `api/src/Binacle.Net.UIModule` (Blazor components) | 959 | 0 |
| `packages/binacle-net-ui/src` (TS) | 533 | 0 |
| `packages/cookies/src` | 40 | 0 |
| `packages/theme-switcher/src` | 39 | 0 |

That is **1571 lines, 22.5% of the whole coverage denominator**, contributing nothing. Overall coverage is 53%;
without these four it would be about 68%. Both numbers are honest - the gap is real, not a measurement artifact.

## Why this is a plan and not a config line

The obvious shortcut is `sonar.coverage.exclusions` over the four paths. That was considered on 2026-08-09 and
**rejected**: it would move the number without changing anything true, and it would hide the gap from the one
place anybody looks at it. The same reasoning already governs Sonar issue findings in this repo - answer them
where a reader can see the answer, never in a config file nobody opens.

So the Sonar quality gate fails its coverage condition on purpose, and will keep failing until there are tests.
Anyone reading a red gate should find this file, not a mystery.

## The Blazor half waits for the Alpine port - decided 2026-08-14

**Do not write bUnit tests for `Binacle.Net.UIModule` yet.** The Alpine port idea deletes most of what would be
tested: `PackingDemo.razor.cs`, `ProtocolDecoder.razor.cs`, `PackingVisualizer.razor.cs`,
`BinacleVisualizerService` and `MessagingService` all go, and their logic moves into TypeScript that already
exists in `packages/binacle-net-ui`.

**Doing the tests first means writing them twice, in two languages.** Doing the port first means writing them
once, in one. That is the whole reason for the order.

**So this plan is in two parts and only one of them is ready:**

| Part | Lines | State |
|---|---|---|
| The TypeScript packages | 612 | **ready - start here** |
| `Binacle.Net.UIModule` (Blazor) | 959 | **waits for the Alpine port** |

**The coverage gate does not move until both are done.** 612 of 1571 lines is not enough to clear an 80%
new-code condition, and it was never going to be. That is expected, not a failure of this plan - the gate is
honest about the UI being untested and stays red until the UI is tested.

## What the TypeScript half needs

jest already runs in this repo (`binacle-compact-notation` and `binacle-vipaq` have suites, and lcov from them
already reaches Sonar), so the runner is not the question. **The question is the DOM:** `binacle-net-ui`,
`cookies` and `theme-switcher` all touch browser APIs, so they need jsdom or a real browser. Decide which before
writing the first test.

**Start with `cookies` and `theme-switcher`.** They are 79 lines between them, they are stable, and the Alpine
port does not touch either - so nothing written there gets rewritten. `binacle-net-ui` is 533 lines and the port
grows it, so its tests are worth writing against the shape it ends up with rather than the one it has now.

## Decisions to make first

- **Is `binacle-net-ui` worth unit-testing, or is it integration-only?** It is largely wiring between the DOM
  and the API. A thin wrapper tested through jsdom can produce coverage without proving much - the failure mode
  this repo already calls out for gates that pass without proving anything.
- **What coverage number is honest to aim for?** UI code has a long tail that is not worth covering. Pick the
  target when the harness exists and a few real tests show what it costs, not now.

**One decision the port removes.** "Where does the demo page get tested - bUnit or a browser?" only exists
because the demo spans two stacks, a Blazor page driving a TS visualizer through JS interop. **After the port
there is one stack and no seam**, so the question answers itself. That is a second reason to let the port go
first.

## Done when

- Both harnesses exist and run as test leaves, like every other suite.
- Their coverage reaches Sonar the same way the existing suites' does, one flat file per suite.
- The four areas above are no longer at zero, and the coverage condition on the quality gate is failing for a
  reason somebody chose rather than because the UI was never tested.
