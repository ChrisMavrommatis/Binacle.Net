---
id: api/v4
description: v4 API — active development. Endpoints, algorithm selection, parameters, contracts, and response shape.
verified: 2026-08-08
check: Endpoint table matches files in api/src/Binacle.Net/v4/Endpoints/; IsExperimental in ApiV4Document.cs matches what this says
also_update:
  - api/v4/contracts
paths:
  - "api/src/Binacle.Net/v4/**"

---

# v4 API

Route prefix: `/api/v4`

Active development version, and **marked experimental**: `ApiV4Document.IsExperimental` is `true`, so the
published OpenAPI description carries a warning that v4 may change at any time. v3 is not marked — it is frozen.

See `$api/v4/add-endpoint` to add a new endpoint.
See `$api/v4/contracts` for the full request/response shape.
See Fit vs Pack (`$concepts`) for the underlying concept.

## Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/api/v4/fit/bin` | Fit-check a custom bin |
| POST | `/api/v4/fit/bin/{preset}/{bin}` | Fit-check one specific bin from a preset |
| POST | `/api/v4/fit/compare-bins` | Fit-check all custom bins, return all results |
| POST | `/api/v4/fit/compare-bins/{preset}` | Fit-check all preset bins, return all results |
| POST | `/api/v4/fit/smallest-bin` | Custom bins → smallest viable fit |
| POST | `/api/v4/fit/smallest-bin/{preset}` | Preset bins → smallest viable fit |
| POST | `/api/v4/pack/bin` | Pack a custom bin |
| POST | `/api/v4/pack/bin/{preset}/{bin}` | Pack one specific bin from a preset |
| POST | `/api/v4/pack/compare-bins` | Pack all custom bins, return all results |
| POST | `/api/v4/pack/compare-bins/{preset}` | Pack all preset bins, return all results |
| POST | `/api/v4/pack/smallest-bin` | Custom bins → smallest successful pack |
| POST | `/api/v4/pack/smallest-bin/{preset}` | Preset bins → smallest successful pack |
| POST | `/api/v4/pack/best-bin` | Custom bins → highest utilization |
| POST | `/api/v4/pack/best-bin/{preset}` | Preset bins → highest utilization |
| GET | `/api/v4/presets` | List all presets with their bins |
| GET | `/api/v4/presets/{preset}` | Get bin definitions for a preset |

All POST (fit/pack) endpoints are rate-limited (`.RequireRateLimiting("ApiUsage")`) and return `429`.
The two `GET /api/v4/presets…` endpoints are **not** rate-limited — their only responses are `200`, `500`,
and, for the single-preset one, `404`.

Every route taking a `{preset}` returns `404` when the preset does not exist; `fit|pack/bin/{preset}/{bin}`
also returns `404` for an unknown bin within a known preset.

## Selecting Endpoints — which one picks what

Six endpoints run every bin and return one answer. They differ only in the selection strategy applied to the
results (`$lib/result-selection`):

| Route | Strategy | Picks |
|---|---|---|
| `pack/smallest-bin`, `pack/smallest-bin/{preset}`, `fit/smallest-bin`, `fit/smallest-bin/{preset}` | `SmallestBin_v2` | Least bin volume, fully-packed first |
| `pack/best-bin`, `pack/best-bin/{preset}` | `BestBin_v2` | Highest `PackedBinVolumePercentage`, fully-packed first |

The two agree whenever some bin packs fully — for a fully-packed result the smallest bin is also the most
filled. They only diverge when nothing packs fully: `smallest-bin` takes the least roomy bin, `best-bin` takes
the one the items fill most.

### Route names say which bin comes back

Every fit/pack route ends in `bin` or `bins`, and the plural is load-bearing: `compare-bins` returns a result
for every bin, `smallest-bin` / `best-bin` / `bin` return exactly one. The name tells you the response shape.

The selecting routes are named after their strategy class — `SmallestBin_v2` → `smallest-bin`, `BestBin_v2` →
`best-bin`. **Never name a route after a packing algorithm.** `best-bin` was called `best-fit` until it was
caught: `BFD` in the `Algorithm` enum *is* Best Fit Decreasing, so `POST /pack/best-fit` with
`{"algorithm": "FFD"}` used two senses of "fit" in one call and named neither of the things it did. The route
picks a bin; `Parameters.Algorithm` picks a packing algorithm. Keep those vocabularies apart. `best` alone is
no good either — `Algorithm.Best` already means "auto-select the algorithm".

### Why there is no `fit/best-bin`

This gap is deliberate — do not "fix" it. In fitting mode the algorithms early-exit: they stop before packing
anything when the items exceed the bin's volume or longest dimension, and stop at the first item that will not
go in. So for a bin that fails a fit, `PackedBinVolumePercentage` records how far the run got before giving up,
not how well the items fill the bin — an early-exited bin reports `0` however close it was. `BestBin_v2` ranks
everything that is not fully packed on exactly that number.

And when a fit succeeds, `best-bin` and `smallest-bin` give the same answer anyway: every bin packs the same
item volume, so the fullest bin is always the smallest one. `fit/best-bin` would therefore be redundant when it
works and misleading when it does not. `fit/smallest-bin` is safe because `SmallestBin_v2` ranks on bin volume
with fully-packed first — geometry, not how far the run got.

`FitBinResponse` still carries `PackedBinVolumePercentage`, inherited from the shared base. On anything that
early-exited, that number is not a fill measurement. Do not rank on it.

To add an endpoint, follow `$api/v4/add-endpoint`.

## Algorithm Selection

Required. `Best` runs more than one heuristic and keeps the best result — **all three (FFD, WFD, BFD) on
`fit/bin` and `pack/bin`, FFD plus BFD on every other route.** It is not all three everywhere; the wording in
`v4/SchemaDescriptions.cs` (`Algorithm`) is the one that ships, and the docs site repeats it.
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

See `$api/v3` for the stable v3 version.
