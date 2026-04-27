---
description: Binacle.Net and Binacle.Net.Kernel — the API layer
---

# API

Two projects make up the API layer:

- `src/Binacle.Net` — entry point; versioned endpoint groups; `Program.cs` wires everything
- `src/Binacle.Net.Kernel` — shared tools: endpoint registration, OpenAPI, feature flags, validation

## Active Development

- **v3** (`/api/v3`) — stable, do not modify
- **v4** (`/api/v4`) — active development

## Docs

- [v3 vs v4](v3-vs-v4.md) — how the two versions differ
- [Endpoints](endpoints.md) — endpoint pattern, registration, request flow
- [Modules](modules.md) — optional modules, feature flags
- [Contracts](contracts.md) — request/response types, validators
- [How to Add an Endpoint](add-endpoint.md)

## Concepts

This slice implements [Fit vs Pack](../concepts/fit-vs-pack.md).
