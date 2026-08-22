---
description: The packing demo's sample data, randomizer and buttons - one component change landing on both hosts, module first
paths:
  - "packages/binacle-net-ui/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "sites/demo/**"
---

# The packing demo - samples, randomize and buttons

**Status:** Not started. Decided 2026-08-22. Every decision below is taken; what is left is one open design
question, marked as such, and the work.

**This spans three slices** - the shared component in `packages/binacle-net-ui`, the UI module in `api/`, and
the demo site in `sites/demo`. That is why it sits at the root of `plans/` rather than in a slice folder.

## The order, and it is not negotiable

**The UI module first, whole. The demo site second, and only once the module is good.**

Both hosts render the same form from the same component, so every change here lands on both. Doing the module
first means the component, the randomizer and the buttons are settled against a host that a coding session
fully controls. The site then takes a finished design and applies it to markup, with no design questions left
open in a place that is harder to iterate on.

**Do not start the site half early to "keep them in step".** They are already in step; they diverge only for
the few hours between the two halves.

## What is wrong today

**The starting data is good and lives in the wrong place.** `packingDemo.ts`'s `init()` prefills three bins -
`60x40x10`, `60x40x20`, `60x40x30` - and three items. Same footprint, three heights: that is the product's
own pitch, "which of my boxes does this order go in", and it makes a good first screen. It is also hardcoded
inside the shared component, which is host data living in the tool.

**Randomize replaces that good demo with a broken one.** `getRandomBin` rolls each side 10-60 and
`getRandomItem` rolls each side 5-30 with a quantity of 1-10, and nothing compares the two. Measured over
200,000 rolls of the current code:

| Outcome | Share |
|---|---|
| plausible | 62.8% |
| the quantity cannot fit by volume | 19.1% |
| the item does not fit in the bin at all | 18.2% |

**37% of clicks produce a result that was impossible before the request was sent**, and that is a floor -
"fits by volume" still often fails a real pack. Randomize is the first button a curious visitor presses.

**`Add Bin` is the same bug in miniature.** It appends `getRandomBin()`, a 10-60 box unrelated to the other
three, which breaks the compare shape the defaults set up. Adding a fourth candidate should keep the footprint.

**Every action looks equally important.** Eight buttons on the page, seven of them filled and stretched
(`class="max"`), and the one action the page exists for is `tertiary` - a violet used nowhere else in the
brand.

## Decisions taken - do not re-open

**1. Samples arrive through the component's signature.** `packing_demo_app` already takes
`PackingDemoOptions`; `samples` is a second field on it. That is what the options object was built for, and
the union type that let the two hosts migrate separately was deleted on 22 Aug precisely so this could be
added cleanly.

**2. The package owns a pre-approved set, and it is the fallback.** `binacle-net-ui` keeps a curated
bins-and-items set - today's three-height footprint is the starting point for it. When a host passes no
`samples`, that set is used.

**3. The demo site gets the pre-approved set.** It passes no `samples` and takes the package default. It has
one public configuration, so a per-instance set would mean nothing there.

**4. The UI module's samples come from the instance's presets.** A self-hoster's first question is whether
their `Presets.json` loaded the way they meant, and a demo that opens on their own boxes answers it before
they read anything.

**5. The module must not fetch them from the API.** No browser call to `/api/v4/presets`, and no
`HttpClient`. The mechanism is the open question below.

## The open question - how the module gets its presets without a call

**This is the first thing to settle and it decides the shape of the module half.** What is known:

- `BinPresetOptions` lives in `api/src/Binacle.Net/Configuration/BinPresetOptions.cs`, in the **entry
  project**. The entry project references the UI module, so a project reference back is a cycle. This route is
  closed, not merely awkward.
- The UI module references `Binacle.Net.Kernel` and nothing else, and the UI module doc records that as a
  property worth keeping.
- **The codebase has already solved this shape twice.** `ReservedPathOptions` and `FeatureOptions` both live
  in `Kernel`; whoever owns a fact fills them, and whoever renders it reads them. `FeatureOptions` is even
  filled lazily from another module's config, through
  `AddOptions<FeatureOptions>().Configure<IOptions<HealthCheckConfigurationOptions>>(...)`, which is exactly
  the binding a preset-backed bag needs.

**The recommendation, and it is a recommendation rather than a decision:** a small options type in `Kernel` -
a list of `(Id, Length, Width, Height)` and nothing more - that `Program.cs` fills from `BinPresetOptions`
and the UI module reads. `Pages/Packing.cshtml.cs` renders it into the `x-data` attribute, the same way
`ApiBaseUrl` is rendered today.

Reasons to prefer it over any fetch, worth stating because they outlive this plan:

- The data is in the HTML. No round trip, nothing to fail on a struggling instance.
- It still works when the API surface is restricted - CORS, an auth layer, a proxy in front.
- It works with no network at all, which matters because an air-gapped install is a normal way to run this.
- **Do not put a `Bin` type from `Binacle.Packing` or `Binacle.Geometry` in it.** `Kernel` references neither,
  and adding one to serve the demo would be the largest cost in this plan. A plain record is enough.

**One consequence, and it must not be missed.** `api/src/Binacle.Net.UIModule/_js/instance.js` currently
fetches `/api/v4/presets` from the browser to fill the presets list on `/instance`. If the server-side bag
lands, that fetch is redundant and inconsistent with the decision above. **Move the instance page onto the
same bag, delete `_js/instance.js`, and drop the `instance` entry from `webpack.config.js`.** The instance
page then loads no script of its own. The UI module doc describes both the entry and the fetch and must be
corrected in the same change.

## The randomize fix

**Roll the box first, then size the items to it.** One function, and the box can come from anywhere - the
rolled default, the package's pre-approved set, or a preset.

```
box    each side 30..60
item   each side 8..(matching box side / 2)     -> eight of them fit before any packing thought
count  chosen so the items fill 45-75% of the box, clamped to 2..20
```

Simulated over 200,000 rolls: **0% impossible**, median fill 53%, 4-20 items per roll. Four real rolls:

```
bin 37x48x47    item 10x19x23 x12    63% of the box
bin 32x49x30    item 15x16x11 x9     51% of the box
bin 52x45x47    item 25x15x20 x9     61% of the box
bin 34x37x50    item 10x16x20 x13    66% of the box
```

Re-run the simulation before trusting these numbers; the ranges above are a starting point, not a result.

**One Randomize, not two.** The two independent Randomize buttons *are* the two-dice bug expressed as UI.
One that fills both panels together cannot produce a box the items do not fit.

**Done, on both hosts.** `randomize` is the only one, and `randomizeBins` / `randomizeItems` are deleted -
the site half landed the same day, so the two never had to survive a gap.

**The rolling itself is not in the component.** It is `packages/binacle-net-ui/src/utils/samples.ts`, which
owns the bin range, the item sizing, the volume budget and `randomSample()`. The component only assigns what
it gets back.

**The starting screen is a random sample too, not fixed data.** Decisions 1-5 above describe samples arriving
through the component's signature from presets; that was dropped on 22 Aug in favour of rolling one on `init()`
like any other. `PackingDemoOptions` still takes `baseUrl` and nothing else. Nothing was built against those
decisions - reopen them as written if the starting data should come from the host after all.

**`Add Bin` copies the last box** rather than rolling a new one, so a fourth candidate keeps the footprint and
varies the height.

## The buttons - both hosts, identical

**Now:**

```
[══ + Add Bin ══][══ Clear All Bins ══][══ Randomize ══]      <- filled, stretched, equal weight
[══ + Add Item ══][══ Clear All Items ══][══ Randomize ══]    <- same, in orange

[ First Fit Decreasing v ]          [═══ Get Results ═══]     <- violet
```

**After:**

```
[ + Add bin ]                                  Clear all
[ + Add item ]                                 Clear all

[ First Fit Decreasing v ]   Randomize   [ Get results ]
```

Four rules, and they are the whole design:

1. **One filled button on the page**, in brand blue - the submit. Never `tertiary`.
2. **Add is outlined** (`border`). It is constructive but it is not the point of the page.
3. **Destructive is text**, and takes the error colour on hover only. Never a fill.
4. **Nothing is stretched.** Drop `max` from the three-button rows so width stops implying importance.

The row-level delete stays as it is - a transparent circle with an icon is already the right weight.

**The visualizer's playback controls are out of scope.** They are a toolbar of peers and the current
treatment is defensible.

## The wording

**Two are mechanical and can be fixed by whoever does the work:**

- `AppletsService.cs` - `"...using it's algorithms."` -> `its`.
- `AppletsService.cs` - `"An interactive tool that lets test different packing algorithms"` -> `lets you test`.

**Three need the maintainer's word before anything is typed. Ask, then write - do not draft a replacement and
show it as a proposal.**

- **`AppletsService.cs` - `"Allows you to visualize how Binacle.Net packs them into a container"`.** "them"
  has no antecedent; nothing has been mentioned. The sentence needs rewriting, not repairing.
- **The panel labels say `Bins` and `Items`.** The public copy says *box* everywhere. Changing the label makes
  the page match the marketing and diverge from the API endpoint the same page calls, which uses `bins`. Both
  are defensible; it is not a coding session's call.
- **Button labels move to sentence case** - `Add bin`, `Clear all`, `Get results`. Mechanical if the labels
  themselves stand; if `Bins` becomes `Boxes` these change with it, so take both answers at once.

**The strings in this module are already claimed by other release work.** Get the current wording before
editing any of them rather than assuming these are the live values.

## Editing `sites/demo`

**A grant was given on 2026-08-22 covering the demo site's packing form**, for this work and nothing else.
The standing rule against editing published sites otherwise applies in full, and the grant does not widen it:

- **In scope:** `sites/demo/collections/_apps/packing-demo.html` - the button markup and the `x-data`
  attribute, and any label whose wording answer says so.
- **Out of scope:** every other page, every layout, every `_config`, every piece of front matter, and both
  other sites. If the work appears to need one of those, stop and write down what it must say.

Record what was changed here when it lands, the way the previous grant was recorded, so the next session can
see exactly how far it reached.

**Used on 2026-08-22. Two files, and both reach past the grant as written - each on the maintainer's own
instruction, given while the work was running:**

- `sites/demo/collections/_apps/packing-demo.html` - the three button rows, byte-identical to the UI module's;
  and the page's `description` front matter, `excerpt` and the `<p>` under it, reworded to match
  `AppletsService.cs` word for word. **The button rows are in scope; the front matter is explicitly out of it**
  and was changed on the instruction "fix the description in web as in ui module".
- `sites/demo/_sass/_components.scss` - three rules appended, the same three the module carries.
  **This is stylesheet, not markup, so it is outside the grant's wording.** It was taken because the markup
  half cannot ship without it: `.border` inherits beercss's `color: var(--primary)`, which measures 1.01:1 on
  the dark items panel - the Add button's label is invisible - and `destructive` has no styling at all without
  it.

Nothing else under `sites/` was touched. The panel legends still say `Bins` and `Items` on both hosts, which
is the answer that was given.

## What will bite

**The markup exists twice and this plan does not fix that.** `sites/demo/collections/_apps/packing-demo.html`
and `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` are the same ~210 lines, and the button change has to
be made in both. That duplication is a known, separately-owned decision. **Do not attempt to unify them here** -
it is a build-system change and it would swallow this work.

The reason it is forked, for whoever is tempted: both template languages claim characters Alpine uses. Razor
eats `@`, so the module writes `@@click`; Jekyll eats `{{`. Writing `x-on:click` instead of `@click` removes
half of that, and is worth doing in this pass because it is free.

**No preset picker.** A dropdown in the form is new markup, so it gets written twice, and on the demo site it
would have one entry. If a picker turns out to be wanted, that is what should force the duplication decision -
it would be paying for itself. Presets arrive as the starting data and nothing else.

**A change to the component lands on both hosts at once.** There is no way to ship the module half of a
component change without the site rendering it too on its next build. That is the point of the shared package;
it is also why the site half must not be left half-done for long.

**Rebuild both bundles after any component change** - `api/src/Binacle.Net.UIModule` and `sites/demo` each run
their own webpack against the same source. A change that compiles in one compiles in the other, but only a
build proves the page loads it.

**Nothing here has a test.** The component, the randomizer and the module are all uncovered. The test harness
is its own plan; do not let this one grow into it, but a randomizer with a measurable contract - "never
produces an impossible pair" - is the cheapest first test anyone will ever write here, and it is worth
leaving that note behind.

## Done when

- `packing_demo_app` takes `samples` on its options object, and the package carries the pre-approved set as
  the fallback.
- The UI module passes the instance's presets in, with no HTTP call from either the server or the browser.
- `/instance` uses the same mechanism and `_js/instance.js` is gone, along with its webpack entry.
- Randomize cannot produce a box the items do not fit, and there is one Randomize rather than two.
- `Add Bin` keeps the footprint.
- Both hosts render the button design above, and neither has more than one filled button.
- The wording answers are in and applied.
- The UI module doc and the shared package doc describe what is actually there.
