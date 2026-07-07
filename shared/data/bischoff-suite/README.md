# Bischoff Suite

Converted **Bischoff & Ratcliff (BR)** container-loading instances, for the **tests kernel** (lib algorithm
tests). Not used by ViPaq.

- **Source:** [`../or-library/`](../or-library/README.md) raw `thpack1..7.txt`.
- **Origin:** E.E. Bischoff and M.S.W. Ratcliff, "Issues in the development of Approaches to Container
  Loading", OMEGA vol. 23 no. 4 (1995), pp. 377–390 — the "BR instances" (BR1–BR7).
- **Scope:** `thpack1`–`thpack7` **only**. `thpack8` (Loh & Nee, 1992) and `thpack9` (Ivancic et al., 1989,
  multi-container) are different sources/problems and are excluded — see the raw data README.

## Format

One file per `thpack`, a JSON array of scenarios in the tests-kernel compact format:

```json
{
  "Name": "OrLibrary_thpack1_1",
  "Bin": "587x233x220",
  "Metrics": "29736390 30089620 112 98.83",
  "Result": "PartiallyPacked PartiallyPacked",
  "Items": ["108x76x30 [40]", "110x43x25 [33]", "92x81x55 [39]"]
}
```

- `Bin` — `LxWxH`.
- `Items` — `LxWxH [Quantity]`, item **types** with a count (not placed items; no coordinates).
- `Metrics` — `ItemsVolume BinVolume ItemsCount Percentage` (totals over all item types, and their volume
  ratio — **not** packed amounts).
- `Result` — `{PackingStatus} {FittingStatus}`, the **expected** outcome the tests assert against.

Both fields need no packer. `Metrics` is pure arithmetic over `Bin` + `Items`. `Result` is a fixed baseline:
every Bischoff instance fills the container to ~98% but never tessellates perfectly, so the outcome is always
`PartiallyPacked` — the converter writes that for both operations. The tests kernel runs the real packer against
this baseline and asserts they match, so if an instance ever comes out `FullyPacked` or `NotPacked` (packed
unusually well, or nothing fit), that test fails. So the converter has no dependency on the packing algorithms.

This folder is the single source. The converter writes **here**, and the tests kernel embeds these files directly
(via `Link`/`LogicalName` in `Binacle.TestsKernel.csproj`) under the manifest name
`Binacle.TestsKernel.Algorithms.Data.BischoffSuite.<file>` — there is no separate kernel copy. The kernel therefore
runs on the converter's 2-decimal `Metrics` % normalization for thpack5–7.
