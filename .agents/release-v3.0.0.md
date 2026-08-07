# Release - Binacle.Net v3.0.0

**Status:** In progress - **Gate A green, beta published and verified, docs unfrozen.** What is left is the
`v3.0.x` documentation (B2, the long pole, now unblocked in every direction) and the small pre-tag items.
**Created:** 2026-07-16. **Restructured:** 2026-07-26. **Status rewritten:** 2026-08-06. **B3 landed:**
2026-08-07.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to the
reference rules: it may point at any file to coordinate the release, and **nothing points back at it**. Delete it
once v3.0.0 is out.

Companions:
- `release-notes-v3.0.0.md` - the GitHub release body, ready to paste.
- `post-release-v3.0.0.md` - what to do once the release is out.

## How to work this file

Two gates. **Gate A** must be green before the beta image is published; **Gate B** before the final tag. Each row
is either a link to a plan under `.agents/plans/` that holds the whole item, or a checkbox for a one-line action
with a known answer.

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
| B2 | Write the `v3.0.x` docs pages, including the two new configuration pages | [docs-v3-pages](plans/docs-v3-pages.md) |
| B3 | Fix the ViPaq protocol page | **done 2026-08-07** - split landed, all four versions written |
| B4 | Generate `swagger/v3.json` and `swagger/v4.json` | **done 2026-08-06** - generated and checked, but they sit in gitignored `build/openapi/`; moving them into `v3.0.x/swagger/` is B2 |
| B5 | Bump the five sample image pins to `3.0` - **no plan** | see below |
| B6 | Run the ServiceModule suite once against Azure Storage - **no plan** | see below |
| B7 | Confirm v4 still ships experimental, then announce all four breaking changes - **no plan** | see below |
| B8 | Flip `current` forward to `v3.0.x` again and redeploy the docs - **no plan** | see below |

**B1 came back clean.** All boxes pass. The three changes the beta existed to test - forwarded headers, the
health check allow-list, and rate limiting on the resolved caller - all behave as designed against a real
proxied deployment, and no defect was found. Worth stating plainly, because the first pass was HTTP-only and
read more confident than its evidence supported; the four boxes it could not reach were closed the same day
from the container log and filesystem, and they closed *in favour* of the release rather than against it.

Two loose ends, neither blocking the tag. They are the whole of what B1 left behind, so they live here now:

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
v3.0.0, no release-notes line owed. It is written down as one sentence for the ServiceModule configuration page
in [docs-v3-pages](plans/docs-v3-pages.md).

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

- [x] **B5 - the sample image pins.** Done 2026-08-07, but **earlier than this item intended** - read the caveat.
  All six samples now pin **`3.0`** (the minor tag, not `3.0.0`): `samples/docker/*/docker-compose.yml` and
  `samples/kubernetes/minimal/binacle-deployment.yaml`. A2 confirmed the beta published no `3.0` tag, so nothing
  points at a prerelease.

  **The caveat.** This item said to bump in the last change before the tag, because a bump sitting on `main`
  names an image that does not exist. That still holds and is now live: `3.0` appears on Docker Hub only when
  v3.0.0 is published. The pins moved early because the samples were restructured in the same pass and the new
  ones (`prod`, `service`, `full`) document v3-only settings - forwarded headers, `RetentionDays`, the
  ServiceModule split - so pinning `2.1.1` would have been wrong in a different and worse way. **Do not leave
  this on `main` long before tagging.**

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

- [ ] **B7a - v4 is still experimental.** `ApiV4Document.IsExperimental` was set `true` on 2026-07-25, so the
  published OpenAPI document carries the warning that v4 may change at any time. Check it is still `true` before
  tagging - shipping v4 as stable would lock contracts that are meant to keep moving. The flip is 3.1.0 work.

- [ ] **B7b - announce all four breaking changes** in the GitHub release body: V2 endpoints removed, ViPaq
  tokens, the flattened packing-logs configuration, and health check `RestrictedIPs`. All four are already
  written into `release-notes-v3.0.0.md`, along with a six-step migration guide - this is the check that they
  made it in. The packing-logs step is the one most easily lost, and leaving it out fails a user's startup with
  no explanation. The two that need the extra explanation are in the section below.

- [ ] **B8 - flip `current` forward again.** B0 points `docs/_data/versions.yml` at `current: v2.1.x` to unfreeze
  the site. That has to be undone as part of releasing the docs, or v3.0.0 ships with the docs site still
  presenting v2.1.x as current and `/version/latest/` still redirecting there. Relist `- id: v3.0.x` at the top
  of `list`, set `current: v3.0.x`, drop whatever B0 did to hide the stub, restore
  `docs/collections/_sitemaps/version-3-0-x.xml` (B0 deleted it, four lines - without it the v3.0.x pages are
  never submitted for indexing), then deploy. **This is the single
  most losable item in Gate B** - it is an undo of a change made weeks earlier, it lives in a file nobody
  otherwise touches, and nothing fails if it is skipped. The site simply stays on the old version.

**Docs are a Gate B item, not a Gate A one.** The beta ships before the docs are written - that is deliberate,
and it is why the beta exists. The site *was* frozen in the meantime: `docs/_data/versions.yml` said
`current: v3.0.x` while that folder held only `index.md`, so `/version/latest/` redirected to an empty version
and the site could not be deployed for any reason - not even a typo fix or the open CodeQL alert. **B0 removed
that freeze on 2026-08-06** and deployed, taking the CodeQL fix with it. `current` is `v2.1.x` and v3.0.x is
delisted until its pages exist. **B8 puts it back**, and the same steps are written into
[docs-v3-pages](plans/docs-v3-pages.md) so the session that writes the pages also relists the version.

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

Rewritten 2026-08-06, after B1 came back clean. Gate A is green, the beta is verified, and the only work left
is docs plus the four small pre-tag items.

1. ~~Gate A.~~ Done - A1, A3, A4 landed 2026-07-27/30, A2 answered 2026-08-06.
2. ~~Publish the beta image and deploy it.~~ Done - published 2026-07-30.
3. ~~B1 - beta verification.~~ Done 2026-08-06, all boxes pass, no defects. Two non-blocking actions remain and
   are listed under Gate B: turn `DEBUG_ENDPOINT` off, and re-confirm the resolved caller once the
   forwarded-headers source header settles.
4. ~~B0 - unfreeze and deploy the docs site.~~ Done 2026-08-06, with the CodeQL `js/xss-through-dom` fix in the
   same deploy.
5. ~~B4 and B3.~~ Both done - B4 on 2026-08-06, B3 written on 2026-08-07 along with the two general-page audit
   fixes. `v3.0.x/vipaq-protocol.md` already exists, so B2 does not write it.
6. **B2 - write the `v3.0.x` pages.** Fully unblocked: B1's answers are in, so the two new configuration pages
   document behaviour observed in a real deployment rather than assumed, and B3 and B4 have both handed over.
   This is the long pole - everything below it is small.
7. B6, B7a.
8. **Release the docs: B2's pages plus B8** (flip `current` back to `v3.0.x`). B8 is the undo of B0 and is the
   easiest item in this file to lose - nothing fails if it is skipped, the site just silently stays on v2.1.x.
9. **B5 as the last change before the tag**, then tag `v3.0.0`. `release-docker-image.yml` publishes the final
   image on `release: published`. A2 confirmed no `3.0` tag exists yet, so the bump is safe.
10. Paste `release-notes-v3.0.0.md` into the release body, with all four breaking changes in it (B7b).
11. **Smoke the published image before announcing anywhere:** `just smoke all binacle/binacle-net:3.0.0`. The
    release workflow pushes without smoking - wiring that in is Gate 5 of `ci-gates` and is not done - so this
    manual run is the only thing between a broken image and the people who pull it. It takes about a minute and
    needs nothing brought up. The same command passed against `3.0.0-beta.1` on 2026-08-07.
12. Work `post-release-v3.0.0.md`.

## Not in this release

Everything else has a plan of its own and is listed in `post-release-v3.0.0.md` or the plans index. Do not pull
any of it in: CI work, the version stamp, the npm publishing decision, the `Parallel*` processors, migrating the
UI clients off v3, the benchmark ledger, TestsKernel fixtures, and v4 going stable in 3.1.0.
