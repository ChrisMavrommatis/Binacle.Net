---
description: Manifest of every file under .agents/memory, grouped by area. Regenerate with just agents all.
---

# Agent Memory Index

Every memory in `.agents/memory/`, grouped by area. Durable facts with no home in a doc or plan —
conventions, decisions, gotchas. See [README.md](README.md) for when and how to add one.

## General

| File | Description |
|---|---|
| [algorithm-identifier-is-a-format.md](algorithm-identifier-is-a-format.md) | The FFD_v2 / BFD_v1 string is a parsed data format, not a naming style - never tidy the underscore out of it |
| [migration-no-silent-deletions.md](migration-no-silent-deletions.md) | Migrations land as small reviewable diffs — every removed test needs a visible successor, never a silent delete |
| [name-each-step.md](name-each-step.md) | Give each step a named local — no nested or chained call expressions squeezed into one statement |
| [no-published-sdks.md](no-published-sdks.md) | We ship an OpenAPI document per version, not client SDKs — consumers generate their own; publishing a package needs real demand first. |
| [no-sonar-issue-ignores.md](no-sonar-issue-ignores.md) | Sonar findings are answered in code, never with a sonar.issue.ignore rule in config/sonar-analysis.xml |
| [results-curated.md](results-curated.md) | results/ is a hand-curated vault — harnesses write to gitignored scratch, never straight into results/ |
| [servicemodule-test-infra.md](servicemodule-test-infra.md) | Test-host config goes through an env var the harness reads, never a .runsettings file — the MTP runner ignores VSTest runsettings |
| [sonar-touching-untested-code.md](sonar-touching-untested-code.md) | Fixing an old Sonar smell in an untested file makes the quality gate worse - changed lines become "new code" and count as uncovered |
| [test-leaf-naming.md](test-leaf-naming.md) | How a test leaf is named - <slice>[-<component>][-<language>]-<kind>, kind spelled out, no two leaves a letter apart |
| [tests-arrange-act-assert.md](tests-arrange-act-assert.md) | A test body shows arrange, act and assert as separate lines — never one helper that does all three |
| [v2-dropped.md](v2-dropped.md) | The v2 API does not exist on this branch — only v3 and v4; never add v2 code, docs, or references |
| [v3-frozen.md](v3-frozen.md) | v3 API is frozen — never modify it; all new endpoints and contract work go in v4 only |
| [version-only-when-published.md](version-only-when-published.md) | A component gets its own version number only once it is published independently; until then the docker image's BINACLE_VERSION is the only version. |
| [vipaq-byte-vectors-agent-owned.md](vipaq-byte-vectors-agent-owned.md) | ViPaq byte-exact golden vectors carry a byte-by-byte comment — a wall of hex nobody can check is not a test |
