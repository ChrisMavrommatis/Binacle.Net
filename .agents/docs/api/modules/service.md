---
id: api/modules/service
description: ServiceModule — JWT auth, rate limiting, account/subscription management. Three projects using clean architecture.
verified: 2026-07-28
check: Routes, config file names, and connection string name match ServiceModule source
also_update:
  - api/configuration
paths:
  - "api/src/Binacle.Net.ServiceModule/**"
  - "api/src/Binacle.Net.ServiceModule.Domain/**"
  - "api/src/Binacle.Net.ServiceModule.Infrastructure/**"

---

# ServiceModule

Three projects, one feature. Enabled by the `SERVICE_MODULE` feature flag.

## Projects

| Project | Layer | What it is |
|---|---|---|
| `api/src/Binacle.Net.ServiceModule` | Application | Endpoints, JWT config, rate limiting, token service |
| `api/src/Binacle.Net.ServiceModule.Domain` | Domain | Entities, repository interfaces, value objects |
| `api/src/Binacle.Net.ServiceModule.Infrastructure` | Infrastructure | Concrete repositories, DB backends, password hashers |

**Dependency direction:** ServiceModule → Domain ← Infrastructure.
Neither ServiceModule nor Infrastructure knows about each other — both depend on Domain abstractions only.

## Registration

```csharp
// Program.cs
if (Feature.IsEnabled("SERVICE_MODULE")) {
    builder.AddServiceModule();   // registers all three layers
}
if (Feature.IsEnabled("SERVICE_MODULE")) {
    app.UseServiceModule();       // wires auth, authz, rate limiter, endpoints
}
```

`AddServiceModule()` internally calls `builder.AddInfrastructure()` which picks the DB backend.

## Endpoints (v0)

`v0` is code organisation only — it does not appear in the URL.

| Method | Route | Auth |
|---|---|---|
| POST | `/api/auth/token` | none |
| POST | `/api/admin/account` | Admin |
| GET | `/api/admin/account/{id}` | Admin |
| PUT | `/api/admin/account/{id}` | Admin |
| PATCH | `/api/admin/account/{id}` | Admin |
| DELETE | `/api/admin/account/{id}` | Admin |
| POST | `/api/admin/account/{id}/subscription` | Admin |
| PUT | `/api/admin/account/{id}/subscription` | Admin |
| PATCH | `/api/admin/account/{id}/subscription` | Admin |
| DELETE | `/api/admin/account/{id}/subscription` | Admin |

Admin policy requires: authenticated + `ClaimTypes.Role == "Admin"`.

## Rate Limiting

Two policies registered by this module:

| Policy name | Applied to |
|---|---|
| `ApiUsage` | Core API endpoints (`v3/`, `v4/`) via `.RequireRateLimiting("ApiUsage")` |
| `AuthToken` | Auth token endpoint |

Rate limiter config is rule-based (sliding window) — loaded from `Config_Files/ServiceModule/RateLimiter.json`.
`RateLimiter.json` has three named configs: `ApiUsageAnonymous`, `AuthToken`, and `ApiUsageDemoSubscription`.
Only `ApiUsage` and `AuthToken` are registered as ASP.NET policies; `ApiUsageDemoSubscription` is used
internally by the `ApiUsage` policy to apply different limits based on subscription type.

When ServiceModule is off, `.RequireRateLimiting("ApiUsage")` on core endpoints is a no-op.

## Domain Layer

**Entities:**
- `Account` — user account with `AccountRole` (Admin, User) and `AccountStatus` (Active, Suspended, Deleted)
- `Subscription` — linked to an account; `SubscriptionType` (Demo, Basic, Pro, Enterprise) and `SubscriptionStatus`

**Patterns:**
- `Entity` → `AggregateRoot` → `AuditableEntity` (adds created/modified timestamps)
- `ValueObject` base for immutable value types
- `Password` is a `ValueObject` — never stored as plain string
- `IPasswordService` is the domain abstraction for hashing
- `ISoftDeletable` for soft-delete support

**Repository interfaces** (in Domain, implemented in Infrastructure):
- `IAccountRepository`
- `ISubscriptionRepository`

## Infrastructure Layer

**DB backend selection** — `Setup.AddInfrastructure()` tries providers in order until one matches a connection string:

1. Azure Storage (connection string name: `AzureStorage`)
2. PostgreSQL (connection string name: `Postgres`)
3. SQLite (connection string name: `Sqlite`)

If none match, startup throws `ApplicationException` — the app will not start.
Each provider registers its own `IAccountRepository`, `ISubscriptionRepository`, and health check.

**Password hashers** — all three registered as `IPasswordHasher`, resolved by `PasswordService`:
- `PlainTextPasswordHasher` (dev only)
- `Sha256PasswordHasher`
- `Pbkdf2PasswordHasher`

**Startup tasks:**
- `EnsureDefaultAdminAccountExistsStartupTask` — creates default admin on first run.
  Credentials come from `BINACLE_ADMIN_CREDENTIALS` env var.
- `EnsureRequired*TablesExistStartupTask` — creates DB schema on startup (one per provider:
  `EnsureRequiredSqliteTablesExistStartupTask`, `…NpgsqlTablesExist…`, `…AzureTablesExist…`).

> **No migration framework.** There is no EF Core and no migration files. Each backend creates its schema
> idempotently at startup (`CREATE TABLE IF NOT EXISTS …`, or `CreateTableIfNotExistsAsync` for Azure Table
> Storage). Don't look for migrations — there aren't any.

> `InMemoryAccountRepository` / `InMemorySubscriptionRepository` exist, but there is **no InMemory provider** in
> `Setup._infrastructureProviders` (Azure Storage, Npgsql, SQLite only), so it cannot be selected by config.

## Config files

| File | Required | What it configures |
|---|---|---|
| `Config_Files/ServiceModule/ConnectionStrings.json` | optional | DB connection strings (AzureStorage, Postgres, Sqlite) |
| `Config_Files/ServiceModule/RateLimiter.json` | required when SERVICE_MODULE=True | Rate limiter rules (sliding window configs) |
| `Config_Files/ServiceModule/JwtAuth.json` | optional | JWT issuer, audience, and secret (`JwtAuthOptions`) |

In Development, `dotnet user-secrets` is also loaded for the `IModuleMarker` assembly (useful for JWT secrets).

## Adding an Admin Endpoint

Admin and v0 endpoints follow the same `IGroupedEndpoint` pattern as v4.
See `$api/v4/add-endpoint` for the template — ServiceModule endpoints use `IGroupedEndpoint<AdminGroup>`
(`v0/Endpoints/Admin/AdminGroup.cs`) instead of `ApiV4EndpointGroup`.

## Related Tests

`api/test/Binacle.Net.ServiceModule.IntegrationTests` (run with `just test api-service-integration
[Sqlite|Postgres|AzureStorage]`) — covers the auth token endpoint and the Admin account and subscription
endpoints. Subscription has Create/Update/Patch/Delete only — there is no Get.
