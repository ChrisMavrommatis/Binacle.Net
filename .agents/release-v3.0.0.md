# Release - Binacle.Net v3.0.0

**Status:** In progress - **Gate A green, beta 1 published and verified, docs written.** B2 landed, so the long
pole is down: the `v3.0.x` folder is a full version and the site builds. What is left is **a second beta**
(decided 2026-08-10 - see Gate B2X), then B6, then the tag and the docs deploy.
**Created:** 2026-07-16. **Restructured:** 2026-07-26. **Status rewritten:** 2026-08-06. **B3 landed:**
2026-08-07. **B2 landed and beta 2 added:** 2026-08-10.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to the
reference rules: it may point at any file to coordinate the release, and **nothing points back at it**. Delete it
once v3.0.0 is out.

Companions:
- `release-notes-v3.0.0.md` - the GitHub release body, ready to paste.
- `post-release-v3.0.0.md` - what to do once the release is out.

## How to work this file

Two gates. **Gate A** had to be green before the **first** beta image was published; **Gate B** before the final
tag. Each row is either a link to a plan under `.agents/plans/` that holds the whole item, or a checkbox for a
one-line action with a known answer.

**Beta 2 sits inside Gate B, as B2X** - it is not a second Gate A. Gate A asked "is this safe to run outside a
test host at all", and that was answered on 2026-07-30. Beta 2 asks a narrower question: does the refactor that
landed after beta 1 still behave in a real deployment.

**When a plan lands, its file is deleted.** Tick the row here and drop the link in the same change, leaving the
text. Otherwise this index rots into a list of dead links within a fortnight.

---

## Gate A - before publishing the beta image

The beta is the first time this code runs outside a test host. Everything here either stops the image from
publishing, or is a new behaviour that only fails in a real deployment - which is what the beta is for.

| # | Item | Plan |
|---|---|---|
| A1 | Publish paths hardcoded in the workflow, no Actions variable needed | **done (working tree)** |
| A2 | Build the image once, and prove a prerelease tag does not move `latest` | **done 2026-08-06** |
| A3 | Health check IP restrictions - four defects and the missing tests | **done 2026-07-27** |
| A4 | Forwarded headers - warn-once diagnostics and the missing tests | **done 2026-07-27** |

- [x] **A1 - publish paths hardcoded.** The publish step read `${{ vars.API_PROJECT_PATH }}` /
  `${{ vars.BUILD_OUTPUT }}`, and the former still pointed at the pre-move `src/Binacle.Net/Binacle.Net.csproj`,
  so it failed after the `src/` -> `api/src/` move. Instead of depending on a repo-settings variable, the
  workflow now hardcodes `api/src/Binacle.Net/Binacle.Net.csproj` and `-o build/binacle-net` - matching the
  Dockerfile's fixed `COPY` source and `build.just`, which cannot drift. No Actions variable needed. Change is
  in the working tree; the human commits. (`BUILD_DOCKERFILE`, `DONET_VERSION`, `DOCKERHUB_*` are still
  variables and were unaffected by the move.)

- [x] **A2 - a prerelease moves neither `latest` nor the minor tag.** Observed on Docker Hub 2026-08-06, after
  `v3.0.0-beta.1` was published on 2026-07-30. `3.0.0-beta.1` exists. `latest` still resolves to digest
  `sha256:f48edc9117714`, last updated 2026-01-12 - byte-identical to `2.1.1`, so it never moved. **No `3.0` tag
  exists at all**, so metadata-action skipped `{{major}}.{{minor}}` for the prerelease exactly as documented.
  Both guards (`latest=auto`, and the same prerelease check on `{{major}}.{{minor}}`) are now observed in this
  repo rather than assumed, and neither workflow fix is needed. The image was also built once against current
  code (`just build image`, green, 2026-07-30).

  Two consequences worth carrying forward. **B5 is unblocked** - the beta published no `3.0` tag, so bumping the
  samples to `3.0` cannot point them at a prerelease. And `3.0` only starts existing when v3.0.0 final is
  published, which is what `version_tag: "3.0"` in the `v3.0.x` docs scope assumes - see B0.

**A3 and A4 gated the beta rather than the final tag** because the beta is deployed behind a proxy with a health
check allow-list: A3 was the allow-list, A4 is what makes its failure modes visible instead of silent. Landing
them after the beta would have wasted the only run that catches them. Both landed 2026-07-27; what they left for
the deployed image is on the beta verification list.

### Already verified - do not re-audit

- **Fitting results are unchanged.** Differential-tested 2026-07-19 against the real `binacle/binacle-net:2.1.1`
  image across all three algorithms, zero disagreements. No release-notes caveat needed. Evidence is folded into
  the lib findings.
- **Old ViPaq tokens fail loudly.** Verified 2026-07-19, locked 2026-07-20. Real old tokens plus adversarial
  header-aligned cases all threw a format exception; zero silent misparses. Four regression vectors are committed
  in `vipaq/test-vectors/serialization/decode-invalid.json`, C# and TS green. Only the announcement remains (B7).
- **The login throttle no longer partitions on a caller-supplied header.** `GetClientIp()` deleted 2026-07-24;
  `AuthTokenRateLimitingPolicy` partitions on `Connection.RemoteIpAddress`. Suites green (ServiceModule 107,
  API core 622).

---

## Gate B - beta is running, before the v3.0.0 tag

**Order is not the numbering.** The IDs are labels, fixed since 2026-07-26. What runs when is "The sequence" at
the bottom of this file, rewritten 2026-08-06 once the beta was actually deployed.

| # | Item | Plan |
|---|---|---|
| B0 | Unfreeze the docs site - point `current` back at `v2.1.x` and deploy | **done 2026-08-06** |
| B1 | Work the beta verification list on the deployed image | **done 2026-08-06** - all boxes pass |
| B2 | Write the `v3.0.x` docs pages, including the two new configuration pages | **done 2026-08-10** - full version, site builds, `/version/latest/` lands on v3.0.x |
| B2X | **Publish and verify beta 2** - **no plan** | see below |
| B3 | Fix the ViPaq protocol page | **done 2026-08-07** - split landed, all four versions written |
| B4 | Generate `swagger/v3.json` and `swagger/v4.json` | **done 2026-08-06** - generated and checked, moved into `v3.0.x/swagger/` by B2 |
| B5 | Move the sample image pins - **no plan** | see below |
| B6 | Run the ServiceModule suite once against Azure Storage - **no plan** | see below |
| B7 | Confirm v4 still ships experimental, then announce all four breaking changes - **no plan** | see below |
| B8 | Flip `current` forward to `v3.0.x` again and redeploy the docs - **no plan** | see below |

**B1 came back clean.** All boxes pass. The three changes the beta existed to test - forwarded headers, the
health check allow-list, and rate limiting on the resolved caller - all behave as designed against a real
proxied deployment, and no defect was found. Worth stating plainly, because the first pass was HTTP-only and
read more confident than its evidence supported; the four boxes it could not reach were closed the same day
from the container log and filesystem, and they closed *in favour* of the release rather than against it.

Two loose ends, neither blocking the tag. They are the whole of what B1 left behind. **Both are now on beta 2's
list (B2X)**, which is the run that closes them - they are described in full here because that list is terse:

- **`DEBUG_ENDPOINT` is still on** and answering publicly. It echoes the caller's own request including their
  `Authorization` header. Turn it off - it is the only real exposure the verification left behind.
- **The forwarded-headers source header moved during verification** (`CF-Connecting-IP` on one boot,
  `X-Forwarded-For` on a later one). Not broken - the resolved caller was correct whenever it was observed -
  but the two are not equivalent behind a CDN, and the allow-list is compared against whatever they resolve to.
  Confirm the caller resolves correctly once more after the setting settles.

One release-notes gap fell out of this: **`RetentionDays`** is new in v3.0.0, deletes packing log files when
set, and was missing from the notes entirely. Added to `🧪 Diagnostics Module` on 2026-08-06. It defaults to
`null` and the beta's log confirms it is off, so it breaks nothing - but an unannounced setting that deletes
files should not ship unmentioned.

Two qualifications on the evidence, so nobody reads it as stronger than it is. **The ViPaq round-trip was not
checked through the beta's own Protocol Decoder** - `UI_MODULE` is off on that deployment. A real token from
the beta's v3 API was decoded with this repo's TS implementation instead, and matched the geometry in the same
response; the four old-format vectors were then rejected with specific format errors, none misparsed. That is
cross-implementation evidence rather than the literal check, and the interop suite covers the same pair
continuously. **And the DataProtection key ring is not persisted** on the beta - stock ASP.NET, not new in
v3.0.0, no release-notes line owed. B2 carried it onto the ServiceModule configuration page, which now tells
operators to mount a volume at `/home/app/.aspnet/DataProtection-Keys`. Nothing left owed.

**B2 landed 2026-08-10.** The `v3.0.x` folder is a full version: API v3 and v4 pages, both new configuration
pages, all six sample folders with the config files they mount, `swagger/v3.json` + `v4.json` moved out of
gitignored `build/openapi/` with a `.md` beside each, `quick-start.md` and `release-notes.md`. Verified rather
than assumed: the site builds clean, `/version/latest/` redirects to `v3.0.x`, `robots.txt` advertises all four
sitemaps, neither swagger document carries an `/api/auth/token` path, v4 keeps its experimental banner, the
docs sample pins read `3.0`, every Kubernetes copy has the hardening, and no tracked file carries a BOM. One
thing the pages still owe is the real release date and tag link - it is under B8, not here.

### B2X - publish and verify beta 2 {#beta-2}

**Decided 2026-08-10.** Beta 1 was published on 2026-07-30 from `8f511ddc`. Twenty-seven commits have landed
since, and the assumption that they do not reach the image is **not true** - the Sonar sweep changed shipping
code in `api/src`, `lib/src` and `vipaq/src`. Beta 2 is what tests that against a real deployment instead of
trusting the refactor.

**What actually changed since beta 1**, checked against the `v3.0.0-beta.1` tag rather than the commit log:

- **The Dockerfile did not change at all.** `/app/data`, `libgssapi-krb5-2` and the OCI labels were all in beta
  1 already. Nothing to re-verify there, and the release notes' `/app/data` line describes an image that has
  already run in a deployment.
- **ViPaq's source changed comments only.** `Header.cs`, `ProtocolEncoder.cs`, `DeflateCodec.cs` and
  `ViPaqSerializer.cs` have no behavioural diff, so the wire format beta 1 produced is the wire format beta 2
  will produce. B1's ViPaq evidence still holds.
- **The refactor is real and it ships.** The largest are `ServiceModule/v0/Endpoints/Auth/Token.cs` (media-type
  constants, a `Reject` helper extracted from the rejection chain, `HandleAsync` made `static`), `Program.cs`,
  `OpenApiUiExtensions.cs`, the UI module's code-behind, and every v3 and v4 endpoint going `static`. All
  behaviour-preserving in intent; the suites agree, and beta 2 is the check that a real host agrees too.
- **The release workflow pins its actions by SHA** now. Beta 2 is the first run of the pinned workflow, so it
  also proves the publish path still works before the real tag depends on it.

The verification list is short, because B1 already covered the deployment-shaped behaviour and none of the
middleware it exercised changed except log-template casing:

- [ ] Publish `v3.0.0-beta.2` and confirm the workflow's SHA-pinned actions still build and push.
- [ ] Confirm the prerelease still moves neither `latest` nor a `3.0` tag - the same A2 check, now against a
      workflow whose actions were re-pinned.
- [ ] Confirm `BINACLE_VERSION` inside the image reads `3.0.0-beta.2`, with no leading `v`. The fix that
      strips the `v` was already in beta 1 (`c6981e90`, checked against the tag), so this is a confirmation
      rather than a first test - but it was never actually observed on a published image, and it is one
      `docker inspect` away.
- [ ] Smoke the published beta: `just smoke all binacle/binacle-net:3.0.0-beta.2`.
- [ ] Deploy it and exercise the auth token endpoint. `Token.cs` is the single most restructured shipping file
      and its rejection chain is now one extracted helper - a wrong branch here returns the wrong status code
      to a real client, which no unit test shape catches as well as one live call.
- [ ] Re-confirm the resolved caller once the forwarded-headers source header settles. This is the loose end
      B1 left behind and beta 2 is the run that closes it.
- [ ] Turn `DEBUG_ENDPOINT` off on the deployment. It is still on and answering publicly, echoing the caller's
      `Authorization` header back. It is the only real exposure B1 left behind, and it should not survive
      beta 2.

**What beta 2 does not need to re-do:** the fitting differential, the old-ViPaq-token rejection, the health
check allow-list, or the login throttle partition. All four are in "Already verified - do not re-audit" or
closed by B1, and nothing behind them changed.

**B3 landed 2026-08-07 - written, not just decided.** The page is split in two: a general `_common_pages` page
with no implementation details and nothing that varies between versions, plus one versioned page per folder
carrying the wire format. `v1.3.x`, `v2.0.x` and `v2.1.x` carry the old text, which was already right for them;
`v3.0.x` is written fresh from `vipaq/PROTOCOL.md` and fixes all three of the errors the old page carried, not
just the gzip one. The three `api/v3.md` links now use `vlink`, the landing page link is unchanged, and the
general page resolves the current version from `site.data.versions.current`. The site builds clean, and the new
page sits in the version sidebar between Configuration and Samples.

The two audit fixes landed with it. `core-concepts.md` no longer ranks the three algorithms against each other -
that was an unverified claim about code that has changed, on a page every version shares - and says instead that
relative speed depends on your data and version. `quick-start.md` keeps `latest` but now warns that it follows
the newest release and says to pin a version for anything kept; its "see the dedicated Quick Start Guide" prose
is a real link now.

**One thing B2 must know:** the `v3.0.x` ViPaq page links the wire spec at
`github.com/ChrisMavrommatis/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md`. That URL 404s until the `v3.0.0` tag is
pushed. It is deliberate - a versioned page should pin the spec it describes - but do not "fix" it to `main`.

~~**B4 covers two documents.**~~ Done 2026-08-06 - `v3.json` and `v4.json` both generated, no `/api/auth/token`
path, v4 carries the experimental banner. Handed to the docs session.

- [ ] **B5 - the image pins move twice now, not once.** They sit at `3.0.0-beta.1` today. Beta 2 adds a step:
  **after `v3.0.0-beta.2` publishes** they move to `3.0.0-beta.2`, and then to **`3.0`** as the last change
  before the v3.0.0 tag. The rule behind both moves is the same one this item has always carried - **a pin on
  `main` must name an image that exists on Docker Hub** - so each bump follows its publish rather than
  preceding it.

  **It is nine files, not six.** The six the item originally named are the pin itself; three more carry the
  beta in prose, and they are the ones that get missed:

  | File | What to change |
  |---|---|
  | `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` | the `image:` line |
  | `samples/kubernetes/minimal/binacle-deployment.yaml` | the `image:` line |
  | `README.md` | the pin warning under Quick Start - **the repo landing page**, currently telling every visitor to pin `3.0.0-beta.1` |
  | `samples/README.md` | the pin paragraph |
  | `samples/docker/README.md` | the pin paragraph |

  **Also drop the two-line comment above each `image:` line at the `3.0` bump.** It explains our release order
  ("does not exist on Docker Hub yet", "move to the 3.0 minor tag once v3.0.0 is published"), which means
  nothing to a reader who copied the file. The published docs copies already trimmed it to the one durable
  sentence: `# Pinned on purpose - a copied sample must not jump to a new major on the next pull.` The repo
  copies should end up matching.

  Two more mention the beta as an **example** rather than a pin - `config/README.md` and `config/smoke.just`,
  both showing "smoke what is actually on Docker Hub". Neither is wrong today and neither ships to a user, but
  they read as stale once the tag is out. Sweep them at the `3.0` bump.

  **The caveat, unchanged.** The pins moved early once before, on 2026-08-07, and sat on `main` naming an image
  that did not exist. They moved because the samples were restructured in the same pass and the new ones
  (`prod`, `service`, `full`) document v3-only settings - forwarded headers, `RetentionDays`, the ServiceModule
  split - so pinning `2.1.1` would have been wrong in a different and worse way. A prerelease pin is the fix
  for the same reason: real and v3-only, without naming a tag that is not there yet. **Do not leave the `3.0`
  bump on `main` long before tagging.**

  The five samples are also no longer the same five. `minimal-setup` -> `minimal`, `ui-setup` -> `quickstart`,
  `service-npgsql` and `service-azure` folded into one `service` carrying all three connection strings, plus new
  `prod` and `full`. Every folder name is now a smoke profile name, so `just smoke` runs each shipped shape.

- [ ] **B6 - Azure Storage.** CI covers SQLite and Postgres only, so the Azure provider ships on trust. The
  cheap cover is one deliberate run before tagging: bring up Azurite with `just serve services -d`, then
  `just test api-service-integration AzureStorage`.

  **This got more important on 2026-08-07, not less.** The old justification was that `samples/docker/service-azure`
  points users at the provider, so it earns its place. That sample is gone - folded into `service`, where Azure
  is now one commented connection string among three. So Azure ships with no dedicated sample, no CI coverage
  and no smoke profile (smoke is SQLite-only by design). This one run is the only thing standing behind it.
  It stays in this release; removal is a stronger idea than it was.

- [x] **B7a - v4 is still experimental.** `ApiV4Document.IsExperimental` is `true`, so the published OpenAPI
  document carries the warning that v4 may change at any time. Re-confirm it is still `true` at tag time -
  shipping v4 as stable would lock contracts that are meant to keep moving. The flip is 3.1.0 work.

- [ ] **B7b - announce all four breaking changes** in the GitHub release body: V2 endpoints removed, ViPaq
  tokens, the flattened packing-logs configuration, and health check `RestrictedIPs`. All four are already
  written into `release-notes-v3.0.0.md`, along with a six-step migration guide - this is the check that they
  made it in. The packing-logs step is the one most easily lost, and leaving it out fails a user's startup with
  no explanation. The two that need the extra explanation are in the section below.

- [ ] **B8 - flip `current` forward again.** **The config half is done**: the docs-v3 work landed on `main` as
  `3dc6f1ac` with `current: v3.0.x`, `- id: v3.0.x` back at the top of `list`, the stub's `sitemap: exclude`
  gone, and `docs/collections/_sitemaps/version-3-0-x.xml` restored. What is left is the **deploy**, plus one
  edit that must go out with it:

  - [ ] **Put the real date and release link in `v3.0.x/release-notes.md`.** The `## v3.0.0` section carries
    interim wording - it asserts no date and links the releases *list* - because the tag did not exist when the
    pages were written. Once v3.0.0 is tagged, swap that italic line for the usual
    *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*, matching every other version folder.
    One line. Deploying with the interim wording is not a failure, but leaving it there permanently means the
    current version's notes never say when it shipped.
  - [ ] **Carry three additions from `release-notes-v3.0.0.md` into `v3.0.x/release-notes.md`.** The two files
    are the same notes in two places, and the GitHub body gained content on 2026-08-10 that the page does not
    have. All three are for the `## v3.0.0` section, worded to match the page's plain-ASCII style:
    - **Overview**, one bullet after the health check line: the image creates `/app/data` and gives it to the
      app user, so a volume mounted there is writable.
    - **Core Changes**, replacing "The `Dockerfile` and existing environment variables are unchanged" (which is
      false - the Dockerfile changed three times this release): the `/app/data` fix, spelled out - docker used
      to create the mount point as root, the app does not run as root, so packing logs and the SQLite database
      could not be written to a fresh named volume; `libgssapi-krb5-2` now ships, so Npgsql stops printing
      "Cannot load library libgssapi_krb5.so.2" at every start, which was harmless but read as fatal; OCI
      labels on the image; and only then "existing environment variables are unchanged".
    - **A `🔌 Service Module` section**, between Diagnostics and UI Module: the auth token rate limit
      partitions on the connection's remote address instead of a caller-supplied header, so varying the header
      no longer resets your own login throttle. The page already carries the exemption note at the top, so it
      needs only the fix itself, not the exemption sentence the GitHub body repeats.
  - [ ] Deploy the docs. It is `workflow_dispatch` only.

  **This is still the single most losable item in Gate B** - nothing fails if the deploy is skipped, the site
  just quietly keeps serving v2.1.x as current. And note the ordering trap the config half creates: `main` now
  says v3.0.x is current, so **deploying before the tag publishes an unreleased version as current**.

**Docs are a Gate B item, not a Gate A one.** The beta ships before the docs are written - that is deliberate,
and it is why the beta exists. The site *was* frozen in the meantime: `docs/_data/versions.yml` said
`current: v3.0.x` while that folder held only `index.md`, so `/version/latest/` redirected to an empty version
and the site could not be deployed for any reason - not even a typo fix or the open CodeQL alert. **B0 removed
that freeze on 2026-08-06** and deployed, taking the CodeQL fix with it. **B8 put it back on 2026-08-10**: the
pages exist, `current` is `v3.0.x` again and the version is relisted, so only the deploy remains.

---

## The two subtle breaking changes, explained

Four break in total. The other two need no explanation here - V2 endpoints are removed, which is the headline of
the release, and the packing-logs configuration was flattened, which the migration guide already walks through
step by step. These two are the ones a reader can misjudge.

1. **ViPaq tokens.** Old tokens no longer decode and there is no fallback reader. Verified to fail loudly rather
   than misparse. Note that images at `v2.1.1` and earlier keep producing the old format - they are unaffected
   and need no change, but a user running an old and a new image side by side will find their tokens do not
   cross. That is step 4 of the migration guide in the notes.

2. **Health check `RestrictedIPs`.** Three changes, one of which **narrows existing allow-lists**:
   - CIDR now means a prefix length. The value after `/` was read as an address mask, so `192.168.1.0/24`
     matched nearly the whole IPv4 range. Anyone relying on a CIDR entry must re-check who is inside it or risk
     locking themselves out.
   - IPv4 callers arriving in IPv4-mapped IPv6 form are unmapped before matching, so the list works in a
     container at all. It previously could match no IPv4 entry.
   - The `start-end` range form is removed and now fails startup validation.

   - Entries are read exactly as written. `010.10.10.10` used to be octal and admit `8.10.10.10`, `10.1` used to
     mean `10.0.0.1`; both now fail startup. IPv6 must be in short lowercase form.

   `IPAddressRange` was deleted; matching is `System.Net.IPNetwork` via `Binacle.Net.Kernel/Network/IPEntry`.

Also new, not breaking: **forwarded headers** (`Config_Files/ForwardedHeaders.json`, disabled by default) and the
**`/_debug` endpoint** (`DEBUG_ENDPOINT`, disabled by default). `ASPNETCORE_FORWARDEDHEADERS_ENABLED` is
deliberately ignored.

---

## The sequence

Rewritten 2026-08-10, when B2 landed and a second beta was decided. Gate A is green, beta 1 is verified, the
docs are written. What is left is beta 2, B6, then the tag and the deploy.

**The ordering rule that drives all of it:** a pin on `main` must name an image that exists, and the docs may
not be deployed before the tag, because `main` already says v3.0.x is current. So each pin bump follows its
publish, and the docs deploy follows the tag.

1. ~~Gate A.~~ Done - A1, A3, A4 landed 2026-07-27/30, A2 answered 2026-08-06.
2. ~~Publish the beta image and deploy it.~~ Done - published 2026-07-30.
3. ~~B1 - beta 1 verification.~~ Done 2026-08-06, all boxes pass, no defects. Two non-blocking actions remain -
   turn `DEBUG_ENDPOINT` off, and re-confirm the resolved caller once the forwarded-headers source header
   settles. Both are now on beta 2's list at step 8, which is the run that closes them.
4. ~~B0 - unfreeze and deploy the docs site.~~ Done 2026-08-06, with the CodeQL `js/xss-through-dom` fix in the
   same deploy.
5. ~~B4 and B3.~~ Both done - B4 on 2026-08-06, B3 written on 2026-08-07 along with the two general-page audit
   fixes. `v3.0.x/vipaq-protocol.md` already exists, so B2 does not write it.
6. ~~B2 - write the `v3.0.x` pages.~~ Done 2026-08-10. The long pole is down; everything below it is small.
7. **Confirm the suites are green before cutting beta 2.** `just test all` - eleven leaves, nothing to bring
   up, about two minutes. This step was never written down and should have been: the sweep that beta 2 exists
   to test is exactly the kind of change a suite catches first and cheapest. Green on 2026-08-10.
8. **B2X - tag `v3.0.0-beta.2`, then work its verification list.** The list is short because B1 covered the
   deployment-shaped behaviour and the middleware it exercised did not change. Turning `DEBUG_ENDPOINT` off and
   re-confirming the resolved caller are both on it.
9. **Bump the nine files from `3.0.0-beta.1` to `3.0.0-beta.2`** (B5), once beta 2 is actually on Docker Hub.
   Six pins plus `README.md`, `samples/README.md` and `samples/docker/README.md`.
10. B6 - the one Azure Storage run.
11. **Bump the same nine files to `3.0` as the last change before the tag**, then tag `v3.0.0`. Drop the
    two-line internal comment above each `image:` line in the same change, and sweep the two `config/` examples.
    Re-confirm B7a - `ApiV4Document.IsExperimental` still `true` - right before tagging.
    `release-docker-image.yml` publishes the final image on `release: published`. A2 confirmed no `3.0` tag
    exists yet, so the bump is safe only once the tag is about to be published.
12. Paste `release-notes-v3.0.0.md` into the release body, with all four breaking changes in it (B7b).
13. **Smoke the published image before announcing anywhere:** `just smoke all binacle/binacle-net:3.0.0`. The
    release workflow pushes without smoking - wiring that in is `ci-release-workflow-build` and is not done - so this
    manual run is the only thing between a broken image and the people who pull it. It takes about a minute and
    needs nothing brought up. The same command passed against `3.0.0-beta.1` on 2026-08-07.
14. **Release the docs - B8's deploy.** B2's pages and B8's config are both on `main` already; this step is the
    deploy, plus the three additions and the real date and release link in `v3.0.x/release-notes.md`. It is the
    easiest item in this file to lose - nothing fails if it is skipped, the site just silently stays on v2.1.x.

    **It has to run after the tag, not before.** Two things need the tag to exist: the release notes date and
    its `releases/tag/v3.0.0` link, and the fact that `main` already says v3.0.x is current, so deploying
    earlier presents an unreleased version as current. It still has to land before anything is announced,
    because the announcement points at pages that must be live. Tag, then deploy the docs, then announce.
15. Work `post-release-v3.0.0.md`.

## Not in this release

Everything else has a plan of its own and is on `board.md`, grouped by area with its blockers named. Do not pull
any of it in: CI work, the version stamp, the npm publishing decision, the `Parallel*` processors, migrating the
UI clients off v3, the benchmark ledger, TestsKernel fixtures, and v4 going stable in 3.1.0.
