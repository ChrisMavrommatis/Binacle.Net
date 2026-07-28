# Design

The settled design *behind* the docs — the **decisions** (why we built it this way) and the **findings**
(the measured evidence). Permanent and maintained like docs, with the same `verified:` / `check:` front matter.
Design cites docs; **docs never cite design** — design can change under them.

Design is **not** documentation. Docs say *what the code is and how to use it*; design says *why it is shaped
this way and what proved it*. Keep the two apart: a description of current behaviour is a doc; a decision record
or a benchmark result is design. Like everything here, group by slice — `design/vipaq/`, etc.

## Referencing

Give each file a stable `id:` in its front matter and cite it with a `$` reference, never a path — see the root
[README.md](../README.md) ("How to reference — the `$` symbol scheme"). Decision headings carry short labels
(`D1`, `O1`) that double as section anchors, so a citation reads `$vipaq#D16`.

Design never references a plan, idea, or memory — it is permanent and may only cite docs, other design, and code.

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate it after adding or renaming a file with
`just agents all` (it also rebuilds the docs, plans, ideas, and memory indexes).
