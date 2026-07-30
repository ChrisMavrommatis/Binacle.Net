# Decide what happens to the three `Parallel*` processors

**Status:** Not started. After v3.0.0. This is an open decision, not a bug - it comes from the open question in
the lib design decisions ledger, and there is no `// TODO` behind it.

## The state

`BinProcessorFactory.Create` and `CreateMultiAlgorithm` take `binCount` and `itemCount` and **ignore both**,
always returning the `Loop` variants. Nothing in `lib/src` or `api/src` constructs any of:

- `lib/src/Binacle.Lib/BinProcessing/ParallelBinProcessor.cs`
- `lib/src/Binacle.Lib/BinProcessing/ParallelMultiAlgorithmBinProcessor.cs`
- `lib/src/Binacle.Lib/AlgorithmProcessing/ParallelAlgorithmProcessor.cs`

Only test code does: the three benchmark bases in `lib/test/Binacle.Lib.Benchmarks/Abstractions/`, plus
`lib/test/Binacle.Lib.UnitTests/Tests/BinProcessingCancellationTests.cs`, which constructs `ParallelBinProcessor`
directly to check it throws on an already-cancelled token. So deleting the classes takes that test with them -
which is fine, since nothing in production can reach the path it guards. The signatures promise a decision that
is never made.

## What the measurement says

On the algorithm set production uses (FFD + BFD), parallel *algorithm* racing runs 0.93x to 1.48x - slower than
`Loop` on the cheapest scenario, and only clearly ahead when the two algorithms take very unequal time. Two
algorithms cap the win at 2x before overhead. That argues against wiring it up.

## The call

Wire the threshold up, or delete the classes. Leaving three unreachable processors in place invites someone to
"fix" a path that never runs, and keeps two parameters in a public factory signature that do nothing.

Two loose ends if they stay:

- **`ParallelBinProcessor` has never been measured.** It parallelises across *bins*, so it scales with bin count
  rather than algorithm count - it is the one that could still pay, and the evidence above does not cover it.
  Measure it before deleting it, or the deletion is a guess.
- **`concurrencyLevel` only sizes the `ConcurrentDictionary`.** It never reaches `MaxDegreeOfParallelism`, so
  the name overpromises.

## Done when

Either the factory uses `binCount` / `itemCount` to pick a parallel variant with a threshold backed by a
measurement, or the three classes and the two unused parameters are gone.
