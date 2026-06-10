---
description: IBinacleService — method reference for SingleBinAsync, MultipleBinsAsync, SmallestBinAsync; return types, call pattern, and algorithm selection
verified: 2026-06-10
check: Method signatures match IBinacleService in api/src/Binacle.Net/Services/BinacleService.cs
---

# IBinacleService

Defined in `api/src/Binacle.Net/Services/BinacleService.cs`. The interface is `internal` — it lives in the
`Binacle.Net` assembly, not in `Binacle.Lib.Abstractions`. Endpoint handlers inject it and call the appropriate
method. They do not touch processors or factories directly.

## Methods

Every method is **generic** and **async**. The full shape is:

```csharp
ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
    Algorithm algorithm, TBin bin, List<TBox> items, TParams parameters)
    where TBin : class, IWithID, IWithReadOnlyDimensions
    where TBox : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
    where TParams : class, IOperationParameters, ILogConvertible;
```

The same generic signature and constraints apply to every method below (the `MultipleBinsAsync` overloads
return `ValueTask<IDictionary<string, OperationResult>>`). The table simplifies argument names for readability.

| Method | Returns | What it does |
|---|---|---|
| `SingleBinAsync(algorithm, bin, items, params)` | `ValueTask<OperationResult>` | Runs one specific algorithm on one bin |
| `SingleBinAsync(bin, items, params)` | `ValueTask<OperationResult>` | Runs all algorithms on one bin, picks `BestAlgorithm` |
| `MultipleBinsAsync(algorithm, bins, items, params)` | `ValueTask<IDictionary<string, OperationResult>>` | Runs one specific algorithm on each bin, returns all results keyed by bin ID |
| `MultipleBinsAsync(bins, items, params)` | `ValueTask<IDictionary<string, OperationResult>>` | Runs all algorithms on each bin, picks best per bin, returns all results keyed by bin ID |
| `SmallestBinAsync(algorithm, bins, items, params)` | `ValueTask<OperationResult>` | Runs one algorithm across all bins, picks `SmallestBin` |
| `SmallestBinAsync(bins, items, params)` | `ValueTask<OperationResult>` | Runs all algorithms across all bins, picks `SmallestBin` |

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
`GetAlgorithm()` returns `null` only for `Best` — that means use the auto-select overload.
(Null from the request body fails the `NotNull()` validator and never reaches the handler.)

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
