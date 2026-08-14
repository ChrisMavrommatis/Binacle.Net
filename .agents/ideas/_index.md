---
description: Manifest of every file under .agents/ideas, grouped by area. Regenerate with just agents all.
---

# Agent Ideas Index

Every idea in `.agents/ideas/`, grouped by area. Ideas are rough and unvetted — read the one you need,
and move it to `.agents/plans/` once it's picked up. See [README.md](README.md) for the conventions.

## General

```yaml
- file: mutation-testing.md
  description: "mutation testing with Stryker.NET"
- file: testing-techniques.md
  description: "testing techniques not in use"
```

## API

```yaml
- file: api/admin-user-management-site.md
  description: "Admin site for user management"
  paths: ["api/**"]
- file: api/openapi-spec-followups.md
  description: "OpenAPI spec follow-ups"
  paths: ["api/**"]
- file: api/pack-first-bin-endpoint.md
  description: "pack/first-bin endpoint"
  paths: ["api/**"]
- file: api/packing-only-image.md
  description: "a packing-only image variant, without the ServiceModule assemblies"
  paths: ["api/**"]
- file: api/per-user-packing-logs.md
  description: "per-user packing logs"
  paths: ["api/**"]
- file: api/reduce-integration-friction.md
  description: "reduce integration friction"
  paths: ["api/**"]
- file: api/refresh-token-endpoint.md
  description: "add refresh-token support to ServiceModule"
  paths: ["api/**"]
- file: api/schema-migrations.md
  description: "a schema-migration path for the ServiceModule store"
  paths: ["api/**"]
- file: api/servicemodule-simplification.md
  description: "simplify ServiceModule - collapse the ceremony, keep the provider seam"
  paths: ["api/**"]
- file: api/uimodule-alpine-port.md
  description: "UIModule - port from Blazor reactivity to Alpine.js"
  paths: ["api/**"]
```

## Shared

```yaml
- file: shared/extend-shared-models.md
  description: "take the shared model leaf further"
  paths: ["shared/**"]
```

## ViPaq

```yaml
- file: vipaq/interop-vector-coverage.md
  description: "more interop vector coverage"
  paths: ["vipaq/**"]
```
