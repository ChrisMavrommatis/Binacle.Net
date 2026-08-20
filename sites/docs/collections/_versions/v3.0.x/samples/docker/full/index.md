---
title: Full
permalink: /version/v3.0.x/samples/docker/full/
nav:
  order: 5
  parent: Docker
  icon: 5️⃣
---

**Everything switched on at once** - the interactive docs, the web UI demo, accounts and auth, health checks,
packing logs and the `/_debug` endpoint. It exists so you can see what every part of Binacle.Net does without
assembling a configuration first.

> **Do not deploy this.** `DEBUG_ENDPOINT` is on, which mounts `/_debug`. That endpoint echoes the caller's
> **entire request** back to them - every header they sent, including `Authorization` - plus the address the
> app resolved them to. It is on here because seeing it is the point. It must not be reachable by anyone you do
> not trust.
{: .block-warning}

For a real deployment use [Prod]({% vlink /samples/docker/prod/index.md %}), or
[Service]({% vlink /samples/docker/service/index.md %}) if you need accounts.

## 🛠️ Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)

## 📥 Download the following files
- [`docker-compose.yml`]({% vlink /samples/docker/full/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/full/Presets.json %}){:download="" target="_blank"}
- [`JwtAuth.json`]({% vlink /samples/docker/full/JwtAuth.json %}){:download="" target="_blank"}

## 🚀 Run it

```bash
docker compose up -d
```

| What | Where |
|---|---|
| Packing demo (web UI) | `http://localhost:8080/` |
| Swagger UI | `http://localhost:8080/swagger/` |
| Scalar UI | `http://localhost:8080/scalar/` |
| Health | `http://localhost:8080/_health` |
| Request echo | `http://localhost:8080/_debug` |

Log in as `admin@binacle.net` / `B1n4cl3Adm!n`, seeded on first start. Both are in `docker-compose.yml` in
plain text, which is another reason this is not a deployment.

`/_health` lists which modules are on under `Features` - a quick way to confirm a flag did what you expected.

## 📄 Additional Resources
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
