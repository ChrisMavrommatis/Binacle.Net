---
id: lib/models
description: Lib model types and IWith* interfaces — Bin, Item, packed/unpacked results, and the constraints used in generic type parameters
verified: 2026-08-13
check: Type and interface names match shared/src/Binacle.Packing/ and lib/src/Binacle.Lib/Abstractions/; generic geometry interfaces match shared/src/Binacle.Geometry/; every file path in the tables resolves
also_update:
  - api/v4/contracts
paths:
  - "lib/src/Binacle.Lib/Models/**"
  - "shared/src/Binacle.Packing/**"
  - "shared/src/Binacle.Geometry/**"

---

# Models

The packing vocabulary lives in `shared/src/Binacle.Packing/`; the engine interfaces in
`lib/src/Binacle.Lib/Abstractions/`. The **generic** geometry interfaces and the concrete `Dimensions<T>` /
`Coordinates<T>` live in the shared `Binacle.Geometry` leaf (`shared/src/Binacle.Geometry/`, namespace
`Binacle.Geometry`) — see [IWith* interfaces](#iwith-interfaces).

Paths in the tables below start at the project folder — `Binacle.Packing/` is `shared/src/Binacle.Packing/`,
`Binacle.Geometry/` is `shared/src/Binacle.Geometry/`, `Binacle.Lib/` is `lib/src/Binacle.Lib/`. The three
projects are mixed on this page, so a bare `Models/…` would not say which one.

## Input models

These are what you pass into the algorithm factory and processors.

| Type | Namespace | Implements |
|---|---|---|
| `Bin` | `Binacle.Lib.Models` (`Binacle.Lib/Models/Bin.cs`) | `IWithID`, `IWithReadOnlyDimensions` |
| `Item` | `Binacle.Lib.Models` (`Binacle.Lib/Models/Item.cs`) | `IWithID`, `IWithReadOnlyDimensions`, `IWithQuantity` |

## Result models

Returned inside `OperationResult` after an algorithm runs.

All three result types extend `ResultItem` (`Binacle.Packing/Models/ResultItem.cs`) — an abstract base that
copies `ID` and `Dimensions` from the source object and computes `Volume`. It is not a reference to the
original input object; it is a snapshot with its own copy of the data.

| Type | File | What it holds |
|---|---|---|
| `ResultItem` (abstract) | `Binacle.Packing/Models/ResultItem.cs` | `ID`, `Dimensions` (copied value), `Volume` (computed) |
| `PackedItem` | `Binacle.Packing/Models/PackedItem.cs` | Extends `ResultItem`; adds `Coordinates` (x/y/z position in bin) |
| `UnpackedItem` | `Binacle.Packing/Models/UnpackedItem.cs` | Extends `ResultItem`; adds `Quantity` (count of items that didn't fit, grouped by ID) |
| `PackedBin` | `Binacle.Packing/Models/PackedBin.cs` | Extends `ResultItem`; carries the bin's ID, dimensions, and volume for the result output |

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
writes through the mutable one. This leaf is the single home — `Binacle.Packing`, `Binacle.CompactNotation`,
and `Binacle.ViPaq` all point at it.

### The non-generic `int` shortcuts

Each generic interface has a non-generic shortcut with the **same name**, bound to `int` (e.g.
`IWithReadOnlyDimensions : IWithReadOnlyDimensions<int>`). This is what `Bin`, `Item` and the result models
implement, so most lib/API code never mentions `<T>`. Use the generic `<T>` form only for non-`int` types
(e.g. vipaq's `ushort` path).

**Each shortcut sits in the same file as its generic, in `Binacle.Geometry`** — not in `Binacle.Packing`. Only
identity and the identifiable markers below are Packing's.

| Interface | File |
|---|---|
| `IWithDimensions` | `Binacle.Geometry/Abstractions/IWithDimensions.cs` |
| `IWithReadOnlyDimensions` | `Binacle.Geometry/Abstractions/IWithReadOnlyDimensions.cs` |
| `IWithQuantity` | `Binacle.Geometry/Abstractions/IWithQuantity.cs` |
| `IWithReadOnlyQuantity` | `Binacle.Geometry/Abstractions/IWithReadOnlyQuantity.cs` |
| `IWithCoordinates` | `Binacle.Geometry/Abstractions/IWithCoordinates.cs` |
| `IWithReadOnlyCoordinates` | `Binacle.Geometry/Abstractions/IWithReadOnlyCoordinates.cs` |

### Identity and volume

`IWithID` is identity, not geometry, so it sits in `shared/src/Binacle.Packing/Abstractions/` rather than in
`Binacle.Geometry`. The volume interfaces stayed in `Binacle.Geometry`, still generic over
`System.Numerics.INumber<T>` — so this table spans both projects, and the file column says which.

| Interface | File | What it requires |
|---|---|---|
| `IWithID` | `Binacle.Packing/Abstractions/IWithID.cs` | `string ID { get; set; }` |
| `IWithReadOnlyID` | `Binacle.Packing/Abstractions/IWithReadOnlyID.cs` | `string ID { get; }` |
| `IWithVolume` | `Binacle.Geometry/Abstractions/IWithVolume.cs` | `int Volume { get; set; }` (non-generic over `IWithVolume<T> : INumber<T>`) |
| `IWithReadOnlyVolume` | `Binacle.Geometry/Abstractions/IWithReadOnlyVolume.cs` | `int Volume { get; }` |

### Identifiable markers — read-only composites

`Binacle.Packing` also defines two read-only composite markers, used where a consumer reads only
id + geometry (chiefly the packing log — a `List<concrete>` hands off with no copy):

| Interface | File | Composes |
|---|---|---|
| `IIdentifiableBin` | `Binacle.Packing/Abstractions/IIdentifiableBin.cs` | `IWithReadOnlyID`, `IWithReadOnlyDimensions` |
| `IIdentifiableItem` | `Binacle.Packing/Abstractions/IIdentifiableItem.cs` | `IWithReadOnlyID`, `IWithReadOnlyDimensions`, `IWithReadOnlyQuantity` |

The v3/v4 `Bin`/`Box` contracts and the preset `BinOption` implement these (on top of `IWithID` /
`IWithDimensions` / `IWithQuantity`), and `IBinacleService`'s generic constraints require them — see
`$api/service`.

## Value types

| Type | File | What it is |
|---|---|---|
| `Dimensions` | `Binacle.Packing/Models/Dimensions.cs` | Length × Width × Height — an `int` `readonly struct`, **`internal`** |
| `Coordinates` | `Binacle.Packing/Models/Coordinates.cs` | X, Y, Z position — an `int` `readonly struct`, **`internal`** |
| `AlgorithmInfo` | `Binacle.Packing/Models/AlgorithmInfo.cs` | Algorithm name + version, used to key results |

`Binacle.Packing` keeps these non-generic `int` `readonly struct`s (namespace `Binacle.Packing`) — `ResultItem`
snapshots a `Dimensions` copy of its source. **Both structs are `internal`**, which is why anything that
fabricates a result rather than reading one needs a friend grant — see `$lib/dependencies`. The **generic**
concrete `Dimensions<T>` / `Coordinates<T>` (mutable classes) live in the `Binacle.Geometry` leaf instead, for
the non-`int` consumers (vipaq, compact notation).

The `IWith*` interfaces in `Binacle.Net.v4.Contracts` (e.g. `IWithBin`, `IWithItems`) are separate — those
are API-level request composition interfaces. See `$api/v4/contracts`.
