# Pending Actions

Things that can't be done from the repo — require external systems or manual steps. **Lives in `.agents/` root
alongside [`release-notes.md`](release-notes.md); maintain both as work lands** (release/CI actions here, changelog
entries there).

---

## GitHub

- [ ] Update the `API_PROJECT_PATH` Actions variable (repo Settings → Secrets and variables → Actions → Variables)
  from `src/Binacle.Net/Binacle.Net.csproj`
  to   `api/src/Binacle.Net/Binacle.Net.csproj`
  Affects: `release-docker-image.yml` workflow (publish step)
  Also tracked in [`release-notes.md`](release-notes.md) so it isn't missed at release time.

---

## Announcements

- [ ] **Announce the ViPaq token break.** The rebuilt format rejects every token an earlier version produced, and
  there is no reader for the old wire. Nothing in the repo says the old format existed — the break is announced
  in [`release-notes.md`](release-notes.md) only. Call it out in the GitHub release body.
  The hard payload break inside the frozen v3 contract is **maintainer-accepted (2026-07-14)**; the saved-token
  stores now clear old tokens on a schema-version marker, so only this announcement remains.
  Note the older docker images (`v2.1.1` and back) keep producing the old format — they are unaffected and need
  no change, but a user running both old and new images side by side will find their tokens do not cross. That
  is called out as step 4 of the migration guide.

---

## Docs site

- [ ] **The ViPaq protocol page describes the old format, for every version of the site.**
  `docs/collections/_common_pages/vipaq-protocol.md` is a **`_common_pages` page — it is shared by every
  version folder, not versioned.** It renders once at `/vipaq-protocol/`. It is unchanged since v2.1.1, so it
  now describes the old format.

  Concretely wrong for v3.0.x: it lists **"Gzip Compression"**. The new codec is **raw DEFLATE (RFC 1951)** with
  no gzip or zlib wrapper — see `vipaq/PROTOCOL.md` §6. The rest of the page (purpose, `[Header][Count][Bin]
  [Items]` shape, Base64, variable-length encoding) still holds for both formats, and the page already says
  *"Consult the documentation for your Binacle.Net version, for availability and support."*

  **The disclaimer cannot simply be added to the old version folders** — there is only one copy of this page.
  Decide between:
  - **Move it into the version folders** (`_versions/<version>/vipaq-protocol.md`), so each version describes its
    own format. Correct, and matches how the rest of the site works — but it stops being a common page, and
    every `{% link _common_pages/vipaq-protocol.md %}` reference has to become a `vlink`.
  - **Keep it shared and make it version-aware** — describe both formats on the one page, saying which images
    produce which. Cheaper, but the page has to stay honest about two incompatible formats forever.

  Either way it must be fixed **before publishing**: today the page tells a v3.0.0 user their tokens are gzip,
  which is wrong, and telling a v2.1.x user they are DEFLATE would be equally wrong.

---

## Verification gaps

- [ ] **Run a docker image build.** The `Binacle.Geometry` extraction was verified against every C# suite
  (including ServiceModule) and the TS suites, all green — but the docker image build was skipped by choice.
  Run it once for a fully green sweep.

- [ ] **Confirm fitting results did not change.** This release unifies fitting and packing onto one algorithm.
  - **Packing is safe** — the shared algorithm is the old packing implementation (version 2) moved and renamed.
    Verified by diff: only the namespace and the algorithm-info members changed.
  - **Fitting is the open question.** At v2.1.1 fitting ran its own family at **version 3**
    (`Binacle.Lib/Fitting/Algorithms/*/`); it now runs the packing lineage at **version 2** with early exit.
    That is a different implementation, and **no design doc records the decision or any equivalence analysis**.
  - Fitting answers a yes/no question with a heuristic, so a different heuristic can plausibly disagree on edge
    cases. This sits inside the **frozen v3 contract**, which is what makes it worth confirming.
  - Resolve it, then either drop the caveat in `release-notes.md` (`📈 Algorithms`) or document the change there.

- [ ] **Check whether old ViPaq tokens fail loudly.** `release-notes.md` tells users old tokens "no longer
  decode". That is certainly true for old **compressed** tokens: the old header packed the compression flag into
  the version field (`Version.CompressedGzip = 1`), so a new decoder reads `Version = 1` and rejects it
  (`PROTOCOL.md` §8).
  An old **uncompressed 8-bit** token is the doubtful case — its single header byte is `0x00`, which is a *valid*
  new 2-byte header's first byte (`Version = 0`, `Compressed = 0`, `Layout = 0`, reserved `0`). Rejection then
  depends on the body-length checks ("body ends before the declared item count", "body has bytes left over")
  rather than on a version discriminator. Confirm it is rejected rather than silently misparsed; if it can
  misparse, say so plainly in the release body.
