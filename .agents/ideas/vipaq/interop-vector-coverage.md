# Idea: more interop vector coverage

**Status:** Unvetted idea. Low value — add only if a real need appears.

The C#/TS interop apparatus is built and green (see `$vipaq/cross-language-testing`). These are
the rows we chose not to add. Each is just new `interop/input.json` rows plus a regen — the matrix fans them
across both suites automatically, so the cost is small. The value is what's unproven.

- Mirror the width-boundary flips in a **coordinate** (a separate encoder from dims, though a shared picker).
- An **empty items list**.
- Many **distinct** items — varied dims and coords, not `:Q` repeats.
- **Compressed payloads at 32/64-bit widths.**

## Already tried and dropped — do not rebuild

Cross-runtime coverage (foreign-runtime gzip blobs, .NET-8/9 rows) was built, then removed. A gzip decoder reads
any valid gzip, so it was belt-and-suspenders, and it needed hand-captured Docker bytes — outside the
"one generator, committed output" discipline the rest of the vectors keep. The finding it demonstrated is
preserved in `vipaq/PROTOCOL.md §6`.

## Note on scope

If a future ViPaq wire version lands, the vectors get regenerated anyway and the boundary rows change with it.
Adding rows before then means doing it twice.

## Related

- `$vipaq/cross-language-testing` — the apparatus and the full vector inventory
