# Sonar - triage the open issues

**Status:** In progress. Opened 2026-08-09 from the 2026-08-08 run (revision `54a94b83`, 509 open issues,
88 distinct rules). One batch has landed; the rest is sorted below by what it is worth, not by count.

**Read [[sonar-touching-untested-code]] first.** None of these issues fails the quality gate - only new code
is graded and `new_maintainability_rating` is A. Fixing them is housekeeping, and fixing one in a file with
no tests actively makes `new_coverage` worse. Sort the work by the coverage of the files it lands in.

**Do not trust Sonar's effort estimates.** It rates S2223 at 380 minutes for 19 issues. Every one of them is
a one-word change. The estimates are per-rule averages and know nothing about our code.

## Done 2026-08-09 (65 issues)

| Rule | n | Fix |
|---|---|---|
| S3260 | 14 | `private class` -> `private sealed class` |
| S1481 | 21 | unused locals: `out _`, dropped assignment, `_ = checked(...)`, `var (_, x)` |
| S6678 | 18 | Serilog placeholders to PascalCase |
| S2933 | 9 | fields made `readonly` |
| CA1816 | 3 of 13 | `GC.SuppressFinalize(this)` in the three `src/` Dispose methods |

Two of those needed care and are worth remembering. Several S1481 hits were `var volume = checked(...)`
inside a FluentValidation `MustNotThrow`, where the unused variable **is** the overflow guard - deleting the
line would have removed a real check silently, so they became `_ = checked(...)`. And three camelCase braces
in the S6678 files were C# interpolation or a route pattern (`{variable}`, `{trustedProxy}`,
`{documentName}`), not log placeholders; renaming those breaks the build.

## Tier A - fix these, they are real {#tier-a}

Worth doing on their merits, not to move a number. **Landed 2026-08-09 except the three noted at the end.**

- **DONE. S2223 (19) + S2743 (6) - mutable public statics, overlapping.** These are the same six lines in `lib`
  plus thirteen more. `internal static ushort TotalOrientations = 6` sits inside the generic
  `BestFitDecreasing_v2<TBin, TItem>`, so it is both writable by anyone and **not shared between closed
  constructions** - each `<TBin, TItem>` pair gets its own copy, which is not what `static` looks like it
  says. `const` fixes both rules in one word. The rest: `Coordinates.Zero` is a `public static` field on a
  struct, so any caller can reassign the origin for the whole process; two `ActivitySource` fields; nine
  `public static string` message constants in `ServiceModule/v0/Resources.cs`; `FeatureManager.None`. All
  become `const` or `static readonly`. **Highest value per keystroke in the whole list.**
- **DONE. S4487 (2) - unread private `logger` field.** In `ApiUsageRateLimitingPolicy` and
  `AuthTokenRateLimitingPolicy` the constructor assigned `this.logger`, but the `onRejected` lambda closes
  over the **parameter** `logger` instead, so the field was dead state. Field and assignment deleted; the
  closure keeps the logger alive exactly as before.
- **DONE, and it grew. S6580 (4) - date parsed with no format provider**, in the two Sqlite repositories.
  Sonar only flags the parse, but the **write** side was `entity.CreatedAtUtc.ToString()`, equally
  culture-dependent. Fixing only the read would have made things *worse*: today both sides use the ambient
  culture and at least agree with each other, so a strict-invariant read against an ambient write breaks a
  `de-DE` machine that works now. Both sides had to move together, and they now go through one type,
  `Common/SqliteDateTime.cs`, which owns the format. Storage form is `yyyy/MM/dd HH:mm:ss`, UTC, big-endian
  so the TEXT column sorts as the instant does. `FromStorage` uses `Parse` rather than `ParseExact` on
  purpose, so rows already written in the old invariant "G" form still read and no migration is needed.
  Verified by the Sqlite-backed integration suite (107 tests), which round-trips both entities.
- **DONE. S2365 (2 of 3) - properties that copy collections.** `ConcurrentSortedDictionary.Keys`/`.Values`
  became `GetKeys()`/`GetValues()`. Both call sites were `_accounts.Values.FirstOrDefault(...)`, which
  copies the whole dictionary to find one row - the cost is now visible where it is paid.
- **DONE. S112 (1)** `ApplicationException` in `EnsureDefaultAdminAccountExistsStartupTask` became
  `InvalidOperationException`. **S1186 (2)** the two empty OpenTelemetry instrumentation hooks now carry a
  comment saying why they are empty; they are called from `ModuleDefinition`, so the seam is real and only
  the body is absent. **Check that comment reads true** - it is inferred from the call site, not from
  anyone's stated intent.

### Left out of Tier A after looking at the code {#tier-a-deferred}

- **S3776 (2) - cognitive complexity 17 vs 15.** `Auth/Token.cs` and `Program.cs`. Token's `HandleAsync` is
  a chain of guard clauses on the authentication path; Program.cs is the composition root. Both are
  refactors with behaviour risk for two points of a metric, and neither should be done in passing. Decide
  whether it is worth it at all.
- **S2365 (1 of 3) - `Navbar.MenuItems`.** The property rebuilds the list with `.Select(...).ToList()` and
  the markup reads it **four times**, so every render pays for four rebuilds. Real, but the fix is a Blazor
  render-path change (cache in a field, or hoist to a local in the markup) in a component with no tests.
  Worth doing with the UI harness, not before.
- **S1075 (2) - moved to Tier D.** The two "hardcoded URIs" are the GPL licence URL and the project's
  GitHub URL in the OpenAPI documents. They are canonical constants, not configuration, and extracting them
  to a named `const` does not satisfy S1075 anyway - the rule is about the literal, wherever it lives.
- **javascript:S1874 + S1121 (2) - moved to Tier D.** Both are in `packages/cookies`, which
  `package.json` describes as "based on js-cookie v3.0.5", pinned to the upstream version. Both flagged
  lines are upstream code, and the `escape` call is deliberate there - it encodes `()` per RFC 6265, which
  `encodeURIComponent` does not. Changing them diverges a fork from the library it tracks, for style.

## Tier B - mechanical and safe, do in batches {#tier-b}

Each batch is one decision then repetition. Build and run the suites after each.

- **DONE, 69 of 82. S2325 (50) + CA1822 (47) - "make this method static".** Applied by adding `static` at
  every flagged declaration, then letting the compiler find the call sites: CS0176 says exactly where an
  instance reference is now illegal, so nothing depends on a search being complete. Two rounds, 18 then 84
  errors - the second wave only appeared once `Binacle.TestsKernel` built again, which is a reminder that a
  green build on a broken dependency graph is not evidence.

  Two follow-on edits came out of it. `ScenarioReader` in both TestsKernel namespaces had only
  `ReadScenarios`, so once that went static the `new ScenarioReader()` in each provider was dead - removed,
  call qualified with the type, and both classes made `static class`. Leaving them instantiable would have
  traded this finding for a fresh S1118.

  **The five `[GlobalCleanup]` methods were removed instead, and that was the important one.** All five
  benchmark bases had an empty `public void GlobalCleanup() { }`, which is why they were flagged - a method
  with no body accesses no instance state. Making them `static` compiles and is silently wrong:
  **BenchmarkDotNet only discovers `[GlobalCleanup]` on instance methods**, so a static one is never called.
  Benchmarks are not in any test suite, so nothing here would have caught it. They were empty, so they are
  gone entirely, which settles the finding and the hook in one move. A sweep of every other method turned
  static in this batch found none sitting under an attribute, so these five were the only framework hooks
  at risk.

  **The lesson worth keeping: `static` is safe for a method the compiler resolves, and a trap for one a
  framework discovers by reflection.** Attribute-driven hooks - `[GlobalSetup]`, `[GlobalCleanup]`, xunit
  lifecycle, model binders - are found by signature, and the compiler will not tell you when the signature
  stops matching. Check what discovers a method before making it static.

  **5 reverted, on purpose:**

  - **5 reverted after the compiler objected** - `CommonTestingFixture.Run`, `.GetScenarioByName`,
    `.AssertResult` and `ResultSelectionTestingFixture.Select`, `.GetScenarioByName`. These are reached as
    `this.Fixture.GetScenarioByName(...)` from 60 test bodies. Making them static forces
    `CommonTestingFixture.GetScenarioByName(...)` at every call site, which stops the tests going through the
    fixture at all and breaks the arrange/act/assert shape in [[tests-arrange-act-assert]] - a pattern the
    maintainer asked for directly. **The rule is wrong about this design.** Mark the five Accepted in the UI.

### What the v3 freeze actually covers {#v3-freeze-scope}

**Decided by the maintainer, 2026-08-09.** The eight v3 sites were held back at first, then applied after a
direct ruling: *"as long as it works it's fine, it doesn't change the contract for an outsider."*

That draws the line in a useful place, and [[v3-frozen]] should be read this way. The freeze protects the
**published contract** - routes, request and response shapes, status codes, behaviour a client can observe.
It does not fence off the implementation behind it. `internal async Task<IResult> HandleAsync` becoming
`internal static async Task<IResult> HandleAsync` changes nothing any caller can see.

Two v3 files had already been edited in the S1481 batch before this came up - a dead `enumValues` local in
`v3/Contracts/Algorithm.cs`, and two `var volume = ...` turned into `_ = ...` in `v3/Contracts/IWithItems.cs`.
Both are behaviour-preserving; the `checked` and `Sum` expressions still run, which is the whole point of
those `MustNotThrow` validators. They fall inside the line drawn above.
- **WON'T FIX. S101 (38) - `BestAlgorithm_v1` and friends.** Renamed to `BestAlgorithmV1` on 2026-08-09,
  then **reverted the same day** on a maintainer ruling: `_v1` lowercase is the house style everywhere, with
  no exceptions. The reason is [[algorithm-identifier-is-a-format]] - `GetAlgorithmIdentifierName()` emits
  `FFD_v2`, the baseline fixtures store it, and `AlgorithmInfoHelper` parses it by splitting on `_`. Every
  version suffix in the codebase matches that format on purpose, and matching a C# naming rule instead would
  make the code disagree with its own data.

  **These 38 are marked Accepted in the SonarCloud UI.** That is the only answer available - a custom quality
  profile needs the Team plan, so the rule cannot be switched off ([[no-sonar-issue-ignores]]). Do not attempt
  this rename again; the finding is answered, not outstanding.

  Worth keeping from the attempt, because it is what made the revert cheap and safe. Nothing is published -
  no `dotnet pack`, no `nuget push`, no `IsPackable` - and **the type names appear in no string literal**: no
  `nameof`, no `GetType().Name`, nothing in `shared/data`, `results/` or config. A rename was therefore purely
  mechanical in both directions. Had any name been reachable as text, the forward rename would have changed
  behaviour while every test still compiled.

  Also learned: a blanket `_v1` -> `V1` would have broken things regardless. Around 35 other `_v1`/`_v2`
  identifiers exist - the `BFD_v1`/`FFD_v1`/`WFD_v1` constants, and benchmark names like
  `OR_Library_Packing_WFD_v2` that the `results/` ledgers key on.

- **S1192 (30) - repeated string literals** to constants. Needs a naming and placement decision per cluster,
  then it is mechanical. Ten of the thirty are in 0%-coverage files.
- **xUnit1042 (22) + xUnit1050 (10)** - `MemberData`/`ClassData` returning untyped `object[]`. The fix is
  `TheoryData<T>`, which is a genuine improvement to the ViPaq suites, but it is a rewrite of each data
  source rather than an edit.
- **CA1873 (13)** guard expensive log arguments, **CA1816 (10 remaining)** see the open question below,
  **CA2208 (9)** exception `paramName` misused as a message - that one needs an exception-type decision,
  not a mechanical fix. **CA1859 (9)** concrete return types, **CA2211 (7)** non-constant public fields.
- **S1117 (7)** parameter hides a field, **S3241 (7)** return value nobody reads, **S4136 (5)** overloads not
  adjacent, **S2326 (5)** unused generic parameter, **S3928 (4)** `paramName` naming, **S3881 (4)** dispose
  pattern, **S3442 (3)** constructor visibility, **S8970 (2)** null-forgiving where nullable is off,
  **S1104 (2)** public field to property, **S3246 (1)** missing `out` for covariance.
- **One-liners:** CA1860 (5) `.Any()` to length check, CA1861 (5) constant arrays to `static readonly`,
  CA1854 (4) `TryGetValue` over `ContainsKey`, S1066 (3) merge nested `if`, CA1510 (2) `ThrowIfNull`,
  CA1850 (2) `SHA256.HashData`, S1125 (2) redundant booleans, S2589 (2) unnecessary null check, S3267 (2)
  loop to `Where`, CA2254 (2) varying log template, CA1866 + S6610 (2) `StartsWith(char)`, CA1847 (1)
  `Contains(char)`, S2971 (1) fold `Where` into `FirstOrDefault`, S2629 (1) interpolation in a log template,
  S3358 (1) nested ternary, S3604 (1) redundant initializer, S927 (1) parameter name vs base, S1172 (1)
  unused parameter, S1118 (1) static class, ASP0025 (1) `AddAuthorizationBuilder`, CA1869 (1) cache
  `JsonSerializerOptions`.
- **shelldre:S7688 (4)** - `[` to `[[` in the `config/` shell scripts. Trivial, and those scripts are being
  converted to `just` recipes anyway ([[scripts-to-just-recipes]]), so it may resolve itself.

## Tier C - TypeScript and JavaScript modernisation (~45) {#tier-c}

Almost all in `packages/` and `vipaq/packages/`, and almost all in files at 0% coverage, so every fix here
costs new-code coverage and buys style. Low priority, and better done alongside the UI test harness.

`javascript:S7761` (7) prefer `.dataset`, `typescript:S1444` (8) public static should be readonly,
`S6557` (4) `startsWith`, `S7772` (4) `node:fs`, `S7773` (3) `Number.parseInt`, `S6582` (3) optional chain,
`S7755` (2) `.at()`, `typescript:S2933` (2) readonly member, `S7758` (2) `codePointAt`, `S4138` (3)
`for-of`, `S7769` (2) `Math.hypot`, `S1128` (2) unused import, `S7754` (1) `.some()`, `S7781` (1)
`replaceAll`, `S6647` (1) useless constructor, `typescript:S1854` (1) dead assignment, `S7726` (1) unnamed
function, `S5906` (1) and `S5914` (1) assertion style in the ViPaq TS tests.

**javascript:S3504 (5)** - `var` instead of `let`/`const` in the UIModule's own `wwwroot` JS. Trivial, and
that code is ours rather than vendored, so it is the one item here worth doing early.

## Tier D - leave them, or mark Accepted in the UI {#tier-d}

We are on the Free plan, so a rule cannot be switched off ([[no-sonar-issue-ignores]]). The only honest
answers are a code change or marking the individual finding Accepted with a reason.

- **S3458 (6) - "remove this empty case clause".** The code is `case 0: default:` in the six `Item.Rotate`
  switches, where orientation 0 and the fallback are deliberately the same branch and `case 0:` documents
  which one is the identity. Removing it satisfies the analyser and loses the point. Mark Accepted.
- **S1854 (3) - "useless assignment to `newSpaces`".** The statement is
  `newAvailableSpaces[--newSpaces] = ...`; the decrement is the index. Only the final write to the variable
  is dead, and there is no way to remove it that reads better. Mark Accepted.
- **S1075 (2) - hardcoded URIs.** The GPL licence URL and the GitHub URL in the OpenAPI documents. Canonical
  constants, and a named `const` does not satisfy the rule anyway. Mark Accepted.
- **javascript:S1874 (1) + S1121 (2 lines) - `packages/cookies`.** A tracked fork of js-cookie v3.0.5. Both
  are upstream lines and `escape` is deliberate (RFC 6265 `()` encoding). Mark Accepted, and say in the
  reason that the file tracks upstream.
- **S1135 (2) - TODO comments.** INFO severity. They are tracked work, which is what a TODO is for.
- **S125 (4) - commented-out code.** Judgement per site; some is explanation, some is leftovers.
- **shelldre:S1192 (1)** - `cd ./config` 12 times in a shell script. Superseded by
  [[scripts-to-just-recipes]].

## Open question - CA1816 on the ten test fixtures {#ca1816-question}

Three of the thirteen are done. The other ten are xunit `IAsyncLifetime` classes in
`ServiceModule.IntegrationTests` whose `DisposeAsync` ends in `await base.DisposeAsync()`. Adding
`GC.SuppressFinalize(this)` to a test fixture that will never have a finalizer is ceremony of exactly the
kind [[no-sonar-issue-ignores]] rejects elsewhere. Either add the line ten times and stop thinking about it,
or mark the ten Accepted with "test fixture, no finalizer". Not decided.

## An observation that is not a Sonar finding {#rotate-cycle}

While reading `Item.Rotate` for S2743: `TotalOrientations` is 6, and the guard is
`if (this.currentOrientation >= TotalOrientations) this.currentOrientation = 0; else this.currentOrientation++;`.
Starting from 0 that cycles 0,1,2,3,4,5,6,0 - seven steps, with 6 falling through to `default`, which is the
same branch as `case 0`. So the identity orientation appears twice per cycle. That may be intended or may be
an off-by-one; all 8679 lib unit tests pass either way, and changing it changes packing results. **Flagged,
not touched.** Someone who knows the algorithm's intent should decide, and if it is intended it deserves a
comment saying so.
