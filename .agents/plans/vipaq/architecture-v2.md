# ViPaq v2 — architecture (policy vs mechanism)

Design captured 2026-07-07. **Guides Sessions 3 (spec) and 4 (implement); not needed for Session 1.** Some of this
is direction, not final — the **Open / unknowns** section at the bottom lists what we can't answer yet. Don't treat
those as settled.

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
- Extend `EncodingInfo` with what it doesn't carry yet: **layout** (row/columnar) and, if the knob lands,
  **compression codec/level**.

## Header-driven decode

For a dumb *decode* to work with no out-of-band hint, the header must be self-describing: the deserializer reads
width/layout/compressed off **byte 0** (+ the reserved bit-0 that Session 3 earmarks for layout). Forced-16-bit-on-
small-data only round-trips because the header stores the *chosen* width, not a re-derived one. So "dumb decode" is
really **header-driven decode** — a Session 3 spec requirement.

## Keep the public surface minimal; expose internals to tests

- Public API stays minimal (`Serialize`/`Deserialize`) so the **permanent harness never churns** (see
  [decisions.md](decisions.md) D4).
- The dumb directive entry stays **`internal` + `InternalsVisibleTo`** the test and benchmark projects.
- Result: public contract doesn't grow, yet experiments and unit tests can force any combo.

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

- **Directive type shape.** Extend `EncodingInfo` in place, or introduce a separate `EncodingDirective`? Unknown
  until Session 3 designs the header. `EncodingInfoNotation.cs` (compact notation) may also need to move with it.
- **How layout is encoded in the header.** Session 3 earmarks reserved bit-0, but the exact bit(s) and codes are
  undesigned. Columnar may need more than one flag.
- **Whether the compression knob is exposed at all in v2.0.** Maybe try-both-keep-smaller is fully internal and no
  codec/level ever reaches the directive. Decide in Session 4 once O1/O2 (decisions.md) are answered with data.
- **Does forcing columnar actually pay?** The whole point of the experiment — unknown until measured in Session 4.
  Row may win; findings suggest Brotli already exploits columnar-like structure. Treat columnar as unproven.
- **`InternalsVisibleTo` vs a small `public` low-level API.** Leaning internal, but if the generators/tools need
  the dumb layer too, that could force it public. Revisit in Session 4.
