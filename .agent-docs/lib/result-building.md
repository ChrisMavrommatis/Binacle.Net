---
description: OperationResultBuilder — how OperationResult is constructed, status rules, volume percentages, and integrity checks
---

# Result Building

## OperationResultBuilder

`OperationResultBuilder<TBin, TItem>` (`lib/src/Binacle.Lib/Models/OperationResultBuilder.cs`) is internal to `Binacle.Lib`.
It's the only way an `OperationResult` is created — `OperationResult` (`lib/src/Binacle.Lib.Abstractions/Algorithms/Models/OperationResultStatus.cs`)
has an internal constructor and cannot be instantiated directly.

Each algorithm creates a builder during initialisation and calls it at the end of `Execute()`.
See [algorithms.md](algorithms.md) for where the algorithm implementations live.

## Usage pattern

```csharp
builder
    .WithPackedItems(packedItems)
    .WithUnpackedItems(unpacked)
    .Complete();       // or .EarlyExit(reason)
```

`EarlyExit(reason)` calls `Complete()` first, then overwrites the status to `EarlyExit` and sets the reason.

`EarlyExitReason` values (from `lib/src/Binacle.Lib.Abstractions/Algorithms/Models/OperationResultStatus.cs`):

| Value | When set |
|---|---|
| `None` | Default — no early exit |
| `ContainerVolumeExceeded` | Total item volume exceeds bin volume |
| `ContainerDimensionExceeded` | An item dimension exceeds a bin dimension |

## Status rules

Status is set in `Complete()`:

| Condition | Status |
|---|---|
| All items packed | `FullyPacked` |
| Zero items packed | `NotPacked` |
| Some items packed | `PartiallyPacked` |
| Early exit called | `EarlyExit` (overrides the above) |

## Unpacked item grouping

`WithUnpackedItems` groups items by ID. A `Box("box_1", qty: 2)` that doesn't fit becomes one
`UnpackedItem("box_1", quantity: 2)`. The response contract mirrors this.

## Volume percentages

```
PackedBinVolumePercentage   = packedItemsVolume / bin.Volume * 100     (rounded to 2dp)
PackedItemsVolumePercentage = packedItemsVolume / totalItemsVolume * 100 (rounded to 2dp)
```

Both are on `OperationResult` and are used by result selection strategies.

## Integrity checks

`Complete()` throws `InvalidOperationException` if:
- `packedCount + unpackedCount != totalItems` (item count mismatch)
- `packedVolume + unpackedVolume != totalItemsVolume` (volume mismatch)

These guard against logic errors in the algorithm — they should never fire in normal operation.
