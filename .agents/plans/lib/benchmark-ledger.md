# Refresh the curated lib benchmark ledger

**Status:** Deferred, not started. The committed lib benchmark results are stale, not just old.

## Why
`results/lib/benchmarks/` stops at 2025-02-10 while `lib/src` has moved on — including the geometry migration,
which moved `Dimensions` / `Coordinates` across an assembly boundary. Those numbers describe code that no longer
exists; do not quote them until they are re-run. (`BestBin_v2` once measured 5–9× faster than v1, 24 B vs
208–336 B allocated — unconfirmed against current code.)

## What
- Re-run the lib benchmarks against current code.
- Curate a keeper into `results/lib/benchmarks/`. The vault is hand-curated — harnesses write to gitignored
  scratch, never straight into `results/`; diff the scratch against the committed files and copy the keeper in
  by hand.
- Algorithm racing was re-measured 2026-07-17 and the evidence lives in the lib design findings; its scratch reports are
  in `BenchmarkDotNet.Artifacts/` and a keeper should be curated in.
