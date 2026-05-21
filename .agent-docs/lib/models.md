---
description: Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters
---

# Models

All types live in `lib/src/Binacle.Lib.Abstractions/` unless noted.

## Input models

These are what you pass into the algorithm factory and processors.

| Type | Namespace | Implements |
|---|---|---|
| `Bin` | `Binacle.Lib.Models` (`LibModels/Bin.cs`) | `IWithID`, `IWithReadOnlyDimensions` |
| `Item` | `Binacle.Lib.Models` (`LibModels/Item.cs`) | `IWithID`, `IWithReadOnlyDimensions`, `IWithQuantity` |

## Result models

Returned inside `OperationResult` after an algorithm runs.

| Type | File | What it holds |
|---|---|---|
| `PackedItem` | `Algorithms/Models/PackedItem.cs` | ID, dimensions, `Coordinates` (x/y/z position in bin) |
| `UnpackedItem` | `Algorithms/Models/UnpackedItem.cs` | ID, dimensions, quantity (grouped by ID) |
| `PackedBin` | `Algorithms/Models/PackedBin.cs` | Bin reference used in result output |

## IWith* interfaces

Used as type constraints on generic methods (e.g. `IAlgorithmFactory.Create<TBin, TItem>`).

| Interface | File | What it requires |
|---|---|---|
| `IWithID` | `Models/IWithID.cs` | `string ID { get; set; }` |
| `IWithReadOnlyID` | `Models/IWithReadOnlyID.cs` | `string ID { get; }` |
| `IWithDimensions` | `Models/IWithDimensions.cs` | `Length`, `Width`, `Height` (settable) |
| `IWithReadOnlyDimensions` | `Models/IWithReadOnlyDimensions.cs` | `Length`, `Width`, `Height` (read-only) |
| `IWithQuantity` | `Models/IWithQuantity.cs` | `int Quantity { get; set; }` |
| `IWithReadOnlyQuantity` | `Models/IWithReadOnlyQuantity.cs` | `int Quantity { get; }` |
| `IWithCoordinates` | `Models/IWithCoordinates.cs` | `Coordinates` (settable) |
| `IWithReadOnlyCoordinates` | `Models/IWithReadOnlyCoordinates.cs` | `Coordinates` (read-only) |
| `IWithVolume` | `Models/IWithVolume.cs` | `int Volume { get; set; }` |
| `IWithReadOnlyVolume` | `Models/IWithReadOnlyVolume.cs` | `int Volume { get; }` |

## Value types

| Type | File | What it is |
|---|---|---|
| `Dimensions` | `LibModels/Dimensions.cs` | Length × Width × Height |
| `Coordinates` | `LibModels/Coordinates.cs` | X, Y, Z position |
| `AlgorithmInfo` | `Models/AlgorithmInfo.cs` | Algorithm name + version, used to key results |

The `IWith*` interfaces in `Binacle.Net.v4.Contracts` (e.g. `IWithBin`, `IWithItems`) are separate — those
are API-level request composition interfaces. See [contracts.md](../api/contracts.md).
