---
name: tests-arrange-act-assert
description: A test body shows arrange, act and assert as separate lines — never one helper that does all three
type: convention
---

A test method must show all three steps in its own body. Not `=> this.Fixture.RunTest(factory, name, op);`,
where a single helper resolves the scenario, runs the algorithm and asserts the result behind one call.

Testing fixtures are shaped to make that possible: an arrange that hands back the scenario, an act that
returns a result and checks nothing, and a separate assert. Where both sides of a comparison have the same
shape, wrap them in one type and pass the pair — `AssertSame(expected, actual)` — rather than a loose
argument list of expected-bin, expected-items, actual-bin, actual-items.

```csharp
var scenario = this.Fixture.GetScenarioByName(scenarioName);

var result = this.Fixture.Run(AlgorithmFactories.FFD_v1, scenario, AlgorithmOperation.Fitting);

this.Fixture.AssertResult(scenario, result);
```

Assert directly in the test body with Shouldly when the check is a single comparison. Only keep a helper when
the assert is genuinely several field checks — the ViPaq `AssertSame` is field-by-field on purpose, so a
wiring bug reads as a clear mismatch rather than a whole-object inequality.

The current fixture surfaces are in `$lib/tests` and `$vipaq`.

**Why:** the maintainer asked for it directly, and it is what makes Sonar's S2699 pass honestly rather than
by suppression. A test whose assertion is invisible is one nobody can review — the analyser complaining is a
symptom, not the reason.

**How to apply:** write the three steps as separate statements with a blank line between them. When a helper
does act-and-assert together, split it rather than wrapping it. If a data row needs a generic type and you
reach for a `Dictionary<Type, Action>` to dispatch, write one test method per type instead — the dictionary
hop hides the assertion from the reader and from the analyser.
