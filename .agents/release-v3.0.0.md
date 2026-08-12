# Release - Binacle.Net v3.0.0

**Status:** In progress - **Gate A green, beta 1 published and verified, docs written.** B2 landed, so the long
pole is down: the `v3.0.x` folder is a full version and the site builds. What is left is **a second beta**
(see Gate B2X), then B6, then the tag and the docs deploy.
**Created:** 2026-07-16. **Restructured:** 2026-07-26. **Status rewritten:** 2026-08-06. **B3 landed:**
2026-08-07. **B2 landed and beta 2 added:** 2026-08-10. **Rewritten for the GHCR pipeline:** 2026-08-11.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to the
reference rules: it may point at any file to coordinate the release, and **nothing points back at it**. Delete it
once v3.0.0 is out.

**The release pipeline was replaced on 2026-08-11, and it changes this file throughout.** The old workflow built,
pushed straight to Docker Hub, smoked there and promoted the moving tags. The new one stages everything on GHCR,
smokes it there, and **copies the smoked digest to Docker Hub** - so nothing unsmoked ever reaches the registry
users pull from. Three consequences run through everything below, so they are stated once here:

- **GHCR is staging only.** Every tag is copied to Docker Hub after it is smoked, betas included - a prerelease
  just gets its immutable tag, never `3.0` or `latest`. Beta 2 shipped on 2026-08-11 under an earlier rule that
  stopped prereleases at GHCR; that rule was reversed the same day, so `3.0.0-beta.2` is on GHCR only and every
  later tag lands on Docker Hub normally.
- **The release body now comes from `CHANGELOG.md`**, extracted by the workflow. `release-notes-v3.0.0.md` was
  deleted on 2026-08-11; its content is the `## [Unreleased]` section, byte for byte.
- **Beta 2 is the first run of the new pipeline**, which is why it is a bigger event than it was.

Companion:
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
| B9 | Strip the `AI-GENERATED` review tokens - **no plan** | see below |

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
- **The release pipeline is new as of 2026-08-11.** Six jobs, GHCR staging, a Docker Hub copy that a prerelease
  skips, cosign signing, an SBOM and provenance. Beta 2 is its first run end to end - see the second job below.
- **The image is framework-dependent as of 2026-08-10, and beta 2 is the first one built that way.** The
  publish dropped `--self-contained`; it had been set while the Dockerfile bases on `aspnet:10.0`, so every
  image carried two copies of .NET - the bundled one the app ran on, and the base image's, which nothing
  loaded. **This is the one change in beta 2 that alters the artifact**, so it is the one to keep in mind if
  the deployed image behaves oddly. Measured and tested locally before promotion:

  | | Before | After |
  |---|---|---|
  | Image | 150.2 MB | **103.2 MB** |
  | Publish output | 123 MB | **18 MB** |
  | `System.*.dll` shipped | 172 | **4** |
  | `runtimeconfig.json` | `includedFrameworks` | `frameworks` |

  All 31 structure assertions, all five smoke profiles and all eleven test leaves green on the rebuilt image.
  The entrypoint did not change - `dotnet Binacle.Net.dll` was always the framework-dependent idiom, which is
  what made the old pairing wrong in the first place.

The verification list is short, because B1 already covered the deployment-shaped behaviour and none of the
middleware it exercised changed except log-template casing:

- [ ] Tag `v3.0.0-beta.2` and confirm all six jobs run: the changelog gate, the suite, the GHCR build, the
      smoke, **`publish` skipped**, and the release created anyway. Nothing has to be set up on GHCR first -
      the build job creates the package on its first push and the `Dockerfile`'s
      `org.opencontainers.image.source` label links it to the repo.
- [ ] **Then set the package public.** It is created private - GHCR's default for every new package, whatever
      the repo's visibility - and it cannot be changed before it exists. This is the only manual step in the
      pipeline, and everything about pulling a beta without a credential depends on it.
- [ ] **Confirm Docker Hub is untouched.** No `3.0.0-beta.2` tag, `latest` and `3.0` where they were. This is
      the A2 check restated for a pipeline that now skips the whole publish job rather than emitting an empty
      tag list.
- [ ] Confirm `BINACLE_VERSION` inside the image reads `3.0.0-beta.2`, with no leading `v`. The fix that
      strips the `v` was already in beta 1 (`c6981e90`, checked against the tag), so this is a confirmation
      rather than a first test - but it was never actually observed on a published image, and it is one
      `docker inspect` away.
- [ ] Confirm the smoke job passed against the GHCR copy, and that the image carries its SBOM, provenance and
      cosign signature: `cosign verify` plus `docker buildx imagetools inspect` on the digest.
- [ ] Pull it on the deployment host **with no `docker login`** -
      `docker pull ghcr.io/chrismavrommatis/binacle-net:3.0.0-beta.2`. This is the check that the public
      package actually works from outside, and the whole beta deployment depends on it.
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

**Beta 2's second job: it is the first run of the new pipeline.** The rebuild landed on 2026-08-11, before
v3.0.0 rather than after it, so beta 2 exercises a publish path nothing has ever run. That is deliberate - a
prerelease tag is the only free test this pipeline will ever get, and a failure costs a deleted tag instead of a
bad release.

**But a prerelease does not test all of it, and the untested part grew.** `publish` is skipped for a beta, so
the Docker Hub copy **and** the second cosign signature both wait for v3.0.0 itself. Under the old design the
untested step was one `imagetools create`; it is now a copy plus a signature against a registry the pipeline has
not touched in that run.

- [ ] **Close it with a throwaway tag before v3.0.0 - but point it at a scratch repo, not the real one.**
      The naive version of this check is a trap, twice over:

      - **The tag must not contain a hyphen.** A hyphen is the prerelease marker, so `v0.0.1-pipeline-test`
        would skip `publish` exactly like a beta does and prove nothing while looking green.
      - **A clean `v0.0.1` would move `latest` on Docker Hub.** `metadata-action` never contacts the registry -
        it reads the git ref and nothing else - so `latest=auto` marks any non-prerelease semver as latest with
        no comparison against what is already published. `0.0.1` would take `latest` off `2.1.1`.

      So repoint the registry rather than weakening the tag:

      1. Change the repo variable `DOCKERHUB_REPO` from `binacle-net` to `binacle-net-pipeline-test`. It is a
         variable precisely so this is one edit and no code change.
      2. Tag `v0.0.1` - no hyphen, so `publish` actually runs.
      3. Confirm `0.0.1`, `0.0` and `latest` all land in the scratch repo on the digest the smoke job passed,
         and that `cosign verify` passes against the Docker Hub copy.
      4. Delete the scratch Docker Hub repo, the git tag, and the GitHub release. Delete the `0.0.1` version
         from the GHCR package too.
      5. **Change `DOCKERHUB_REPO` back.** This is the step that gets forgotten, and forgetting it publishes
         v3.0.0 to the wrong repo - see step 13 of the sequence, which re-checks it.

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

- [ ] **B5 - the image pins move once, to `3.0`, as the last change before the tag.** They sit at
  `3.0.0-beta.1` today and **they stay there until then.**

  **This item said "twice" and could go back to saying it, but should not.** The rule it has always carried
  still applies - **a pin on `main` must name an image that exists on Docker Hub** - and betas do reach Docker
  Hub again, so a `3.0.0-beta.2` bump is possible. It is not worth it: `3.0.0-beta.2` is not on Docker Hub
  (it shipped under the brief rule that stopped prereleases at GHCR), beta 3 may never happen, and churning
  nine files twice to land on `3.0` a week later buys nothing a reader would notice.

  **It is nine files, not six.** The six the item originally named are the pin itself; three more carry the
  beta in prose, and they are the ones that get missed:

  | File | What to change |
  |---|---|
  | `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` | the `image:` line |
  | `samples/kubernetes/minimal/binacle-deployment.yaml` | the `image:` line |
  | `README.md` | the pin warning under Quick Start - **the repo landing page**, currently telling every visitor to pin `3.0.0-beta.1` |
  | `samples/README.md` | the pin paragraph |
  | `samples/docker/README.md` | the pin paragraph |

  **The two-line comment above each `image:` line is already dropped** - done ahead of the bump, in the
  comment-pruning pass, because the durable sentence is right at any pin value. All six now read exactly
  `# Pinned on purpose - a copied sample must not jump to a new major on the next pull.`, matching the
  published docs copies. Only the `image:` lines themselves are left to move.

  Two more mention the beta as an **example** rather than a pin - `tooling/README.md` and `tooling/smoke.just`,
  both showing "smoke what is actually on Docker Hub". Neither is wrong today and neither ships to a user, but
  they read as stale once the tag is out. Sweep them at the `3.0` bump.

  **The caveat, unchanged.** The pins moved early once before, on 2026-08-07, and sat on `main` naming an image
  that did not exist. They moved because the samples were restructured in the same pass and the new ones
  (`prod`, `service`, `full`) document v3-only settings - forwarded headers, `RetentionDays`, the ServiceModule
  split - so pinning `2.1.1` would have been wrong in a different and worse way. A prerelease pin is the fix
  for the same reason: real and v3-only, without naming a tag that is not there yet. **Do not leave the `3.0`
  bump on `main` long before tagging.**

  **One sentence to re-read at the bump.** `README.md`, `samples/README.md` and `samples/docker/README.md` all
  explain the beta pin with some version of "since `3.0` does not exist on Docker Hub yet". That reason expires
  the moment v3.0.0 publishes, so the prose goes with the pin - not just the number.

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
  written into the `## [Unreleased]` section of `CHANGELOG.md`, along with a six-step migration guide - this is
  the check that they made it in. The packing-logs step is the one most easily lost, and leaving it out fails a
  user's startup with no explanation. The two that need the extra explanation are in the section below.

  **Preview it rather than reading the file:** `just changelog extract Unreleased` prints exactly what the
  workflow will publish, headings and all.

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
  - [ ] **Carry three additions from `CHANGELOG.md` into `v3.0.x/release-notes.md`.** The two are the same
    notes in two places, and the release body gained content on 2026-08-10 that the page does not have. Run
    `just changelog extract Unreleased` to see the current text. All three are for the `## v3.0.0` section,
    worded to match the page's plain-ASCII style:
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
  - [ ] **Replace the two stale swagger documents under `docs/collections/_versions/v3.0.x/swagger/`.** They
    are frozen copies of the generated OpenAPI documents, and as of the `servers` fix on 2026-08-10 they no
    longer match what `just openapi generate` produces (both v3 and v4 now carry a `servers` entry with the
    single relative `/`; these copies still have none). Regenerate with `just openapi generate` and copy
    `build/openapi/Binacle.Net_v3.json` -> `docs/collections/_versions/v3.0.x/swagger/v3.json` and
    `build/openapi/Binacle.Net_v4.json` -> `docs/collections/_versions/v3.0.x/swagger/v4.json` - note the
    generator's file names differ from what the site expects, so the rename is part of the handover, not
    a detail to skip. Shipping the docs without this leaves the published spec disagreeing with the released
    image.
  - [ ] Deploy the docs. It is `workflow_dispatch` only.

  **This is still the single most losable item in Gate B** - nothing fails if the deploy is skipped, the site
  just quietly keeps serving v2.1.x as current. And note the ordering trap the config half creates: `main` now
  says v3.0.x is current, so **deploying before the tag publishes an unreleased version as current**.

- [x] **B9 - strip the `AI-GENERATED` review tokens. Done 2026-08-11 - zero left, whole repo.** A
  comment-provenance pass on 2026-08-10 tagged every
  comment written by an agent with a marker line (`# AI-GENERATED` / `// AI-GENERATED`) directly above the
  block, so the maintainer can rewrite each one by hand and keep the comments in a human voice. **They are
  scaffolding and must not ship.** The first pass marked 1,223 comments, which was unreviewable; it was then
  triaged down on the same day to the ones that actually carry a claim worth checking - a reason, a risk, a
  measurement, a "never do X". Comments that only restate what the code does had their marker dropped and are
  considered accepted as written.

  The maintainer cleared `samples/**` and `docs/**` first - readers copy those files verbatim - then worked
  the rest down to zero. **`grep -rn "AI-GENERATED"` now returns nothing anywhere in the repo.** The only
  remaining hits for that string are this file and the pre-existing `AI-generated search summaries` line in
  `docs/pages/robots.txt` and `web/pages/robots.txt`, neither of which is a marker.

  **Where an agent's rewrite was kept on purpose, that is the decision** - the pass was never "revert
  everything an agent wrote", and several agent comments were judged better than the line they replaced
  (the unchecked-multiply overflow note on the packing algorithms, the empty-catch explanation in
  `ConnectionString.cs`, the curated-scenario table in `BischoffCuratedProblemsProvider.cs`). A later
  session must not treat a surviving agent comment as damage to undo.

  **The `docs/` half was done by a coding session, against the usual rule**, at the maintainer's explicit
  instruction on 2026-08-10. It touched no prose, no front matter and no `.md` file - only sample YAML,
  `_config.yml`, `_data/versions.yml` and one plugin. Recorded here because that rule says every crossing
  gets recorded. `web/` was scanned and needed nothing: 60 human comment lines, no agent-written ones.

**Docs are a Gate B item, not a Gate A one.** The beta ships before the docs are written - that is deliberate,
and it is why the beta exists. The site *was* frozen in the meantime: `docs/_data/versions.yml` said
`current: v3.0.x` while that folder held only `index.md`, so `/version/latest/` redirected to an empty version
and the site could not be deployed for any reason - not even a typo fix or the open CodeQL alert. **B0 removed
that freeze on 2026-08-06** and deployed, taking the CodeQL fix with it. **B8 put it back on 2026-08-10**: the
pages exist, `current` is `v3.0.x` again and the version is relisted, so only the deploy remains.

---

## The release notes - how to keep them, and what they must say {#release-notes}

**The notes live in `CHANGELOG.md` at the repo root, in the `## [Unreleased]` section.** The workflow extracts
that section and publishes it as the release body. `release-notes-v3.0.0.md` was deleted on 2026-08-11 - its
content moved across byte for byte, and keeping both would have been two sources of truth for one paragraph.

**The one edit this adds to the release.** `[Unreleased]` is renamed to `## 3.0.0` as the **last change before
the v3.0.0 tag**, alongside the B5 pin bump. Every beta publishes `[Unreleased]`, so nothing is renamed until
the real tag. If you forget, the `notes` job fails in under a minute and nothing is built - which is the point
of putting that gate first.

**In the file, a section's own headings are `###`**, nested under the `##` version heading. `just changelog
extract` shifts them back to `##` on the way out, so the published body matches every earlier release. Do not
"fix" the file to use `##` throughout - that breaks the nesting under `# Changelog` and the extractor's
terminator both.

**Preview before you tag:** `just changelog extract Unreleased` prints exactly what will be published.

**So when you change the notes, change `CHANGELOG.md` and keep this section true.** If a fact below stops
holding - a new commit that a user *can* observe, a section that gains a caveat - both have to move.

### Style, taken from the maintainer's published releases

See https://github.com/ChrisMavrommatis/Binacle.Net/releases.

- Opens with one line: `Binacle.Net vX.Y.Z is a major update from vA.B.C.`
- A breaking release uses a GitHub alert - `> [!Warning]` - then `---`.
- Sections, in this order, **only the ones that apply**: `🔎 Overview`, `⚙️ Core Changes`,
  `🧪 Diagnostics Module`, `🔌 Service Module`, `🎨 UI Module`, `📈 Algorithms`, `🏗️ Internal Work`,
  `📚 Versioned Docs`, `🛠️ Migration Guide`.
- Bullets are short, past tense, **bold** on the key term, code and paths in backticks. Lines end with two
  spaces (a markdown line break). **No tables** - the published releases use none.
- Migration guide: `To upgrade to **vX.Y.Z**, follow these steps:` then numbered `**Bold title**` steps with
  indented `-` sub-bullets.
- Closes with `---` and a `**Full Changelog**:` compare link.
- A minor/patch release drops all of this and is just `## Overview` with a few plain bullets.

### Scope, and why the body needs nothing for the post-beta work

**Scope:** `v2.1.1` (2026-01-13, the last shipped image) -> now. `v3.0.0-beta.1` sits at 186 commits past
`v2.1.1`, and 27 more have landed since that tag. **The body needs no change for any of them** - but note the
reason, because an earlier version of this got it wrong. It is not that the later work misses the image; part
of it ships. It is that none of it changes anything a user can observe, so there is nothing to announce.
Checked 2026-08-06, re-checked through `ea9f035b` on 2026-08-10, the second time against the `v3.0.0-beta.1`
tag rather than the commit log:

- `npm audit fix` / `bundle audit-fix` on the root, `docs/` and `web/` lockfiles. Every advisory closed was a
  devDependency (`npm audit --omit=dev` on the pre-fix lockfile returned 0). The Dockerfile copies only
  `build/binacle-net`, and the UI module's JavaScript is hand-written and committed, not bundled.
- The docs site unfreeze - `current` back at `v2.1.x` until the `v3.0.x` pages exist. Site content, not product.
- The CodeQL `js/xss-through-dom` fix in `docs/_js/main.js`. Docs-site hardening, and not exploitable as the
  code stood (both inputs were build-time constants).
- `.agents/` notes and `.nvmrc`.
- **The Sonar sweep (2026-08-07 -> 08-09) and the BOM removal.** Large by file count and **it ships** - this is
  the part the old wording denied. Every change is a refactor: extracted methods, media-type constants over
  string literals, handlers made `static`, discards for unused locals, `{Placeholder}` casing in log templates.
  The largest single file is `ServiceModule/v0/Endpoints/Auth/Token.cs`, whose rejection chain became one
  extracted `Reject` helper. The two behaviour-shaped ones are not: collapsing nested `if`s in `PackResponse` /
  `BinResponseBase` keeps the same condition, and `WriteAsync` on `/_debug` merely takes the request's
  cancellation token. The forwarded-headers and health check middleware changed only log-template casing, so
  **B1's beta verification still holds against this code** - and **beta 2 is what proves the rest of it in a
  real deployment** rather than on the strength of this paragraph.
- **ViPaq's source changed comments only.** `Header.cs`, `ProtocolEncoder.cs`, `DeflateCodec.cs` and
  `ViPaqSerializer.cs` have no behavioural diff against the beta 1 tag, so the wire format is untouched and the
  ViPaq lines in the body need no caveat.
- **The Dockerfile did not change after beta 1 at all.** `/app/data`, `libgssapi-krb5-2` and the OCI labels
  were all in that image already, so the `⚙️ Core Changes` lines describing them are announcing something that
  has already run in a deployment.
- The docs-v3 merge (`3dc6f1ac`) and the sample hardening under `docs/collections/_versions/**` - site content.

**Three more that land after this section was written**, and the third is the one that actually earns a line:

- The `servers` entry added to both OpenAPI documents on 2026-08-10. It is the value OpenAPI already defaults
  to, so nothing observable changed.
- The release-pipeline rebuild on 2026-08-11. The workflow does not touch the app.
- **The image now carries an SPDX SBOM, SLSA provenance and a cosign signature.** This one *is* user-visible -
  it is something a consumer can verify that they could not verify before - so **it wants a bullet in
  `⚙️ Core Changes`**, with the `cosign verify` invocation. It is the only post-beta change in this release that
  a user can observe, and it was not in the body when the pipeline landed. Check it is there before publishing.

### Before publishing

- Confirm the version number and the compare link at the bottom.
- Fitting was verified unchanged (2026-07-19), so `📈 Algorithms` needs no caveat.
- All four breaking changes must be present (B7b), along with the migration guide.
- The signing/SBOM bullet is present - see the third item above.
- `## [Unreleased]` has been renamed to `## 3.0.0`. The `notes` job fails the build if not.
- The same content has to reach `v3.0.x/release-notes.md` on the docs site - B8 lists the three additions that
  page is still missing.

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

Rewritten 2026-08-11, when the GHCR pipeline landed. Gate A is green, beta 1 is verified, the docs are written,
the pipeline is rebuilt. What is left is the GHCR setup, beta 2, B6, then the tag and the deploy.

**The ordering rule that drives all of it:** a pin on `main` must name an image that exists **on Docker Hub**,
and the docs may not be deployed before the tag, because `main` already says v3.0.x is current. There is
exactly **one** pin bump left - to `3.0`, just before the tag - and the docs deploy still follows the tag. See
B5 for why the intermediate beta bump is not worth doing.

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
8. ~~Land the release-pipeline rebuild.~~ Done 2026-08-11, and it went in **before** v3.0.0 rather than after -
    six jobs, GHCR staging, a Docker Hub copy a prerelease skips, cosign signing, SBOM and provenance. It sits
    in the working tree, unshipped. The plan on finishing the GHCR pipeline holds what is left of it.
9. **B2X - tag `v3.0.0-beta.2`, then work its verification list.** This is the first end-to-end run of the new
    pipeline as well as the code check. Expect five jobs to run and `publish` to be skipped. Turning
    `DEBUG_ENDPOINT` off and re-confirming the resolved caller are both on the list.

    **No GHCR setup is needed first.** The build job creates the package on its first push - `packages: write`
    is enough, and the `Dockerfile`'s `org.opencontainers.image.source` label is what links it to the repo. The
    smoke job pulls it back with the same run's token, so the whole pipeline works on a package nobody has
    touched.
10. **Set the GHCR package to public** - the one manual step, and it can only happen **after** step 9, because
    the package does not exist until the first push and GHCR defaults every new one to private. Package
    settings, change visibility. Then confirm a credential-free `docker pull
    ghcr.io/chrismavrommatis/binacle-net:3.0.0-beta.2` from the deployment host - until this is done the pull
    needs a token, which is exactly what the public package exists to avoid.
11. **Do the throwaway-tag check.** A beta skips `publish` entirely, so the Docker Hub copy and the release
    signature are still untested. **Point `DOCKERHUB_REPO` at a scratch repo first**, then tag `v0.0.1` - with
    no hyphen, or `publish` skips again, and against a scratch repo, or `latest=auto` moves `latest` off
    `2.1.1`. Full detail is on B2X's list. **Do this before v3.0.0, not after** - it is the only way that job
    runs twice.
12. B6 - the one Azure Storage run.
13. **The last change before the tag, all in one commit:** rename `## [Unreleased]` to `## 3.0.0` in
    `CHANGELOG.md`, bump the nine files from `3.0.0-beta.1` to `3.0`, and sweep the "since `3.0` does not exist
    yet" prose in the three READMEs. Then re-confirm B7a - `ApiV4Document.IsExperimental` still `true` - and
    tag `v3.0.0`. Also sweep the two `tooling/` examples that name a beta tag.

    **First, check `DOCKERHUB_REPO` reads `binacle-net` again.** Step 11 changes it and changing it back is the
    easiest thing in this release to forget. Nothing fails if it is wrong - v3.0.0 just publishes to the scratch
    repo, `latest` never moves, and the first person to notice is a user.
14. **The pipeline does the rest.** The tag triggers it: the changelog gate, the suite, the GHCR build, the
    smoke, the Docker Hub copy under all three tags, the signature, and the release created from the `3.0.0`
    section. **Nothing here is manual any more** - what used to be steps 13 and 14 (paste the body, smoke the
    published image by hand) are both inside the workflow now. Watch the run instead, and check the rendered
    body and `docker buildx imagetools inspect` afterwards.
15. **Release the docs - B8's deploy.** B2's pages and B8's config are both on `main` already; this step is the
    deploy, plus the three additions and the real date and release link in `v3.0.x/release-notes.md`. It is the
    easiest item in this file to lose - nothing fails if it is skipped, the site just silently stays on v2.1.x.

    **It has to run after the tag, not before.** Two things need the tag to exist: the release notes date and
    its `releases/tag/v3.0.0` link, and the fact that `main` already says v3.0.x is current, so deploying
    earlier presents an unreleased version as current. It still has to land before anything is announced,
    because the announcement points at pages that must be live. Tag, then deploy the docs, then announce.
16. Work `post-release-v3.0.0.md`.

## Not in this release

Everything else has a plan of its own and is on `board.md`, grouped by area with its blockers named. Do not pull
any of it in: the version stamp, the npm publishing decision, the `Parallel*` processors, migrating the UI
clients off v3, the benchmark ledger, TestsKernel fixtures, and v4 going stable in 3.1.0.

**A second exception, decided 2026-08-10: dropping `--self-contained`.** Landed in `build.just` and the release
workflow after being measured and fully tested locally - 150.2 MB down to 103.2 MB, all smoke and test suites
green. It is in because beta 2 is the run that proves it on a real deployment, and because shipping v3.0.0 with
two .NET runtimes in the image is a worse outcome than testing one flag during a beta. The rest of the image
work - chiseled bases and the hardened-image question - stays out.

**One exception, decided 2026-08-10: the release-workflow rebuild is in, and beta 2 is its test.** CI work was
on this list until then. The reason for the change is that a prerelease tag is the only free test that pipeline
will ever get - it runs the whole path, and metadata-action's guards mean it moves neither `latest` nor the
minor tag, so a failure costs a deleted tag instead of a bad release. Deferring it meant the first run of a
rebuilt publish path would be v3.0.0 itself, which is the one release that cannot be taken back.

Scope is capped at what a prerelease can prove: build through the recipe, the tag-triggered restructure with
digest promotion, and the smoke gate. SBOM, provenance and multi-arch stay out - they change the artifact, and
nothing about them needs the beta. The CI plan holds the detail, including the one step a prerelease **cannot**
test and the five-minute manual check that closes it.
