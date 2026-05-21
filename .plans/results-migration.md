# Results Migration Plan

Status: **deferred — decisions needed before any work starts**

`results/` (renamed from `doc/`) holds benchmark reports and packing efficiency analysis.
Currently raw markdown sitting in the repo. Goal is to surface this content somewhere useful.

---

## Decision 1 — Where does the content live?

- **Option A: docs site** — add a "Performance" or "Benchmarks" section under the Jekyll docs site
- **Option B: web site** — data-driven page on the marketing/web site
- **Option C: stay in `results/`** — keep as raw markdown, just add a better README (least effort)

---

## Decision 2 — What happens to the raw BenchmarkDotNet files?

- Keep raw JSON/MD in `results/raw/` as an archive
- Or discard and only keep rendered output in the site

---

## Tasks (fill in once decisions above are made)

- [ ] Decide Option A / B / C
- [ ] If A or B: move or link benchmark markdown into the site's collections or pages
- [ ] If A or B: add navigation entry in the site's `_data/` header/footer config
- [ ] If A or B: decide what to do with raw result files
- [ ] If C: write a README explaining the folder contents and file format
