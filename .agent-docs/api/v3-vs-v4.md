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

**v4:**
- `POST /api/v4/fit/bin`
- `POST /api/v4/fit/bin/{preset}/{bin}`
- `POST /api/v4/pack/bin`
- `POST /api/v4/pack/bin/{preset}/{bin}`
- `POST /api/v4/pack/smallest-bin`

See [.plans/v4-endpoints.md](../../.plans/v4-endpoints.md) for the full planned v4 endpoint list.

## Fit Response

**v3:** No coordinates for fit — only pass/fail status and volume percentages.

**v4:** Both fit and pack return item coordinates (X, Y, Z).

| | v3 Fit | v3 Pack | v4 Fit | v4 Pack |
|---|---|---|---|---|
| Early exit | yes | no | yes | no |
| Coordinates | no | yes | yes | yes |
| ViPaqData | no | optional | optional | optional |
