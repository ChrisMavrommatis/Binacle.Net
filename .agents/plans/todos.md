# TODOs

Mostly `// TODO` comments found across `lib/`, `api/`, `vipaq/`, `shared/`, grouped by area. A few entries have
no comment behind them — an open decision that needs a call. Those say so.

---

## Lib

- **Decide what to do with the three `Parallel*` processors** — no code comment; this comes from the open
  question in the lib design decisions ledger.

  `BinProcessorFactory.Create` and `CreateMultiAlgorithm` take `binCount` and `itemCount` and ignore both, always
  returning the `Loop` variants. Nothing in `lib/src` or `api/src` constructs
  `lib/src/Binacle.Lib/BinProcessing/ParallelBinProcessor.cs`,
  `lib/src/Binacle.Lib/BinProcessing/ParallelMultiAlgorithmBinProcessor.cs`, or
  `lib/src/Binacle.Lib/AlgorithmProcessing/ParallelAlgorithmProcessor.cs` — only the benchmarks do. The
  signatures promise a decision that is never made.

  The measurement argues against wiring it up: on the algorithm set production uses (FFD+BFD), parallel
  *algorithm* racing runs 0.93× to 1.48× — slower than `Loop` on the cheapest scenario, and only clearly ahead
  when the two algorithms take very unequal time. Two algorithms cap the win at 2× before overhead.

  So: wire the threshold up, or delete the classes. Leaving three unreachable processors in place invites
  someone to "fix" a path that never runs. Two loose ends if they stay — `ParallelBinProcessor` (many *bins* at
  once, which scales with bin count rather than algorithm count) has never been measured and is the one that
  could still pay; and `concurrencyLevel` only sizes the `ConcurrentDictionary`, never reaching
  `MaxDegreeOfParallelism`, so the name overpromises.

---

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.

---

## Integration Tests — General

- `api/test/Binacle.Net.IntegrationTests/BinacleApi.cs:35`
  Run integration tests with all modules enabled (currently only core is active).

- `api/test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
  Same — run with all modules enabled.

- `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`
  ServiceModule integration tests should also run with all modules enabled.
