# Release Notes — Unreleased

Ongoing changelog for the next GitHub release. **Lives in `.agents/` root; maintain it (and `pending-actions.md`)
as work lands** — append an entry whenever a change would matter to a release.

Format mirrors the maintainer's GitHub release bodies (semver `vX.Y.Z` tags, emoji section headings) so an entry
can be pasted into a release with minimal editing. Keep bullets terse.

**How the maintainer writes releases** (observed on the Releases page):
- Tag scheme: `vMAJOR.MINOR.PATCH` (e.g. `v2.1.1`).
- Sections, in this order, only the ones that apply:
  `🔎 Overview`, `⚙️ Core Changes`, `🧪 Diagnostics Module`, `🔌 Service Module`, `🎨 UI Module`,
  `📈 Algorithms`, `🏗️ Internal Work`, `📚 Versioned Docs`, `🛠️ Migration Guide`.
- Overview is a short plain-language summary of the headline changes.
- Bullets are concise, past tense, code/paths in backticks.
- Breaking changes get a bold warning banner at the top plus a numbered `🛠️ Migration Guide`.

---

## ⚠️ Warning — review before upgrading

**ViPaq tokens are a breaking change. Tokens produced by earlier versions no longer decode and cannot be
recovered — they must be re-generated from the source packing result.**

**This release also changes the packing-log configuration shape and log location. A deployment that enables
packing logs with the old nested config will fail startup validation until the config is flattened. See the
Migration Guide.** (Remove this banner if no breaking/migration items remain at release time.)

---

## 🔎 Overview

- **Rebuilt the ViPaq token format.** Smaller, simpler, and not backwards compatible — old tokens are rejected.
- Flattened the packing-log configuration (the nested `Packing` block became keys directly under `PackingLogs`)
  and moved the default log location from `data/pack-logs/packing/` to `data/pack-logs/`.
- Removed the dead `Fitting` packing-log config block (fit and pack have shared one log since the Jan 2026
  algorithm unification).
- Refactored the packing-log pipeline: generic log infrastructure moved to the Kernel; NDJSON lines now carry a
  `Timestamp`.

## 🧪 Diagnostics Module

- **Flattened the packing-log config.** `PackingLogs` now takes `Path`, `FileName`, `DateFormat`, `ChannelLimit`
  directly (previously nested under `Fitting` / `Packing`). Default `Path` moved to `data/pack-logs/`.
- **Removed the `Fitting` config block.** Dead since the Jan 2026 algorithm unification (`9b4d06f6`) — fit and
  pack log through one channel. Removed from `PackingLogsConfigurationOptions`, its validator, and
  `Config_Files/DiagnosticsModule/PackingLogs.json`.
- The NDJSON packing-log line now includes a `Timestamp` field (additive; existing consumers unaffected).

## 🏗️ Internal Work

- **Packing-log pipeline refactor.** Generic log infrastructure now lives in the Kernel
  (`LogsProcessor<TRequest,TLog>`, `ILogEntryConvertible`); the concrete packing request/entry types moved into
  the Diagnostics Module. Per-user logging (`UserId`) is deferred to the Service Module.

## 🛠️ Migration Guide

1. **ViPaq tokens must be re-generated.** The token format was rebuilt and is not backwards compatible. A token
   from an earlier version will be rejected by the decoder; there is no migration path and no fallback reader.
   Re-run the packing request to get a new token. Anything holding a stored token — a saved link, a bookmarked
   result — is stale.

Only affects deployments that enable packing logs (`PackingLogs.Enabled: true`) with a customized
`DiagnosticsModule/PackingLogs.json`:

2. **Flatten `PackingLogs.json`.** Move the keys out of the old nested `Packing` block up under `PackingLogs`, and
   delete the `Fitting` block. If left in the old nested shape with `Enabled: true`, startup validation now fails
   (`Path` is required and no longer binds from the `Packing` sub-object).
   ```jsonc
   // before                              // after
   "PackingLogs": {                       "PackingLogs": {
     "Enabled": true,                       "Enabled": true,
     "Fitting":  { ... },        →          "Path": "data/pack-logs/",
     "Packing":  { "Path": "...", ... }     "FileName": "{0}.ndjson",
   }                                        "DateFormat": "yyyyMMdd",
                                            "ChannelLimit": 100
                                          }
   ```
3. **Log location moved** from `data/pack-logs/packing/` to `data/pack-logs/`. Repoint log collection at the new
   path; the old `packing/` (and any orphaned `fitting/`) directories are safe to move or delete.

---

## Release / CI checklist (do before publishing the release)

These live in [`pending-actions.md`](pending-actions.md); surfaced here so they aren't missed at release time.

- [ ] **Update the `API_PROJECT_PATH` Actions variable** (repo Settings → Secrets and variables → Actions →
  Variables) from `src/Binacle.Net/Binacle.Net.csproj` to `api/src/Binacle.Net/Binacle.Net.csproj`.
  The `src/` → `api/src/` move breaks the `release-docker-image.yml` publish step until this is changed.
