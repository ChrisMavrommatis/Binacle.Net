---
description: simplify ServiceModule - collapse the ceremony, keep the provider seam
paths:
  - "api/**"
---

# Idea: simplify ServiceModule — collapse the ceremony, keep the provider seam

**Status:** Unvetted idea. Post-v4, not mid-release.

## What

ServiceModule is three projects (`ServiceModule` / `ServiceModule.Domain` / `ServiceModule.Infrastructure`) with a
full DDD layering — aggregate roots, value objects, an auditable-entity hierarchy — for a domain that is **two
entities**: `Account` and `Subscription` (the ServiceModule doc). Collapse that ceremony while keeping the one
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

## What makes this risky to start

It re-architects the module wholesale, so it wants a window where nothing else is moving through the same code.
There is no external forcing function - nothing breaks if it never happens, which is why it has never been
urgent and why it keeps being the thing five other items wait on.

## Azure Table Storage is the weak provider, and it may go

**It is the one backend that cannot serve an admin screen.** It has no secondary indexes, so it cannot sort by
any field but `RowKey`, cannot skip, and cannot count without reading every row it counts. Filters are the one
thing it does push down. Everything else has to be done in memory by the provider.

That is not what "NoSQL" costs - it is what *this* store costs. A document store with a query engine (MongoDB,
Cosmos DB's NoSQL API, RavenDB) sorts, pages and counts natively, the same as the two SQL backends. The line is
not relational versus document; it is whether the store has secondary indexes. Table Storage and DynamoDB sit
below it. Almost everything else sits above.

So keeping the document door open costs nothing. **Dropping Table Storage does not close it.** It also has no
dedicated sample and no smoke profile of its own since the Azure smoke profile was folded into the plain
service one - though it does run on the PR gate, against Azurite.

If it stays, its admin list paths read every matching row on every request. That is bounded by the account
count and it never touches the auth path, where get-by-id and get-by-username are point lookups - the one thing
the store is genuinely good at.

## The storage shape is portable. Four things about it are not free

**No migration is needed to add a provider.** The entities are flat scalars with no nesting and no collections,
so an account maps to a relational row, a document and a table entity identically. Ids are version 7 Guids, so
they are portable and already in creation order in every store. Enums are stored as strings, so a new
`SubscriptionType` costs nothing. Soft deletion is modelled in the data rather than delegated to a store
feature. The password is one `type::hash::salt` string, so it says which hasher wrote it and a hasher change
can be a lazy per-row upgrade.

Four things are worth fixing while the data is small, because each gets harder as real accounts accumulate:

- **There is no index on `Username`, and no unique constraint.** The primary key is the only index on any of the
  four tables. Every login is a full table scan on both SQL backends, and two concurrent creates of the same
  username both pass the check-then-write in the create endpoint. Adding a unique index later is the change that
  **fails if duplicates already exist by then**.

  It needs to be a partial index - unique on `Username` where the row is not deleted. Deletion is soft, the row
  and its username survive it, and the by-username lookup filters deleted rows out, so a username is reusable
  today and a plain unique index would silently take that away. SQLite and Postgres both do partial indexes.
  Azure Table Storage has no indexes to add.

- **The account/subscription link is stored on both sides** - `Account.SubscriptionId` and
  `Subscription.AccountId` - and kept in step by hand across two writes with no transaction. Creating a
  subscription writes it, sets it on the account, updates the account, and deletes the subscription if that
  update fails. A crash in between leaves an orphan either way. Choosing one side is a reconciliation pass over
  whatever data exists when it happens.

- **One subscription per account is enforced in code only.** Nothing in any schema stops a second one. If
  subscriptions ever become a history rather than one current row - upgrade, downgrade, cancel - then
  `Account.SubscriptionId` is the wrong column, and that one is a real data migration.

- **There is no mechanism to apply a schema change at all.** Every backend runs `CREATE TABLE IF NOT EXISTS` at
  startup and nothing else, so adding a column or an index to an already-deployed database does nothing today.
  The schema migrations idea covers the mechanism. Worth knowing here because it is what turns any of the three
  above from a small fix into a blocked one. Indexes are the exception: `CREATE INDEX IF NOT EXISTS` is
  idempotent and does work on a table that already exists.

## Open questions

- How much of the `Entity`/`AggregateRoot`/`AuditableEntity` chain carries real behaviour vs. exists for the
  pattern — decides how far the flattening goes.
- Does merging the projects break any test-project references or DI registration that assumed three assemblies.
- If refresh-token support is added later, its `IRefreshTokenRepository` should land in this reworked shape —
  keep the repository seam so it drops in cleanly.

## Related

- the ServiceModule doc (current three-project structure and patterns)
