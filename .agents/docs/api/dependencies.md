---
id: api/dependencies
description: API slice dependency tree — Binacle.Net as composition root, the Kernel floor, the always-compiled modules (Diagnostics, Service, UI), the ServiceModule clean-architecture split, and who sees internals.
verified: 2026-08-21
check: ProjectReference and InternalsVisibleTo entries in api/**/*.csproj match the graph, the table and the walls below, including every test project and the entry point's Using Include items
paths:
  - "api/**"
---

# API — project dependencies

The web layer. One entry point (`Binacle.Net`) composes a shared `Kernel` and three modules; the modules are
always compiled and switched on or off at runtime by feature flags (see `$api/modules`).

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals. Names below drop the
`Binacle.Net.` prefix except for the entry point.

```
Binacle.Net  (Web SDK, entry / composition root)
   [IVT → UnitTests, IntegrationTests, ServiceModule.IntegrationTests]
   refs: Binacle.Lib, Binacle.ViPaq, DiagnosticsModule, ServiceModule, UIModule
      │
      ├── DiagnosticsModule        → Kernel, Binacle.Packing, Binacle.CompactNotation
      │      [IVT → DiagnosticsModule.UnitTests]
      │
      ├── ServiceModule            → Kernel, ServiceModule.Domain, ServiceModule.Infrastructure
      │      [IVT → ServiceModule.UnitTests, ServiceModule.IntegrationTests]
      │        ├── ServiceModule.Infrastructure → Kernel, ServiceModule.Domain   [IVT → SM.IntegrationTests]
      │        └── ServiceModule.Domain         → (nothing)                       [IVT → SM.IntegrationTests]
      │
      └── UIModule  (Razor SDK)    → Kernel

Kernel  → Binacle.CompactNotation                                shared API floor (every module refs it)

Tests  (all xUnit v3, all OutputType Exe)
   UnitTests                       → Binacle.Net
   IntegrationTests                → Binacle.Net, Binacle.Packing, Binacle.TestsKernel
   Kernel.UnitTests                → Kernel
   DiagnosticsModule.UnitTests     → DiagnosticsModule
   ServiceModule.UnitTests         → ServiceModule
   ServiceModule.IntegrationTests  → ServiceModule, Binacle.Net
```

## Projects at a glance

| Project | Kind | References | Sees internals of | Role |
|---|---|---|---|---|
| `Binacle.Net` | Web exe | Lib, ViPaq, Diagnostics/Service/UI modules | — | entry point + composition root; registers the concrete lib |
| `Binacle.Net.Kernel` | library | CompactNotation | — | shared API tooling: endpoint registration, OpenAPI, flags, validation |
| `Binacle.Net.DiagnosticsModule` | library | Kernel, Packing, CompactNotation | — | always-on logging / telemetry / health |
| `Binacle.Net.ServiceModule` | library | Kernel, Domain, Infrastructure | — | JWT auth, rate limiting, accounts (composes its own layers) |
| `Binacle.Net.ServiceModule.Domain` | library | — | — | entities + repository interfaces (pure) |
| `Binacle.Net.ServiceModule.Infrastructure` | library | Kernel, Domain | — | DB providers |
| `Binacle.Net.UIModule` | Razor library | Kernel | — | Razor Pages demo host |
| `Binacle.Net.UnitTests` | xUnit exe | Binacle.Net | Binacle.Net | entry-point units |
| `Binacle.Net.IntegrationTests` | xUnit exe | Binacle.Net, Packing, TestsKernel | Binacle.Net | v3/v4 HTTP tests |
| `Binacle.Net.Kernel.UnitTests` | xUnit exe | Kernel | — (public surface only) | Kernel units |
| `Binacle.Net.DiagnosticsModule.UnitTests` | xUnit exe | DiagnosticsModule | DiagnosticsModule | log/telemetry units |
| `Binacle.Net.ServiceModule.UnitTests` | xUnit exe | ServiceModule | ServiceModule | auth/accounts units |
| `Binacle.Net.ServiceModule.IntegrationTests` | xUnit exe | ServiceModule, Binacle.Net | Binacle.Net, ServiceModule, Domain, Infrastructure | auth + rate-limit tests |

## The walls

1. **`Binacle.Net` is the only composition root.** It is the single project that references the concrete
   `Binacle.Lib` (and `ViPaq`, and all three modules). Nothing else in the slice references the packer at all -
   the two modules that need the result vocabulary and the integration suite take `Binacle.Packing` in
   `shared/src` instead. So the algorithms are wired once, here.

2. **Modules never reference the entry point.** The arrows go one way: `Binacle.Net` → each module. A module
   depending back on `Binacle.Net` would be a cycle and a design break. Modules are compiled in regardless; feature
   flags decide whether they run.

3. **`Kernel` is the shared floor**, referenced by every module (Diagnostics, ServiceModule,
   ServiceModule.Infrastructure, UIModule). `Binacle.Net` picks it up transitively through the modules rather than
   referencing it directly.

4. **ServiceModule is clean-architecture three projects.** `Domain` references nothing (pure entities +
   repository interfaces); `Infrastructure` implements them over `Kernel` + `Domain`; `ServiceModule` composes the
   two plus `Kernel`. Keep `Domain` dependency-free — that is what the layering buys.

5. **`UIModule` references nothing but `Kernel`.** Both its demos are TypeScript running in the browser, so it
   talks to the API over HTTP like any other client and needs no packing, ViPaq or compact-notation reference.

6. **`Binacle.Net` reaches `Binacle.Packing` and `Binacle.Geometry` transitively, and imports both globally.**
   Its csproj carries `<Using Include="Binacle.Packing" />` and `<Using Include="Binacle.Geometry" />`, so
   `Algorithm`, `OperationResult` and the dimension types resolve in entry-point code with no `using` line. The
   chain that supplies them is `Binacle.Lib` → `Binacle.Packing` → `Binacle.Geometry` (`$shared/dependencies`).
