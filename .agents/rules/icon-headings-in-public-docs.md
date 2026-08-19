---
description: Public markdown uses one emoji icon on each section heading, matching README.md and the docs site.
load: on-trigger
when: writing or editing a heading in a public markdown file
paths:
  - "README.md"
  - "**/README.md"
  - "SECURITY.md"
  - "DEVELOPMENT.md"
  - ".github/dockerhub-overview.md"
  - "samples/**"
  - "docs/**"
---

# Public headings carry an icon

Every section heading in a public markdown file opens with one emoji, then a space, then the title:

```markdown
## 🚀 Quick Start
### 🌐 Access the Interface
```

**This is the house style and it is not decoration.** `README.md` and every page on the docs site are written
this way. A reader lands on a long page, scrolls, and the icons are what let them find the section they came
for without reading the headings. Dropping them makes a page that is harder to scan, not more serious.

## How to apply it

- **One icon per heading, on `##` and `###`.** The `#` title of a file takes none.
- **Reuse the icon that already means that thing.** 🚀 quick start, 🔗 links, 🏷️ tags, ⚙️ configuration,
  🔒 security and verification, 📂 structure, 🛠️ building, 📄 licence, 📝 overview. Grep the existing files
  before inventing one - the same section wearing two different icons across two files is the failure this
  rule exists to prevent.
- **One icon, never two.** And never inside a sentence, a list item, a table cell or a link.
- **A heading still has to work with the icon stripped.** Terminals, `grep` output and some readers drop it.
  The words carry the meaning; the icon only speeds up finding them.

## Where it does not apply

- **Anything under `.agents/`.** Read by us, kept plain, no icons.
- **Code comments.** See `comments-are-for-humans.md`.
- **Strings a program emits** - validation and exception messages, log lines, OpenAPI descriptions, UI
  strings. Those stay plain ASCII and this rule does not reach them: `plain-ascii-for-user-text.md` binds
  there and the two never overlap, because that rule covers text inside source and this one covers markdown
  a person reads.
- **`CHANGELOG.md` entries for shipped versions**, which are a record of what was written at the time.

**Why:** it is the maintainer's documentation style, used consistently across the README and the docs site
since the project started. A page written without it does not read as more professional - it reads as written
by someone else.
