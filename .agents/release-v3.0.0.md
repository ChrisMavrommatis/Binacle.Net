---
description: Release - Binacle.Net v3.0.0
---

# Release - Binacle.Net v3.0.0

**Status:** In progress. Betas 1, 2 and 3 published, verified and deployed. The pipeline is rebuilt and proven
end to end. The architecture branch is merged, the suite is green and the OpenAPI documents are proven unmoved.
**The repository moved to the `binacle-labs` organization on 2026-08-16, mid-release**, and `v3.0.0-beta.3`
proved the whole pipeline under the new owner. What is left is the last commit and the tag - plus the items
that run alongside and hold nothing up.

**Created:** 2026-07-16. **Rewritten for the GHCR pipeline:** 2026-08-11. **Scope reset:** 2026-08-14 - the
maintainer proposed eight more items; five came in, three were refused with reasons recorded below.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to
the reference rules: it may point at any file to coordinate the release, and **nothing points back at it**.
Delete it once v3.0.0 is out.

Companion: `post-release-v3.0.0.md` - the checks to run once the release is out.

**The org move, in one paragraph.** The repository moved from `ChrisMavrommatis/Binacle.Net` to
`binacle-labs/Binacle.Net` on 2026-08-16, while this release was in flight. It is done and proven end to end;
nothing on the gate waits on it. What outlives the release is in `.agents/design/decisions.md` - what moved,
what deliberately did not, and the three signing identity bands. The one thing that shapes work still open
here: **`3.0.0-beta.3` is the only image that verifies under the current identity**, so it is the tag to name
wherever an example needs a real one until v3.0.0 is out.

---

## The scope decision - 2026-08-14

Eight items were proposed. Each was read against its own plan. **Seven ship, and the eighth ships in part.**
What is left out is named below, and each piece of it carries a trap or a dependency that a release week is the
worst time to meet.

### Ships

| Item | Why it earned a place |
|---|---|
| **Rate limiter tests** | Two of this release's claims rest on behaviour nothing tests. |
| **Rate limiting owned by the ServiceModule** | The durable fix for the bug the two-guard transformer patches. Pulled in on 2026-08-14 at the maintainer's call. |
| **Image verification** | The release advertises signing, SBOM and provenance. Today no user can check any of it. |
| **The Docker Hub page** | It advertises 2.1.1 as latest. The tag is what makes it wrong rather than stale. |
| **The PR gate change** | Image build + OpenAPI lint + **three architecture checks**. One workflow edit, everything green on arrival. |
| **The client-generation page** | The spec is published and nobody knows they can generate a client from it. One docs page, and it applies to every version. |
| **More ViPaq interop vectors** | Four rows of fixture data. Cheap, and the format froze in this release so they will not need redoing. |
| **The compose stacks** | Pulled in on 2026-08-15, after the scope reset. Every decision is taken and the compose behaviour is tested, so it is one sitting with nothing left to figure out. |

### Does not ship, and why

| Item | The blocker |
|---|---|
| **The heavy architecture tools** | ArchUnitNET, dependency-cruiser and lychee. **Three lighter checks ship instead** - see the PR gate section. What is deferred is everything that needs a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose - the mismatch throws before a single test runs. dependency-cruiser has no root `tsconfig.json` to work from; there are four, and `web/` has none. |
| **CI gates 2 and 3** | **Deferred at the maintainer's call, 2026-08-14.** Gate 2 runs the all-modules integration tests, which are not being written here. Gate 3 is Sonar and coverage, and its own plan says **do not make coverage blocking yet** - the condition is red before anyone writes a line. Gate 1 ships; these two have nothing to gate. |
| **Raising test coverage** | **Decided 2026-08-14: do not test the Blazor UIModule.** The Alpine port deletes most of what would be tested, so writing bUnit tests now means writing them twice, in two languages. The port goes first. What is left that could be tested today is 612 lines of TypeScript, which does not move an 80% new-code gate on its own. **What ships here is the modest bump the rate limiter tests bring, and nothing more.** |

---

## How to work this file

**Two lists.** The gate is what must be green before the tag. Everything under "Runs alongside" is real work
that does **not** hold the tag - if one is not ready, the release goes without it.

Each row is either a **link to a plan** that holds the whole item, with a line saying what that plan is, or a
**self-contained checklist** where the item is small enough that a separate file would be overhead.

**When a plan lands, its file is deleted.** Tick the row here and drop the link in the same change. Otherwise
this index rots into dead links.

**How the pipeline works**, because it shapes every item below. Every build is staged on GHCR, smoked there,
and the smoked digest is **copied** to Docker Hub - so nothing unsmoked reaches the registry users pull from.
A prerelease gets its immutable tag only, never `3.0` or `latest`. The release body is extracted from
`CHANGELOG.md` by the workflow.

---

## The gate - all of this before the tag

| # | Item | State |
|---|---|---|
| 1 | Rate limiter tests | **done - 2026-08-14** |
| 2 | Rate limiting owned by the ServiceModule | **done - 2026-08-14** |
| 3 | The Azure Storage run | **done - 2026-08-14, and now on the PR gate** |
| 4 | Beta 3 | **published 2026-08-16** - the pipeline half is proven, the deployment checks are open |
| 5 | The last commit: pins, prose and the changelog rename | open |
| 6 | Tag `v3.0.0` | open |

### 1. Rate limiter tests - done 2026-08-14

**Built in `api/test/Binacle.Net.ServiceModule.IntegrationTests/RateLimiting/`**, four tests, whole suite green.
The module-off half is there too, so the pair proves limiting belongs to the module - which is what the `429`
OpenAPI guard claims and nothing executable said before.

The route list is **derived from the route table**, not typed out: every POST under `/api/v3` and `/api/v4`, 18
of them, matching the 18 rate limited endpoints in the source. Dropping the attribute from one
endpoint fails the test with that route named - checked by mutation, along with disabling each limiter.

Only the module-matrix plan it was carved from stays open, minus this finding.

### 2. Rate limiting owned by the ServiceModule - done 2026-08-14

The 18 core endpoints now call `.RateLimited()`, a Kernel marker naming no policy. The ServiceModule registers
an `IEndpointConvention` that turns the marker into `[EnableRateLimiting("ApiUsage")]`, and the Kernel's
endpoint registrar runs every registered convention inside one `Finally` - so no module can hit the trap where
an `Add` convention reads metadata before the endpoint has written it.

**The feature-flag guard is gone and the `429` is safer for it.** Only the module attaches the attribute, so a
module-off build has nothing for the transformer to find. The decisions ledger is rewritten to say so, and to
say that the old second guard was load-bearing at the time rather than always redundant.

Proven, not assumed: the module-on host reports policy `ApiUsage` on a marked endpoint and none on `presets`,
its `/openapi/v4.json` carries the 14 endpoint `429`s, the rate limiting tests pass on the new path, and both
generated module-off documents are byte-identical to what they were before the change - so the frozen v3
contract did not move.

`.RequireRateLimiting("AuthToken")` on the ServiceModule's own token endpoint stays as it is.

**The ServiceModule simplification idea inherits a smaller surface**, which does not make that direction
decided. It is on the board.

### 3. The Azure Storage run - done 2026-08-14

Run locally against Azurite: 111 passed, 1 skipped (the SQLite pragma test, which skips itself off its
backend). Postgres re-run green alongside it.

**It is no longer a hand-run.** `run-tests.yml` now brings up an Azurite service container and runs a third
ServiceModule step, so every PR exercises all three backends - which is what made this a gate item in the
first place. The local runner is unchanged: `just test all` stays the set that needs nothing brought up, and
a backend leg is still a deliberate `just test api-service-integration <backend>`.

### 4. Beta 3

**Why there is one.** The architecture merge changed shipping code, so beta 2's image evidence describes a tree
that is not the one shipping. Same reasoning that produced beta 2 out of beta 1, and it costs one tag.

**It is narrower than beta 2 was.** Beta 2 proved the rebuilt pipeline; that is proven. Beta 3 exists to put
the restructured tree on a real host and pay two live checks owed from beta 2.

**Correction, 2026-08-14.** An earlier draft justified this on `Auth/Token.cs` being the most restructured
shipping file. **The merge does not touch that file at all** - verified against the diff. The two checks below
are debts from beta 2, not consequences of the merge. Do not scope this as a re-audit of the restructure.

**Published 2026-08-16, and it paid for itself twice.** The tag also became the proof of the org move: the whole
pipeline ran under the new owner, `just image verify 3.0.0-beta.3` passed all four checks, and the command
printed in `SECURITY.md` passed verbatim from a clean shell. It is now the reference tag - **the only image
that verifies under the current identity**.

- [x] **Cut it from the merge commit.**
- [ ] **Exercise the auth token endpoint.** `ServiceModule/v0/Endpoints/Auth/Token.cs` - its rejection chain is
      one extracted `Reject` helper, and a wrong branch returns the wrong status code to a real client, which
      no unit test shape catches as well as one live call.
- [ ] **Re-confirm the resolved caller** once the forwarded-headers source header settles. During beta 1 it
      moved between boots (`CF-Connecting-IP` on one, `X-Forwarded-For` on a later one). The resolved caller
      was correct whenever observed, but the two are not equivalent behind a CDN and the health check
      allow-list is compared against whatever they resolve to.
- [ ] **Confirm the image runs and the version stamp reads `3.0.0-beta.3`.** The pipeline smokes the GHCR copy
      before anything is copied, so a broken image cannot reach Docker Hub. This is the deployment half, which
      the pipeline does not see.

**What beta 3 does not re-do:** everything in "Already verified", plus every structural thing beta 2 proved -
the six jobs, the digest-preserving copy, the signature on both registries, the SBOM and provenance, the
release body extraction. None of it moves because a namespace did.

### 5. The last commit before the tag - all in one

- [ ] **Rename `## [Unreleased]` to `## 3.0.0`** in `CHANGELOG.md`.
- [ ] **Move eight pins from `3.0.0-beta.1` to `3.0`:**

  | File | What to change |
  |---|---|
  | `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` | the `image:` line |
  | `samples/kubernetes/minimal/binacle-deployment.yaml` | the `image:` line |
  | `samples/README.md` | the pin paragraph |
  | `samples/docker/README.md` | the pin paragraph |

  **`README.md` was moved early, on 2026-08-17**, when the beta names came off the public surfaces. Its pin
  warning already reads `binacle/binacle-net:3.0`, which does not exist on Docker Hub yet - so **the repo
  landing page currently breaks the rule below**. One decision, either way: revert it until the tag, or accept
  it and note that the rule now has an exception. It is the most read file in the repo.

- [ ] **The prose goes with the number, and it is more than nine files.** **Corrected 2026-08-14 - an earlier
      draft claimed the comment above each `image:` line was already dropped. It is not.** All six compose and
      manifest files still carry two extra comment lines: *"Pinned to the beta patch for now because
      `binacle/binacle-net:3.0` does not exist on Docker Hub yet - move to the 3.0 minor tag once v3.0.0 is
      published."* Delete those two lines in all six, leaving only *"Pinned on purpose - a copied sample must
      not jump to a new major on the next pull."* The three READMEs carry the same reason in prose. **That
      reason expires the moment v3.0.0 publishes.**
- [x] **Done 2026-08-17 - the two that named a beta as an example.** `tooling/README.md` and
      `tooling/smoke.just`, both showing "smoke what is actually on Docker Hub", now say `3.0.0`. They name a
      tag that does not exist until the tag lands, which is the same early-move trade as `README.md` above and
      resolves the moment v3.0.0 publishes.
- [ ] **Re-confirm `ApiV4Document.IsExperimental` is still `true`.** Shipping v4 as stable would lock contracts
      meant to keep moving. The flip is 3.1.0 work.
- [ ] **Preview the body:** `just changelog extract 3.0.0` after the rename. That is exactly what publishes.

**The rule that drives the pin timing: a pin on `main` must name an image that exists on Docker Hub.** The pins
sit at `3.0.0-beta.1` through the whole sequence, beta 3 included. They moved early once before, on 2026-08-07,
and sat on `main` naming an image that did not exist. **Do not leave the `3.0` bump on `main` long before
tagging.**

### 6. Tag

- [ ] **Tag `v3.0.0`.** The pipeline does the rest: the changelog gate, the suite, the GHCR build, the smoke,
      the Docker Hub copy under all three tags, the signature, and the release created from the `3.0.0`
      section. **Nothing here is manual any more.** Watch the run, then check the rendered body and
      `docker buildx imagetools inspect`.

---

## Runs alongside - real work, does not hold the tag

Each is cheaper now than after. **If one is not ready, the release goes without it** - with one exception,
named below.

### Image verification

**Done 2026-08-15, except the Docker Hub page.** `just image verify <version> [check]` runs the four checks,
all four proven green against `3.0.0-beta.2` and watched to fail against `2.1.1`. Four of the five surfaces
that tell users about it are written: `CHANGELOG.md`, `SECURITY.md`, `README.md` and the docs-site page. **The
plan file is gone** - what survived it went to the ci-cd decisions ledger (the one verify invocation, what
changes it, and the beta 2 floor) and the tooling reference doc (the checks and the two just traps).

**Re-proven 2026-08-17 against `3.0.0-beta.3`, and the beta 2 results below are now history.** The org move
re-keyed the certificate identity the recipe matches on, so **beta 2 no longer passes `signature`** - it is
signed, under the old owner. Every "green against `3.0.0-beta.2`" in this section is a record of what was
true then. **`3.0.0-beta.3` is the tag to run anything against**, and `2.1.1` is still the one to watch fail.

**Reduced from five checks to four, later the same day.** The maintainer's call: **only the release workflow
touches GHCR.** The `digest` check compared the tag across both registries, so it went, and the signature
check is Docker Hub only. Re-run green against `3.0.0-beta.2` after the change. The reasoning is in the ci-cd
decisions ledger, along with what still carries the property that check used to prove.

**The one thing left is the Docker Hub page**, and it belongs to that page's own work below.

- [x] **The two surfaces a coding session owns.** Done 2026-08-15. `SECURITY.md` has a "Verifying a Release"
      section - what is signed, both commands version-neutral, why both cosign flags matter, and that a pass
      means it came from the release workflow and **not** that the image is free of vulnerabilities.
      `README.md` has one line under the pin note pointing at it, no commands.
- [x] **The docs-site page is written**, 2026-08-15, at the maintainer's explicit go-ahead to touch repo-root
      `docs/` for it - `docs/collections/_versions/v3.0.x/verifying-a-release.md`, registered by front matter
      `nav.order: 8` and confirmed with a clean `jekyll build`. **It made the deadline**: the docs deploy runs
      once, straight after the tag, and a surface that misses it waits for the next one. Every runnable block
      uses a `<version>` placeholder or `3.0.0-beta.2`; `3.0`, `latest` and `2.1.1` appear only in prose.
- [x] **The placement is decided - 2026-08-14, and built there 2026-08-15.** It is in the `image` module:
      `just image verify <version>`, with an optional check name, and the checks as private helpers.

      **The reasoning, because it changes another item too.** The maintainer's call was that `image` is for the
      image, and the supporting services it currently stands up - Postgres, Azurite, the telemetry collector -
      belong in `serve`, which is the local dev toolkit. That makes `image` mean "the image", local and
      published, which is exactly the module a verification recipe belongs to. **The stack move came into this
      release on 2026-08-15** and landed first; this recipe went in on top of it, and the module
      header now carries both - the stacks that run `binacle-net:local`, and the one recipe that reads a
      registry instead.
- [x] **The recipe is built and every check is proven.** Version required and never defaulted, no
      `docker login` anywhere, and all of them run even when one fails. `cosign` 3.1.3 is in `DEVELOPMENT.md`,
      pinned, the same single-binary shape as the smoke tools. Green against `3.0.0-beta.2` on Docker Hub, and
      watched to fail against `2.1.1` - `no signatures found`, no SBOM, and the 172 System dlls of the old
      self-contained build.
- [ ] **Write the Docker Hub page's verification section from the same wording.** `SECURITY.md` is the source
      now; that page's own plan says to copy it rather than edit its draft.

**One ordering constraint that binds every surface, and the org move tightened it.** Any example naming a tag
must name a tag that **passes today**. That is now a shorter list than "signed": signing started with beta 2,
but the move re-keyed the certificate identity, so `3.0.0-beta.2` fails too and only `3.0.0-beta.3` passes.
`3.0` and `latest` do not point at a signed image until v3.0.0 publishes.

**The floors were moved to `3.0.0` on 2026-08-17, before the tag rather than after it** - the maintainer's
call, taken with the decision that no public surface names a beta image at all. `SECURITY.md`, `README.md`,
`tooling/README.md` and `tooling/image.just` all read `3.0.0`. **That closes what used to be a post-release
item**; the only surface left is the worked example on the docs-site page, which quotes real output and is in
the docs-deploy checklist below.

### The Docker Hub page

**[ci-cd/dockerhub-overview](plans/ci-cd/dockerhub-overview.md)** - a large plan covering the credential test,
the file, the workflow that pushes it, the full page draft, the logo and the categories. The page advertises
`2.1.1` as latest and hand-lists fifteen tags, none of them 3.x. For a lot of people it is the first thing they
read about the project.

- [ ] **Test the credential first - five minutes, and it decides the whole item.** The plan has the exact
      calls. **Two traps in it.** Test `POST /v2/auth/token`, which is what the action calls - not the legacy
      `/v2/users/login/`, whose 403s would condemn the plan for no reason. And **back the page up, then PATCH
      the current text straight back**, never a placeholder: a green result and a defaced public page in the
      same second. Record the answer and the date in the plan either way.
- [ ] **Write `.github/dockerhub-overview.md`, with a placeholder wherever a version appears.**
- [ ] **Add the page update to the release workflow** - **decided 2026-08-14, and it changed the design.** The
      page is published by the release run, in the same place the release notes are posted. **Not** by a
      workflow triggered on a push to the page's own path, which was the earlier draft.

      **Why it is better.** The page describes the tags a release writes, so the release is the moment its
      content becomes true. It also kills a trap outright: with a path trigger, landing the file *is* publishing
      it, so the file could never sit on `main` waiting for a tag.

      Three constraints on the step: **run it last**, after the Docker Hub copy and signature have succeeded,
      with nothing depending on it - a cosmetic failure must not redden a release that shipped a correct image.
      **Gate it on a non-prerelease**, the same rule the moving tags already follow, so a beta never rewrites a
      page describing the stable line. And **keep `workflow_dispatch`**, so a typo is fixed without cutting a
      release.
- [ ] **Substitute the version at publish time.** Keep concrete version numbers out of the committed file. This
      is what stops the page rotting - without it the file names `3.0` forever and is wrong the day 3.1.0 ships.
- [ ] **The logo and the categories** - pure web form, nothing gated.
- [ ] **Take the verification section's wording from the image verification work**, not a second draft of it.

**The file can land on `main` whenever.** Nothing publishes it until a release runs. That is the whole point of
the trigger change, and it means this item no longer has to be finished in any particular order against the tag.

### Docker Hub tag immutability - the rule only

**Small enough to be self-contained. The plan that holds the rest stays on the board**, because the part worth
having later is the switch and that is not a release item.

Read from the repository API on 2026-08-13: `"immutable_tags_settings": { "enabled": false, "rules": [".*"] }`.

**The trap: a rule marks matching tags immutable - it does not exempt them.** So `".*"` would freeze **every**
tag, `latest` and `3.0` included, and those two are designed to move.

- [ ] **Confirm the setting is offered** in the repository's settings UI. It appears in the API response, so
      plan availability is probably not the blocker, but the sponsored org's entitlements decide it.
- [ ] **Correct the rule to `^\d+\.\d+\.\d+$`** - released versions only. Not full semver: `3.0.0-beta.2` was
      re-cut on 2026-08-13, that is a normal thing to do to a beta, and a prerelease-matching rule would have
      blocked it with the release half shipped. Read the value back from the API rather than trusting the form.
- [ ] **Leave the switch off until after v3.0.0.** Turning it on with a wrong rule fails the publish job
      *after* the image has been built, smoked and copied - a red at the last step of an otherwise good
      release, with the moving tags half written. **There is no version of this worth risking the release for.**

### The PR gate - one workflow change

**Everything below lands in `run-tests.yml` in one change.** They all add to the same gate, they are all ready,
and doing them separately means touching that file five times and arguing about job ordering five times.
**Every one lands green**, which is the state a new gate wants - a check that is red on arrival teaches everyone
to ignore it.

- [ ] **Build the image on every PR.** The step is `just build image` - publishes and builds with no push, no
      login, no `sudo`, nothing interactive. Today the image is built in CI only when a release is published,
      so a PR never proves the image still builds, and a break is found at release time. That is exactly what
      happened after the `Binacle.Geometry` extraction, where the image had not been built for the whole
      restructure. **Use the same Dockerfile and publish arguments the release workflow uses, or the gate
      proves nothing** - the release path now goes through `just build publish`, so it does. Confirmed to
      build clean on 2026-08-17.

      **It costs a second image build on every release, and that is the price.** The release workflow calls
      `run-tests.yml` as its "this commit passed CI" gate, and that file takes no inputs on purpose - the
      release needs the same run a PR gets, not a variant of it. So a release run builds the image in the gate
      and again in the release job. Making the step conditional to save a minute is what turns the gate back
      into something a release does not exercise.
- [ ] **Lint the OpenAPI documents.** One step: `just openapi lint`. It generates the documents itself and needs
      nothing brought up. **Set it to fail on warnings** - the `servers` block landed on 2026-08-10 and the lint
      is clean at 0 errors and 0 warnings, confirmed again on 2026-08-14 and on 2026-08-17, so there is nothing
      left to argue about.

      **The recipe needs `--fail-severity=warn` for that, and the flag is the whole item.** Spectral's default
      is to exit 0 on warnings and fail only on errors, so the step as it stands prints `No results with a
      severity of 'error' found!` and passes whatever the warnings say. Adding the step without the flag looks
      exactly like a working gate.
**Then three architecture checks**, from **[architecture-boundaries](plans/architecture-boundaries.md)** - a
large plan whose file half already shipped on the merged branch along with all 27 comment fixes.

**Why these three and not the graph tools.** The branch built the foundation and stopped: `architecture.yml`
exists, the repo now conforms to it (no upward edges, the graph re-derived from every `ProjectReference` to
prove it), and the 27 comment sites are fixed. **What does not exist is anything at all that reads any of it.**
A declarative file nothing checks is a lockfile - regenerated, never read - which is the exact failure the
plan's own first goal warns about.

All three below **read files the repo already has, need no new dependency, and are green today.** That is the
whole selection rule. Everything needing a new toolchain is deferred.

- [ ] **The comment check.** A `just` recipe plus a step, not Semgrep. **Three arms, each blind to the other
      two:** a derived filename list, `$id`-style references matched against the ids the docs declare, and bare
      ref codes matched against the headings that define them. **Derive every list, never hardcode it** - and
      fail loudly when a derived list comes back empty, because an empty list makes the check report clean
      forever. **Two shell traps, both of which produced a green run over 14 real violations while the prototype
      was written:** `xargs` returns 123 when any grep batch finds nothing, so test the output for emptiness and
      never the exit status; and six algorithm folders have spaces in their names, so `-d '\n'` is not optional.
- [ ] **The graph check - this is the one that stops `architecture.yml` rotting.** Re-derive the edges from
      every `ProjectReference` in the repo and compare them against what the file declares. Fail on an
      undeclared edge and on a declared entry nothing uses. **A script already did exactly this once**, when the
      file was written, and came back clean - no undeclared edges, no dead entries, no cycles. It was not kept.
      Keep it this time.

      **Two things it must handle.** A bare target in the file means that slice's `src`, so resolve it before
      comparing or the declared graph reads as cyclic. And **the project graph is not the whole graph** -
      `vipaq/tools` reads `shared/data` by resolving a path at run time, which no `ProjectReference` audit sees.
      That edge is already declared; the check just must not report it as dead.
- [ ] **The `InternalsVisibleTo` check.** Every grant must name an assembly that references the granter,
      directly or transitively. **A grant that does not is dead weight - it grants access nobody can take.**
      Confirmed on 2026-08-14: **19 grants across 9 granting projects, all passing.** It has already caught one
      dead grant, to a test project that never touched the internals, since deleted. **Cheaper than any of the
      graph tools and it found something on its first run.**

      **The trap: expand `$(ProjectName)` first.** Most grants are written `$(ProjectName).UnitTests` rather
      than spelled out, so a naive string compare matches nothing and the check passes for the wrong reason.

- [ ] **Do not go looking for violations to fix first.** All three land green. Confirm each catches nothing,
      then keep it that way.

**What is deliberately left out of this bundle.** The global-`Using`-with-no-`ProjectReference` check would be
**red** - there are 19 such declarations across 13 projects, every one resolves transitively today, and whether
the fix is 19 added references or a decision that transitive resolution is fine here **has never been settled.**
A check that lands red on a question nobody has answered is the thing this whole bundle is shaped to avoid.

**Gates 2 and 3, and the heavy architecture tools, do not come with this** - see the scope decision at the top.

### More ViPaq interop vectors - done 2026-08-17

**Seven rows added, 7 scenarios to 14**, in `vipaq/test-vectors/interop/input.json`. Both suites green - C# 421,
TS 380 - and the idea file is deleted.

- [x] **Width-boundary flips in a coordinate**, mirroring the three already covered for dimensions: 255, 256 and
      65535. They produce header `8_8_16`, the mirror of the dims rows' `8_16_8`, and neither combination was in
      the interop matrix before.
- [x] **An empty items list**, at a 16-bit bin. That pairing checks the protocol's rule that with no items both
      item widths are written `Eight` whatever the bin needs - header `16_8_8`.
- [x] **Many distinct items** - two rows, 30 items each, no repeats: one row-major at 8-bit, one columnar at
      16-bit.
- [x] **Compressed payloads at both widths.** **The plan and the idea both said "32-bit and 64-bit widths",
      which do not exist** - ViPaq has two widths, 8 and 16, and `PROTOCOL.md §4` says a value above 65,535 is
      an error rather than a wider encoding. Read as the widths that do exist, and the gap was real: no vector
      produced a substantial compressed body at 16-bit. One row does now, 45 items of realistic pallet data,
      550 bytes to ~190.
- [x] **Regenerate and run both suites.** `npm run regen:interop -w binacle-vipaq` runs the C# generator and
      then the TS one, so the two halves cannot drift.

**The compressible row earns its place twice.** It is the **first vector where the two producers emit different
compressed bytes** - 191 from `DeflateStream`, 189 from `CompressionStream` - which is exactly the case
`PROTOCOL.md §6.1` documents and the reason the interop contract is decode-to-input rather than byte-equality.
Every other row happened to agree by coincidence, so nothing executable demonstrated the rule the design rests
on. The two 30-item rows pay separately: their bodies do not compress, so DEFLATE emits **stored** blocks, a
decoder path no other vector reaches.

Mutation-checked: flipping three bytes deep in one TS deflate artifact fails exactly one C# test and nothing
else.

**Do not rebuild the cross-runtime rows.** Foreign-runtime gzip blobs and .NET 8/9 rows were built once and
removed on purpose - a gzip decoder reads any valid gzip, so they proved nothing, and they needed hand-captured
Docker bytes, which breaks the one-generator-committed-output discipline the rest of the vectors keep. The
finding they demonstrated is preserved in the protocol spec.

### The client-generation page

**Pulled in on 2026-08-14 at the maintainer's call**, from the OpenAPI follow-ups idea - which is down to this
one item, so **delete that idea file when this lands.**

**The payoff for publishing a spec at all.** A short page with copy-paste commands that generate a client from
the published per-version spec - `hey-api` for TypeScript, `kiota` for C#, and whatever else is worth naming.
**Today the spec is published and nothing tells anyone they can do this.** It turns "there is a spec" into "here
is your client in thirty seconds" for the cost of one page.

**It applies to every version, not just v3.0.x** - the maintainer's call. Each version folder publishes its own
`swagger/v3.json` and `swagger/v4.json`, so the commands work against v1.3.x, v2.0.x, v2.1.x and v3.0.x alike.
**Write it so the version is a placeholder the reader substitutes**, rather than four near-identical pages that
drift apart.

- [ ] **Repo-root `docs/` is off limits from a coding session**, so this is the docs session's work. It goes out
      with the docs deploy below.
- [ ] **Do not publish SDKs to close this.** The deliverable is a spec plus a generation guide, not shipped
      packages. That decision is recorded as a memory and is not this item's to change.

### The compose stacks

**The compose stacks** - **pulled in on 2026-08-15 at the maintainer's call, executed and verified the same
day.** All three stacks were run end to end, not just resolved. **The plan file is gone**: what it proved
about compose - the `include:` path resolution, the key-by-key override, the volume/bind switch, the shared
fixed-name volume - was folded into the tooling reference doc under `docs/`, which is now the authority on it,
along with the list of things not to undo. Nothing is left open here.

**It is the 2026-08-14 call finally carried out.** `image` is for the image; the backing services belong to
`serve`. They used to be declared in **two** compose files with the same credentials in both - now in one,
which `image.full.yml` includes.

**Nothing here ships to a user and no workflow calls it.** It gates nothing, and if it is not done by step 5 the
release goes without it.

- [x] **Four files become three**, each named after the `just` module that runs it: `serve.services.yml`,
      `image.local.yml`, `image.full.yml`. `docker-compose.build.yml` builds nothing and `docker-compose.yml` is
      not the repo's main stack - both names go.
- [x] **`image.full.yml` uses compose `include:`**, not `-f a -f b`. The plan holds the tested proof of why:
      `include:` resolves each file's relative paths against **its own** directory, which is what makes the
      backing services declarable once. `-f` resolves them against the first file, and that is the exact
      failure that got the 2026-08-07 subfolder attempt reverted.
- [x] **`volume` and `bind` keep both names and share one file.** The recipe passes the project name and, for
      `bind`, `BINACLE_DATA_DIR`. **`bind` is the maintainer's primary stack** - confirmed 2026-08-15 - so
      nothing about typing `just image up bind` may change, and comparing the two must stay a one-word edit.
      The compose file itself must still resolve an unset variable to the named volume.
- [x] **`just serve services` becomes `just serve services-up`** so it pairs with `services-down`. Trivial in
      itself, but **grep for the recipe name as well as the filenames** - `tooling/tests.just` and the api
      tests doc name the recipe and no compose file at all, so a filename-only sweep misses them.
- [x] **Both named volumes get a fixed name** - `binacle-net-postgres` and `binacle-net-data` - so the same
      declaration under two project names is one database rather than two that look like one. **This is the one
      visible change:** one volume was orphaned by it, `binacle-net-services_postgres_data`. The new
      `binacle-net-postgres` started empty and re-seeded the admin user. Delete the old one when ready.
- [x] **The bare `up` default is settled: it stays `full`.** Raised and answered on 2026-08-15 - `full` is the
      stack that exercises the whole image, which is what the module is for. No code changed, and the decision
      is recorded in the tooling doc so it is not reopened by accident.
- [x] **The port 5432 collision is now run, not just reasoned.** Starting the services while `full` is up
      fails on `Bind for 0.0.0.0:5432 failed: port is already allocated` and leaves the running stack alone.
      Nothing half-connected starts.

### The docs deploy - after the tag

**The config half is done:** `main` carries `current: v3.0.x`, `- id: v3.0.x` back at the top of `list`, and
`docs/collections/_sitemaps/version-3-0-x.xml` restored - all verified 2026-08-14. What is left is the deploy
plus six edits that must go out with it. Repo-root `docs/` is off limits to a coding session; this is the docs
session's work, written here for it.

- [ ] **Re-cut the worked example in `v3.0.x/verifying-a-release.md` against the real `3.0.0`.** It is the
      **last place any public surface still names a beta image**, and it cannot be fixed before the tag
      because it quotes real output. Everything else - the signing floor, the sample commands, the "cannot be
      verified" note - was moved off beta names on 2026-08-17. Run `just image verify 3.0.0` and replace
      three things with what it prints: the Docker Hub digest, the package count, and the provenance run URL.

      **It is now wrong, not just stale, and that raises the stakes on this row.** The example says the verify
      passes against `3.0.0-beta.2` under the identity printed above it. Since the move that identity names
      `binacle-labs`, and beta 2 was signed under the old one - so the block as written asks the reader to run
      a command that fails. **This page must not deploy before this row is done.**

      **The rule that decided this, worth keeping for future releases: name a version where the version is
      the fact, never as a floor or an example.** A floor or a sample tag goes stale on its own; a record of
      what was signed does not.

- [ ] **Put the real date and release link in `v3.0.x/release-notes.md`.** The `## v3.0.0` section carries
      interim wording because the tag did not exist when the pages were written. Swap the italic line for
      *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*, matching every other version
      folder.
- [ ] **Carry three additions from `CHANGELOG.md` into `v3.0.x/release-notes.md`.** Same notes in two places,
      and the release body gained content on 2026-08-10 the page does not have.

      **Decided 2026-08-14: this page stays hand-copied.** It is not generated from `CHANGELOG.md`. The drift
      below is the accepted cost, so **this checklist is the control** - every future release's docs handover has
      to list what the changelog gained since the page was last written. Run
      `just changelog extract Unreleased` to see the current text. All three go in the `## v3.0.0` section, in
      the page's plain-ASCII style:
  - **Overview**, one bullet after the health check line: the image creates `/app/data` and gives it to the app
    user, so a volume mounted there is writable.
  - **Core Changes**, replacing *"The `Dockerfile` and existing environment variables are unchanged"* - which is
    false, the Dockerfile changed three times this release. Spell out the `/app/data` fix (docker created the
    mount point as root, the app does not run as root, so packing logs and the SQLite database could not be
    written to a fresh named volume); `libgssapi-krb5-2` now shipping, so Npgsql stops printing "Cannot load
    library libgssapi_krb5.so.2" at every start, which was harmless but read as fatal; OCI labels on the image;
    and only then "existing environment variables are unchanged".
  - **A `🔌 Service Module` section**, between Diagnostics and UI Module: the auth token rate limit partitions
    on the connection's remote address instead of a caller-supplied header, so varying the header no longer
    resets your own login throttle.
- [ ] **Replace the two swagger documents under `docs/collections/_versions/v3.0.x/swagger/`.** Copy
      `artifacts/openapi/Binacle.Net_v3.json` -> `swagger/v3.json` and `artifacts/openapi/Binacle.Net_v4.json`
      -> `swagger/v4.json`; the generator's file names differ from what the site expects, so the rename is part
      of the handover.

      **The diff is already measured - 2026-08-14, do not re-derive it.** Freshly generated against the frozen
      copies, the **only** differences are: both documents gain a `servers` entry with the single relative `/`,
      and **the `429` responses come out** - 4 mentions in v3 and 14 in v4 go to zero. Nothing else moves. No
      schema name changed despite the namespace restructure, which is the thing that was worth checking. For v3
      this **restores** the shape v2.1.x shipped rather than changing it, so nothing about the frozen v3
      contract moves. It is still a visible change to the published spec, so mention it wherever the update is
      described.
- [ ] **Carry the signature-verification page.** The image verification work writes down what the page must
      say - the verified `cosign verify` invocation, the three points it has to make, and the rule that any
      example tag names a signed image. **Check that text actually exists before deploying**; if it does not,
      the page waits for the next docs deploy, which is the reason that work is pre-tag.
- [x] **GHCR is cut out of the verification page - done 2026-08-16.**
      `docs/collections/_versions/v3.0.x/verifying-a-release.md`. **Decided 2026-08-15: only the release
      workflow touches GHCR** - `$ci-cd/decisions#D14`. It stages there, smokes there and copies to Docker Hub,
      and nothing else reads it. The word no longer appears on the page. Three edits were made:

  - The **"Which registry you verify"** section is deleted whole - the `ghcr.io` cosign block, the "verify
    against the registry you actually pulled from" paragraph and the GHCR referrers note went with it.
  - The worked example says the tag resolves to that digest on Docker Hub, not on both registries.
  - The closing tip says **four** checks, not five - `tags`, `signature`, `attestations`, `metadata`. The
    `digest` check compared the two registries and went with this decision.

      Nothing else on the page moved; the two commands at the top already named Docker Hub only.

      **The edit is in the file and not on the site.** That deploy is `workflow_dispatch`, so it goes live with
      the item at the end of this section, not before it.

      **A coding session made this edit rather than the docs session**, at the maintainer's explicit call after
      the rule was raised. The page itself was also added by a coding session, in the commit that built the
      verify recipe - that is what put the cut on this list in the first place.

- [ ] **Write the client-generation page**, per the item above. It is cross-version, so decide once where a
      page that is not version-specific lives on that site rather than copying it into four folders.
- [ ] **Deploy.** It is `workflow_dispatch` only.

**This is the single most losable item in the release** - nothing fails if the deploy is skipped, the site just
quietly keeps serving v2.1.x as current. **It has to run after the tag**, because the notes need the date and
the `releases/tag/v3.0.0` link, and `main` already says v3.0.x is current, so deploying earlier presents an
unreleased version as current. It has to land before anything is announced. **Tag, then deploy the docs, then
announce.**

**One deliberate 404, do not "fix" it.** The `v3.0.x` ViPaq page links the wire spec at
`github.com/binacle-labs/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md`, which 404s until the tag is pushed. A
versioned page should pin the spec it describes; do not repoint it at `main`.

---

## Already done - do not re-audit

**Gate A, all four items.** Publish paths hardcoded in the workflow (2026-07-27). A prerelease moves neither
`latest` nor the minor tag (observed on Docker Hub 2026-08-06). Health check IP restrictions, four defects and
the missing tests (2026-07-27). Forwarded headers, warn-once diagnostics and the missing tests (2026-07-27).

**Verified behaviour:**

- **Fitting results are unchanged.** Differential-tested 2026-07-19 against the real
  `binacle/binacle-net:2.1.1` image across all three algorithms, zero disagreements. No release-notes caveat
  needed.
- **Old ViPaq tokens fail loudly.** Verified 2026-07-19, locked 2026-07-20 with four regression vectors in
  `vipaq/test-vectors/serialization/decode-invalid.json`. Zero silent misparses.
- **The login throttle no longer partitions on a caller-supplied header.** `GetClientIp()` deleted 2026-07-24;
  `AuthTokenRateLimitingPolicy` partitions on `Connection.RemoteIpAddress`. Unit tests cover the partition keys,
  and as of 2026-08-14 an integration test covers the wiring - a forged forwarded header per attempt still hits
  the limit.
- **ViPaq's wire format did not move after beta 1** - source changed comments only.
- **The Dockerfile did not change after beta 1 at all.**

**Release milestones:**

- **The docs pages** (2026-08-10) - site builds, `/version/latest/` lands on v3.0.x. **The ViPaq protocol page
  split** (2026-08-07) - all four versions written. **The `AI-GENERATED` review token strip** (2026-08-11) -
  zero left, whole repo.

  On that last one: the pass was never "revert everything an agent wrote". Several agent comments were kept on
  purpose because they were better than the line they replaced - the unchecked-multiply overflow note on the
  packing algorithms, the empty-catch explanation in `ConnectionString.cs`, the curated-scenario table in
  `BischoffCuratedProblemsProvider.cs`. **A surviving agent comment is not damage.**

- **Beta 2** - published 2026-08-13 from `d317cd2b`, digest `sha256:ccce2a44`, deployed to the test server
  2026-08-14. All six jobs green with `publish` included, the GHCR package public to an anonymous puller,
  Docker Hub took `3.0.0-beta.2` and nothing else moved, the copy preserved the digest, the image is signed on
  both registries, SBOM and provenance are in the index, 31/31 structure assertions and all five smoke profiles
  pass against the published image, and the release body is byte-identical to `just changelog extract
  Unreleased`. `DEBUG_ENDPOINT` was confirmed off on the deployment - it had been on and answering publicly
  since beta 1, echoing the caller's `Authorization` header back, and it was the only real exposure beta 1 left
  behind.

- **The release pipeline rebuild** - proven end to end by beta 2. What remains of that plan is the one docs
  decision above, plus a post-release check.

- **The architecture merge** - **done, and it went in the other direction.** `main` was merged into
  `features/arch_tests` at `16289d4d`, so the branch now contains all of `main` and landing it is a
  fast-forward. **All the conflict-resolution instructions this file used to carry are spent.** Verified
  2026-08-14: front matter is on all five files added since the fork, six `_index.md` are generated, `just test
  all` is green across eleven leaves (72 / 35 / 622 / 107, 0 failed), `just openapi lint` reports 0 errors and 0
  warnings, and the OpenAPI regenerate-and-diff came back with no schema movement at all.

- **The `429` OpenAPI guard** - committed at `dec5212c`. Both guards are in
  `RateLimiterResponseOperationTransformer.cs` with the reasoning comment.

**Two things beta 2 does not prove**, both structural rather than untried. **The moving tags** - a prerelease
withholds `{{major}}.{{minor}}` and `latest`, so `3.0` and `latest` are written for the first time by v3.0.0
itself; that is one extra argument to an `imagetools create` call that has run several times. And
**`latest=auto` does not consult the registry** - it reads the git ref, so any non-prerelease semver takes
`latest`. Right for v3.0.0, and the reason a throwaway `v0.0.1` against the real repo would have moved `latest`
off `2.1.1`. **Recorded because the trap outlives the plan that found it.**

---

## The release notes

**They live in `CHANGELOG.md`, in the `## [Unreleased]` section**, and the workflow extracts that section as
the release body. The content is complete and was checked byte-for-byte against the published beta 2 body on
2026-08-13 - all four breaking changes, the six migration steps, the signing and SBOM bullet, the image-size
drop, and `RetentionDays`. Nothing since then changes what a user can observe.

Three mechanics:

- **`[Unreleased]` is renamed to `## 3.0.0` as the last change before the tag.** Every beta publishes
  `[Unreleased]`. If you forget, the `notes` job fails in under a minute and nothing is built - which is why
  that gate runs first.
- **A section's own headings are `###`**, nested under the `##` version heading. `just changelog extract` shifts
  them back to `##` on the way out. **Do not "fix" the file to use `##` throughout** - that breaks the nesting
  under `# Changelog` and the extractor's terminator both.
- **The compare link at the bottom already reads `v2.1.1...v3.0.0`** - correct from the tag onward, and a 404
  on every beta release page until then. Left as it is deliberately; the alternative is editing it twice.

**The restructure gets no changelog line - decided 2026-08-14.** No user-observable behaviour changes, nothing
is published to NuGet, and no contract moves - the OpenAPI diff proves the last one. The four breaking changes
stay four. Anyone building from source sees `Binacle.Lib.Abstractions` disappear, and that is not worth a line.

---

## The sequence

1. **Rate limiter tests.**
2. **Rate limiting owned by the ServiceModule.** After 1, never before - it is the test that makes this safe.
3. **The Azure Storage run.**
4. **Cut, deploy and verify beta 3.** After 2, so the beta carries the endpoint change.
5. **The last commit:** changelog rename, nine pins, six comment blocks, three READMEs, two tooling examples,
   and the `IsExperimental` re-confirm.
6. **Tag `v3.0.0`.** The pipeline does the rest.
7. **Deploy the docs**, with the six edits above.
8. **Work `post-release-v3.0.0.md`.**

**Everything under "Runs alongside" happens any time from now to step 5**, in whatever order suits, with two
orderings that matter: **image verification goes before the Docker Hub page**, or the verification section gets
written twice; and **the docs-site text from image verification must exist before step 7**, because that deploy
runs once.

---

## Not in this release

Everything else has a plan or an idea of its own and is on the board, grouped by area with its blockers named.
**Do not pull any of it in.**

**Held back on 2026-08-14, with reasons:** the heavy architecture tools (ArchUnitNET, dependency-cruiser,
lychee), CI gates 2 and 3, and raising test coverage. The scope decision at the top of this file carries the
reasoning.

**Three exceptions were taken earlier and are all in.** Dropping `--self-contained` (2026-08-10, 150.2 MB ->
103.2 MB, proven by beta 2). The release-pipeline rebuild (2026-08-10 - a prerelease tag is the only free test
that pipeline will ever get). The architecture restructure (2026-08-14 - merged, green, and proven not to move
a single schema).
