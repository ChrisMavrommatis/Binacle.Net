---
description: A human-readable architecture.yml stating what each part of the repo may and may not reference, readable on its own and consumable by off-the-shelf tools.
---

# architecture.yml - state the boundaries, then let tools read them

**Status:** Part done. Designed 2026-08-12, revised twice the same day - once after an independent dependency
audit, once after an adversarial review that rejected the first proposed file. Master plan for one topic.

**`architecture.yml` exists at the repo root and states the real shape as of 2026-08-13** - corrected after the
`Binacle.Packing` extraction and the tests-kernel split, then checked by re-deriving the graph from every
`ProjectReference`. So phase 1's *file* half is done and the tooling phases are unblocked. **The comment check
and the fifteen comment fixes are not done** - that is what phase 1 still owes, and it is the visible win.

## The goal, in order

1. **A file a person can read to understand this repo's architecture.** What must reference what, what may
   never reference what, and why the odd ones are odd. It has to work as documentation even if no tool reads it.
2. **A file tools can read.** Off-the-shelf ones, not a bespoke engine.

That order matters. A file that only makes sense to a checker gets maintained like a lockfile - regenerated,
never read - and the architecture goes back to living in one person's head.

**Scoping, after review.** The YAML earns its keep as documentation: goal 1 is real and unmet today. The
comment rule earns its keep as enforcement: fifteen sites accumulated silently and that failure is mechanical
and recurring. Graph enforcement earns least - an audit of every `ProjectReference` in the repo found exactly
one surprising edge, already known. So build the file and the comment check first, and let the graph tooling
follow only when something breaks. The strong case for a type-level tool is not the thirty slice edges that
will be green forever; it is the two rules a graph walk can never see (the api module boundary, and v3-frozen).

## Why this exists

**Fifteen comment lines in thirteen files point at agent guidance.** Fourteen are under `vipaq/`, citing `decisions.md`,
`findings.md` or `architecture.md` by bare filename, several with a bare ref code (`D9`, `D14`, `D16`) that
means nothing to a human reader:

- `vipaq/src/Binacle.ViPaq/ViPaqSerializer.cs:12`
- `vipaq/src/Binacle.ViPaq/ViPaqSerializationOptions.cs:6`
- `vipaq/src/Binacle.ViPaq/Models/Header.cs:17`, `:42`, `:45`
- `vipaq/src/Binacle.ViPaq/Compression/GzipCodec.cs:14`
- `vipaq/src/Binacle.ViPaq/Layouts/ColumnarCodec.cs:12`
- `vipaq/packages/binacle-vipaq/src/ViPaqSerializationOptions.ts:7`
- `vipaq/test/Binacle.ViPaq.TestsKernel/ViPaq/ViPaqEncoder.cs:12`
- `vipaq/test/Binacle.ViPaq.TestsKernel/Providers/SyntheticDataProvider.cs:11`
- `vipaq/test/Binacle.ViPaq.Benchmarks/Abstractions/SyntheticBenchmarkBase.cs:8`
- `vipaq/test/Binacle.ViPaq.Benchmarks/Benchmarks/CompressionCostBenchmarks.cs:14`
- `vipaq/test/Binacle.ViPaq.Benchmarks/Benchmarks/SyntheticDecodeBenchmarks.cs:13`
- `vipaq/test/Binacle.ViPaq.Benchmarks/Benchmarks/SyntheticEncodeBenchmarks.cs:12`

The fifteenth is in a different slice and takes a different form:

- `api/src/Binacle.Net/v4/Contracts/ExampleData.cs:63` ends `Formula per $lib/result-building.`

**That one matters out of proportion to its size.** It uses the `$` reference scheme rather than a bare
filename, so a check built only from agent-doc basenames sails straight past it. It was found by traversing
the whole repo, not by looking where the first fourteen were. The check needs a second arm: **any `` `$id` ``
style reference appearing outside the agent guidance directory.** That arm is also the cheaper of the two,
because the `$` scheme is used nowhere else.

They slipped through because the rule is phrased as a ban on pointing at that directory, so everyone greps for
the directory name. Fourteen name the file with the path filed off; the fifteenth names no file at all.

**All fifteen are deletions, not rewrites.** `(PROTOCOL.md §4, decisions.md D16)` becomes `(PROTOCOL.md §4)`.
`ColumnarCodec.cs:11-12` reads "Whether it actually pays is unmeasured - see architecture.md"; dropping the
pointer leaves a complete sentence. `Header.cs:45` reads `(findings.md: Bischoff packs to 16/8/16)` and only
loses the filename. `PROTOCOL.md §4` in those same comments is **correct** and must survive - it is the
normative spec the code implements, in the same slice, and it stands alone.

**Slices are not shaped the way they are described.** Every edge was derived from `*.csproj`, `package.json`,
`Binacle.Net.slnx`, both Gemfiles, both webpack configs, the Dockerfile, the gulpfile and the just modules, then
verified three ways (XML parse, independent shell re-derivation, npm's own resolver):

- **`lib` and `vipaq` are not standalone.** Both sit on `shared/Binacle.Geometry` in `src`, not just in tests.
- **One inversion, repo-wide:** `shared/test/Binacle.TestsKernel.csproj:41` references
  `lib/src/Binacle.Lib.Abstractions`. The bottom layer's fixture kernel depends on a slice above it.
- **`vipaq/tools` reaches into both `lib` and `shared`** - `PackedDataGenerator.csproj:12,13` for the packer,
  and `:15,16,17` plus `VectorGenerators.csproj:9,11` for compact notation, geometry and the report writer.
- **`docs/` and `web/` are not graph leaves.** Both Gemfiles load `../ruby/jekyll-gtm` by path
  (`docs/Gemfile:26`, `web/Gemfile:26`), and `web/webpack.config.js:53,60` names `packages/binacle-net-ui` and
  `vipaq/packages/binacle-vipaq`. "Agents must not edit it" and "it is a leaf in the graph" are different claims.
- **There is no cycle between `packages` and `vipaq`.** `vipaq/packages/binacle-vipaq/src/` imports nothing
  from `packages/`; only `tests/support/vectorParser/index.ts:7` and `tools/interopArtifactGenerator.ts:3` do,
  mirroring where the C# `test` and `tools` scopes reference `Binacle.CompactNotation`. Shipped code flows one
  way. `binacle-compact-notation` sitting in `dependencies` rather than `devDependencies` is the single line
  that makes npm's graph look cyclic.

## The file

`architecture.yml` at the repo root. It must live outside the agent guidance directory: nothing outside that
directory may point into it, so a boundary file kept there could never be cited by the code it governs.

**The file is written and verified: `architecture.yml` at the repo root.** 13 slices, 57 scopes, three edges
carrying a `why`. It is not reproduced here - a plan gets deleted when the work lands, and a copy of a live
file in a doomed file is a copy that will disagree with it. Read the real one.

Its shape, so this plan can be read on its own:

```yaml
slices:
  <slice>:
    spec: <path>              # optional. The one doc this slice's comments may cite.
    <scope>: [<target>]       # scope = second path segment. "." = files at the slice root.
    <scope>:
      - <target>              # ordinary edge: points at something lower down
      - slice: <target>       # sideways or upward: must say what the code does with it
        why: <one line>

mirrors:                      # a port of a canonical implementation into another language.
  - canonical: <path>         # the port's comments may name the canonical's types;
    port: <path>              # the canonical never names the port.

sinks: [results, artifacts]   # may be pointed at, never point out
never_referenced: [.agents]   # points outward at anything; nothing outside may point back
```

### What writing it out for real changed

The file was built by traversing every tracked file, then checked by a script that re-derives the graph a
different way. It came back clean - no undeclared edges, no dead entries, no cycles. Three things the earlier
drafts had wrong:

- **Three edges carry a `why`, not two.** `vipaq/packages -> packages` is real: the TypeScript port's
  `tests/support/vectorParser/index.ts:7` and `tools/interopArtifactGenerator.ts:3` import
  `binacle-compact-notation`. Both files sit inside the `vipaq/packages` scope, so the edge counts. It is
  worth reading, because it is the C# `test`/`tools -> shared` shape repeated in TypeScript.
- **`.github` is a slice.** Nobody had counted it. `run-tests.yml:74` and `sonar-analysis.yml:58` hash
  `.config/dotnet-tools.json`, and `sonar-analysis.yml:83` names `tooling/sonar-analysis.xml`. It sits above
  `tooling`.
- **A runtime edge no `ProjectReference` audit can see:** `vipaq/tools` reads `shared/data` by resolving
  `["shared","data","custom-problems"]` at run time (`PackedDataGenerator/Program.cs:21,25`). Already covered
  by the declared `vipaq/tools -> shared`, but it is a reminder that the project graph is not the whole graph.

**Coverage is complete, and two gaps were closed after that.** Every tracked top-level directory is accounted
for - 13 slices, `results` and `artifacts` as sinks, and the agent guidance directory under its own key.

- **Repo-root files now have a `root:` block.** `Binacle.Net.slnx`, `justfile`, `gulpfile.js`,
  `jest.config.js`, `package.json`, `Dockerfile`, `.dockerignore` and `.spectral.yaml` sit in no slice, and
  naming slices is their whole job. They are listed one per file so a reader sees which one reaches where, and
  so adding a root file is a decision rather than a blind spot. The rest of the root - READMEs, licences,
  `Directory.*.props`, `global.json`, `NuGet.Config`, the dotfiles - names no slice and is left out on purpose.
- **Three slices have a different name in the agent guidance.** `.github` is `ci-cd` there, `docs` is
  `docs-site`, `web` is `web-site`. `architecture.yml` names them after the directory, because that is what a
  tool resolves, and notes the alias beside each. Worth knowing before writing any check that reads both.

### Why this shape

- **slice** is the unit people talk about. "lib is standalone" is a sentence about a slice.
- **scope** is where every real exception lives. `vipaq/tools -> lib` is fine; `vipaq/src -> lib` would not be.
  Without it you allow `vipaq -> lib` wholesale and lose the rule you cared about.
- **a list of targets** is the shape the tools want, so nothing needs translating.
- **`spec:`** keeps `PROTOCOL.md §4` legal on the same line where `decisions.md D16` fails.
- **`mirrors:`** is the only way a cross-language pair can be stated. It is not a code dependency - TypeScript
  cannot reference C# - it is a *reference* rule, and without it a comment check flags all 42 `Ports C#:`
  lines as violations.

### Decisions baked into the shape

- **A bare target means `<slice>/src`.** Without this sentence the declared graph is cyclic (`shared/test ->
  lib` and `lib/src -> shared`), depth derivation is undefined, and "sideways or upward" - the thing that
  decides which edges need a `why` - has no meaning. With it, `shared/test -> lib/src -> shared/src` is
  acyclic and `vipaq/tools -> lib` correctly lands as the same-depth edge that must explain itself.
- **No `layer:` number.** A declared layer is the only field that is not a fact about the code, and it can
  disagree with the edges beside it. Derive it by topological sort. An early draft declared layers and one
  questionable edge renumbered five slices.
- **`why` is not on every edge.** It exists to make an exception cost a sentence someone defends in review. A
  draft that put a `why` on all thirty edges destroyed the mechanism - the one real inversion was buried among
  twenty-nine ordinary ones.
- **No `components:` level.** A draft added one for `api` alone. It does not survive the code: `kernel: []`
  is false (`Binacle.Net.Kernel.csproj` references `Binacle.CompactNotation`; it referenced
  `Binacle.Lib.Abstractions` too until that project was deleted), and `modules: [kernel]` breaks because a
  module is three projects
  (`Binacle.Net.ServiceModule.csproj:26-28` references `.Domain` and `.Infrastructure`). The rule it was
  trying to express is type-level anyway - see below.

## The api module boundary is a type rule, not a graph rule

`Kernel` names no module (verified: no hits under `api/src/Binacle.Net.Kernel/`). Modules never reference each
other (verified: none). So the intended shape is Kernel as the plug-in contract, modules as plug-ins,
`Binacle.Net` as the composition root.

**Wiring is the composition root's job and stays.** `Program.cs:128-137` and `:175-184` call
`builder.AddServiceModule()` / `app.UseServiceModule()` behind `Feature.IsEnabled`. A composition root that
does not name its parts is not one.

**Behaviour is the line, and it is crossed in exactly two files.** Only three files under `api/src/Binacle.Net/`
name a module namespace: `Program.cs` (legitimate), `Services/BinacleService.cs:8`, and
`ExtensionMethods/LogChannelExtensions.cs:4`. `BinacleService` is the core packing service and holds a
`Channel<AlgorithmOperationLogChannelRequest>?`, a type from
`api/src/Binacle.Net.DiagnosticsModule/Logs/Models/AlgorithmOperationLogChannelRequest.cs:8`.
`IOptionalDependency<T>` softens it at runtime - it resolves via `GetService<T>()` and is null when the module
is absent - but the type leaks and the core cannot compile without the module.

The asymmetry gives it away: `builder.AddDiagnosticsModule()` at `Program.cs:128` is unconditional while the
other two sit behind feature flags. `IOptionalDependency` says optional; the registration says otherwise.

**The fix is two types, not one.** Kernel already owns `ILogEntryConvertible`, `ILogParametersProvider` and
`LogsProcessor`, and `AlgorithmOperationLogChannelRequest` implements `ILogEntryConvertible<PackingLogEntry>` -
but `PackingLogEntry` is declared in the same module file at `:93`, so moving the request drags the log-entry
shape with it or needs the interface split first.

**No graph walk can catch this.** `Binacle.Net -> DiagnosticsModule` is a declared, legitimate reference. Only
an assembly-level rule sees the difference between `Program.cs` naming a module and `BinacleService.cs` naming
one. With v3-frozen, it is the concrete case for a type-level tool.

## `InternalsVisibleTo` is not an edge, and the file must say so

Settled 2026-08-13 while designing the packing-contract extraction. It belongs in the preamble beside "what
counts as a reference", because a checker will otherwise have to guess and would guess wrong.

When `A` grants `InternalsVisibleTo(B)`, nothing in `A` resolves `B` - `A` compiles fine if `B` does not exist.
The grant records that **`B` depends on `A`**, more deeply than usual: on its internals rather than its public
API. So the dependency runs `B -> A`, the same direction `B`'s `ProjectReference` already points. **A grant
never adds an edge. It annotates one that is already declared.**

Without that sentence the extraction reads as introducing a `shared/src -> lib` upward edge the moment
`Binacle.Packing` inherits the grants, which would undo the thing this file exists to claim.

**The rule that falls out is worth a check.** If a grant marks an existing edge, every grant must have one: the
named assembly must reference the granter, directly or transitively. A grant naming an assembly that does not is
dead weight - it grants access nobody can take. Run by hand against the repo's six grants, this already found
one - a grant to `Binacle.Lib.UnitTests` for internals that project never touched, since deleted. Cheaper than
any of the graph tools and it caught something on its first run.

## Enforcement

**Start with a `just` recipe, not Semgrep.** The whole comment job is one regex over `.cs` and `.ts` comments -
fifteen lines in thirteen files. Semgrep means a Python toolchain, a new CI job, a committed generated artifact
and a generator recipe to do what `rg -n '(decisions|findings|architecture)\.md'` already does. A recipe under
`tooling/` plus a step in `.github/workflows/run-tests.yml` is the same check in the repo's existing idiom.
Adopt Semgrep the day a rule appears that a regex genuinely cannot express.

**The banned-name list is hardcoded; the generator checks for collisions.** The obvious design - generate the
alternation from every `.md` basename under the agent guidance directory - was measured and is unusable:
78 distinct basenames produce **94 hits** across the repo against **15** for the narrow list. `README.md` alone
accounts for most, and hits land in `Binacle.Net.slnx`, `DEVELOPMENT.md` and four `docs/collections/_versions/`
pages that a coding session may not edit. So invert it: keep a short list of names distinctive to agent
guidance, and have the generated check fail the build when a **new** agent-doc basename is distinctive enough
to belong on the list, or when a listed name gains a twin elsewhere in the repo. That turns "the regex has
never heard of the name" into a build error rather than into noise.

**The check has two arms, and the second is the cheap one.** Arm one is the filename list above. Arm two is
any `` `$id` `` style reference outside the agent guidance directory - one pattern, no list to maintain,
because the `$` scheme is used nowhere else in the repo. Arm two is what catches
`api/src/Binacle.Net/v4/Contracts/ExampleData.cs:63`, which arm one misses entirely. Build both, or the check
ships with a hole in it that a whole slice already fell through.

**Then the graph tools, if and when they are wanted.**

| Tool | Version | How |
|---|---|---|
| `TngTech.ArchUnitNET` + `.xUnitV3` | 0.13.3 | reads the YAML directly - rules are runtime objects |
| `dependency-cruiser` | 18.2.0 | reads the YAML from its `.cjs` config |

Three things to settle before adopting ArchUnitNET:

- **Check `.xUnitV3`'s transitive xunit dependency first.** `Directory.Packages.props:88-93` documents that
  this repo pins `xunit.v3.mtp-v2` 3.2.2 precisely because mixing the MTP v1 and v2 adapters throws
  `TypeLoadException` before a test runs. If `.xUnitV3` pulls plain `xunit.v3`, the arch leaf reproduces it.
- **Decide which graph is authoritative.** ArchUnitNET measures *type* dependencies from loaded assemblies; the
  YAML's edges came from `ProjectReference`. They disagree - `api/src/Binacle.Net/Binacle.Net.csproj:73`
  declares `<Using Include="Binacle.Geometry" />` with no reference to it.
- **The arch test project must reference every slice it inspects**, becoming a node with an edge to
  everything. Declare that exemption in the YAML, not in the test.

For dependency-cruiser, "reads it directly" is the easy half. There is no root `tsconfig.json` - there are four,
and `web/` has none despite running `ts-loader` - and imports are bare specifiers resolved through npm
workspace symlinks (`packages/binacle-net-ui/src/core/protocolDecoder.ts:4` imports `"binacle-vipaq"`), so
rules must be written against resolved real paths with symlink handling pinned.

**Say plainly which edges no tool will ever check.** `docs`/`web -> ruby` are Gemfile `path:` gems;
`web -> packages`/`vipaq` are webpack `splitChunks` regexes; `tooling -> everything` is path strings in just
recipes; `assets -> docs`/`web` is a gulp copy. No off-the-shelf tool reads any of it. Those edges belong in
the file for goal 1, but mark them documentation-only or a green run gets read as "all of this is checked".

## Phases

1. **`architecture.yml`, hand-written to be read, plus the comment check and the fifteen fixes.** The file is
   **done**; the check and the fifteen fixes are **not**, and they still ship together. A check that is red the
   day it lands teaches everyone to ignore it, so the fixes go in the same change as the check. Re-run the greps
   first - the fifteen sites were counted on 2026-08-12 and have not been re-checked.
2. **ArchUnitNET as a test leaf**, leading with the two type-level rules - the api module boundary and
   v3-frozen - not the thirty slice edges.
3. **dependency-cruiser**, config at the repo root so it sees both workspaces.
4. **lychee** for dead links.

## The dependency landscape changed under this plan on 2026-08-13

**Everything below about a pending TestsKernel move is gone, and so is the upward edge.** The packing-contract
extraction landed: `Binacle.Lib.Abstractions` was broken up, the geometry half folded into `Binacle.Geometry`,
the packing vocabulary became `shared/src/Binacle.Packing`, and the engine interfaces went into `Binacle.Lib`,
which is now the only project in `lib/src`. The fixture kernel never moved - it did not need to, because the
types it needed came down to `shared/src` instead.

**What that means for this file:**

- **The repo has no upward edge**, which is the claim this whole plan exists to be able to make. Nothing under
  `shared/` references `lib/` or `api/`.
- **Two `why` edges remain, not three** - `vipaq/tools -> lib` and `vipaq/packages -> packages`. Both sideways,
  neither upward.
- **`api/test -> lib` and `api/src/UIModule -> lib` are also gone**, along with a reference in
  `api/src/Binacle.Net.Kernel` that no file in that project used. Only `Binacle.Net` itself now names the packer,
  and only to wire it. That is a different rule from the api module boundary above, but the same idea one layer
  out - and unlike that one, a graph walk can see it.
- **`architecture.yml` is now correct - done 2026-08-13.** `shared: test: []`, `api: test: [shared]`, the
  `api: src` comment, a `lib: data` scope for the result-selection fixtures, the `vipaq: test` comment (vipaq
  has never loaded the shared kernel), and the friend-assembly rule in the preamble. The graph was re-derived
  from every `ProjectReference` in the repo and matches the file: no upward edges, no undeclared edges. **The
  tooling phases are no longer blocked.**

**Three claims earlier in this file are now stale and were left in place deliberately**, because they are the
audit's record of what was true on 2026-08-12 and the reasoning around them still reads correctly:

- "One inversion, repo-wide: `shared/test/Binacle.TestsKernel.csproj:41` references `lib/src/Binacle.Lib.Abstractions`."
  That reference is gone and so is the project it pointed at.
- "An audit of every `ProjectReference` found exactly one surprising edge, already known." That edge has since
  been removed rather than documented, which is the outcome the audit was arguing for.
- The fifteen comment sites and the counts around them were measured on 2026-08-12 and have not been re-checked
  since. Re-run the greps before relying on the number; the sites themselves are still unfixed.

**One structural change since the audit.** `shared/test/Binacle.TestsKernel` was split on 2026-08-13: its
result-selection half became `lib/test/Binacle.Lib.TestsKernel`, and the fixtures it embeds moved from
`shared/data/result-selection` to `lib/data/result-selection`. That adds a `lib: data` scope and a fourth
project under `lib/test`, both already in `architecture.yml`. It changes no edge - the new project sits on
`shared/src` like the old half did. `Binacle.Packing`'s friend grant now names `Binacle.Lib.TestsKernel`
instead of `Binacle.TestsKernel`; per the rule above, that is still not an edge.

## Decisions needed

- **Does the file state what is true today, or what is wanted?** True today is green immediately with the odd
  edges written down. What is wanted is red on day one and is a to-do list, not a gate.
- **Leaf naming.** The convention is slice, optional component, optional language, then kind spelled out. This
  leaf has no slice.
- **Where the test project lives.** `shared/test/` is the home for cross-cutting test infrastructure, but this
  tests the repo, and putting it there adds another `shared/test` edge to the graph it checks.

## Loose ends found during the audit, worth fixing whatever happens here

None of these is an illegal edge, and no boundary file would have caught any of them - they are declaration and
duplication bugs. That is worth knowing when judging how much the graph half is worth.

- `binacle-compact-notation` moves from `dependencies` to `devDependencies` in
  `vipaq/packages/binacle-vipaq/package.json`.
- `api/src/Binacle.Net/Binacle.Net.csproj:73` declares `<Using Include="Binacle.Geometry" />` with no
  `ProjectReference` - Geometry arrives transitively through `Binacle.Lib`. Breaks the day `lib` stops
  referencing `shared`.
- **The UIModule vendored assets have already drifted, not "will drift".** `beer.css`, `beer.js`,
  `beer.min.css` and `beer.min.js` differ between `assets/lib/beercss/` and
  `api/src/Binacle.Net.UIModule/wwwroot/vendor/beercss/`; only the four `.woff2` files still match. And
  `assets/lib/beercss/version` (3.11.11) has no counterpart under `wwwroot/`, so the shipped version cannot be
  determined.

## What stays unenforced

The internal layer rules for agent guidance - permanent files never pointing at ephemeral ones, docs
referencing only docs - are graph-shaped over the `$` reference scheme. A regex can ban a pattern within a
path; it cannot check that a `$` reference resolves to a file in an allowed layer. No off-the-shelf tool does.
Either it stays prose-only, or it is the one place a small custom check is worth owning.

## Watch out

- **The published documentation site and the marketing site are off limits.** If any of this needs a page
  written, record what the page must say here and leave the writing to that session.
- **Nothing is committed by an agent.** Leave every change in the working tree.
