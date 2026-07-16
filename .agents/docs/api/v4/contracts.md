---
id: api/v4/contracts
description: Request/response contract types, validators, and OpenAPI examples for v4 (v3 follows the same shape)
verified: 2026-07-16
check: Types and validators match api/src/Binacle.Net/v4/Contracts/; mappers match v4/ExtensionMethods/
also_update:
  - api/v4
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
| `CustomBinsRequestBase` | `IWithOperationParameters`, `IWithBins`, `IWithItems` | `PackCustomSmallestBinRequest`, `PackCustomCompareRequest`, `PackCustomBestFitRequest`, `FitCustomCompareRequest`, `FitCustomSmallestBinRequest` |
| `PresetBinsRequestBase` | `IWithOperationParameters`, `IWithItems` | `PackPresetCompareRequest`, `PackPresetSmallestBinRequest`, `PackPresetBestFitRequest`, `FitPresetCompareRequest`, `FitPresetSmallestBinRequest` |

`PresetBinRequestBase` and `PresetBinsRequestBase` carry identical members — the bins come from the route
either way. They stay separate because the singular one names one bin and the plural one names the whole
preset, and their validation problem examples differ.

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
Subclasses call `From<T>(parameters, operationResult)` to populate common fields — see step 3 in `$api/v4/add-endpoint`.

| Field | Type | Notes |
|---|---|---|
| `Bin` | `Bin` | Echo of the bin used |
| `AlgorithmUsed` | `string` | Name of the algorithm that produced this result |
| `PackedItems` | `List<PackedBox>?` | Items that were packed, with coordinates |
| `UnpackedItems` | `List<UnpackedBox>?` | Items that didn't fit |
| `PackedItemsVolumePercentage` | `decimal` | Percentage of total item volume that was packed |
| `PackedBinVolumePercentage` | `decimal` | Percentage of bin volume occupied by packed items |

Volume percentage formulas and rounding rules are in `$lib/result-building`.
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

### FitCompareResponse / PackCompareResponse

The compare endpoints return a wrapper with a single `Results` field — `List<FitBinResponse>` or
`List<PackBinResponse>`, one entry per bin. There is no envelope beyond that; each entry is exactly the
response shape the single-bin endpoints return.

`From(parameters, bins, operationResults)` walks the **requested** bins, not the result dictionary, so the
order the caller sent the bins in (or the order the preset declares them) is the order they come back in. A bin
missing from the results is skipped rather than faked.

## Response Mapping (lib `OperationResult` → v4 contract)

The mappers live in `api/src/Binacle.Net/v4/ExtensionMethods/FittingMapperExtensions.cs` and
`PackingMapperExtensions.cs`. They are called by `FitBinResponse.From` / `PackBinResponse.From`, which the
endpoint handlers invoke inside a `"Create Response"` activity.

`BinResponseBase.From<T>(parameters, operationResult)` populates the common fields: `Bin` from `result.Bin`,
`AlgorithmUsed` from `result.AlgorithmInfo.Algorithm`, `PackedItems` via `PackedBox.From`, `UnpackedItems` via
`UnpackedBox.From`, the two volume percentages, and — only when `parameters.IncludeViPaqData` **and** there is at
least one packed item — `ViPaqData = Convert.ToBase64String(ViPaqSerializer.SerializeInt32(...))`.

Status mapping (`OperationResultStatus` is the lib enum):

| lib `OperationResultStatus` | `BinFitResultStatus` (fit) | `BinPackResultStatus` (pack) |
|---|---|---|
| `Unknown` | `Unknown` | `Unknown` |
| `FullyPacked` | `Fits` | `FullyPacked` |
| `PartiallyPacked` | `DoesNotFit` | `PartiallyPacked` |
| `NotPacked` | `DoesNotFit` | `NotPacked` |
| `EarlyExit` | `EarlyExit` | *(not mapped — pack never early-exits)* |

Unmapped values throw `NotSupportedException`. Fit also maps `EarlyExitReason` 1:1
(`None`/`ContainerVolumeExceeded`/`ContainerDimensionExceeded`); pack has no early-exit reason. `PackedBox.From`
copies ID + dimensions + coordinates (and implements the shared `Binacle.Geometry.IWithDimensions<int>` /
`IWithCoordinates<int>` so it can be serialized by vipaq directly); `UnpackedBox.From` copies ID + quantity.

## OpenAPI Examples

Each request and response type has a companion example class in the same file.
Request examples implement `ISingleOpenApiExamplesProvider<T>`.
Response examples implement `IMultipleOpenApiExamplesProvider<T>`.

Shared 400 and 500 response examples live in:
- `Status400ResponseExamples.cs`
- `Status500ResponseExample.cs`

Validation problem examples (422) live alongside the request base class they cover
(e.g., `CustomBinValidationProblemResponseExamples` in `CustomBinRequestBase.cs`).

### Where the example data comes from

**Never write the sample geometry into an example class.** It lives in one place:

- `Contracts/ExampleData.cs` — the items, the bins, and the packed/unpacked layouts. Every member is a method
  returning a fresh instance, because callers mutate what they get back.
- `Contracts/Pack/PackExampleResponses.cs`, `Contracts/Fit/FitExampleResponses.cs` — the three outcomes every
  example is one of (`FullyPacked`/`PartiallyPacked`/`NotPacked`, `Fits`/`DoesNotFit`/`EarlyExit`). Each takes
  either a bin id (the canonical bin) or a `Bin`. An example class picks an outcome and names a bin; that's all.

A compare example builds its own `Results` from those outcomes rather than going through a helper:

```csharp
new PackCompareResponse
{
    Results = ExampleData.Bins("custom_bin")
        .Select(bin => PackExampleResponses.FullyPacked(bin))
        .ToList()
}
```

**Derived, never typed:** both volume percentages come from `ExampleData.WithVolumePercentages()` and the
ViPaq token from `WithViPaqData()`, each computed from the geometry beside it using the real formula
(`$lib/result-building`). Hand-written numbers drift — the partially-packed examples had claimed 79.37/12.58
for a layout the formula puts at 94.74/15.00. Add a new example by composing these, not by copying literals.
