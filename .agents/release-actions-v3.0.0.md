# Release Actions — Binacle.Net v3.0.0

Manual and external steps for the v3.0.0 release — things that can't be done from the repo, or that need a human
to verify. Work these until all are checked, then cut the release.

---

## Infrastructure — the release cannot publish without these

- [ ] **Set the `API_PROJECT_PATH` Actions variable.** Repo Settings → Secrets and variables → Actions →
  Variables: `src/Binacle.Net/Binacle.Net.csproj` → `api/src/Binacle.Net/Binacle.Net.csproj`.
  The `src/` → `api/src/` move breaks the `release-docker-image.yml` publish step until this changes.

- [ ] **Run a docker image build.** Never run since the `Binacle.Geometry` extraction — every C# suite
  (including ServiceModule) and the TS suites are green, but the image build was skipped by choice. No CI runs
  tests, so run the image build **and** the full suite once for a genuinely green sweep before tagging.

## Correctness — verify before shipping a frozen contract

- [x] **Confirm fitting results did not change — VERIFIED 2026-07-19.** Differential-tested old (`v2.1.1` image)
  vs new: ~5,400 fit requests across all three algorithms, weighted to the near-full boundary, zero
  disagreements. No behaviour change; no release-notes caveat needed. Original context below.
  This release unifies fitting and packing onto one algorithm.
  - **Packing is safe** — the shared algorithm is the old packing implementation (version 2), moved and
    renamed. Verified by diff: only the namespace and the algorithm-info members changed.
  - **Fitting is the open question.** At v2.1.1 fitting ran its own family at **version 3**
    (`Binacle.Lib/Fitting/Algorithms/*/`); it now runs the packing lineage at **version 2** with early exit.
    That is a different implementation, and no design doc records the decision or any equivalence analysis.
  - Fitting answers a yes/no question with a heuristic, so a different heuristic can plausibly disagree on edge
    cases. This sits inside the **frozen v3 contract**, which is what makes it worth confirming.
  - Resolve it, then either drop the fitting caveat in the release notes' `📈 Algorithms` section, or document
    the behaviour change there.

- [x] **Check whether old ViPaq tokens fail loudly — VERIFIED 2026-07-19.** 250 real old tokens plus the
  adversarial header-aligned cases all threw `ViPaqFormatException`; zero silent misparses. Locking it in as
  regression vectors is planned (`plans/vipaq/old-format-rejection-tests.md`). Original analysis below.
  The release notes tell users old tokens "no longer
  decode." That is certainly true for old **compressed** tokens: the old header packed the compression flag into
  the version field (`Version.CompressedGzip = 1`), so a new decoder reads `Version = 1` and rejects it
  (`vipaq/PROTOCOL.md` §8).
  An old **uncompressed 8-bit** token is the doubtful case — its single header byte is `0x00`, which is a
  *valid* new 2-byte header's first byte (`Version = 0`, `Compressed = 0`, `Layout = 0`, reserved `0`).
  Rejection then depends on the body-length checks ("body ends before the declared item count", "body has bytes
  left over") rather than on a version discriminator. Confirm it is rejected rather than silently misparsed; if
  it can misparse, say so plainly in the release body.

## At release time

- [ ] **Announce the ViPaq token break** in the GitHub release body. The rebuilt format rejects every token an
  earlier version produced, and there is no reader for the old wire — nothing else in the repo tells users this.
  The break inside the frozen v3 contract is **accepted (2026-07-14)**; the saved-token stores clear old tokens
  on a schema-version marker, so only the announcement remains.
  Note the older docker images (`v2.1.1` and back) keep producing the old format — they are unaffected and need
  no change, but a user running both old and new images side by side will find their tokens do not cross. That
  is step 4 of the migration guide.

## Docs site — fix before publicising (see also `post-release-v3.0.0.md`)

- [ ] **Fix the shared ViPaq protocol page.** `docs/collections/_common_pages/vipaq-protocol.md` is a
  `_common_pages` page shared by **every** version folder, not versioned — it renders once at
  `/vipaq-protocol/`, unchanged since v2.1.1, so it now describes the old format. It lists **"Gzip Compression"**;
  the new codec is **raw DEFLATE (RFC 1951)**, no gzip/zlib wrapper (`vipaq/PROTOCOL.md` §6). The rest of the
  page still holds for both formats. It cannot simply be edited per-version — there is one copy. Decide:
  - **Move it into the version folders** (`_versions/<version>/vipaq-protocol.md`) so each version describes its
    own format — correct, but it stops being a common page and every `{% link %}` reference becomes a `vlink`.
  - **Keep it shared and make it version-aware** — describe both formats on the one page, saying which images
    produce which. Cheaper, but it must stay honest about two incompatible formats forever.
  Either way, fix it before publishing: today it tells a v3.0.0 user their tokens are gzip, which is wrong.
