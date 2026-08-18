---
description: Manifest of every file under .agents/rules, grouped by area. Regenerate with just agents all.
---

# Agent Rules Index

Every standing rule, one file per rule. `load: always` must be known before you start; the rest
are fetched when their `when:` fires. See [README.md](README.md) for the trigger table.

## General

```yaml
- file: comments-are-for-humans.md
  description: "Comments explain the trap in front of them, for the person editing that line. Thin. Anything an agent needs goes in .agents/."
  when: "writing or editing a code comment"
  load: on-trigger
  paths: ["**/*.cs", "**/*.ts", "**/*.js", "**/*.csproj", "**/*.props", "**/*.just", "**/*.yml"]
- file: keep-verified-current.md
  description: "When you edit a doc, update its verified date and check its also_update list. One carve-out - a prose-only edit that checks nothing against code does not bump the date."
  when: "editing any doc or design record"
  load: on-trigger
  paths: [".agents/docs/**", ".agents/design/**"]
- file: never-commit.md
  description: "The human commits, stages and pushes. An agent never does."
  when: "before any git command, and before saying a task is finished"
  load: always
- file: never-edit-published-sites.md
  description: "Repo-root docs/ and web/ publish to the internet and are written in their own session. One carve-out for security fixes to sample files."
  when: "before editing anything under repo-root docs/ or web/"
  load: always
  paths: ["docs/**", "web/**"]
- file: one-fact-one-place.md
  description: "Put a fact in exactly one place and cross-link. A fact written twice will disagree."
  when: "adding a fact to any file under .agents/"
  load: on-trigger
  paths: [".agents/**"]
- file: plain-ascii-for-user-text.md
  description: "Text that reaches a user stays plain ASCII - no em dashes, curly quotes, ellipsis characters or arrows."
  when: "writing text a user will see - validation and exception messages, log lines, OpenAPI descriptions, UI strings"
  load: on-trigger
  paths: ["api/src/**", "packages/**", "**/Config_Files/**"]
- file: plain-language.md
  description: "Plain, simple language in docs, comments and explanations. Cut any word that does not change the meaning."
  when: "writing any doc, comment or explanation"
  load: on-trigger
- file: plans-do-not-schedule-themselves.md
  description: "A plan or idea says what the work is, never when it happens. Scheduling lives on the board and in the release set."
  when: "writing or editing a plan or an idea"
  load: on-trigger
- file: ref-codes-stay-in-the-agent-docs.md
  description: "A label like D16 is an agent cross-reference. Never put a bare code in anything a human reads."
  when: "writing anything a human reads outside .agents/ - a comment, release notes, PR text, a chat reply"
  load: on-trigger
- file: talking-to-the-maintainer.md
  description: "In chat, use plain English and simple language. Less is more."
  when: "every reply you write to the maintainer"
  load: always
- file: the-board-and-the-release-set.md
  description: "The board and the release set divide all work between them. An agent maintains both, but never decides placement, readiness or priority."
  when: "touching board.md or a release file, or deciding what to work on next"
  load: on-trigger
  paths: [".agents/board.md", ".agents/release-v*.md", ".agents/post-release-v*.md"]
- file: who-references-whom.md
  description: "The one reference matrix - what every file type may point at, what it may never point at, and the three exceptions."
  when: "adding any link, $ reference or pointer, anywhere in the repo"
  load: always
```
