---
description: API slice dependency tree — Binacle.Net as composition root, the Kernel floor, the always-compiled modules (Diagnostics, Service, UI), the ServiceModule clean-architecture split, and who sees internals.
verified: 2026-07-14
check: ProjectReference and InternalsVisibleTo entries in api/**/*.csproj match the graph and the walls below
---

# API — project dependencies

The web layer. One entry point (`Binacle.Net`) composes a shared `Kernel` and three modules; the modules are
always compiled and switched on or off at runtime by feature flags (see [modules/README.md](modules/README.md)).

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals. Names below drop the
`Binacle.Net.` prefix except for the entry point.

```
Binacle.Net  (Web SDK, entry / composition root)   [IVT → IntegrationTests, ServiceModule.IntegrationTests]
   refs: Binacle.Lib, Binacle.ViPaq, DiagnosticsModule, ServiceModule, UIModule
      │
      ├── DiagnosticsModule        → Kernel, Binacle.CompactNotation
      │
      ├── ServiceModule            → Kernel, ServiceModule.Domain, ServiceModule.Infrastructure
      │      [IVT → ServiceModule.IntegrationTests]
      │        ├── ServiceModule.Infrastructure → Kernel, ServiceModule.Domain   [IVT → SM.IntegrationTests]
      │        └── ServiceModule.Domain         → (nothing)                       [IVT → SM.IntegrationTests]
      │
      └── UIModule  (Razor SDK)    → Kernel, Binacle.Lib.Abstractions, Binacle.CompactNotation, Binacle.ViPaq

Kernel  → Binacle.Lib.Abstractions, Binacle.CompactNotation      shared API floor (every module refs it)

Tests
   IntegrationTests               xUnit  → Binacle.Net, Binacle.Lib.Abstractions, Binacle.TestsKernel
   ServiceModule.IntegrationTests xUnit  → ServiceModule, Binacle.Net
```

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.Net` | Web exe | Lib, ViPaq, Diagnostics/Service/UI modules | grants IVT | entry point + composition root; registers the concrete lib |
| `Binacle.Net.Kernel` | library | Lib.Abstractions, CompactNotation | — | shared API tooling: endpoint registration, OpenAPI, flags, validation |
| `Binacle.Net.DiagnosticsModule` | library | Kernel, CompactNotation | — | always-on logging / telemetry / health |
| `Binacle.Net.ServiceModule` | library | Kernel, Domain, Infrastructure | grants IVT | JWT auth, rate limiting, accounts (composes its own layers) |
| `Binacle.Net.ServiceModule.Domain` | library | — | grants IVT | entities + repository interfaces (pure) |
| `Binacle.Net.ServiceModule.Infrastructure` | library | Kernel, Domain | grants IVT | DB providers |
| `Binacle.Net.UIModule` | Razor library | Kernel, Lib.Abstractions, CompactNotation, ViPaq | — | Blazor packing demo |
| `Binacle.Net.IntegrationTests` | xUnit exe | Binacle.Net, Lib.Abstractions, TestsKernel | via Binacle.Net IVT | v3/v4 HTTP tests |
| `Binacle.Net.ServiceModule.IntegrationTests` | xUnit exe | ServiceModule, Binacle.Net | via IVT | auth + rate-limit tests |

## The walls

1. **`Binacle.Net` is the only composition root.** It is the single project that references the concrete
   `Binacle.Lib` (and `ViPaq`, and all three modules). Everything downstream depends on `Lib.Abstractions`, so the
   algorithms are wired once, here.

2. **Modules never reference the entry point.** The arrows go one way: `Binacle.Net` → each module. A module
   depending back on `Binacle.Net` would be a cycle and a design break. Modules are compiled in regardless; feature
   flags decide whether they run.

3. **`Kernel` is the shared floor**, referenced by every module (Diagnostics, ServiceModule.Infrastructure,
   UIModule). `Binacle.Net` picks it up transitively through the modules rather than referencing it directly.

4. **ServiceModule is clean-architecture three projects.** `Domain` references nothing (pure entities +
   repository interfaces); `Infrastructure` implements them over `Kernel` + `Domain`; `ServiceModule` composes the
   two plus `Kernel`. Keep `Domain` dependency-free — that is what the layering buys.

5. **`UIModule` references `ViPaq` directly** to decode packing tokens in the Blazor demo — the only module that
   touches the format.
