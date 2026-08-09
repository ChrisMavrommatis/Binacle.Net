# Sonar - what is left after the 2026-08-09 sweep

**Status:** Sweep done. Rewritten 2026-08-09 against the run on `016d7478`, which is the first analysis with
the corrected scope. **509 open issues -> 305**, and the C# reduction is bigger than that looks: 24 of the 305
are new arrivals from `docs/` and `web/` coming back into scope, so ~228 were actually cleared.

| | before | after |
|---|---|---|
| Open issues | 509 | 305 |
| ncloc | 24,931 | 30,559 |
| Coverage / new coverage | 53.3% / 31.4% | 52.8% / **45.4%** |
| Duplication | 3.4% | 2.9% |
| Bugs / hotspots | 0 / 0 | 0 / 0 |
| **Vulnerabilities** | **0** | **7** |
| Security rating | A | **E** |
| Quality gate | ERROR | ERROR (`new_coverage` only) |

Read [[sonar-touching-untested-code]] before fixing anything else here, and
[[algorithm-identifier-is-a-format]] before renaming anything.

## The seven vulnerabilities, and why they only just appeared {#vulnerabilities}

**All seven are in `docs/collections/_versions/**` - the versioned sample files users download.** They were
invisible until the exclusion change on 2026-08-09, because `docs/**` was excluded whole. This is the single
most valuable thing the sweep produced, and it is exactly the argument that motivated narrowing the exclusion:
the two published sites are the only public attack surface in the repo, and they were the one thing not being
looked at.

**None of this is fixable from a coding session** - repo-root `docs/` is off limits per `CLAUDE.md`. It belongs
to the docs/web session. Written up here so it is not lost.

- **BLOCKER, `json:S6418` - FIXED 2026-08-09.** `v1.3.x/samples/docker/full-deployment/aspire-dashboard-config.json`
  shipped a GUID as `PrimaryApiKey`, and the matching `docker-compose.yml` repeated it in
  `OTEL_EXPORTER_OTLP_HEADERS`. Both now read `ThisIsAPlaceholderOtlpApiKeyPleaseGenerateYourOwn`, matching
  the `TokenSecret` placeholder style already used in `samples/docker/*/JwtAuth.json`. That is the fix S6418
  actually wants and the one [[no-sonar-issue-ignores]] prescribes: change the value so it stops looking like
  a credential, rather than hiding the finding. **The two files must always agree** - the sample breaks if
  only one is edited. This was the finding driving the security rating to E.
- **6 x kubernetes rules - FIXED 2026-08-09.** `S6865` (automounted service account), `S6864` (no memory
  limit) and `S6870` (no storage limit) on `binacle-deployment.yaml` in both `v2.0.x` and `v2.1.x`.

  **The cause was drift, not a missing decision.** `samples/kubernetes/minimal/binacle-deployment.yaml` already
  had `automountServiceAccountToken: false` and a full `resources:` block with requests and limits, added in
  `938c6d7e`. The two frozen copies under `docs/` never got it, so a reader following the published v2.0.x or
  v2.1.x instructions downloaded the unhardened manifest. Both now carry the same two blocks, comments
  included, with their own image tags untouched (`2.0.1` and `2.1.1`). Purely additive - 16 lines each, nothing
  removed.

  That also cleared six maintainability findings in the same files: `S6873`, `S6892` and `S6897` all wanted the
  `requests:` entries the same block provides. **12 kubernetes findings closed by one change.**

  **The lesson is the general one:** a fix applied to `samples/` does not reach the versioned copies under
  `docs/collections/_versions/`, and nothing enforces that it does. Worth a sweep whenever a sample changes.

Note the gate still passes on security. The BOM commit touched line 1 of those files but the findings sit on
lines 10-16, so they count as old code. The gate is not wrong - it just is not the place these will surface.

## What is left, by size {#remaining}

- **xUnit1042 (22) + xUnit1050 (10)** - `MemberData`/`ClassData` returning untyped `object[]`. The fix is
  `TheoryData<T>`, a real improvement to the ViPaq and Kernel suites, but a rewrite per data source rather
  than an edit. Biggest remaining item.
- **S101 (38) - closed as WON'T FIX.** Renamed and reverted on 2026-08-09; `_v1` lowercase is the house style
  because `GetAlgorithmIdentifierName()` emits `FFD_v2` and the fixtures parse it. See
  [[algorithm-identifier-is-a-format]]. **Mark these Accepted in the UI.** Do not attempt the rename again.
- **CA1873 (13)** guard expensive log arguments. **CA1816 (10)** the xunit `DisposeAsync` fixtures - see the
  open question below. **CA1859 (9)** concrete return types, mostly in test helpers. **CA2208 (9)** exception
  `paramName` misused as a message, which needs an exception-type decision rather than a swap.
- **S1192 (11)** - the media-type half is done (150 literals, `MediaTypeNames.Application.Json`/`.ProblemJson`,
  no new constants). Left: `box_1/2/3` in both `ExampleData.cs` (file-private consts, values unchanged so the
  OpenAPI examples stay identical), `first`/`previous`/`repeat` in `PackingVisualizer` (dictionary keys, so a
  const prevents a runtime `KeyNotFoundException` - but UIModule is 0% covered), and two canonical URLs.
- **S2325 (8) + CA1822 (13)** - the residue of the make-static sweep. 5 are the testing-fixture methods
  reverted on purpose (below); the rest are worth another pass.
- **S3776 (2)** cognitive complexity 17 vs 15 in `Auth/Token.cs` and `Program.cs`, **S2365 (1)**
  `Navbar.MenuItems` rebuilding its list on each of four reads per render, **ASP0025**, **CA1869**, and a
  thin tail of one-liners.
- **~45 TypeScript/JavaScript** modernisation items in `packages/` and `vipaq/packages/`, nearly all in
  0%-coverage files, so each fix costs new-code coverage and buys style. Do these with the UI harness.

## Decisions on record - do not redo these {#decided}

- **S101 renames** - reverted, house style is `_v1`. Mark Accepted.
- **5 testing-fixture methods** (`CommonTestingFixture.Run`, `.GetScenarioByName`, `.AssertResult`,
  `ResultSelectionTestingFixture.Select`, `.GetScenarioByName`) - static was applied, then reverted. They are
  reached as `this.Fixture.X(...)` from 60 test bodies; static forces `CommonTestingFixture.X(...)` and stops
  the tests going through the fixture at all, breaking [[tests-arrange-act-assert]]. Mark Accepted.
- **S3458 (6)** `case 0: default:` in the six `Item.Rotate` switches - `case 0` documents the identity
  orientation. Mark Accepted.
- **S1854 (3)** `newAvailableSpaces[--newSpaces] = ...` - the decrement is the index. Mark Accepted.
- **S1075 (2)** the GPL licence and GitHub URLs in the OpenAPI documents - canonical constants, and a named
  `const` does not satisfy the rule anyway. Mark Accepted.
- **javascript:S1874 + S1121** in `packages/cookies` - upstream js-cookie v3.0.5 lines, and the `escape` is
  deliberate (RFC 6265 `()` encoding). Mark Accepted, with the reason that the file tracks upstream.
- **S1135 (2)** TODO comments at INFO severity. Leave.

## Open question - CA1816 on the ten test fixtures {#ca1816-question}

Three of thirteen are done (`TimedOperation`, `TimedActivityOperation`, `PackingVisualizer`). The other ten
are xunit `IAsyncLifetime` classes whose `DisposeAsync` ends in `await base.DisposeAsync()`. Adding
`GC.SuppressFinalize(this)` to a fixture that will never have a finalizer is ceremony. Either add the line ten
times, or mark the ten Accepted with "test fixture, no finalizer". Not decided.

## Two traps this sweep produced, both worth remembering {#traps}

- **A script that prepends a line to a file relocates the BOM instead of preserving it.** The media-type pass
  inserted `using System.Net.Mime;` ahead of a BOM-carrying first line, leaving a stray `U+FEFF` stranded at
  the start of line 2 in 16 files. It survived a full build and 10,041 tests, and no BOM tool would find it,
  because position 0 was no longer a BOM. Write after the BOM, not before it.
- **Removing one redundant `?.` can introduce `CS8602`.** The S2589 fix on `EnumStringsSchemaTransformer`
  dropped `context?.` on line 27 but left it on line 14, so flow analysis still treated `context` as
  possibly-null and warned on the bare dereference. It did not show in an incremental Debug build - only a
  `--no-incremental -c Release` build surfaced it, which is what CI runs. **Verify a warning-count claim with
  a clean Release build**, not an incremental one.

## Still true, and still what the gate hangs on {#gate}

`new_coverage` is the only failing condition: **45.4% against 80%**, up from 31.4% as the August cleanup
commits age out of the rolling 30-day window. Nothing else fails. The 305 open issues block nothing -
`new_maintainability_rating` is A.

The 80% cannot be lowered: custom quality gates need the Team plan, and the project is on Free. So the gate
goes green when the UI gets tested, not by configuration - which is [[ui-test-harness]], and the reason
[[ci-gates]] says not to make coverage blocking on the PR gate yet.
