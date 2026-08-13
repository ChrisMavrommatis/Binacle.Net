---
description: add refresh-token support to ServiceModule
paths:
  - "api/**"
---

# Idea: add refresh-token support to ServiceModule

**Status:** Unvetted idea. Likely done together with the ServiceModule rework (see Timing).

## Why

The web (Alpine) client calls the packing API with no token, so with ServiceModule on every call is anonymous and
bound by `ApiUsageAnonymous` — the site gets 429s under normal use (the ServiceModule doc). Authenticating fixes
that, but a bare access token expires (`JwtAuth.ExpirationInSeconds`, 3600) and there is **no way to renew it
without re-sending credentials** — there is no refresh grant today. Refresh tokens let a client authenticate once
and stay authenticated silently until the refresh token itself expires.

## What to add

- **`/api/auth/refresh`** — takes a refresh token, returns a fresh access token. No credentials.
- **`/api/auth/token`** — extend to return an access token **and** a refresh token, not just the access token.
- **A refresh-token store** — an `IRefreshTokenRepository` in Domain, with SQLite / Postgres / Azure
  implementations in Infrastructure, mirroring `IAccountRepository`. Row: token hash, `accountId`, `expiresAt`,
  `revoked`. Stored server-side (unlike the stateless access JWT) so tokens can be **revoked**.
- **Rotation** — each refresh issues a new refresh token and invalidates the old one; a reused old token means
  theft, so revoke the chain.
- **Revoke on logout** (and optionally "revoke all for an account").

## Client side (the original motivation)

Once the endpoint exists, the Alpine client authenticates once, stores the access token, and on `401` calls
`/api/auth/refresh` then retries once. Refresh token ideally in an **httpOnly cookie** (JS can't read it, so XSS
can't steal it); access token in memory or localStorage. Blazor UIModule is out of scope for now.

## Timing / dependency

Pairs with the planned **ServiceModule rework** — collapse the three
ServiceModule projects and the DDD ceremony while **keeping the provider seam** (the repository interfaces +
per-provider implementations that make the DB swappable). A new `IRefreshTokenRepository` drops cleanly into
whatever shape that rework settles on, so it is cheaper to add during the rework than before it.

## Open questions

- **Browser storage of the refresh token** — httpOnly cookie (server sets it, needs CSRF handling) vs localStorage
  (simpler, more exposed).
- **Lifetimes** — access token in minutes, refresh token in days; pick concrete numbers.
- **Reuse detection** — how aggressively to revoke when an already-used refresh token is presented again.

## Related

- the ServiceModule doc (auth, token endpoint, rate-limiting tiers)
