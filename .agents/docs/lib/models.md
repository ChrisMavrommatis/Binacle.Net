---
description: Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters
verified: 2026-07-05
check: Type and interface names match lib/src/Binacle.Lib.Abstractions/; generic geometry interfaces match shared/src/Binacle.Geometry/
also_update:
  - api/v4/contracts.md
---

# Models

All types live in `lib/src/Binacle.Lib.Abstractions/` unless noted. The **generic** geometry interfaces and the
concrete `Dimensions<T>` / `Coordinates<T>` live in the shared `Binacle.Geometry` leaf (`shared/src/Binacle.Geometry/`,
namespace `Binacle.Geometry`) — see [IWith* interfaces](#iwith-interfaces).

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

### Generic geometry interfaces — the `Binacle.Geometry` leaf

The generic dimensions/coordinates/quantity interfaces live in the shared `Binacle.Geometry` leaf
(`shared/src/Binacle.Geometry/`, namespace `Binacle.Geometry`). BCL-only, all constrained
`where T : struct, IBinaryInteger<T>`, split read-only base + mutable derived:

| Interface | What it requires |
|---|---|
| `IWithReadOnlyDimensions<T>` | `Length`, `Width`, `Height` (read-only) |
| `IWithDimensions<T>` | inherits the above; adds settable `Length`, `Width`, `Height` |
| `IWithReadOnlyCoordinates<T>` | `X`, `Y`, `Z` (read-only) |
| `IWithCoordinates<T>` | inherits the above; adds settable `X`, `Y`, `Z` |
| `IWithReadOnlyQuantity<T>` | `Quantity` (read-only) |
| `IWithQuantity<T>` | inherits the above; adds settable `Quantity` |

The read-only vs mutable split is deliberate: formatting reads through the read-only interface, vipaq deserialize
writes through the mutable one. This leaf is the single home — `Binacle.Lib.Abstractions`, `Binacle.CompactNotation`,
and `Binacle.ViPaq` all point at it (they used to each define their own copies).

### Lib's non-generic `int` shortcuts

`Binacle.Lib.Abstractions.Models` keeps non-generic shortcut interfaces with the **same names** — they inherit the
leaf's generics bound to `int` (e.g. `IWithReadOnlyDimensions : Binacle.Geometry.IWithReadOnlyDimensions<int>`). This
is what `Bin`, `Item`, and the result models implement, so most lib/API code never mentions `<T>`. Use the generic
`<T>` form only for non-`int` types (e.g. vipaq's `ushort` path).

| Interface | File |
|---|---|
| `IWithDimensions` | `Models/IWithDimensions.cs` |
| `IWithReadOnlyDimensions` | `Models/IWithReadOnlyDimensions.cs` |
| `IWithQuantity` | `Models/IWithQuantity.cs` |
| `IWithReadOnlyQuantity` | `Models/IWithReadOnlyQuantity.cs` |
| `IWithCoordinates` | `Models/IWithCoordinates.cs` |
| `IWithReadOnlyCoordinates` | `Models/IWithReadOnlyCoordinates.cs` |

### Lib-owned interfaces (not in the leaf)

`IWithID` and the volume interfaces stay in `lib/src/Binacle.Lib.Abstractions/Models/` — they are not geometry, so
they never moved. Volume is still generic over `System.Numerics.INumber<T>`.

| Interface | File | What it requires |
|---|---|---|
| `IWithID` | `Models/IWithID.cs` | `string ID { get; set; }` |
| `IWithReadOnlyID` | `Models/IWithReadOnlyID.cs` | `string ID { get; }` |
| `IWithVolume` | `Models/IWithVolume.cs` | `int Volume { get; set; }` (non-generic over `IWithVolume<T> : INumber<T>`) |
| `IWithReadOnlyVolume` | `Models/IWithReadOnlyVolume.cs` | `int Volume { get; }` |

## Value types

| Type | File | What it is |
|---|---|---|
| `Dimensions` | `LibModels/Dimensions.cs` | Length × Width × Height — an `int` `readonly struct` |
| `Coordinates` | `LibModels/Coordinates.cs` | X, Y, Z position — an `int` `readonly struct` |
| `AlgorithmInfo` | `Models/AlgorithmInfo.cs` | Algorithm name + version, used to key results |

Lib keeps these non-generic `int` `readonly struct`s (in `Binacle.Lib.Models`) — `ResultItem` snapshots a
`Dimensions` copy of its source. The **generic** concrete `Dimensions<T>` / `Coordinates<T>` (mutable classes) live
in the `Binacle.Geometry` leaf instead, for the non-`int` consumers (vipaq, compact notation).

The `IWith*` interfaces in `Binacle.Net.v4.Contracts` (e.g. `IWithBin`, `IWithItems`) are separate — those
are API-level request composition interfaces. See [contracts.md](../api/v4/contracts.md).
