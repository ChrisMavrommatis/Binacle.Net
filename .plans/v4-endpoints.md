# v4 Endpoint Plan

## Single Bin

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/pack/bin` | Pack custom bin |
| POST | `/api/v4/pack/bin/{preset}/{bin}` | Pack one specific bin from a preset |
| POST | `/api/v4/fit/bin` | Fit-check custom bin |
| POST | `/api/v4/fit/bin/{preset}/{bin}` | Fit-check one specific preset bin |

## Multi-Bin — All Results

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/pack/compare` | Pack all custom bins, return all results |
| POST | `/api/v4/pack/compare/{preset}` | Pack all bins in a preset, return all results |
| POST | `/api/v4/fit/compare` | Fit-check all custom bins, return all results |
| POST | `/api/v4/fit/compare/{preset}` | Fit-check all preset bins, return all results |

## Multi-Bin — Single Winner

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/pack/smallest` | Custom bins → smallest successful pack |
| POST | `/api/v4/pack/smallest/{preset}` | Preset bins → smallest successful pack |
| POST | `/api/v4/pack/best-fit` | Custom bins → highest utilization |
| POST | `/api/v4/pack/best-fit/{preset}` | Preset bins → highest utilization |
| POST | `/api/v4/pack/first-fit` | Custom bins → first success |
| POST | `/api/v4/pack/first-fit/{preset}` | Preset bins → first success |
| POST | `/api/v4/fit/smallest` | Custom bins → smallest viable fit |
| POST | `/api/v4/fit/smallest/{preset}` | Preset bins → smallest viable fit |

## Discovery

| Method | Route | Description |
|---|---|---|
| GET | `/api/v4/presets` | List all preset names |
| GET | `/api/v4/presets/{preset}` | Get bin definitions for a preset |

## Current State (as of 2026-04-27)

Existing endpoints in `src/Binacle.Net/v4/Endpoints/`:
- `Fit/CustomBin.cs` — maps to `POST /api/v4/fit/bin`
- `Fit/PresetBin.cs` — maps to `POST /api/v4/fit/bin/{preset}/{bin}`
- `Pack/CustomBin.cs` — maps to `POST /api/v4/pack/bin`
- `Pack/PresetBin.cs` — maps to `POST /api/v4/pack/bin/{preset}/{bin}`
- `Pack/CustomSmallestBin.cs` — maps to `POST /api/v4/pack/smallest`
- `Presets/List.cs` — maps to `GET /api/v4/presets`

## Notes

- `compare` endpoints are new (multi-bin, all results)
- `best-fit` and `first-fit` winner endpoints are new
- `fit/smallest` is new
- `GET /api/v4/presets/{preset}` (get bins for one preset) is new
