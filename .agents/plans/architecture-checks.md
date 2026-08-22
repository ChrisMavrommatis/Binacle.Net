---
description: Derive the repo's dependency graph into a generated file, draw it, and lint it with a small ruleset.
---

# Architecture checks - derive the facts, lint the rules

**Status:** Designed 2026-08-12, twice revised, built once and reverted, then redesigned on 2026-08-17. The
audit work behind it is done and holds: `architecture.yml` exists at the repo root and states the real shape,
checked by re-deriving the graph from every `ProjectReference`. **What does not exist is anything that reads
any of it.**

**The comment rule is no longer part of this plan.** It was folded in because both were "checks", which is not
a relationship. It has its own file now.

## The goal, in order

1. **Something a person can look at to understand this repo's shape.** A picture, kept current by a machine.
2. **A small set of rules that fail loudly when the shape moves in a way that matters.**
3. **A record of what no tool will ever check**, so a green run is not read as "all of this is covered".

## The design - one generator, two readers

**Walk the project files, derive the graph, write it out. Then read it two ways.**

- **A diagram**, written as a Mermaid graph. Mermaid needs nothing installed and nothing hosted - a viewer
  draws it from text. This is goal 1, and it is the cheapest part of the whole plan.
- **A ruleset**, run over the graph with Spectral - the linter this repo already runs on the OpenAPI documents.
  Spectral is not an OpenAPI tool; the OpenAPI rules are a ruleset that ships with it. **So there is no new
  dependency and no new toolchain.**

### Nothing it writes is committed

**Everything the generator produces goes to `artifacts/architecture/`**, which is a declared sink and is not
tracked. The graph and the diagram are both things you produce when you want them, the way a report is.

That settles two questions at once:

- **CI has nothing to compare.** It runs the generator, lints the output, throws it away. `regen` is
  deliberately never called from CI - `tooling/regen.just` says so in its own header - and this respects that
  rather than arguing with it.
- **Nothing can go stale**, because there is no second copy to fall behind. A committed diagram would have had
  exactly that failure mode: forget to regenerate and the picture quietly lies.

**What it costs:** the diagram is not browsable. A committed markdown file renders on the repository page; a
file under `artifacts/` is something you generate and open locally. If being able to link someone at the
picture turns out to matter, publishing it as a CI build artifact is the cheap answer - not committing it.

### Never abbreviate architecture to "arch"

**Recipes, folders, files, job names, output paths - spell it out.** `arch` already means CPU architecture in
this repo, and there is a plan on the board to publish images for a second one. `just architecture`, not
`just arch`. `artifacts/architecture/`, not `artifacts/arch/`. A name that means two things is a name that
sends someone to the wrong file.

### Why this shape and not a hand-written declaration

The earlier design was one hand-written file declaring every allowed edge, compared against reality. It was
built end to end, run green, mutation-tested, and reverted. What it cost to learn is worth keeping:

- **The compiler already enforces the project graph.** A reference that does not exist cannot be used. No check
  can prevent an edge; it can only notice one was added. **Knowing which of those two a check is changes how it
  should be built.**
- **A hand-written file drifts in two directions.** Reality can move away from it, and it can move away from
  reality. Reconciling those needs shorthand rules, resolution rules and carve-outs - and **every line of that
  is a place the check can pass for the wrong reason.** A silently green check is worse than no check.
- **A generated graph cannot be wrong.** It is derived on the spot from the project files, so there is no
  second copy of the truth to reconcile and nothing to keep in step.
- **The rules are not the declaration.** A ruleset asserts; it is never compared against anything. That is why
  it can be exhaustive at a fraction of the cost - there is no reconciliation layer, no shorthand to resolve,
  no entries that name things no tool can see.

**`architecture.yml` stays, as prose.** It is the readable statement of intent and it earns its keep - see the
preamble it already carries about what counts as a reference. For the parts a tool can see, it should point at
the ruleset rather than restate the edges, so nothing is written twice and nothing can disagree.

### The rules to write - one per slice, and write them all

**Write the ruleset exhaustively.** Every slice gets a rule naming exactly what it may reference. This is the
opposite of the advice that came out of the first pass, and the first pass was wrong: it was arguing against an
exhaustive *declaration file*, which had to be kept in step with reality. **A ruleset is never compared against
anything**, so an exhaustive one carries none of that cost.

The shape is about four lines per slice, so thirteen slices is roughly sixty lines. Adding a legitimate new
dependency then costs one edit to one rule - **which is the friction you want.** A dependency worth having is
worth a line stating that it is allowed.

**Write them from the generated graph as it stands, or the whole set lands red on day one.** The graph is
already known to be clean, so this is transcription, not investigation.

Spectral's built-in functions cover all of it. No custom code.

```yaml
rules:
  shared-references-nothing:
    given: $.slices.shared.*
    then: { function: length, functionOptions: { max: 0 } }
    severity: error

  lib-only-references-shared:
    given: $.slices.lib.*[*]
    then: { function: enumeration, functionOptions: { values: [shared] } }
    severity: error

  nothing-references-tools:
    given: $.slices.*.*[*]
    then: { function: pattern, functionOptions: { notMatch: "tools$" } }
    severity: error
```

Two properties of this shape are the reason to prefer it:

- **A rule is one readable thing**, and its failure message names it. Not "an undeclared edge appeared" but
  "lib may only reference shared".
- **Severity is built in**, so a rule that cannot be made true today can land as a warning and be tightened
  later. That is a better answer to "a check must land green" than hunting for violations to fix first.

### Coverage - what the generator can and cannot see

**The .NET project references are the obvious half** - 32 project files, and the edges come straight out of
them. **The npm workspace packages are just as easy**, since each package file names the others it depends on.
Together that is most of the code.

**What no generator will read**, and what therefore stays prose in `architecture.yml`, labelled as such:

- `docs` and `web` depending on `ruby` - Gemfile `path:` gems.
- `web` depending on `packages` and `vipaq` - webpack chunk regexes.
- `tooling` depending on everything - path strings inside just recipes.
- `assets` depending on `docs` and `web` - a gulp copy.
- `vipaq/tools` reading `shared/data` by resolving a path at run time, which no reference audit sees.

**Say which ones those are, in the output.** Silence about them is how a green run gets read as total coverage.

### Traps

- **Assert every derived list is non-empty.** A generator that finds no projects writes an empty file, the
  ruleset passes over nothing, and the check reports clean forever. This is the most common way a check like
  this dies.
- **Test the checks by breaking things on purpose.** The reverted build did this and it was the best part of
  it - a rule that has never been seen to fail has not been shown to work.
- **Do not reach for `xargs`.** Both traps recorded against the earlier attempt - exit code 123 when a batch
  matches nothing, and six algorithm folders whose names contain spaces - are `xargs` artifacts. Nothing in
  this repo uses it, and `grep` over a directory has neither problem.
- **A bare target in `architecture.yml` means that slice's `src`.** Without that, the declared graph reads as
  cyclic (`shared/test -> lib` against `lib/src -> shared`). It is a fact about how to read the file, not about
  the generator, but anything comparing against the file needs it.

## The `InternalsVisibleTo` check

Settled 2026-08-13. When `A` grants `InternalsVisibleTo(B)`, nothing in `A` resolves `B` - `A` compiles fine if
`B` does not exist. The grant records that **`B` depends on `A`**, more deeply than usual: on internals rather
than the public API. So the dependency runs `B -> A`, the same direction `B`'s own reference already points.
**A grant never adds an edge. It annotates one that is already declared.**

**The rule that falls out:** every grant must name an assembly that references the granter, directly or
transitively. One that does not is dead weight - it grants access nobody can take. It has already found one, a
grant for internals the named test project never touched, since deleted.

**This is a query over the generated graph, not a separate program.** The transitive closure it needs is what
the generator produces anyway.

**Two things about it.** Expand `$(ProjectName)` before comparing - most grants are written that way, and one
of them expands in a way a suffix rule would not guess (`Binacle.Net` grants
`$(ProjectName).ServiceModule.IntegrationTests`). And **it is tidiness, not architecture** - nothing breaks
when a dead grant stays. Re-derive the count rather than trusting one:
`grep -rn 'InternalsVisibleTo' --include=*.csproj .` read 19 grants across 9 projects on 2026-08-17, all
passing.

## The api check - three greps, and it is a different kind of check

**Audited 2026-08-17 by reading every source file under `api/src/`.** The numbers below are measured, not
estimated.

**The slice ruleset above cannot express this one, and never will.** It reads project references; these rules
are about which *types* a file names, and the reference they would have to flag is legitimate and declared. So
this is a separate check scoped to `api/` alone.

### The intended shape, and how close the code already is

`Binacle.Net` is the composition root: it references all three modules plus `lib` and `vipaq`, and its job is
`.Add*` and `.Use*`. Each module references `Kernel` and shared, nothing else. `Kernel` references only shared.
**Modules are agnostic about each other.**

**Two thirds of that is already true and would land green today.**

- **No module names another module** - zero hits, at project level and at type level.
- **`Kernel` names no module** - zero hits.
- **Only the composition root may name a module** - **red, and it is one type in two files.**

### The one violation

`Services/BinacleService.cs:5` and `ExtensionMethods/LogChannelExtensions.cs:3` both carry
`using Binacle.Net.DiagnosticsModule.Logs.Models;`, and both need exactly one type from it:
`AlgorithmOperationLogChannelRequest`. Every other type on those lines comes from `Kernel`, `lib` or shared.
`Program.cs` names all three modules, which is what a composition root is for.

**A trap for whoever writes the check: exclude `obj/`.** The generated assembly info under
`api/src/Binacle.Net/obj/` names all three modules in `ApplicationPartAttribute` and `InternalsVisibleTo`
lines. A naive grep returns eight files, six of them build output, and the check is red forever for no reason.

### The fix - and it is not the one this plan used to record

The earlier answer was "move the two types into Kernel". **Reading the code says otherwise.** Kernel's
`AddLogProcessor<TChannelRequest, TLog>` is fully generic, and DiagnosticsModule supplies the concrete types in
its own `AddOptionsBasedPackingLogProcessor`. **That split is already correct**, and it is evidence the concrete
type is meant to stay in the module.

The real problem is that **the core is doing diagnostics work.** `BinacleService` packs, then hands its bins,
items and results to an extension method that builds a log request and enqueues it - and that extension sits in
the composition root, which is log-channel plumbing in the wiring project.

Two answers, and the second is the one that puts the feature where it belongs:

- **Cheap.** Move the request and its `PackingLogEntry` record into Kernel. Two `using` lines change, Kernel
  gains a `Binacle.Packing` reference. The boundary is clean and it is about half an hour. It also moves a type
  into Kernel that the generic mechanism says should not be there.
- **Right.** Kernel declares an observer contract, the core calls it with its own types, and DiagnosticsModule
  implements it and owns the request-building. **`LogChannelExtensions.cs` leaves the composition root
  entirely**, because building a packing log line is a diagnostics feature.

**Fix it before building the check that watches it.** A mistake made impossible beats a mistake detected. The
check is still worth having afterwards - the rule is permanent - but it should not be built to watch a
violation standing there waiting to be moved.

### The rules

Three greps over `api/src/`, excluding `obj/`. **Derive the module list; do not type it out.**

1. **Only `Program.cs` may name a module**, within `api/src/Binacle.Net/`. Red today, green after the fix.
2. **No module may name another module.** Green today.
3. **`Kernel` may name no module.** Green today.

No toolchain. The surface is one directory, which is why the cheap answer is the right one here rather than
ArchUnitNET.

**The ServiceModule simplification decides one detail, so write the check to survive it.** Today that module is
three assemblies sharing a prefix - `ServiceModule`, `.Domain`, `.Infrastructure` - so rule 2 must not read
`.Domain` as a different module and go red on a legitimate reference. **Group the derived list on the segment
ending in `Module`**, and the check handles both today's shape and the collapsed one instead of being rewritten
by that plan.

### DiagnosticsModule is always on, and that is deliberate

**Settled 2026-08-17.** `builder.AddDiagnosticsModule()` at `Program.cs:128` and `app.UseDiagnosticsModule()`
at `:174` are unconditional, while ServiceModule and UIModule sit behind `Feature.IsEnabled`. **That asymmetry
is by design and is not evidence of anything** - an earlier draft of this plan read it as a sign the boundary
was being crossed, and that argument is withdrawn.

**So "every module is registered behind a feature check" is not a rule to write.** It would be red on a
decision that has been taken.

**`IOptionalDependency<T>` is still doing real work**, which is a separate thing: the module is always
registered, but the packing log processor inside it is not - `ModuleDefinition.cs:136-143` registers it only
when its configuration turns it on. So the channel can be absent while the module is present, which is exactly
what that abstraction covers.

## The heavier tools, if they are ever wanted

| Tool | Version | How it would read the file |
|---|---|---|
| `TngTech.ArchUnitNET` + `.xUnitV3` | 0.13.3 | rules are runtime objects, so it reads YAML directly |
| `dependency-cruiser` | 18.2.0 | reads the YAML from its `.cjs` config |

Three things to settle before adopting ArchUnitNET:

- **Check `.xUnitV3`'s transitive xunit dependency first.** `Directory.Packages.props:88-93` records that this
  repo pins `xunit.v3.mtp-v2` 3.2.2 precisely because mixing the MTP v1 and v2 adapters throws
  `TypeLoadException` before a test runs. If `.xUnitV3` pulls plain `xunit.v3`, the test leaf reproduces it.
  **That trap decides whether this is an afternoon or a week.**
- **Decide which graph is authoritative.** ArchUnitNET measures *type* dependencies from loaded assemblies; the
  generated graph comes from project references. They disagree - `api/src/Binacle.Net/Binacle.Net.csproj:73`
  declares `<Using Include="Binacle.Geometry" />` with no reference to it.
- **Its test project must reference every slice it inspects**, becoming a node with an edge to everything.
  That exemption belongs in the declaration, not in the test.

For dependency-cruiser, reading the file is the easy half. There is no root `tsconfig.json` - there are five,
and `sites/demo/` has none despite running `ts-loader` - and imports are bare specifiers resolved through npm workspace
symlinks (`packages/binacle-net-ui/src/core/protocolDecoder.ts:4` imports `"binacle-vipaq"`), so rules must be
written against resolved real paths with symlink handling pinned.

## What the audit established, and still holds

Every edge was derived from `*.csproj`, `package.json`, `Binacle.Net.slnx`, both Gemfiles, both webpack configs,
the Dockerfile, the gulpfile and the just modules, then verified three ways - XML parse, independent shell
re-derivation, and npm's own resolver.

- **`lib` and `vipaq` are not standalone.** Both sit on `shared/Binacle.Geometry` in `src`, not just in tests.
- **The repo has no upward edge.** Nothing under `shared/` references `lib/` or `api/`. The one inversion the
  2026-08-12 audit found was removed by the packing-contract extraction rather than documented.
- **`vipaq/tools` reaches into both `lib` and `shared`.**
- **The two sites are not graph leaves.** Both Gemfiles load `../../ruby/jekyll-gtm` by path, and
  `sites/demo/webpack.config.js` names `packages/binacle-net-ui` and `vipaq/packages/binacle-vipaq`. "Agents
  must not edit it" and "it is a leaf in the graph" are different claims.
- **`api/src/Binacle.Net.UIModule` is a javascript consumer too**, since 2026-08-21. It has its own
  `package.json`, `tsconfig.json` and `webpack.config.js`, and its webpack config names the same two packages
  as `sites/demo`'s. A slice that was C#-only is now on both graphs.
- **There is no cycle between `packages` and `vipaq`.** Shipped code flows one way; only test support and tool
  files cross back. `binacle-compact-notation` sitting in `dependencies` rather than `devDependencies` is the
  single line that makes npm's graph look cyclic.
- **`.github` is a slice.** It hashes `.config/dotnet-tools.json` and names `tooling/sonar-analysis.xml`, so it
  sits above `tooling`.
- **The slice names differ between the agent guidance and disk.** `.github` is `ci-cd` there, and the two
  sites moved under `sites/` on 2026-08-20 while `architecture.yml` still lists them as top-level `docs` and
  `web`. Anything reading both needs to know that, and the declaration needs a decision on how a `sites/`
  slice is written.

## Loose ends found during the audit

None is an illegal edge, and no boundary file would have caught any of them. That is worth knowing when judging
how much the graph half is worth.

- `binacle-compact-notation` moves from `dependencies` to `devDependencies` in
  `vipaq/packages/binacle-vipaq/package.json`.
- **A global `Using` with no matching `ProjectReference`** - 19 declarations across 13 projects, in every slice
  that has C#. Every one resolves transitively, so they all compile today, and every one breaks the day the
  project it borrows from stops referencing what it borrows. **Whether the fix is 19 added references or a
  decision that transitive resolution is fine here has never been settled**, and until it is, a check for it
  would land red on a question nobody has answered.

## What stays unenforced

The internal layer rules for agent guidance - permanent files never pointing at ephemeral ones, docs
referencing only docs - are graph-shaped over the `$` reference scheme. A regex can ban a pattern within a
path; it cannot check that a `$` reference resolves to a file in an allowed layer. No off-the-shelf tool does.
Either it stays prose-only, or it is the one place a small custom check is worth owning.

## Watch out

- **The published documentation site and the demo site are off limits.** If any of this needs a page
  written, record what the page must say here and leave the writing to that session.
- **Nothing is committed by an agent.** Leave every change in the working tree.
