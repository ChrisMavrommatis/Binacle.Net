# Session 3 — Write the v2 spec

**Goal:** write the normative v2 wire spec in `vipaq/PROTOCOL.md` **before** any implementation. Greenfield /
experimental → no back-compat burden, but version-tag everything. Scope v2.0 to **8/16 + reserved codes** (varint
deferred to session 7). Spec-first is mandatory because C# and TS must conform to the same contract.

**Prereq reading:** [findings.md](findings.md), [decisions.md](decisions.md) (D7 settles compression; D2 settles the
cap), [architecture-v2.md](architecture-v2.md) (header-driven decode is a spec requirement).

## Do NOT (this session) — on top of the README standing fence
- Write **only** `PROTOCOL.md`. No C#/TS implementation, no code changes.
- Do not spec 24-bit, coords-ride-bin, or a varint *body* — ruled out or deferred; reserve the code only.
- Do not re-open the fixed-threshold compression question — D7 settled try-both-keep-smaller.

## The v2.0 design to spec
- **Purpose statement:** storage-first; **base64 is the token** — formalize the text form in-spec (alphabet,
  padding, no line wrapping). It's the real artifact; stop treating it as "outside the spec."
- **Header byte** (1 byte). Keep it simple; remember header *bits* don't affect base64 size (findings). Proposed:
  ```
  bit 7   : Compressed        0 = raw, 1 = compressed
  bits 6-5: Version           0 = v2; 1-3 reserved (evolution lever)
  bits 4-3: Bin-dims width    codes below
  bits 2-1: Item-dims width
  bit 0   : reserved-zero     (write 0, reject non-zero; future layout/flag hook)
  ```
  Width codes (2 bits): **0 = 8-bit, 1 = 16-bit, 2 = reserved (varint, session 7), 3 = reserved.**
  DECIDE whether item-**coordinates** get their own width field or ride bin-dims width. Three inputs, all pointing
  the same way: coords-ride-bin was measured to **regress** (findings, ruled out); its only benefit (a freed bit)
  doesn't shrink base64; and real Bischoff data packs to **`16/8/16`** — bin and coords need 16-bit while item dims
  stay 8-bit (findings, real data), so the sections genuinely disagree. **Keep an independent coordinate width.** That
  makes 3 width fields + version(2) + compressed(1) = 9 bits, so either compression folds into version, or the
  header is 2 bytes. Resolve this explicitly in the spec.
- **Value range:** unsigned; **v2.0 caps at 65,535** (8/16 only). Encoding a value > 65,535 **throws** (documented
  limit). Varint (session 7) lifts this via width-code 2.
- **Body layout:** columnar (all lengths, then widths, …, then X, Y, Z) — small compression benefit, and it's the
  layout the decode-span path likes. (DECIDE: columnar now, or row now + columnar as a versioned option. Columnar
  now is simplest since it's greenfield.)
- **Compression:** **try-both-keep-smaller** — settled, see [decisions.md](decisions.md) D7. Compress, keep whichever
  is shorter, never inflate. Do **not** re-open the fixed-threshold option: Session 1 measured that a threshold is
  wrong in both directions (it inflates random data and misses small compressible data). Default codec
  **gzip-Optimal or brotli-Optimal** (O2 still open — Session 4 picks). The `Compressed` bit records the outcome.
  q11 is **not** in v2.0 (a session-7 opt-in archival mode; it would be a distinct codec value/version).
- **Evolution mechanism (change-first):** the **`Version` field is the one forward-door**; a future version may
  mean "extended header follows." Reserve width-code 2/3 and the version codes in prose. **Reserve no structural
  bits speculatively** — future formats ride new version numbers.
- **Item count:** keep the fixed 2-byte count (adaptive count is marginal — findings/base64 rule).

## Steps
1. Draft the `PROTOCOL.md` v2 section: header, width codes, columnar body, count, compression, base64 text form,
   value range + throw rule, reserved codes, decisions log entry (§10).
2. Keep the wire **little-endian**, integers unsigned.
3. Write the throw conditions (dims ≤ 0, value > 65,535, etc.) into the error table (§ like today's §7/§8).

## Watch-outs
- Don't spec 24-bit / coords-ride-bin (ruled out — findings).
- Don't spec varint yet — reserve its code so it slots in without a new version (session 7).
- Keep the spec language-agnostic (it's the contract for C# **and** TS).

## Deviation note
The coord-width call is settled (independent — see above). What is still genuinely open is **how the 9th bit is
paid for**: a 2-byte header, or fold `Compressed` into `Version`. Decide it here with the base64 rule in mind —
header bits are ~free for size, so favour clarity over cramming.

## References
[findings.md](findings.md) · [decisions.md](decisions.md) (D7 compression, D2 cap) ·
[architecture-v2.md](architecture-v2.md) (header-driven decode) · `vipaq/PROTOCOL.md` (change here first) ·
`.agents/docs/vipaq/cross-language-testing.md` (the spec is what both implementations conform to).
