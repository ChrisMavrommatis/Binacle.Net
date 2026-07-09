# Session 7 — Decide additional features

**Goal:** with the format shipped and measured, decide which extras to add — each behind a reserved code or a new
version, each justified by its measured gain. Nothing here is committed; it's the menu.

**Prereq reading:** [findings.md](findings.md), `vipaq/PROTOCOL.md` (reserved codes: §2.3, §4).

## Do NOT (this session) — on top of the README standing fence
- Nothing here is committed. Do not build any candidate without a measured gain in base64 + encode/decode ms.
- Each feature rides a reserved code or a new `Version`, through the full spec → C# → TS → vectors pipeline. No shortcuts.

## Candidates, ranked by measured value

### 1. Varint (width-code 2) — the deferred size lever
- **What:** flip reserved width-code `2` to LEB128 varint, per section, encoder picks smallest of {8, 16, varint}.
  Also lifts the 65,535 cap (varint has no ceiling in our range).
- **Measured gain (raw):** −6.6% on mixed 16-bit; **−25 to −32% on 100k–1M**; −3 to −13% on 2–16M; **+9% (worse)
  above 268M** (irrelevant to our range). After a fast matched codec the gain shrinks to **~3%** — compression
  already crushes fixed-tier waste. **So varint mainly helps the small / uncompressed tokens** (below the compress
  crossover), where its raw win applies directly.
- **Cost:** branchier decode (varint is per-value length); our decode is already fast post-span-fix. Adds encoder
  complexity (pick-smallest per section).
- **Verdict:** worth it primarily to (a) lift the 16-bit cap gracefully and (b) shrink small uncompressed tokens.
  Not worth it for large compressed tokens alone.

### 2. Optional q11 "archival" compression mode
- **What:** an opt-in "max compression" mode (Brotli SmallestSize).
- **Measured:** ~16% smaller than fast codecs, but **~98–106 ms encode** (260× slower). Decode is fine.
- **Verdict:** only for **write-once / store-forever** tokens where encode time is amortized. Never a default.
- **Costs more than it looks:** there is no codec field — the codec is pinned by `Version` (§6). A second codec
  burns one of the three remaining version codes.

### 3. Columnar layout — already shipped
Both layouts are in the format, chosen per blob via the `Layout` bit (§3); Session 4 races them. Nothing to add.
If columnar always wins, the *chooser* stops offering row — the bit stays.

### 4. Fixed-point decimals (future, if fractions ever appear)
- **What:** a numeric-domain version carrying a scale factor (store value × 10^k). Today: no fractions.
- **Verdict:** reserve the direction only; a new version when a real need appears. Reserving a scheme now = guessing
  it wrong.

### 5. Per-item widths — NOT recommended
- Rescue for wildly-varied items, but varint subsumes it and it's more complex. Skip unless a real skewed-data case
  appears.

## How to decide each
Prototype in the (now permanent) benchmark first, measure in base64 + encode/decode ms, then spec → C# → TS →
vectors (same pipeline as sessions 3–6). Each rides a reserved width-code or a new `Version`.

## References
[findings.md](findings.md) · `vipaq/PROTOCOL.md` (reserved codes) · harness (prototype + measure).
