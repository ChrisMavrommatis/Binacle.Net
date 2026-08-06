# Release Notes — Binacle.Net v3.0.0

The GitHub release body for v3.0.0, ready to paste. Everything below the line is the body; the notes above it
are for whoever cuts the release.

Style is taken from the maintainer's published releases (https://github.com/ChrisMavrommatis/Binacle.Net/releases):

- Opens with one line: `Binacle.Net vX.Y.Z is a major update from vA.B.C.`
- A breaking release uses a GitHub alert — `> [!Warning]` — then `---`.
- Sections, in this order, **only the ones that apply**: `🔎 Overview`, `⚙️ Core Changes`,
  `🧪 Diagnostics Module`, `🔌 Service Module`, `🎨 UI Module`, `📈 Algorithms`, `🏗️ Internal Work`,
  `📚 Versioned Docs`, `🛠️ Migration Guide`.
- Bullets are short, past tense, **bold** on the key term, code and paths in backticks. Lines end with two
  spaces (a markdown line break). **No tables** — the published releases use none.
- Migration guide: `To upgrade to **vX.Y.Z**, follow these steps:` then numbered `**Bold title**` steps with
  indented `-` sub-bullets.
- Closes with `---` and a `**Full Changelog**:` compare link.
- A minor/patch release drops all of this and is just `## Overview` with a few plain bullets.

**Scope:** `v2.1.1` (2026-01-13, the last shipped image) → now; 189 commits. `v3.0.0-beta.1` sits at 186 of
them. The 3 commits after the beta tag are `npm audit fix` / `bundle audit-fix` on the root, `docs/` and `web/`
lockfiles, plus `.agents/` notes and `.nvmrc`. **None reach the image** — the Dockerfile copies only
`build/binacle-net`, and every advisory they closed was a devDependency (`npm audit --omit=dev` on the pre-fix
lockfile: 0). Checked 2026-08-06; the body below needs no change for them.

**Before pasting:** confirm the version number / compare link. Fitting was verified unchanged (2026-07-19), so
the `📈 Algorithms` section needs no caveat. The manual steps are the release actions.

---

Binacle.Net v3.0.0 is a major update from v2.1.1.

> [!Warning]
> **v3.0.0 introduces breaking changes. Existing integrations must be reviewed and updated. V2 endpoints are removed, ViPaq tokens from earlier versions no longer decode, and health check IP restrictions are matched differently.**

---

## 🔎 Overview
- **V2 endpoints** were removed.  
- **V4 endpoints** were introduced as experimental.  
- **V3 endpoints** remain stable and unchanged, and are the recommended version.  
- **ViPaq** was rebuilt with a smaller, simpler format. Tokens from earlier versions no longer decode.  
- **Algorithms** were unified — fitting and packing now share one implementation.  
- **Packing Logs** configuration was flattened, with breaking changes for existing integrations.  
- **Forwarded headers** are now supported, so the real caller is resolved when running behind a proxy or CDN.  
- **Health check IP restrictions** are matched differently, with breaking changes for existing allow-lists.  
- The project was **restructured**, separating the API, library, and ViPaq into their own roots.  
- **Versioned documentation** now covers every minor line, so older images keep their docs.  

## ⚙️ Core Changes
- Removal of all V2 endpoints.  
- Added **16 experimental V4 endpoints**, covering everything V3 does.  
- V4 splits a request into three shapes. **One bin, one answer** — `fit/bin`, `pack/bin`, and their `{preset}/{bin}` variants.  
- **Many bins, one answer** — `pack/smallest-bin`, `pack/smallest-bin/{preset}`, `fit/smallest-bin`, and `fit/smallest-bin/{preset}` return the smallest bin that works; `pack/best-bin` and `pack/best-bin/{preset}` return the bin the items fill the most.  
- **Many bins, every answer** — `fit/compare-bins`, `pack/compare-bins`, and their `{preset}` variants return one result per bin, in the order the bins were sent.  
- Presets can be **listed** with `presets` or **fetched one at a time** with `presets/{preset}`.  
- V4 is **experimental and can change at any time**. V3 remains stable and is the recommended version.  
- V3 endpoints are unchanged and remain stable, apart from the ViPaq payload.  
- Added **forwarded headers** support, configured in `Config_Files/ForwardedHeaders.json`. **Disabled by default.**  
- When enabled, the caller's address and scheme are resolved from `X-Forwarded-For` and `X-Forwarded-Proto` before anything reads them, so rate limiting and health check IP restrictions see the real caller rather than the proxy.  
- Trust is explicit — a proxy on loopback or a private network is trusted by default, anything else must be named. The app **refuses to start** if nothing is trusted, because that would make every caller's header believable.  
- A different header can be read instead, for CDNs that send one — `CF-Connecting-IP`, `X-Real-IP`, `X-Azure-ClientIP`.  
- `ASPNETCORE_FORWARDEDHEADERS_ENABLED` is **ignored**. It switches the underlying middleware on with no proxy verification, which lets any caller choose their own address.  
- `TrustedProxies` entries are **read exactly as written**, the same rule as health check `RestrictedIPs`. `010.10.10.10` used to be read as octal and trust `8.10.10.10`, and `172.17.1` used to mean `172.17.0.1`; both now fail startup validation rather than trusting a host you did not name.  
- Added a **`/_debug` endpoint**, off by default, enabled with `DEBUG_ENDPOINT=True`. It echoes the caller's own request — connection address and headers — for working out what a proxy is sending.  
- A **startup warning** when a forwarding header arrives and does not take effect, either because the feature is off or because the trust list does not name your proxy. Logged once. Without it both states are silent and the app quietly reads the proxy as the caller.  
- The `Dockerfile` and existing environment variables are unchanged.  

## 🧪 Diagnostics Module
- Packing Logs configuration was **flattened** — `Path`, `FileName`, `DateFormat`, and `ChannelLimit` now sit directly under `PackingLogs`.  
- Removed the **fitting** configuration block, now that fitting and packing share one log.  
- Implementations depending on the old nested shape must be updated, or startup validation will fail.  
- The default log path changed from `data/pack-logs/packing/` to `data/pack-logs/`.  
- Packing log entries now include a `Timestamp` field.  
- Added **`RetentionDays`** to `PackingLogs`. When set, packing log files older than that many days are deleted once a day, and each deletion is logged. **Off by default** (`null`) — files are kept until you remove them yourself. Only files matching the configured `FileName` pattern in the configured `Path` are touched, and only at the top level.  
- Health check **`RestrictedIPs` now uses CIDR notation correctly**. The value after `/` was previously read as an address mask, so `192.168.1.0/24` covered nearly the whole IPv4 range instead of 256 addresses. Existing CIDR entries are now **much narrower** than they were.  
- Health check `RestrictedIPs` now matches **IPv4 callers in containers**. Addresses arriving in IPv4-mapped IPv6 form are unmapped before comparison, which they previously were not — no IPv4 entry could match.  
- Removed the **`start-end` range form** from `RestrictedIPs`. Entries such as `192.168.1.0-192.168.1.255` now fail startup validation. Use CIDR instead.  
- `RestrictedIPs` entries are now **read exactly as written**. An IPv4 address must be four plain decimal parts with no leading zeros, and an IPv6 address must be in its short, lowercase form. `010.10.10.10` used to be read as octal and admit `8.10.10.10`; `10.1` used to mean `10.0.0.1`; `167772161` meant the same. All of these now fail startup validation instead of quietly admitting a host you did not name. `192.168.1.1/24` still means the whole `192.168.1.0/24` — that is what CIDR notation means — but the startup log now says so.  

## 🎨 UI Module
- The Protocol Decoder reads the **new ViPaq format only**. Tokens from earlier versions are rejected.  

## 📈 Algorithms
- **Fitting and packing now share one algorithm.** Fitting stops early on the first item that does not fit.  
- Packing results are unchanged — the shared algorithm is the previous packing implementation.  
- The separate fitting algorithm family was retired.  

## 🏗️ Internal Work
- Restructured the repository — the API, library, ViPaq, and shared test data now live in their own roots.  
- Extracted **Binacle.Geometry** into its own library.  
- Reworked the packing log pipeline, moving the generic parts into the Kernel.  
- Added benchmark suites for algorithms, bin processing, result selection, and ViPaq.  
- Added cross-language ViPaq interop tests between C# and TypeScript.  
- Patched two **high-severity advisories** in transitive dependencies — `Microsoft.OpenApi` and the bundled **SQLite** native library.  

## 📚 Versioned Docs
- Documentation is now versioned per minor line — `v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x` — so any image can be matched to its docs.  
- Backfilled the `v2.0.x` and `v2.1.x` documentation, which was previously missing.  
- The `latest` documentation now redirects to the current version, so existing links keep working.  

## 🛠️ Migration Guide
To upgrade to **v3.0.0**, follow these steps:

1. **Remove all V2 usage**  
   - Any calls to V2 endpoints must be removed or migrated.
   - Replace `/api/v2/presets`, `/api/v2/fit/by-custom`, `/api/v2/fit/by-preset/{preset}`, `/api/v2/pack/by-custom`, and `/api/v2/pack/by-preset/{preset}` with their V3 equivalents.

2. **Switch to V3 endpoints**  
   - V3 requires an algorithm to be selected, where V2 used a fixed one, and drops V2's other parameters.  
   - See the [v2.1.x documentation](https://docs.binacle.net/version/v2.1.x/) for the old contract.

3. **Regenerate all ViPaq tokens**  
   - The format was rebuilt and is not backwards compatible.  
   - Tokens from earlier versions no longer decode, and there is no fallback reader.  
   - Re-run the packing request to get a new token. Any stored token — a saved link or a bookmarked result — is stale.  
   - This applies to V3 responses as well, even though V3 is otherwise unchanged.

4. **Do not mix versions**  
   - Images before v3.0.0 produce the old ViPaq format; v3.0.0 onward produces and reads only the new one.  
   - An encoder and a decoder on different sides of this release will not interoperate.

5. **Update Packing Logs configuration**  
   - Move `Path`, `FileName`, `DateFormat`, and `ChannelLimit` out of the nested `Packing` block, directly under `PackingLogs`, and delete the `Fitting` block.  
   - Left in the old shape with `Enabled: true`, startup validation now fails.  
   - Repoint log collection from `data/pack-logs/packing/` to `data/pack-logs/`. The old `packing/` and `fitting/` directories are safe to remove.

6. **Review health check `RestrictedIPs`**  
   - Replace any `start-end` entries with CIDR — `192.168.1.0-192.168.1.255` becomes `192.168.1.0/24`. Left as they are, startup validation now fails.  
   - Re-check any CIDR entry. It now covers what it says, which is far less than before — confirm the addresses you expect are still inside it, or you will lock yourself out.  
   - A range that does not line up with a CIDR boundary must be split into several entries, or widened to the enclosing subnet.  
   - Drop any leading zeros — `010.10.10.10` becomes `10.10.10.10`, and note it used to admit `8.10.10.10`, so check that host was not the one you meant. Write IPv6 entries in the short lowercase form: `2001:0DB8::1` becomes `2001:db8::1`.  
   - If Binacle.Net runs behind a proxy, load balancer or CDN, enable **forwarded headers** as well. Without it the list is compared against the proxy's address and can never match your monitoring system.

---

**Full Changelog**: https://github.com/ChrisMavrommatis/Binacle.Net/compare/v2.1.1...v3.0.0
