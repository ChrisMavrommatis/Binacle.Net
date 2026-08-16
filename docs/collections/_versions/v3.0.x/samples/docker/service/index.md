---
title: Service
permalink: /version/v3.0.x/samples/docker/service/
nav:
  order: 4
  parent: Docker
  icon: 4️⃣
---

**Binacle.Net offered to other people.** The Service Module is on, so accounts, JWT authentication and
per-caller rate limiting, with a database behind it. Plus the interactive docs, health checks and packing logs.

Use this when callers you do not control reach the API directly. If your own backend is the only caller you do
not need any of it - use [Prod]({% vlink /samples/docker/prod/index.md %}), which has no accounts, no auth and
no database.

The web UI and `/_debug` are deliberately off. Other people can reach this deployment, and `/_debug` echoes the
caller's whole request back including their `Authorization` header. The docs stay on, since they expose nothing
about the caller.

> The Service Module has **no public documentation** - see the
> [Service Module]({% vlink /configuration/service-module/index.md %}) page for why. This sample is a working
> starting point.
{: .block-note}

## 🛠️ Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)

## 📥 Download the following files
- [`docker-compose.yml`]({% vlink /samples/docker/service/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/service/Presets.json %}){:download="" target="_blank"}
- [`JwtAuth.json`]({% vlink /samples/docker/service/JwtAuth.json %}){:download="" target="_blank"}
- [`Cors.json`]({% vlink /samples/docker/service/Cors.json %}){:download="" target="_blank"} - only if a browser calls the API directly

## 🚀 Run it

```bash
docker compose up -d
```

Get a token, then call the API with it:

```bash
curl -X POST http://localhost:8080/api/auth/token \
  -H 'Content-Type: application/json' \
  -d '{"Username":"admin@binacle.net","Password":"B1n4cl3Adm!n"}'
```

## 🔐 Change these before anyone else can reach it

1. **`JwtAuth.json`** - `TokenSecret` is a placeholder and it signs every token. Anyone with it can mint one.
2. **`BINACLE_ADMIN_CREDENTIALS`** - the first admin, seeded once on first start, in plain text in
   `docker-compose.yml`. Change it, then change the password again after logging in.
3. **`Presets.json`** - your bin set. The shipped file is an example.

## 🗄️ Choosing a database

Storage providers are tried in order - Azure Storage, Postgres, SQLite - and **the first one with a connection
string wins**, so set exactly one.

SQLite is active in this sample: it needs nothing else running and keeps the database in the mounted `data`
folder. It is a real choice for a single instance.

Postgres and Azure Storage are commented in `docker-compose.yml`. Both point at infrastructure you already run
- a production deployment should not start its own database in the same compose file. For Postgres the host is
your server's name, not `localhost`, which inside a container means the container itself.

## 🔑 Keep the key ring

Mount a volume at `/home/app/.aspnet/DataProtection-Keys` as well. Without it the keys live only inside the
container, so replacing the container makes anything protected with them unreadable. The container warns about
this on every boot.

## 🌍 CORS

Only needed when a **browser** calls this API directly. `Cors.json` is not in the image - you supply it, and
nothing is allowed through until you do, which is a valid closed default. Uncomment the mount and list your
origins with exact scheme, host and port.

## 🌐 Behind a proxy

Copy `ForwardedHeaders.json` from the [Prod]({% vlink /samples/docker/prod/index.md %}) sample and uncomment the
mount. It matters more here than anywhere else: without it, rate limiting partitions on your proxy's address
rather than the caller's, so one heavy client exhausts the bucket for everybody. See
[Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}).

## 📄 Additional Resources
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
