---
description: The instance page reads its presets over HTTP from the browser - move it to server-side state
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "api/src/Binacle.Net.Kernel/**"
---

# The instance page's presets, without a call

**Status: ready, with one design question to settle first.** One coding session. Repository code only.

## The item

**`/instance` reads its preset list over HTTP from the browser, and it should not.**

`api/src/Binacle.Net.UIModule/_js/instance.js` calls `GET /api/v4/presets`, relative, and renders the reply
into the table on the page. That is the module's only browser fetch that is not part of a demo, and it is the
only one asking the instance about itself.

**Four reasons it is wrong, and they outlive this plan:**

- **An air-gapped install is a normal way to run this.** The rest of the module already holds to that. This is
  not an internet fetch, but it is a fetch, and it fails for reasons that have nothing to do with the page.
- **It breaks behind an auth layer, a proxy or a CORS rule.** The instance page is exactly the page an
  operator opens when something is already wrong with one of those.
- **It is inconsistent with everything else on the page.** Version, environment and the switch list are all
  rendered server-side from `IOptions`. The presets are the odd one out.
- **The data is already in the process.** The server is holding `BinPresetOptions` when it renders the page.
  Going out to the network to ask itself a question it can answer is the whole defect in one sentence.

**Decided, and it does not reopen: no browser fetch and no `HttpClient`.** The module makes no server-side
HTTP calls at all today and its csproj has one project reference. Neither changes for this.

## The design question - settle it before writing code

**How does the UI module see the preset list, when the type that holds it lives in the project that
references the module?**

Measured 22 Aug 2026:

- `BinPresetOptions` is at `api/src/Binacle.Net/Configuration/BinPresetOptions.cs` - the **entry project**.
  The entry project references the UI module, so a project reference back is a cycle. **This route is closed,
  not merely awkward.**
- The UI module references `Binacle.Net.Kernel` and nothing else, and that is a property worth keeping.
- **The codebase has solved this exact shape twice already.** `FeatureOptions` and `ReservedPathOptions` both
  live in `Binacle.Net.Kernel`; `Program.cs` fills them, and the module reads them through `IOptions`.
  `Pages/Instance.cshtml.cs` already injects `IOptions<FeatureOptions>` and calls `IsFeatureEnabled` and
  `PathFor` on it. **The instance page is already three quarters of this pattern.**

**The recommendation - a third bag in `Kernel`, filled in `Program.cs`, read by the page.**

`Program.cs:146` is the model, and it is nine lines:

```csharp
builder.Services.Configure<FeatureOptions>(options =>
{
    if (swaggerEnabled) { options.AddFeature("SwaggerUI", "/swagger"); }
    ...
});
```

The new one carries a name and a list of `(Id, Length, Width, Height)` per preset, and nothing else.

**Do not put a `Bin` type from `Binacle.Packing` or `Binacle.Geometry` in it.** `Kernel` references neither,
and pulling one in to serve a demo page would be the largest cost in this plan by a wide margin. A plain
record with four fields is enough - it is what the page prints.

**One wrinkle worth settling deliberately.** `BinPresetOptions.ReloadOnChange` is `true`, so `Presets.json` is
re-read when it changes on disk. **A bag filled once at startup goes stale the moment an operator edits the
file** - and the operator editing that file and refreshing that page is the exact person this feature is for.
Either bind it through `IOptionsMonitor<BinPresetOptions>` on the filling side, or accept the staleness
deliberately and say so in a comment. **Do not leave it undecided**; a page that lies about the config it
loaded is worse than one that fetches.

## The consequence that must not be missed

**If the bag lands, `_js/instance.js` is dead.** Delete it, drop the `instance` entry from
`api/src/Binacle.Net.UIModule/webpack.config.js`, and the instance page then loads no script of its own.

**The UI module doc describes both, and both descriptions become wrong in the same change:**

- Its webpack entry table names four entries - `main`, `instance`, `packing_demo`, `protocol_decoder`.
- Its line saying `instance` imports nothing, so it is its own 1 KB file and pulls in no shared chunk.
- Its whole section on the module making no server-side HTTP calls ends with three paragraphs explaining
  **why** the instance page reads presets over HTTP. **That reasoning is what this plan reverses**; it does not
  get edited, it gets replaced with the reason it stopped being true.

That doc's `check:` line asserts the script paths match the webpack entries, so it fails honestly if this is
missed.

## Two things already settled - do not reopen as a tidy-up

- **`Bins` and `Items` stay as the panel labels** on both hosts. Answered 22 Aug 2026.
- **A presets picker in the packing form was rejected.** New markup would get written twice and would have one
  entry on the demo site.

## The markup exists twice, and it is not this plan's to fix

`sites/demo/collections/_apps/packing-demo.html` and `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` are
the same page. That is known and separately owned. It is a build-system change and it would swallow anything
it is attached to.

## There is something to break now

**All of it has tests as of 22 Aug 2026** - the component, the randomizer and the module's C#. The
randomizer's contract, *never produces a box the items cannot fit*, is asserted in
`packages/binacle-net-ui/tests/model/samples.test.ts`. **A change to the instance page now has something to
break**, which was not true when this work was first written down.

Run `just test api-ui-unit` and `just test api-ui-integration` after touching the page model.

## Done when

- The instance page renders its preset list from server-side state, with no HTTP call from either side.
- The reload question is answered in code and the answer is readable at the point it was taken.
- `_js/instance.js` is gone, along with its webpack entry.
- `Kernel` has gained one plain options type and no new project reference.
- The UI module doc describes what is there, including why the fetch went away.
- The image builds and `just smoke all` is green.

**Do not commit.** Everything lands in the working tree.
