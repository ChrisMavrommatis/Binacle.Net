# ViPaq v2 — architecture (policy vs mechanism)

Design captured 2026-07-07, aligned to `vipaq/PROTOCOL.md` 2026-07-09. **Guides Session 4.** The spec confirmed
this split — it is why widths, `Layout` and `Compressed` are all encoder policy recorded in the header (D14).

## The core split

v2 separates **policy** (what to do) from **mechanism** (how to do it):

- **Dumb serializer (mechanism):** does *exactly* what it's told — "write 16-bit, columnar, no compression" — even
  when that's "pointless." Makes no decisions.
- **Smart chooser (policy):** looks at the payload, decides width/layout/compress, and hands the dumb layer an
  explicit instruction. The public `Serialize` is this layer.

Why: it makes **every combination forceable and testable**. You can force 16-bit on sub-255 data, force columnar
where it "makes no sense," force raw vs compressed — which is exactly what the v2 "worth it?" experiments
(columnar-vs-row, compressed-vs-raw) need, and what unit tests need to pin behavior.

## The instruction is a real, supplied type

`EncodingInfo` (per-section `BitSize` + `Version`) is already ~90% of the instruction object. Today it's *computed*
internally by `CreateEncodingInfo`. v2 **inverts control** so it can also be *supplied*:

- `Serialize` (smart) computes an `EncodingInfo` and calls the dumb layer.
- The dumb layer (`SerializeWithDirective`-style) takes a caller-supplied `EncodingInfo` and obeys it.
- Extend `EncodingInfo` with what it doesn't carry yet: **`Layout`** and **`Compressed`** (both are header fields
  now — `PROTOCOL.md` §2.1). Codec/level never reach the wire: the codec is pinned by `Version`, level is invisible.

## Header-driven decode

For a dumb *decode* to work with no out-of-band hint, the header must be self-describing. It is: `Version`,
`Compressed` and `Layout` are byte 0; the three widths are byte 1 (`PROTOCOL.md` §2). Forced-16-bit-on-small-data
only round-trips because the header stores the *chosen* width and the decoder is forbidden from re-deriving it
(§4). So "dumb decode" is really **header-driven decode**, and the spec now requires it.

## Keep the public surface minimal; expose internals to tests

- Public API stays minimal (`Serialize`/`Deserialize`) so the **permanent harness never churns** (see
  [decisions.md](decisions.md) D4).
- The dumb directive entry stays **`internal` + `InternalsVisibleTo`** the projects that need it.
- Result: public contract doesn't grow, yet experiments and unit tests can force any combo.

**Check before relying on this.** `Binacle.ViPaq.csproj` grants `InternalsVisibleTo` to **only** `.UnitTests` and
`.VectorGenerators` today — **not** `.Benchmarks`, `.PerformanceTests`, or `.TestsKernel`. That is deliberate and
consistent with D4 (the permanent harness lives on the public API and computes widths itself). So "the benchmark
projects can reach the directive" is **not** true today; granting it would need a new entry and would weaken D4.
Decide in Session 4 whether the one-off codec experiment gets access, or stays a throwaway that does.

## What becomes testable (Session 4 + UnitTests)

- **Forced-combo matrix:** `(width × layout × compress)`, each → encode-with-directive → assert header reflects the
  directive → decode → assert equals input. Oracle is **decode-to-input, not byte-equality**.
- **Chooser picks smallest:** enumerate all combos through the dumb layer; assert the smart layer picked the
  min-base64 one. Policy becomes a checkable function.
- **Invalid directive throws:** dumb layer trusts the directive but rejects the impossible (8-bit forced on a >255
  value). Expected-throw test.

## Cross-language (Sessions 5–6)

TS auto-selection could differ from C#, so interop vectors need **forced-width/forced-layout rows**, not just
natural selection — else the two languages could silently disagree in a "pointless" mode. The directive must be
language-neutral in the spec.

## Open / unknowns — do NOT assume these

- **Directive type shape.** Extend `EncodingInfo` in place, or introduce a separate `EncodingDirective`?
  `EncodingInfoNotation.cs` (compact notation) may need to move with it.
- **Does forcing columnar actually pay?** The whole point of the experiment — unknown until measured in Session 4.
  Row may win; findings suggest a good codec already exploits columnar-like structure. Treat columnar as unproven.
- **`InternalsVisibleTo` vs a small `public` low-level API.** Leaning internal, but if the generators/tools need
  the dumb layer too, that could force it public. Revisit in Session 4.

Answered since: layout is byte 0 bit 4 (`PROTOCOL.md` §2.1); the compression override **is** exposed for phase-1
measurement (D13).
