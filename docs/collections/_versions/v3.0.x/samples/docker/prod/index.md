---
title: Prod
permalink: /version/v3.0.x/samples/docker/prod/
nav:
  order: 3
  parent: Docker
  icon: 3️⃣
---

**The API behind your own backend.** Health checks and packing logs on, and nothing else: no interactive docs,
no web UI, no `/_debug`, no accounts and no database.

This is what most deployments should be. Your server calls Binacle.Net, so it does not need users or JWT auth,
and every surface you do not switch on is one you do not have to defend. If callers you do not control reach
the API directly, use [Service]({% vlink /samples/docker/service/index.md %}) instead.

## 🛠️ Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://www.docker.com/get-started) (included with Docker Desktop)

## 📥 Download the following files
- [`docker-compose.yml`]({% vlink /samples/docker/prod/docker-compose.yml %}){:download="" target="_blank"}
- [`Presets.json`]({% vlink /samples/docker/prod/Presets.json %}){:download="" target="_blank"}
- [`ForwardedHeaders.json`]({% vlink /samples/docker/prod/ForwardedHeaders.json %}){:download="" target="_blank"} - only if you run behind a proxy
- [`OpenTelemetry.Production.json`]({% vlink /samples/docker/prod/OpenTelemetry.Production.json %}){:download="" target="_blank"} - only if you send traces somewhere

The last two are mounted behind comments in the compose file. Download them if you uncomment their mount.

## 🚀 Run it

```bash
docker compose up -d
```

The API answers on `http://localhost:8080`, and `http://localhost:8080/_health` reports whether it is up. There
is no browsable documentation in this configuration by design - the [API]({% vlink /api/index.md %}) pages
cover the endpoints.

## ✏️ Change this first

**`Presets.json` is your bin set.** The shipped file is an example. Replacing it with your own boxes, lockers
or pallets is the first thing an integrator does, and until you do the answers describe someone else's
packaging.

## 🌐 Behind a proxy, a load balancer or a CDN

Uncomment the `ForwardedHeaders.json` mount in `docker-compose.yml` and edit that file.

Without it the app treats your proxy as the caller. Nothing crashes, which is what makes it worth calling out:
health check IP restrictions match the proxy instead of the real client, rate limiting puts every caller in one
bucket, and every log line records your proxy's address.

`TrustPrivateNetworks` is on by default and already covers `10.0.0.0/8`, `172.16.0.0/12` and
`192.168.0.0/16`, so in a normal container deployment you only need to set `Enabled`.

If a CDN sits in front, set `ForwardedForHeaderName` to the header **it** sends - `CF-Connecting-IP` for
Cloudflare, `X-Real-IP` for many nginx setups, `X-Azure-ClientIP` for Azure Front Door.

The full settings are on the [Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}) page.

## 📡 Observability

Uncomment the `OpenTelemetry.Production.json` mount and edit the endpoint. The commented `aspire-dashboard`
service at the bottom of `docker-compose.yml` is the quickest thing to point it at while you are setting it up;
replace it with your real collector. Azure Monitor is the other exporter, in the same file.

See [OpenTelemetry]({% vlink /configuration/diagnostics-module/open-telemetry.md %}) for every setting.

## 💾 It is not stateless

Packing logs are written to `/app/data`, which is why there is a volume. Keep it, or the logs go when the
container does.

`PackingLogs__RetentionDays` deletes files older than N days once a day; leaving it unset keeps everything
forever, which fills a disk eventually. See [Packing Logs]({% vlink /configuration/diagnostics-module/packing-logs.md %}).

## 📄 Additional Resources
- [Docker Compose Reference](https://docs.docker.com/compose/)

Happy packing! 📦✨
