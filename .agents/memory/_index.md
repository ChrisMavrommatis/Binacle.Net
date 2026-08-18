---
description: Manifest of every file under .agents/memory, grouped by area. Regenerate with just agents all.
---

# Agent Memory Index

Every memory in `.agents/memory/`, grouped by area. Durable facts with no home in a doc or plan —
conventions, decisions, gotchas. See [README.md](README.md) for when and how to add one.

## General

```yaml
- file: algorithm-identifier-is-a-format.md
  description: "The FFD_v2 / BFD_v1 string is a parsed data format, not a naming style - never tidy the underscore out of it"
  when: "renaming an algorithm class, enum member or identifier string"
  paths: ["lib/**", "shared/src/Binacle.Packing/**"]
- file: bulk-rename-traps.md
  description: "Four traps when sweeping a namespace or type rename across this repo — spaces in Algorithms/ folder names, unstaged files and git mv, global usings that collide, and fully-qualified names"
  when: "sweeping a namespace or type rename across the repo"
- file: migration-no-silent-deletions.md
  description: "Migrations land as small reviewable diffs — every removed test needs a visible successor, never a silent delete"
  when: "deleting or replacing a test during a migration"
  paths: ["**/test/**"]
- file: name-each-step.md
  description: "Give each step a named local — no nested or chained call expressions squeezed into one statement"
  when: "writing C# with nested or chained call expressions"
  paths: ["**/*.cs"]
- file: no-published-sdks.md
  description: "We ship an OpenAPI document per version, not client SDKs — consumers generate their own; publishing a package needs real demand first."
  when: "anyone asks for a client SDK or package"
  paths: ["api/**"]
- file: no-sonar-issue-ignores.md
  description: "Sonar findings are answered in code, never with a sonar.issue.ignore rule in tooling/sonar-analysis.xml"
  when: "answering a Sonar finding"
  paths: ["tooling/sonar-analysis.xml", "Directory.Build.props"]
- file: results-curated.md
  description: "results/ is a hand-curated vault — harnesses write to gitignored scratch, never straight into results/"
  when: "writing anything into results/"
  paths: ["results/**"]
- file: servicemodule-test-infra.md
  description: "Test-host config goes through an env var the harness reads, never a .runsettings file — the MTP runner ignores VSTest runsettings"
  when: "changing ServiceModule test-host configuration"
  paths: ["api/test/Binacle.Net.ServiceModule.IntegrationTests/**"]
- file: sonar-no-quality-profile.md
  description: "Sonar rules cannot be switched off on this project - custom quality profiles start at the Team plan and this one is on Free, so \"Sonar way\" is read-only"
  when: "someone proposes turning a Sonar rule off"
  paths: ["tooling/sonar-analysis.xml"]
- file: sonar-scope-exclusions.md
  description: "sonar.exclusions and friends are scope exclusions, not issue ignores - they are allowed and already in use"
  when: "reading or editing the exclusion lists in tooling/sonar-analysis.xml"
  paths: ["tooling/sonar-analysis.xml", "Directory.Build.props"]
- file: sonar-touching-untested-code.md
  description: "Fixing an old Sonar smell in an untested file makes the quality gate worse - changed lines become \"new code\" and count as uncovered"
  when: "fixing a Sonar smell in a file with no test coverage"
- file: test-leaf-naming.md
  description: "How a test leaf is named - <slice>[-<component>][-<language>]-<kind>, kind spelled out, no two leaves a letter apart"
  when: "adding or renaming a test leaf"
  paths: ["tooling/tests.just", "**/test/**"]
- file: tests-arrange-act-assert.md
  description: "A test body shows arrange, act and assert as separate lines — never one helper that does all three"
  when: "writing a test body"
  paths: ["**/test/**"]
- file: v2-dropped.md
  description: "The v2 API does not exist on this branch — only v3 and v4; never add v2 code, docs, or references"
  when: "touching API versioning, routes or docs"
  paths: ["api/**"]
- file: v3-frozen.md
  description: "v3 API is frozen — never modify it; all new endpoints and contract work go in v4 only"
  when: "changing anything under api/src/Binacle.Net/v3"
  paths: ["api/src/Binacle.Net/v3/**"]
- file: version-only-when-published.md
  description: "A component gets its own version number only once it is published independently; until then the docker image's BINACLE_VERSION is the only version."
  when: "adding a version number or a Directory.Build.props to a component"
  paths: ["**/Directory.Build.props", "**/*.csproj"]
- file: vipaq-byte-vectors-agent-owned.md
  description: "ViPaq byte-exact golden vectors carry a byte-by-byte comment — a wall of hex nobody can check is not a test"
  when: "editing ViPaq byte-exact golden vectors"
  paths: ["vipaq/test-vectors/**"]
```
