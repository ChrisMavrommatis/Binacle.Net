---
description: v4 API — active development. Endpoints (implemented and planned), algorithm selection, parameters, contracts, and response shape.
---

# v4 API

Route prefix: `/api/v4`

Active development version. See [add-endpoint.md](add-endpoint.md) to add a new endpoint.
See [contracts.md](contracts.md) for the full request/response shape.
See [Fit vs Pack](../../concepts.md) for the underlying concept.

## Implemented Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/fit/bin` | Fit-check a custom bin |
| POST | `/api/v4/fit/bin/{preset}/{bin}` | Fit-check one specific bin from a preset |
| POST | `/api/v4/pack/bin` | Pack a custom bin |
| POST | `/api/v4/pack/bin/{preset}/{bin}` | Pack one specific bin from a preset |
| POST | `/api/v4/pack/smallest-bin` | Custom bins → smallest successful pack |
| GET | `/api/v4/presets` | List all preset names |

## Planned Endpoints (not yet implemented)

Base request types exist: `CustomBinsRequestBase` (custom multi-bin) and `PresetBinsRequestBase` (preset multi-bin)
in `api/src/Binacle.Net/v4/Contracts/`. No concrete request types or endpoint classes exist yet.
To add one, follow [add-endpoint.md](add-endpoint.md) — start from step 1.

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/pack/compare` | Pack all custom bins, return all results |
| POST | `/api/v4/pack/compare/{preset}` | Pack all preset bins, return all results |
| POST | `/api/v4/fit/compare` | Fit-check all custom bins, return all results |
| POST | `/api/v4/fit/compare/{preset}` | Fit-check all preset bins, return all results |
| POST | `/api/v4/pack/smallest/{preset}` | Preset bins → smallest successful pack |
| POST | `/api/v4/pack/best-fit` | Custom bins → highest utilization |
| POST | `/api/v4/pack/best-fit/{preset}` | Preset bins → highest utilization |
| POST | `/api/v4/pack/first-fit` | Custom bins → first success |
| POST | `/api/v4/pack/first-fit/{preset}` | Preset bins → first success |
| POST | `/api/v4/fit/smallest` | Custom bins → smallest viable fit |
| POST | `/api/v4/fit/smallest/{preset}` | Preset bins → smallest viable fit |
| GET | `/api/v4/presets/{preset}` | Get bin definitions for a preset |

## Algorithm Selection

Required. Use `Best` to auto-select the best result across all algorithms (FFD, WFD, BFD).
`GetAlgorithm()` maps `Best` → `null` internally, which triggers the multi-algorithm path.
Any other value picks a specific algorithm. You cannot omit the field — null fails the `NotNull()` validator.

## Parameters

One type for all operations: `OperationParameters`.
Set the mode with `.ForFittingOperation()` or `.ForPackingOperation()` before calling the service.

## Response Shape

| | Fit | Pack |
|---|---|---|
| Early exit | yes | no |
| Coordinates | yes | yes |
| ViPaqData | optional | optional |

Both fit and pack return item coordinates (X, Y, Z). ViPaqData is included when `IncludeViPaqData: true`.

See [v3/README.md](../v3/README.md) for the stable v3 version.
