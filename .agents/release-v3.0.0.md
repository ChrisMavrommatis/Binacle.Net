# Release - Binacle.Net v3.0.0

**Status:** Not started. **Created:** 2026-07-16. **Restructured:** 2026-07-26.

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
| A1 | Set the `API_PROJECT_PATH` Actions variable - **external, no plan** | see below |
| A2 | Build the image once, and prove a prerelease tag does not move `latest` | [docker-release-tagging](plans/docker-release-tagging.md) |
| A3 | Health check IP restrictions - four defects and the missing tests | **done 2026-07-27** |
| A4 | Forwarded headers - warn-once diagnostics and the missing tests | **done 2026-07-27** |

- [ ] **A1 - `API_PROJECT_PATH`.** Repo Settings -> Secrets and variables -> Actions -> Variables:
  `src/Binacle.Net/Binacle.Net.csproj` becomes `api/src/Binacle.Net/Binacle.Net.csproj`. The `src/` -> `api/src/`
  move breaks the publish step in `release-docker-image.yml` until this changes. Nothing publishes without it.

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

| # | Item | Plan |
|---|---|---|
| B1 | Work the beta verification list on the deployed image | [beta-verification](plans/beta-verification.md) |
| B2 | Write the `v3.0.x` docs pages, including the two new configuration pages | [docs-v3-pages](plans/docs-v3-pages.md) |
| B3 | Decide how the shared ViPaq protocol page is versioned, then fix it - it still says gzip | [docs-vipaq-protocol-page](plans/docs-vipaq-protocol-page.md) |
| B4 | Generate `swagger/v3.json` and `swagger/v4.json` - the v4 "all algorithms" claim is corrected (2026-07-28) | [docs-swagger-documents](plans/docs-swagger-documents.md) |
| B5 | Decide how the samples pin the docker image | [sample-image-pinning](plans/sample-image-pinning.md) |
| B6 | Run the ServiceModule suite once against Azure Storage - **no plan** | see below |
| B7 | Confirm v4 still ships experimental, then announce all four breaking changes - **no plan** | see below |

**B3 is decided before B2 starts.** Moving the ViPaq protocol page into the version folders means every version
folder needs a copy landed at once, because `vlink` fails the build on a missing target. That changes how big B2
is, so the call is made first rather than discovered inside the docs session.

**B4 covers two documents.** `v3.0.x` needs `v3.json` as well as `v4.json` - every version folder carries its own
swagger set, and v3's document changed in this release because the ViPaq payload did. Both need a running API, so
they are produced here and handed to the docs session, which writes only the pages beside them.

**B5's deadline is set by A2.** All five repo samples pull `latest`. If the prerelease tag turns out to move
`latest`, sample users get v3.0.0 at the beta rather than at the tag, and this moves into Gate A.

- [ ] **B6 - Azure Storage.** CI covers SQLite and Postgres only, so the Azure provider ships on trust even
  though `samples/docker/service-azure` points users at it. It stays in this release; removal is a later idea.
  The cheap cover is one deliberate run before tagging: bring up Azurite with
  `just serve services -d`, then `just test api-service-integration AzureStorage`.

- [ ] **B7a - v4 is still experimental.** `ApiV4Document.IsExperimental` was set `true` on 2026-07-25, so the
  published OpenAPI document carries the warning that v4 may change at any time. Check it is still `true` before
  tagging - shipping v4 as stable would lock contracts that are meant to keep moving. The flip is 3.1.0 work.

- [ ] **B7b - announce all four breaking changes** in the GitHub release body: V2 endpoints removed, ViPaq
  tokens, the flattened packing-logs configuration, and health check `RestrictedIPs`. All four are already
  written into `release-notes-v3.0.0.md`, along with a six-step migration guide - this is the check that they
  made it in. The packing-logs step is the one most easily lost, and leaving it out fails a user's startup with
  no explanation. The two that need the extra explanation are in the section below.

**Docs are a Gate B item, not a Gate A one.** The beta ships before the docs are written - that is deliberate,
and it is why the beta exists. But the docs site is frozen in the meantime: `docs/_data/versions.yml` already
says `current: v3.0.x` and that folder holds only `index.md`, so `/version/latest/` points at an empty version
and the site cannot be deployed for any reason until B2 lands.

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

1. Gate A green.
2. **Publish the beta image and deploy it.**
3. **Work B1 on the running beta**, and B2-B5 in parallel while it is up.
4. B6, B7a.
5. **Release the docs**, then tag `v3.0.0`. `release-docker-image.yml` publishes the final image on
   `release: published`.
6. Paste `release-notes-v3.0.0.md` into the release body, with both breaking changes in it (B7b).
7. Work `post-release-v3.0.0.md`.

## Not in this release

Everything else has a plan of its own and is listed in `post-release-v3.0.0.md` or the plans index. Do not pull
any of it in: CI work, the version stamp, the npm publishing decision, the `Parallel*` processors, migrating the
UI clients off v3, the benchmark ledger, TestsKernel fixtures, and v4 going stable in 3.1.0.
