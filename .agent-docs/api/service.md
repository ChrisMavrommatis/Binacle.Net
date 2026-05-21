---
description: IBinacleService — method reference for SingleBinAsync, MultipleBinsAsync, SmallestBinAsync; return types, call pattern, and algorithm selection
---

# IBinacleService

Defined in `api/src/Binacle.Net/Services/BinacleService.cs`.
Endpoint handlers inject this and call the appropriate method. They do not touch processors or factories directly.

## Methods

| Method | Returns | What it does |
|---|---|---|
| `SingleBinAsync(algorithm, bin, items, params)` | `OperationResult` | Runs one specific algorithm on one bin |
| `SingleBinAsync(bin, items, params)` | `OperationResult` | Runs all algorithms on one bin, picks `BestAlgorithm` |
| `MultipleBinsAsync(algorithm, bins, items, params)` | `IDictionary<string, OperationResult>` | Runs one specific algorithm on each bin, returns all results keyed by bin ID |
| `MultipleBinsAsync(bins, items, params)` | `IDictionary<string, OperationResult>` | Runs all algorithms on each bin, picks best per bin, returns all results keyed by bin ID |
| `SmallestBinAsync(algorithm, bins, items, params)` | `OperationResult` | Runs one algorithm across all bins, picks `SmallestBin` |
| `SmallestBinAsync(bins, items, params)` | `OperationResult` | Runs all algorithms across all bins, picks `SmallestBin` |

## Choosing the right method

| Endpoint scenario | Method to use |
|---|---|
| Single bin, user picked an algorithm | `SingleBinAsync(algorithm, bin, items, params)` |
| Single bin, auto-select best | `SingleBinAsync(bin, items, params)` |
| All bins, return all results, user picked algorithm | `MultipleBinsAsync(algorithm, bins, items, params)` |
| All bins, return all results, auto-select best | `MultipleBinsAsync(bins, items, params)` |
| Many bins, return smallest that fits | `SmallestBinAsync(algorithm/bins, items, params)` |

## Calling it

The handler reads the algorithm from `request.Parameters.GetAlgorithm()`.
`GetAlgorithm()` returns `null` for `Best` or when no algorithm is set — that means use the auto-select overload.

```csharp
var algorithm = request.Parameters.GetAlgorithm();

OperationResult result;
if (algorithm.HasValue)
{
    result = await binacleService.SingleBinAsync(
        algorithm.Value, bin, request.Items, request.Parameters.ForFittingOperation()
    );
}
else
{
    result = await binacleService.SingleBinAsync(
        bin, request.Items, request.Parameters.ForFittingOperation()
    );
}
```

Always call `.ForFittingOperation()` or `.ForPackingOperation()` on `Parameters` before passing them in.
This sets the `AlgorithmOperation` that the algorithm uses — it is not in the JSON body.

> **Warning:** these methods mutate the `OperationParameters` instance. Do not call both on the same object —
> the second call will overwrite the first. Each endpoint should call exactly one.

See [Processors](../lib/processors.md) for how the service uses factories internally.
See [Result Selection](../lib/result-selection.md) for how `BestAlgorithm`, `SmallestBin`, and `BestBin` strategies are scored.
