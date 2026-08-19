---
description: Release - Binacle.Net v3.0.0
---

# Release - Binacle.Net v3.0.0

**Status:** In progress. Betas 1, 2 and 3 published, verified and deployed; beta 3's checks closed 2026-08-19. The pipeline is rebuilt and proven
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
| **The PR gate change** | A new PR workflow calling `shared-test-suite.yml`, plus the image build. Everything green on arrival. **The OpenAPI lint and the Spectral move landed separately on 2026-08-17** - the lint is a step in `shared-test-suite.yml`, not a job in the new workflow. |
| **The client-generation page** | The spec is published and nobody knows they can generate a client from it. One docs page, and it applies to every version. |
| **More ViPaq interop vectors** | Fixture data, and the format froze in this release. **Done 2026-08-17** - 7 scenarios to 14. |
| **The compose stacks** | Pulled in on 2026-08-15, after the scope reset. **Done the same day** - what it proved about compose is in the tooling reference doc. |

### Does not ship, and why

| Item | The blocker |
|---|---|
| **All the architecture checks** | The heavy tools - ArchUnitNET, dependency-cruiser, lychee - and the three lighter checks that were going to ship with the PR gate. **The lighter three left the release on 2026-08-17**, when re-examining how to build them produced a better design that is a fresh design rather than a ready item. They are on the board as `architecture-checks`. What was already deferred is everything needing a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose - the mismatch throws before a single test runs. dependency-cruiser has no root `tsconfig.json` to work from; there are four, and `web/` has none. |
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
| 4 | Beta 3 | **done - 2026-08-19** |
| 5 | Admin read endpoints - list accounts, list subscriptions, get subscription | **done - 2026-08-19** |
| 6 | The last commit: pins, prose and the changelog rename | open |
| 7 | Tag `v3.0.0` | open |

### 4. Beta 3 - done 2026-08-19

Cut from the merge commit, published 2026-08-16, and it paid for itself twice: the whole pipeline ran under the
new owner, `just image verify 3.0.0-beta.3` passed all four checks, and the command printed in `SECURITY.md`
passed verbatim from a clean shell. **It is the reference tag - the only image that verifies under the current
identity** until v3.0.0 is out.

The three live checks it existed for:

- **The version stamp reads `3.0.0-beta.3`.** Read from `/_health` on the host, environment `Production`, both
  entries green. The pipeline never sees this - the smoke only asserts the stamp is not `Unknown`.
- **The resolved caller.** Settled; closed by the maintainer.
- **The auth token endpoint.** An active account returns 200 and an inactive one 401, both live. **The
  suspended account returns 403 and that branch was not exercised** - no suspended account on the host.

**The 403 is untested by anything, and that outlives the release.** `IntegrationTests/Endpoints/Auth/Token.cs`
covers 200, 401 and 422 only, so the one branch in `Reject` that returns a different status code has neither a
test nor a live call behind it. Deferred at the maintainer's call, and **written into
[api/integration-test-additions](plans/api/integration-test-additions.md)**, which is already on the board - it
is a test to write, not a check to run, so it does not belong in post-release.

### 5. Admin read endpoints - done 2026-08-19

**Pulled in on 2026-08-19, at the maintainer's call, from testing beta 3.** The admin API could create an
account but never enumerate one: every read needed an id, and the only place an id appeared was the `Location`
header of the create call. A running instance had accounts in it and no way to ask what they were.

Three endpoints, all additive - no existing contract moves:

- `GET /api/admin/accounts` - offset paged, `page` / `pageSize` / `allowDeleted`.
- `GET /api/admin/subscriptions` - the same, and the flat list across accounts.
- `GET /api/admin/account/{id}/subscription` - the read that was missing next to update, patch and delete.

**Paging is offset, not cursor**, so the shape supports page numbers, a total, and sortable columns later
without reshaping the contract or a client built on it. SQLite and Postgres serve it with `LIMIT`/`OFFSET` and
a `COUNT`. **Azure Table Storage can do none of those three**, so its repositories read the matching partition
whole and sort, slice and count it in memory - bounded by `PageQuery.MaxPageSize` and never on the auth path.
Whether that provider stays is a separate question, recorded in the ServiceModule simplification idea along
with what its storage shape costs.

**One existing response changed.** Creating a subscription returned a `Location` of
`/api/admin/account/{id}/subscription/{subscriptionId}`, which matched no route and 404ed. It now points at
`/api/admin/account/{id}/subscription`, which the new Get answers.

- [x] The three endpoints, with OpenAPI examples and request files.
- [x] `ListAsync` on both repository interfaces and all four implementations of each.
- [x] Integration tests - green on **all three backends**: SQLite, Postgres and Azure Storage.
- [x] Verified live against a running instance on the Azure Storage backend, which is the faked path.

### 6. The last commit before the tag - all in one

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

### 7. Tag

- [ ] **Tag `v3.0.0`.** The pipeline does the rest: the changelog gate, the suite, the GHCR build, the smoke,
      the Docker Hub copy under all three tags, the signature, and the release created from the `3.0.0`
      section. **Nothing here is manual any more.** Watch the run, then check the rendered body and
      `docker buildx imagetools inspect`.

---

## Runs alongside - real work, does not hold the tag

Each is cheaper now than after. **If one is not ready, the release goes without it** - with one exception,
named below.

### Image verification

**Done 2026-08-15, re-proven 2026-08-17 against `3.0.0-beta.3`.** `just image verify <version> [check]` runs
four checks. What survived the plan is in the ci-cd decisions ledger and the tooling reference doc. Every
public surface reads `3.0.0`. One thing is left.

- [ ] **Write the Docker Hub page's verification section from the same wording.** `SECURITY.md` is the source
      now; that page's own plan says to copy it rather than edit its draft.

**The constraint that binds every surface:** any example naming a tag must name a tag that **passes today**.
Only `3.0.0-beta.3` does - the org move re-keyed the certificate identity, so beta 2 is signed under the old
owner and fails. `3.0` and `latest` point at nothing signed until v3.0.0 publishes.

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

### The PR gate - one change

**The image build landed 2026-08-18**, in `pull-request.yml` as the `image` job beside the test suite call.
**[ci-gates](plans/ci-cd/ci-gates.md)** holds the shape, the naming rework and the two traps - read it before
touching a workflow file.

- [ ] **Point branch protection at `Pull Request / Gate`.** It is the one required check now; until this
      happens the old entry reports nothing.

### The client-generation page

**Pulled in on 2026-08-14 at the maintainer's call**, from
**[api/openapi-spec-followups](ideas/api/openapi-spec-followups.md)** - which is down to this one item, so
**delete that idea file when this lands.**

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

1. ~~Rate limiter tests, rate limiting moved to the ServiceModule, the Azure Storage run.~~ All done
   2026-08-14.
2. ~~Finish beta 3.~~ Published 2026-08-16, closed 2026-08-19.
3. **The last commit:** changelog rename, nine pins, six comment blocks, three READMEs, two tooling examples,
   and the `IsExperimental` re-confirm.
4. **Tag `v3.0.0`.** The pipeline does the rest.
5. **Deploy the docs**, with the six edits above.
6. **Work `post-release-v3.0.0.md`.**

**Everything under "Runs alongside" happens any time from now to step 3**, in whatever order suits, with two
orderings that matter: **image verification goes before the Docker Hub page**, or the verification section gets
written twice; and **the docs-site text from image verification must exist before step 7**, because that deploy
runs once.

---

## Not in this release

Everything else has a plan or an idea of its own and is on the board, grouped by area with its blockers named.
**Do not pull any of it in.**

**Held back on 2026-08-14, with reasons:** the heavy architecture tools (ArchUnitNET, dependency-cruiser,
lychee), CI gates 2 and 3, and raising test coverage. **The three lighter architecture checks joined them on
2026-08-17.** The scope decision at the top of this file carries the reasoning.

**Three exceptions were taken earlier and are all in.** Dropping `--self-contained` (2026-08-10, 150.2 MB ->
103.2 MB, proven by beta 2). The release-pipeline rebuild (2026-08-10 - a prerelease tag is the only free test
that pipeline will ever get). The architecture restructure (2026-08-14 - merged, green, and proven not to move
a single schema).
