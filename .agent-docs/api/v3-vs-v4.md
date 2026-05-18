---
description: How v3 and v4 differ in algorithm selection, parameters, endpoints, and responses
---

# v3 vs v4

See [Fit vs Pack](../concepts/fit-vs-pack.md) for the underlying concept.

## Algorithm Selection

**v3:** Required. Must be `FFD`, `WFD`, or `BFD`.

**v4:** Optional. Use `Best` (or leave it out) to let the service try all algorithms and pick the best result.
`GetAlgorithm()` maps `Best` → `null`, which triggers the multi-algorithm path.

## Parameters Object

**v3:** Separate types: `FitRequestParameters` and `PackRequestParameters`.

**v4:** One type: `OperationParameters`.
Set the mode with `.ForFittingOperation()` or `.ForPackingOperation()` before calling the service.

## Endpoints

**v3:**
- `POST /api/v3/fit/by-custom`
- `POST /api/v3/fit/by-preset`
- `POST /api/v3/pack/by-custom`
- `POST /api/v3/pack/by-preset`

**v4 — implemented:**

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/fit/bin` | Fit-check a custom bin |
| POST | `/api/v4/fit/bin/{preset}/{bin}` | Fit-check one specific bin from a preset |
| POST | `/api/v4/pack/bin` | Pack a custom bin |
| POST | `/api/v4/pack/bin/{preset}/{bin}` | Pack one specific bin from a preset |
| POST | `/api/v4/pack/smallest-bin` | Custom bins → smallest successful pack |
| GET | `/api/v4/presets` | List all preset names |

**v4 — planned (not yet implemented):**

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

## Fit Response

**v3:** No coordinates for fit — only pass/fail status and volume percentages.

**v4:** Both fit and pack return item coordinates (X, Y, Z).

| | v3 Fit | v3 Pack | v4 Fit | v4 Pack |
|---|---|---|---|---|
| Early exit | yes | no | yes | no |
| Coordinates | no | yes | yes | yes |
| ViPaqData | no | optional | optional | optional |
