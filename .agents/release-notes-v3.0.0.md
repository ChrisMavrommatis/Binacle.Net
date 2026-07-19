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

**Scope:** `v2.1.1` (2026-01-13, the last shipped image) → now; 149 commits. `main` is 2 commits past `v2.1.1`;
both are included.

**Before pasting:** confirm the version number / compare link. Fitting was verified unchanged (2026-07-19), so
the `📈 Algorithms` section needs no caveat. The manual steps are the release actions.

---

Binacle.Net v3.0.0 is a major update from v2.1.1.

> [!Warning]
> **v3.0.0 introduces breaking changes. Existing integrations must be reviewed and updated. V2 endpoints are removed, and ViPaq tokens from earlier versions no longer decode.**

---

## 🔎 Overview
- **V2 endpoints** were removed.  
- **V4 endpoints** were introduced as experimental.  
- **V3 endpoints** remain stable and unchanged, and are the recommended version.  
- **ViPaq** was rebuilt with a smaller, simpler format. Tokens from earlier versions no longer decode.  
- **Algorithms** were unified — fitting and packing now share one implementation.  
- **Packing Logs** configuration was flattened, with breaking changes for existing integrations.  
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
- Configuration files, environment variables, and the `Dockerfile` are unchanged.  

## 🧪 Diagnostics Module
- Packing Logs configuration was **flattened** — `Path`, `FileName`, `DateFormat`, and `ChannelLimit` now sit directly under `PackingLogs`.  
- Removed the **fitting** configuration block, now that fitting and packing share one log.  
- Implementations depending on the old nested shape must be updated, or startup validation will fail.  
- The default log path changed from `data/pack-logs/packing/` to `data/pack-logs/`.  
- Packing log entries now include a `Timestamp` field.  

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

---

**Full Changelog**: https://github.com/ChrisMavrommatis/Binacle.Net/compare/v2.1.1...v3.0.0
