# Design

The settled design *behind* the docs — the **decisions** (why we built it this way) and the **findings**
(the measured evidence). Permanent and maintained like docs, with the same `verified:` / `check:` front matter.

Design is **not** documentation. Docs say *what the code is and how to use it*; design says *why it is shaped
this way and what proved it*. Keep the two apart: a description of current behaviour is a doc; a decision record
or a benchmark result is design. Design also owns the **history** — superseded evidence and reversed decisions —
which can go in a dedicated `<slice>/history.md`. Like everything here, group by slice — `design/vipaq/`, etc.

## Anchors

Decision and option headings carry short labels (`D1`, `O1`) that double as section anchors, so a section can be
cited as `$vipaq#D16`. Give every file a stable `id:` in its front matter so there is something to cite.

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate with `just agents all` after adding,
renaming or re-describing a file.
