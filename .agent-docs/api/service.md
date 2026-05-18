---
description: IBinacleService — the main service endpoint handlers call to run bin operations
---

# IBinacleService

Defined in `src/Binacle.Net/Services/BinacleService.cs`.
Endpoint handlers inject this and call the appropriate method. They do not touch processors or factories directly.

## Methods

| Method | What it does |
|---|---|
| `SingleBinAsync(algorithm, bin, items, params)` | Runs one specific algorithm on one bin |
| `SingleBinAsync(bin, items, params)` | Runs all algorithms on one bin, picks `BestAlgorithm` |
| `MultipleBinsAsync(algorithm, bins, items, params)` | Runs one specific algorithm on each bin, returns all results |
| `MultipleBinsAsync(bins, items, params)` | Runs all algorithms on each bin, picks best per bin, returns all results |
| `SmallestBinAsync(algorithm, bins, items, params)` | Runs one algorithm across all bins, picks `SmallestBin` |
| `SmallestBinAsync(bins, items, params)` | Runs all algorithms across all bins, picks `SmallestBin` |

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

See [Processors](../lib/processors.md) for how the service uses factories internally.
