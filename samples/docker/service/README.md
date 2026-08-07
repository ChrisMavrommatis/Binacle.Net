# Service

**Binacle.Net offered to other people.** ServiceModule on, so accounts, JWT authentication and per-caller rate
limiting, with a database behind it. Plus the interactive docs, health checks and packing logs.

Use this when callers you do not control reach the API directly. If your own backend is the only caller, you do
not need any of it - use [prod](../prod), which has no accounts, no auth and no database.

The web UI and `/_debug` are deliberately off. Other people can reach this deployment, and `/_debug` echoes the
caller's whole request back including their `Authorization` header. The docs stay on because they are
documentation, not a debug surface.

## Run it

```bash
docker compose up -d
```

Get a token, then call the API with it:

```bash
curl -X POST http://localhost:8080/api/auth/token \
  -H 'Content-Type: application/json' \
  -d '{"Username":"admin@binacle.net","Password":"B1n4cl3Adm!n"}'
```

## Change these before anyone else can reach it

1. **`JwtAuth.json`** - `TokenSecret` is a placeholder and it signs every token. Anyone with it can mint one.
2. **`BINACLE_ADMIN_CREDENTIALS`** - the first admin, seeded once on first start, in plain text in
   `docker-compose.yml`. Change it, then change the password again after logging in.
3. **`Presets.json`** - your bin set. The shipped file is an example.

## Choosing a database

Storage providers are tried in order - Azure Storage, Postgres, SQLite - and **the first one with a connection
string wins**, so set exactly one.

SQLite is active in this sample: it needs nothing else running and keeps the database in the mounted `data`
folder. It is a real choice for a single instance.

Postgres and Azure Storage are commented in `docker-compose.yml`. Both point at infrastructure you already run
- a production deployment should not start its own database in the same compose file, and neither commented
line does. For Postgres the host is your server's name, not `localhost`, which inside a container means the
container itself
## CORS

Only needed when a **browser** calls this API directly. `Cors.json` is not in the image - you supply it, and
nothing is allowed through until you do, which is a valid closed default. Uncomment the mount and list your
origins with exact scheme, host and port.

## Behind a proxy

Copy `ForwardedHeaders.json` from the [prod](../prod) sample and uncomment the mount. It matters more here than
anywhere else: without it, rate limiting partitions on your proxy's address rather than the caller's, so one
heavy client exhausts the bucket for everybody.

## What this maps to

`config/smoke/service.yml` runs this same configuration against the image on every release.
