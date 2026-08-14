---
id: api/v3
description: v3 API — stable, do not modify. Endpoints, algorithm selection, response shape, field names, and enum values.
verified: 2026-07-15
check: Endpoint list, field names, and enum values match api/src/Binacle.Net/v3/
also_update:
  - api/v3/contracts
paths:
  - "api/src/Binacle.Net/v3/**"

---

# v3 API

> **Do not modify v3. Do not add endpoints here.** It is stable and locked. All active development goes in v4.

Route prefix: `/api/v3`

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/v3/presets` | List all configured presets |
| POST | `/api/v3/fit/by-preset/{preset}` | Fit-check all bins in a preset (one result per bin) |
| POST | `/api/v3/fit/by-custom` | Fit-check a list of custom bins (one result per bin) |
| POST | `/api/v3/pack/by-preset/{preset}` | Pack all bins in a preset (one result per bin) |
| POST | `/api/v3/pack/by-custom` | Pack a list of custom bins (one result per bin) |

v3 custom endpoints take `List<Bin> Bins` — multiple bins per request. Each bin is packed/fitted independently
and the response contains one result per bin. This is different from v4, which takes a single bin per request.

## Algorithm Selection

Required on all fit and pack requests. Must be `FFD`, `WFD`, or `BFD`. No auto-select path.

## Parameters

Separate types per operation: `FitRequestParameters` and `PackRequestParameters`.

`includeViPaqData` (bool, optional) — pack endpoints only. When `true`, each packed bin result includes a
`viPaqData` field with the packing encoded in the ViPaq format.

## Response Shape

All v3 responses have an outer wrapper:

```json
{ "result": "Success" | "Failure", "data": [...] }
```

`result` at the top level reflects whether any bin succeeded, not an individual bin's result.

| | Fit | Pack |
|---|---|---|
| Early exit | yes | no |
| Coordinates | no | yes |
| ViPaqData | no | optional (`includeViPaqData: true`) |

Both fit and pack return a list of per-bin results. Each bin result includes the items that were
fitted/packed and the items that were not.

## Field Names

v3 uses different field names from v4:

| v3 field | v4 equivalent | Notes |
|---|---|---|
| `result` | `status` | Per-bin result (string enum) |
| `fittedItems` | `packedItems` | Fit only — items that fit (no coordinates) |
| `unfittedItems` | `unpackedItems` | Fit only |
| `fittedBinVolumePercentage` | `packedBinVolumePercentage` | Fit only |
| `fittedItemsVolumePercentage` | `packedItemsVolumePercentage` | Fit only |
| `packedItems` | `packedItems` | Pack — same name, includes coordinates |
| `unpackedItems` | `unpackedItems` | Pack — same name |

## Fit Status Enum (`BinFitResultStatus`)

```
AllItemsFit
NotAllItemsFit
EarlyFail_TotalVolumeExceeded
EarlyFail_ItemDimensionExceeded
```

## Pack Status Enum (`BinPackResultStatus`)

```
Unknown
NotPacked
PartiallyPacked
FullyPacked
EarlyFail_ContainerVolumeExceeded   ← dead code; pack never triggers early exit
EarlyFail_ContainerDimensionExceeded ← dead code; pack never triggers early exit
```

The two `EarlyFail_*` values on pack are never set at runtime — pack always runs to completion.
They exist in the enum but are unreachable.

See `$api/v4` for how v4 differs.

## Request Example

Pack by preset with FFD, including ViPaq data.

<!-- sourced from docs site; verify against current code if behaviour changes -->

```json
{
  "parameters": {
    "algorithm": "FFD",
    "includeViPaqData": true
  },
  "items": [
    { "id": "box_1", "quantity": 2, "length": 2, "width": 5, "height": 10 },
    { "id": "box_2", "quantity": 1, "length": 12, "width": 15, "height": 10 },
    { "id": "box_3", "quantity": 1, "length": 12, "width": 10, "height": 15 }
  ]
}
```

## Response Example

<!-- sourced from docs site; verify against current code if behaviour changes -->

```json
{
  "result": "Success",
  "data": [
    {
      "result": "FullyPacked",
      "bin": { "id": "preset_bin_1", "length": 10, "width": 40, "height": 60 },
      "packedItems": [
        { "id": "box_2", "length": 10, "width": 12, "height": 15, "x": 0, "y": 0, "z": 0 },
        { "id": "box_1", "length": 2, "width": 5, "height": 10, "x": 0, "y": 12, "z": 0 }
      ],
      "unpackedItems": [],
      "packedItemsVolumePercentage": 100,
      "packedBinVolumePercentage": 7.92,
      "viPaqData": "AAQACig8CgwPAAAACgwPAAwAAgUKAAAPAgUKABgA"
    },
    {
      "result": "FullyPacked",
      "bin": { "id": "preset_bin_2", "length": 20, "width": 40, "height": 60 },
      "packedItems": [
        { "id": "box_2", "length": 12, "width": 15, "height": 10, "x": 0, "y": 0, "z": 0 },
        { "id": "box_1", "length": 2, "width": 5, "height": 10, "x": 12, "y": 0, "z": 0 }
      ],
      "unpackedItems": [],
      "packedItemsVolumePercentage": 100,
      "packedBinVolumePercentage": 3.96,
      "viPaqData": "AAQAFCg8DA8KAAAADAoPAA8AAgUKDAAAAgUKAAAK"
    }
  ]
}
```
