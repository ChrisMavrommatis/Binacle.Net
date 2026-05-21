---
description: ServiceModule — JWT auth, rate limiting, account/subscription management. Three projects using clean architecture.
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

| Method | Route | Auth |
|---|---|---|
| POST | `/api/v0/auth/token` | none |
| POST | `/api/v0/admin/accounts` | Admin |
| GET | `/api/v0/admin/accounts/{id}` | Admin |
| PUT | `/api/v0/admin/accounts/{id}` | Admin |
| PATCH | `/api/v0/admin/accounts/{id}` | Admin |
| DELETE | `/api/v0/admin/accounts/{id}` | Admin |
| POST | `/api/v0/admin/accounts/{id}/subscriptions` | Admin |
| PUT | `/api/v0/admin/accounts/{id}/subscriptions/{subId}` | Admin |
| PATCH | `/api/v0/admin/accounts/{id}/subscriptions/{subId}` | Admin |
| DELETE | `/api/v0/admin/accounts/{id}/subscriptions/{subId}` | Admin |

Admin policy requires: authenticated + `ClaimTypes.Role == "Admin"`.

## Rate Limiting

Two policies registered by this module:

| Policy name | Applied to |
|---|---|
| `ApiUsage` | Core API endpoints (`v3/`, `v4/`) via `.RequireRateLimiting("ApiUsage")` |
| `AuthToken` | Auth token endpoint |

Rate limiter config is rule-based (window, token count, operations) — loaded from
`Config_Files/ServiceModule/ConnectionStrings.json` or env vars.

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
2. PostgreSQL / Npgsql (connection string name: `Npgsql`)
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
- `EnsureRequired*TablesExistStartupTask` — creates DB schema on startup (one per provider).

## Config files

| File | What it configures |
|---|---|
| `Config_Files/ServiceModule/ConnectionStrings.json` | DB connection strings, rate limiter rules, JWT options |
| `Config_Files/ServiceModule/ConnectionStrings.{Environment}.json` | Environment overrides |

JWT secret, issuer, and audience are in `JwtAuthOptions` (loaded from same file).
In Development, `dotnet user-secrets` is also loaded for the `IModuleMarker` assembly.

## Adding an Admin Endpoint

Admin and v0 endpoints follow the same `IGroupedEndpoint` pattern as v4.
See [add-endpoint.md](add-endpoint.md) for the template — use `ApiV0EndpointGroup` as the group type instead of `ApiV4EndpointGroup`.

## Related Tests

`api/test/Binacle.Net.ServiceModule.IntegrationTests` (alias: `api_service`) — covers auth token endpoint
and all Admin account/subscription CRUD endpoints.
