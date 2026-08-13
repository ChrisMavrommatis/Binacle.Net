# CLAUDE.md

All guidance lives in `.agents/`, fetched on demand. This is the door and stays minimal on purpose.

## Four rules that always apply

- **Never commit, stage or push.** Leave changes in the working tree. The human commits.
- **Never edit repo-root `docs/` or `web/`.** They publish to the internet and are written in their own
  session. (`.agents/docs/` is a different thing - editing it is fine.)
- **Nothing outside `.agents/` may point a reader into it.** Not a filename, not a `$ref`, not a bare `D16`.
  This file is the only exception. A path a tool operates on is an operand, not a pointer. The full matrix of
  what may reference what is `.agents/rules/who-references-whom.md`.
- **Plain, short language everywhere** - chat, docs, comments. Cut any word that does not change the meaning.

## Fetch the rest when it applies

`.agents/rules/` holds one file per rule. Each declares in its front matter when it applies:

```yaml
load: always | on-trigger
when: writing or editing a code comment
paths: ["**/*.cs", "**/*.ts"]
```

**Before you edit, read the rule that covers what you are about to touch** - a code comment, a user-facing
message, a doc, a plan, the board. `.agents/rules/README.md` is a trigger table: match the row, open that one
file. It is short; read it once at the start of any task that writes anything.

## Where to look

- `.agents/rules/README.md` - every rule, one line each, indexed by trigger.
- `.agents/README.md` - the map: what each layer is for and how they reference each other.
- `.agents/docs/README.md` - "Common Tasks" maps a job to the docs it needs. `docs/_index.md` is the full list.

Open the file before you work. Do not answer from a vague memory of it.
