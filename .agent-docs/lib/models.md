---
description: Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters
verified: 2026-06-10
check: Type and interface names match lib/src/Binacle.Lib.Abstractions/
also_update:
  - api/v4/contracts.md
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

All three result types extend `ResultItem` (`Algorithms/Models/ResultItem.cs`) — an abstract base that
copies `ID` and `Dimensions` from the source object and computes `Volume`. It is not a reference to the
original input object; it is a snapshot with its own copy of the data.

| Type | File | What it holds |
|---|---|---|
| `ResultItem` (abstract) | `Algorithms/Models/ResultItem.cs` | `ID`, `Dimensions` (copied value), `Volume` (computed) |
| `PackedItem` | `Algorithms/Models/PackedItem.cs` | Extends `ResultItem`; adds `Coordinates` (x/y/z position in bin) |
| `UnpackedItem` | `Algorithms/Models/UnpackedItem.cs` | Extends `ResultItem`; adds `Quantity` (count of items that didn't fit, grouped by ID) |
| `PackedBin` | `Algorithms/Models/PackedBin.cs` | Extends `ResultItem`; carries the bin's ID, dimensions, and volume for the result output |

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
| `IWithCoordinates` | `Models/IWithCoordinates.cs` | `X`, `Y`, `Z` (settable) |
| `IWithReadOnlyCoordinates` | `Models/IWithReadOnlyCoordinates.cs` | `X`, `Y`, `Z` (read-only) |
| `IWithVolume` | `Models/IWithVolume.cs` | `int Volume { get; set; }` |
| `IWithReadOnlyVolume` | `Models/IWithReadOnlyVolume.cs` | `int Volume { get; }` |

Each of these is the non-generic shorthand for a generic interface over `System.Numerics.INumber<T>` — e.g.
`IWithCoordinates : IWithCoordinates<int>`, `IWithDimensions : IWithReadOnlyDimensions` (which is
`IWithReadOnlyDimensions<int>`), `IWithVolume : IWithVolume<int>`. The non-generic versions bind `T = int`,
which is what `Bin`, `Item`, and the result models use. Use the generic `<T>` form only for non-`int` coordinate
or dimension types.

## Value types

| Type | File | What it is |
|---|---|---|
| `Dimensions` | `LibModels/Dimensions.cs` | Length × Width × Height |
| `Coordinates` | `LibModels/Coordinates.cs` | X, Y, Z position |
| `AlgorithmInfo` | `Models/AlgorithmInfo.cs` | Algorithm name + version, used to key results |

The `IWith*` interfaces in `Binacle.Net.v4.Contracts` (e.g. `IWithBin`, `IWithItems`) are separate — those
are API-level request composition interfaces. See [contracts.md](../api/v4/contracts.md).
