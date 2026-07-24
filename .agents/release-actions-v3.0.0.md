# Release Actions — Binacle.Net v3.0.0

Manual and external steps for the v3.0.0 release — things that can't be done from the repo, or that need a human
to verify. Work these until all are checked, then cut the release.

---

## Infrastructure — the release cannot publish without these

- [ ] **Set the `API_PROJECT_PATH` Actions variable.** Repo Settings → Secrets and variables → Actions →
  Variables: `src/Binacle.Net/Binacle.Net.csproj` → `api/src/Binacle.Net/Binacle.Net.csproj`.
  The `src/` → `api/src/` move breaks the `release-docker-image.yml` publish step until this changes.

- [ ] **Run a docker image build.** Never run since the `Binacle.Geometry` extraction. The suites themselves
  are covered by `run-tests.yml` (every C# suite including ServiceModule on SQLite and Postgres, plus the TS
  suites) — but it runs only on `pull_request` / `workflow_dispatch`, and it does **not** build the image.
  Build the image once before tagging, and make sure the PR that lands the release went green.

- [ ] **Run the ServiceModule suite once against Azure Storage.** CI covers SQLite and Postgres only, so the
  Azure provider ships on trust even though `samples/docker/service-azure` points users at it. It stays in this
  release (removal comes later), so the cheap cover is one deliberate run before tagging: bring up Azurite with
  `docker compose -f config/docker-compose.yml up -d`, then `config/tests.api.sh service AzureStorage`.

## Correctness — verify before shipping a frozen contract

- [x] **Confirm fitting results did not change — VERIFIED 2026-07-19.** Differential-tested old (`v2.1.1` image)
  vs new across all three algorithms, zero disagreements. No behaviour change; no release-notes caveat needed.
  Full evidence and the version-2-vs-version-3 background: `$lib/findings#F3`.

- [x] **Old ViPaq tokens fail loudly — VERIFIED 2026-07-19; regression vectors committed 2026-07-20.** Real old
  tokens plus adversarial header-aligned cases all threw `ViPaqFormatException`; zero silent misparses (the
  body-length check is the backstop). Vectors in `vipaq/test-vectors/serialization/decode-invalid.json` (C# + TS
  green); format detail in `vipaq/PROTOCOL.md`. Only the release-body announcement remains — below.

## Code loose ends from the forwarded-headers work

- [x] **`AuthTokenRateLimitingPolicy` now partitions on `Connection.RemoteIpAddress` — DONE 2026-07-24.**
  `ServiceModule/ExtensionMethods/HttpContextExtensions.GetClientIp()` is deleted (it returned the raw
  `X-Forwarded-For` value, falling back to `X-Real-IP`, so the login throttle partitioned on a string the caller
  wrote — varying it reset the limit). Verified: with the feature off, a forged `X-Forwarded-For` and a forged
  `X-Real-IP` both leave the resolved caller unchanged. Suites green (ServiceModule 107, API core 622).

- [ ] **Add the two warn-once diagnostics.** Both wrong states are silent: a mismatched trust list only logs
  `Unknown proxy` at Debug. Neither check trusts a header for anything — each reads one only to decide whether to
  warn. (1) Feature disabled but a request carries a forwarding header → the client IP is the proxy's. (2) Feature
  enabled and a forwarding header present but `X-Original-For` absent → the trust list does not match. Open
  question: whether these sit beside the extension in the API project or in `DiagnosticsModule` next to `/_debug`.

- [ ] **Tests for both pieces.** None exist yet: trusted hop resolves the caller; untrusted hop leaves the socket
  address; an entry beyond `ForwardLimit` is ignored; a vendor header name resolves; startup fails when both trust
  flags are off and `TrustedProxies` is empty. For the health check: CIDR matches, single address matches, an
  IPv4-mapped caller matches, and the `start-end` form fails validation.

## At release time

- [ ] **Announce the ViPaq token break** in the GitHub release body. The rebuilt format rejects every token an
  earlier version produced, and there is no reader for the old wire — nothing else in the repo tells users this.
  The break inside the frozen v3 contract is **accepted (2026-07-14)**; the saved-token stores clear old tokens
  on a schema-version marker, so only the announcement remains.
  Note the older docker images (`v2.1.1` and back) keep producing the old format — they are unaffected and need
  no change, but a user running both old and new images side by side will find their tokens do not cross. That
  is step 4 of the migration guide.

## Docs site — fix before publicising (see also `post-release-v3.0.0.md`)

- [ ] **Write the two new configuration pages.** Neither exists in any earlier version, so there is nothing to
  copy forward:
  - **Forwarded headers** — running behind a proxy or CDN. The trust settings, the container and tunnel cases,
    using `/_debug` to read the proxy's address, the vendor headers, and why `ASPNETCORE_FORWARDEDHEADERS_ENABLED`
    is ignored.
  - **Health checks** — carry the page forward, then document the three breaking `RestrictedIPs` changes: CIDR now
    means a prefix length (which **narrows** existing entries), IPv4-mapped callers now match so the list works in
    a container at all, and the `start-end` range form is removed and now fails startup validation. Also that the
    list needs forwarded headers to match anything behind a proxy.

- [ ] **Fix the shared ViPaq protocol page.** `docs/collections/_common_pages/vipaq-protocol.md` is a
  `_common_pages` page shared by **every** version folder, not versioned — it renders once at
  `/vipaq-protocol/`, unchanged since v2.1.1, so it now describes the old format. It lists **"Gzip Compression"**;
  the new codec is **raw DEFLATE (RFC 1951)**, no gzip/zlib wrapper (`vipaq/PROTOCOL.md` §6). The rest of the
  page still holds for both formats. It cannot simply be edited per-version — there is one copy. Decide:
  - **Move it into the version folders** (`_versions/<version>/vipaq-protocol.md`) so each version describes its
    own format — correct, but it stops being a common page and every `{% link %}` reference becomes a `vlink`.
  - **Keep it shared and make it version-aware** — describe both formats on the one page, saying which images
    produce which. Cheaper, but it must stay honest about two incompatible formats forever.
  Either way, fix it before publishing: today it tells a v3.0.0 user their tokens are gzip, which is wrong.
