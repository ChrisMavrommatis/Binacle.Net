---
name: algorithm-identifier-is-a-format
description: The FFD_v2 / BFD_v1 string is a parsed data format, not a naming style - never tidy the underscore out of it
type: gotcha
when: renaming an algorithm class, enum member or identifier string
paths:
  - "lib/**"
  - "shared/src/Binacle.Packing/**"
---

`AlgorithmExtensions.GetAlgorithmIdentifierName()` builds `$"{ShortName}_v{Version}"`, giving `FFD_v2`,
`BFD_v1` and so on. That string looks like a C# identifier written in the wrong style. It is not a name -
it is a **wire format**, and three things already depend on parsing it:

- `LoopAlgorithmProcessor` and `ParallelAlgorithmProcessor` key their results dictionary with it, and
  `ResultSelector` reads those keys.
- `lib/data/result-selection/*/baseline.json` stores it as fixture data, in both `ExpectedResult` and the
  `Results` keys - `"BFD_v2"`, `"60x40x10 FFD_v2 PartiallyPacked 70 95"`.
- `AlgorithmInfoHelper.ParseFromCompactString` reads it back by splitting on `_` and requiring the second
  half to start with `v`. Change the separator or drop the `v` and it throws on every fixture row.

It also reaches the log channel through `BinacleService`, so it shows up in log output people search.

**Never "make it consistent" with C# naming.** The `_v` is load-bearing. It is not part of the HTTP response
today - the v3/v4 pack and fit responses key on `bin.ID` - but the fixture corpus and the log stream are
enough to make it a format with readers.

## `_v1` is the house style, everywhere {#house-style}

**Decided by the maintainer, 2026-08-09:** lowercase `_v1` / `_v2` on every version suffix in the codebase,
with no exceptions, because the format above is the thing they all have to agree with.

That covers the lib types (`BestFitDecreasing_v1`, `BestAlgorithm_v2`, `AlgorithmFactory_v1`), the
`FFD_v1`/`BFD_v1`/`WFD_v1` factory constants in all three `AlgorithmFactories.cs` copies, the unit test method
names (`CustomProblems_Fitting_BFD_v1`), and the benchmark class names (`AlgorithmRacing_Packing_v1`).

An S101 sweep on 2026-08-09 renamed the 14 lib types to `BestFitDecreasingV1` and so on, then **reverted the
whole thing** on that ruling. Do not redo it. The 38 S101 findings it would have cleared are marked Accepted
in the SonarCloud UI instead - a custom quality profile is not available on the Free plan, so per-finding is
the only way to answer a rule here (see the memory on Sonar issue ignores). The Sonar triage plan records
this as decided.

The test and benchmark names were never flagged anyway: they live in projects that `Directory.Build.props`
marks `SonarQubeTestProject`, and S101 is scope MAIN. Only the 14 product types ever appeared.

**Why:** a string that is parsed is an interface, whatever it looks like. The underscore here carries the
split point.

**How to apply:** treat `FFD_v2` as data. If it ever has to change, the baseline fixtures, the parser and the
log consumers change with it, in one commit, deliberately.
