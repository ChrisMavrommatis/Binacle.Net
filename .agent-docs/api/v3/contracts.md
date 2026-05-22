---
description: v3 request and response contracts — field names, outer response wrapper, and enum values for fit and pack.
verified: 2026-05-23
---

# v3 Contracts

All v3 contracts live under `api/src/Binacle.Net/v3/Contracts/`.

## Outer Response Wrapper

Every v3 response wraps its data:

```json
{
  "result": "Success" | "Failure",
  "data": [ ... ]
}
```

`result` is `Success` if at least one bin succeeded; otherwise `Failure`.
`data` is a list of per-bin results.

## Fit Response

Each item in `data` is a `BinFitResult`:

| Field | Type | Notes |
|---|---|---|
| `result` | `BinFitResultStatus` | Per-bin fit outcome (string enum) |
| `bin` | `Bin` | ID and dimensions of the bin |
| `fittedItems` | `FittedBox[]?` | Items that fit (ID + dimensions, no coordinates) |
| `unfittedItems` | `UnfittedBox[]?` | Items that did not fit (ID + quantity) |
| `fittedBinVolumePercentage` | `decimal?` | Percentage of bin volume occupied by fitted items |
| `fittedItemsVolumePercentage` | `decimal?` | Percentage of total item volume that was fitted |

### `BinFitResultStatus` enum

```
AllItemsFit
NotAllItemsFit
EarlyFail_TotalVolumeExceeded
EarlyFail_ItemDimensionExceeded
```

## Pack Response

Each item in `data` is a `BinPackResult`:

| Field | Type | Notes |
|---|---|---|
| `result` | `BinPackResultStatus` | Per-bin pack outcome (string enum) |
| `bin` | `Bin` | ID and dimensions of the bin |
| `packedItems` | `PackedBox[]?` | Items that were packed, with coordinates (X, Y, Z) |
| `unpackedItems` | `UnpackedBox[]?` | Items that did not fit (ID + quantity) |
| `packedItemsVolumePercentage` | `decimal` | Percentage of total item volume that was packed |
| `packedBinVolumePercentage` | `decimal` | Percentage of bin volume occupied by packed items |
| `viPaqData` | `string?` | Base64 ViPaq payload — only present if `includeViPaqData: true` and items were packed |

### `BinPackResultStatus` enum

```
Unknown
NotPacked
PartiallyPacked
FullyPacked
EarlyFail_ContainerVolumeExceeded   ← dead code; pack never triggers early exit
EarlyFail_ContainerDimensionExceeded ← dead code; pack never triggers early exit
```

## Request Fields

All fit and pack requests share the same structure:

| Field | Type | Notes |
|---|---|---|
| `parameters.algorithm` | `Algorithm` | Required. `FFD`, `WFD`, or `BFD`. `Best` is not valid in v3. |
| `parameters.includeViPaqData` | `bool` | Pack only. Default `false`. |
| `bins` | `Bin[]` | Custom endpoints only. One result per bin. |
| `items` | `Box[]` | Required on all requests. |

Preset endpoints take `{preset}` in the route instead of `bins` in the body.

## Item Types

**`FittedBox`** (fit response): `ID`, `Length`, `Width`, `Height` — no coordinates.

**`PackedBox`** (pack response): `ID`, `Length`, `Width`, `Height`, `X`, `Y`, `Z`.

**`UnfittedBox` / `UnpackedBox`**: `ID`, `Quantity`.
