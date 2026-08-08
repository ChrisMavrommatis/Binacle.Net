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

## What it needs

Two harnesses, because it is two stacks:

- **Blazor components** (`Binacle.Net.UIModule`). bUnit is the usual answer, rendering components in-process
  and asserting on markup. Worth confirming it works with the module's DI wiring, because the components take
  their dependencies through `[Inject]` properties that are declared non-null and set by the container.
  `PackingVisualizer` also talks to JS interop, which has to be faked.
- **The TypeScript packages.** jest already runs in this repo (`binacle-compact-notation` and `binacle-vipaq`
  have suites, and lcov from them already reaches Sonar), so the runner is not the question. The question is the
  DOM: `binacle-net-ui`, `cookies` and `theme-switcher` all touch browser APIs, so they need jsdom or a real
  browser. Decide which before writing the first test.

## Decisions to make first

- **Is `binacle-net-ui` worth unit-testing, or is it integration-only?** It is largely wiring between the DOM
  and the API. A thin wrapper tested through jsdom can produce coverage without proving much - the failure mode
  this repo already calls out for gates that pass without proving anything.
- **Where does the demo page get tested - bUnit or a browser?** The packing demo spans both stacks: a Blazor page
  driving a TS visualizer through JS interop. Testing each half in isolation may miss the seam that actually
  breaks. A browser-driven test over the built image would cover the seam, at a much higher runtime cost.
- **What coverage number is honest to aim for?** UI code has a long tail that is not worth covering. Pick the
  target when the harness exists and a few real tests show what it costs, not now.

## Done when

- Both harnesses exist and run as test leaves, like every other suite.
- Their coverage reaches Sonar the same way the existing suites' does, one flat file per suite.
- The four areas above are no longer at zero, and the coverage condition on the quality gate is failing for a
  reason somebody chose rather than because the UI was never tested.
