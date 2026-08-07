# Full

**Everything switched on at once** - the interactive docs, the web UI demo, accounts and auth, health checks,
packing logs and the `/_debug` endpoint. It exists so you can see what every part of Binacle.Net does without
assembling a configuration first.

## Do not deploy this

`DEBUG_ENDPOINT` is on, which mounts `/_debug`. That endpoint echoes the caller's **entire request** back to
them - every header they sent, including `Authorization` - plus the connection address the app resolved. It is
on here because seeing it is the point. It must not be reachable by anyone you do not trust.

For a real deployment use [prod](../prod), or [service](../service) if you need accounts.

## Run it

```bash
docker compose up -d
```

| What | Where |
|---|---|
| Packing demo (web UI) | http://localhost:8080/ |
| Swagger UI | http://localhost:8080/swagger/ |
| Scalar UI | http://localhost:8080/scalar/ |
| Health | http://localhost:8080/_health |
| Request echo | http://localhost:8080/_debug |

Log in as `admin@binacle.net` / `B1n4cl3Adm!n`, seeded on first start. Both are in `docker-compose.yml` in
plain text, which is another reason this is not a deployment.

`/_health` lists which modules are on under `Features` - a quick way to confirm a flag did what you expected.

## What this maps to

`config/smoke/full.yml` runs this same configuration against the image on every release.
