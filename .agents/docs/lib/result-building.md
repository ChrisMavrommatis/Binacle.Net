---
id: lib/result-building
description: OperationResultBuilder — how OperationResult is constructed, status rules, volume percentages, and integrity checks
verified: 2026-08-19
check: The builder's methods, status branches, integrity checks and rounding match lib/src/Binacle.Lib/Models/OperationResultBuilder.cs; the entry point matches ExtensionMethods/AlgorithmResultBuilderExtensions.cs; OperationResult and its enums are still in shared/src/Binacle.Packing/Models/OperationResultStatus.cs
paths:
  - "lib/src/Binacle.Lib/Models/OperationResultBuilder.cs"
  - "shared/src/Binacle.Packing/Models/**"

---

# Result Building

## OperationResultBuilder

`OperationResultBuilder<TBin, TItem>` (`lib/src/Binacle.Lib/Models/OperationResultBuilder.cs`) is internal to `Binacle.Lib`.
It's the only way an `OperationResult` is created — `OperationResult` (`shared/src/Binacle.Packing/Models/OperationResultStatus.cs`)
has an internal constructor and cannot be instantiated directly.

Each algorithm creates its builder at the **top of `Execute()`**, through the
`CreateResultBuilder<TBin, TItem>` extension in `lib/src/Binacle.Lib/ExtensionMethods/`, and finishes with it
at the end. The extension is what builds the `AlgorithmInfo` from the instance's own `Algorithm` and `Version`,
so a result can never report a heuristic it did not run.
See `$lib/algorithms` for where the algorithm implementations live.

## Usage pattern

```csharp
builder
    .WithPackedItems(packedItems)
    .WithUnpackedItems(unpacked)
    .Complete();       // or .EarlyExit(reason)
```

`EarlyExit(reason)` calls `Complete()` first, then overwrites the status to `EarlyExit` and sets the reason.

`EarlyExitReason` values (from `shared/src/Binacle.Packing/Models/OperationResultStatus.cs`):

| Value | When set |
|---|---|
| `None` | Default — no early exit |
| `ContainerVolumeExceeded` | Total item volume exceeds bin volume |
| `ContainerDimensionExceeded` | An item dimension exceeds a bin dimension |

## Status rules

`Complete()` builds the result with status `Unknown` and then narrows it:

| Condition | Status |
|---|---|
| `packedCount == totalItems` | `FullyPacked` |
| else `unpackedCount == totalItems` | `NotPacked` |
| else `packedCount > 0` | `PartiallyPacked` |
| none of the above | stays `Unknown` |
| Early exit called | `EarlyExit` (overrides the above) |

**The `Unknown` fall-through cannot be reached**, and that is worth knowing because nothing downstream guards
against it (`$lib/result-selection`). The integrity check above has already forced
`packedCount + unpackedCount == totalItems`, so `packedCount == 0` implies `unpackedCount == totalItems` and
the second branch takes it. `Unknown` survives as the sentinel a new code path would land on, not as a state
the current one produces.

## Unpacked item grouping

`WithUnpackedItems` groups items by ID. A `Box("box_1", qty: 2)` that doesn't fit becomes one
`UnpackedItem("box_1", quantity: 2)`. The response contract mirrors this.

## Volume percentages

```
PackedBinVolumePercentage   = packedItemsVolume / bin.Volume * 100       (rounded to 2dp)
PackedItemsVolumePercentage = packedItemsVolume / totalItemsVolume * 100 (rounded to 2dp)
```

Both are computed in `decimal`, not `double`, and rounded with `Math.Round(value, 2)`. **That overload rounds
half to even**, which is the BCL default and not what most people picture: `0.125` goes to `0.12`, `0.135` to
`0.14`. Anything reproducing these numbers outside the builder — an OpenAPI example, a client, a test
expectation — has to round the same way or it will disagree in the last digit on exact halves.

Both are on `OperationResult` and are used by result selection strategies.

## Integrity checks

`Complete()` throws `InvalidOperationException`, count first and volume second, if:
- `packedCount + unpackedCount != totalItems` (item count mismatch)
- `packedVolume + unpackedVolume != totalItemsVolume` (volume mismatch)

Both run **before** the percentages and the status, so a broken algorithm throws rather than returning a
plausible-looking wrong answer. They guard against logic errors in the algorithm and should never fire in
normal operation.
