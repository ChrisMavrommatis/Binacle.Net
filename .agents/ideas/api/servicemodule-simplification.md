# Idea: simplify ServiceModule — collapse the ceremony, keep the provider seam

**Status:** Unvetted idea. Post-v4, not mid-release.

## What

ServiceModule is three projects (`ServiceModule` / `ServiceModule.Domain` / `ServiceModule.Infrastructure`) with a
full DDD layering — aggregate roots, value objects, an auditable-entity hierarchy — for a domain that is **two
entities**: `Account` and `Subscription` (`$api/modules/service`). Collapse that ceremony while keeping the one
abstraction that earns its keep.

## The distinction that matters

Two different things hide under "clean architecture" here, and they deserve opposite verdicts:

- **The provider seam** — `IAccountRepository` / `ISubscriptionRepository` in Domain, with SQLite / Postgres /
  Azure implementations in Infrastructure, selected by connection string. **Keep this.** It is the DB-swap
  mechanism — the thing that lets the store move off any one backend without touching callers. It pays for itself.
- **The DDD ceremony** — three `.csproj`, `Entity → AggregateRoot → AuditableEntity`, `ValueObject` bases, an
  `IPasswordService` abstraction — for two CRUD entities. **This is the overkill.** It's the weight without the
  payoff.

## Collapse

- **Three projects → one.** Fold `Domain` and `Infrastructure` into `ServiceModule` as folders, not separate
  csproj. The dependency direction was the only reason for three assemblies, and one module can hold that with
  namespaces.
- **Keep the repository interfaces and the per-provider implementations** — as folders. The seam survives; only
  the project boundary goes.
- **Flatten the entity/value-object hierarchy** to plain records where the base-class behaviour isn't actually
  used. Keep whatever genuinely carries behaviour (e.g. password hashing), drop the inheritance that only exists
  to satisfy the pattern.

## Don't touch

The packing core (`Binacle.Lib`, `Binacle.Geometry`, `Binacle.Net.Kernel`). That is a genuinely reusable library,
not ceremony — the "simplify" instinct is only about ServiceModule's layering.

## Timing

After v4 ships. Re-architecting the module mid-release adds risk to a release that is about the API version, not
the auth layer. There is no external forcing function, so it waits.

## Open questions

- How much of the `Entity`/`AggregateRoot`/`AuditableEntity` chain carries real behaviour vs. exists for the
  pattern — decides how far the flattening goes.
- Does merging the projects break any test-project references or DI registration that assumed three assemblies.
- Sequence with `refresh-token-endpoint` — the new `IRefreshTokenRepository` should land in the reworked shape,
  so design the two together.

## Related

- `$api/modules/service` (current three-project structure and patterns)
- `ideas/api/refresh-token-endpoint.md` — adds a repository that should land in the reworked shape.
