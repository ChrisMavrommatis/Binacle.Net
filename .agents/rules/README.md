# Rules

Every standing rule, one file per rule. **This page is the whole set.** Match what you are about to do against
the trigger, open that one file, skip the rest.

A rule is a standing instruction that does not change per task. How something *works* is a doc; what is not
done yet is a plan.

## Read before you start - always

Getting one of these wrong is expensive or cannot be undone. `CLAUDE.md` states them in one line each; the
files hold the carve-outs.

| Rule | In one line |
|---|---|
| [never-commit](never-commit.md) | Never commit, stage or push. Leave changes in the working tree. |
| [never-edit-published-sites](never-edit-published-sites.md) | Never edit anything under `sites/`. One carve-out, for security fixes to sample files. |
| [who-references-whom](who-references-whom.md) | The one reference matrix. Every layer, the outward boundary, the three exceptions. |
| [talking-to-the-maintainer](talking-to-the-maintainer.md) | In chat: plain English, short. Less is more. |

## Read when the trigger fires

| About to... | Read |
|---|---|
| write or edit a **code comment** | [comments-are-for-humans](comments-are-for-humans.md) |
| write **text a user will see** - exception, log line, OpenAPI description, UI string | [plain-ascii-for-user-text](plain-ascii-for-user-text.md) |
| write **any doc, comment or explanation** | [plain-language](plain-language.md) |
| write a **heading in a public markdown file** - README, samples, the Docker Hub page, the docs site | [icon-headings-in-public-docs](icon-headings-in-public-docs.md) |
| add a **folder**, or write or edit a **`README.md`** outside `.agents/` | [every-folder-has-a-readme](every-folder-has-a-readme.md) |
| write or edit a **plan or an idea** | [plans-do-not-schedule-themselves](plans-do-not-schedule-themselves.md) |
| write anything a human reads **outside `.agents/`** | [ref-codes-stay-in-the-agent-docs](ref-codes-stay-in-the-agent-docs.md) |
| add a **fact** to any file under `.agents/` | [one-fact-one-place](one-fact-one-place.md) |
| **edit a doc or design record** | [keep-verified-current](keep-verified-current.md) |
| add **any link or `$` reference**, anywhere | [who-references-whom](who-references-whom.md) |
| touch **`board.md`** or a **release file**, or pick what to work on next | [the-board-and-the-release-set](the-board-and-the-release-set.md) |

## The front matter is the fetch key

Every rule carries the same fields, so the decision to open it can be made without opening it:

```yaml
description: what the rule says, in one line
load: always | on-trigger
when: the plain-language trigger
paths: [optional globs where it bites]
```

`_index.md` lists all of them as yaml with these fields. To find the rules for a file you are about to edit,
match its path against `paths:`.

Every other layer under `.agents/` carries the same keys for the same reason - `description`, and `when` or
`paths` where the file only matters for certain work. The `_index.md` of each layer is the searchable manifest.

## Adding one

One rule, one file, named after the rule. Same front matter. State the rule, then a short **Why**. Add a row
above. If it applies to only one topic it is probably not a rule - put it in the doc or memory that owns it.

**No duplication.** If two rules would restate the same thing, they are one rule: merge them and keep the
table. That is why the whole reference matrix is a single file.
