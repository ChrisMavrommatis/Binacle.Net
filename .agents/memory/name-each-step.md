---
name: name-each-step
description: Give each step a named local — no nested or chained call expressions squeezed into one statement
type: convention
---

Do not nest calls or chain them into one dense statement. Give each step a named local, even where the
expression-bodied one-liner compiles perfectly well. Production and test code alike.

Dense:

```csharp
public static BinContents<T> RoundTrip<T>(Header header, BinContents<T> binContents)
    => Deserialize<T>(Encode(header, binContents.Bin, binContents.Items));

var expected = new BinContents<int>(BuildBin<int>(Width.Eight), Enumerable.Range(0, 60).Select(...).ToList());
```

Unpacked:

```csharp
public static BinContents<T> RoundTrip<T>(Header header, BinContents<T> binContents)
{
    var encoded = Encode(header, binContents.Bin, binContents.Items);
    return Deserialize<T>(encoded);
}

var bin = BuildBin<int>(Width.Eight);
var items = Enumerable.Range(0, 60).Select(...).ToList();
var expected = new BinContents<int>(bin, items);
```

`new Foo(new Bar())` as an argument is the same problem — hoist the inner `new` to its own local.

An expression body is still fine when it is one call and nothing is nested inside it
(`public Scenario GetScenarioByName(string name) => AllScenariosProvider.GetScenarioByName(name);`).
Object initialisers in a builder are not affected.

**Why:** each step gets a name, so the code reads top to bottom instead of being unwrapped inside-out. It is
also what keeps arrange, act and assert on separate lines in a test.

**How to apply:** default to a block body with named locals when writing C#. Watch bulk find-and-replace
especially — a regex rewrite across many call sites is what collapsed a set of test arranges into dense
constructor calls and had to be undone by hand.
