---
description: Request/response contract types, validators, and OpenAPI examples for v4 (v3 follows the same shape)
---

# Contracts

All contracts live under `api/src/Binacle.Net/v4/Contracts/` (v4) or `api/src/Binacle.Net/v3/Contracts/` (v3).

## IWith* Interfaces

These compose request types. Each one carries a validator in the same file.

| Interface | Property | Validator checks |
|---|---|---|
| `IWithBin` | `Bin Bin` | not null, not empty, valid dimensions and ID |
| `IWithBins` | `List<Bin> Bins` | not null, not empty, all IDs unique, valid dimensions and ID per bin |
| `IWithItems` | `List<Box> Items` | not null, not empty, all IDs unique, valid dimensions/quantity per item, no volume overflow |
| `IWithOperationParameters` | `OperationParameters Parameters` | not null, algorithm value valid |

## Request Base Classes

Concrete request types extend these. They compose the `IWith*` interfaces and chain their validators.

| Base class | Implements | Used by |
|---|---|---|
| `CustomBinRequestBase` | `IWithOperationParameters`, `IWithBin`, `IWithItems` | `FitCustomBinRequest`, `PackCustomBinRequest` |
| `PresetBinRequestBase` | `IWithOperationParameters`, `IWithItems` | `FitPresetBinRequest`, `PackPresetBinRequest` |
| `CustomBinsRequestBase` | `IWithOperationParameters`, `IWithBins`, `IWithItems` | `PackCustomSmallestBinRequest` (only current user) |
| `PresetBinsRequestBase` | `IWithOperationParameters`, `IWithItems` | nothing yet — base exists for planned endpoints |

Concrete types are thin — usually just a one-liner:

```csharp
public class FitCustomBinRequest : CustomBinRequestBase;
internal class FitCustomBinRequestValidator : CustomBinRequestBaseValidator<FitCustomBinRequest>;
```

## OperationParameters

Sent in every request as `Parameters`.

| Field | Type | Notes |
|---|---|---|
| `Algorithm` | `Algorithm` | Required. `FFD`, `WFD`, `BFD`, or `Best`. Null fails the `NotNull()` validator — you cannot omit this field. |

> **Two `Algorithm` enums exist.** `Binacle.Net.v4.Contracts.Algorithm` (API layer) has `FFD`, `WFD`, `BFD`, and `Best`.
> `Binacle.Lib.Algorithm` (Lib layer) has only `FFD`, `WFD`, `BFD` — no `Best`.
> `GetAlgorithm()` maps the API enum to the Lib enum, converting `Best` → `null` to trigger the multi-algorithm path.
| `IncludeViPaqData` | `bool` | Default `false`. If `true`, response includes a base64 ViPaq payload. |
| `Operation` | `AlgorithmOperation` | Not in JSON — set by the endpoint via `.ForFittingOperation()` or `.ForPackingOperation()`. |

`GetAlgorithm()` maps `Best` → `null`, which tells the service to run all algorithms.

## Response Types

Both fit and pack share a common base (`BinResponseBase` in `api/src/Binacle.Net/v4/Contracts/BinResponseBase.cs`).
Subclasses call `From<T>(parameters, operationResult)` to populate common fields — see step 3 in [add-endpoint.md](add-endpoint.md).

| Field | Type | Notes |
|---|---|---|
| `Bin` | `Bin` | Echo of the bin used |
| `AlgorithmUsed` | `string` | Name of the algorithm that produced this result |
| `PackedItems` | `List<PackedBox>?` | Items that were packed, with coordinates |
| `UnpackedItems` | `List<UnpackedBox>?` | Items that didn't fit |
| `PackedItemsVolumePercentage` | `decimal` | Percentage of total item volume that was packed |
| `PackedBinVolumePercentage` | `decimal` | Percentage of bin volume occupied by packed items |

Volume percentage formulas and rounding rules are in [result-building.md](../lib/result-building.md).
| `ViPaqData` | `string?` | Base64 ViPaq payload — only present if `IncludeViPaqData: true` and items were packed |

### FitBinResponse

Adds:
- `Status` (`BinFitResultStatus`): `Unknown = -1`, `Fits`, `DoesNotFit`, `EarlyExit`
- `EarlyExitReason`: `None`, `ContainerVolumeExceeded`, `ContainerDimensionExceeded`

### PackBinResponse

Adds:
- `Status` (`BinPackResultStatus`): `Unknown = -1`, `FullyPacked`, `PartiallyPacked`, `NotPacked`

### PackedBox

`ID`, `Length`, `Width`, `Height` + position `X`, `Y`, `Z`.

### UnpackedBox

`ID` + `Quantity` (how many of that item didn't fit).

## OpenAPI Examples

Each request and response type has a companion example class in the same file.
Request examples implement `ISingleOpenApiExamplesProvider<T>`.
Response examples implement `IMultipleOpenApiExamplesProvider<T>`.

Shared 400 and 500 response examples live in:
- `Status400ResponseExamples.cs`
- `Status500ResponseExample.cs`

Validation problem examples (422) live alongside the request base class they cover
(e.g., `CustomBinValidationProblemResponseExamples` in `CustomBinRequestBase.cs`).
