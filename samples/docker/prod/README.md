# Prod

**The API behind your own backend.** Health checks and packing logs on, and nothing else: no interactive docs,
no web UI, no `/_debug`, no accounts and no database.

This is what most deployments should be. Your server calls Binacle.Net, so it does not need users or JWT auth,
and every surface you do not switch on is one you do not have to defend. If you need accounts, tokens and rate
limiting - because other people call it directly - use the [service](../service) sample instead.

## Run it

```bash
docker compose up -d
```

The API answers on `http://localhost:8080`, and `http://localhost:8080/_health` reports whether it is up. There
is no browsable documentation in this configuration by design; the OpenAPI documents for the version you are
running are on the documentation site.

## Change this first

**`Presets.json` is your bin set.** The shipped file is an example. Replacing it with your own boxes, lockers
or pallets is the first thing an integrator does, and until you do the answers describe someone else's
packaging.

## Behind a proxy, a load balancer or a CDN

Uncomment the `ForwardedHeaders.json` mount in `docker-compose.yml` and edit that file.

Without it the app treats your proxy as the caller. Nothing crashes - which is what makes it worth calling out.
Health check IP restrictions match the proxy instead of the real client, rate limiting puts every caller in one
bucket, and every log line records your proxy's address. `TrustPrivateNetworks` is on by default and already
covers `10/8`, `172.16/12` and `192.168/16`, so in a normal container deployment you only need to set `Enabled`.

If a CDN sits in front, set `ForwardedForHeaderName` to the header **it** sends - `CF-Connecting-IP` for
Cloudflare, `X-Real-IP` for many nginx setups, `X-Azure-ClientIP` for Azure Front Door. Trusting a header
nothing sets means the caller never resolves.

| Setting | What it means |
|---|---|
| `Enabled` | Off by default. With nothing in front of the app, the connection address is already the caller's and cannot be forged, so reading the headers would only replace it with a value the caller controls |
| `TrustLoopback` | A proxy on the same machine. Nothing outside the host can present a loopback address, so this costs nothing |
| `TrustPrivateNetworks` | A proxy on a container or local network - `10/8`, `172.16/12`, `192.168/16`. Usually all you need. **Do not restate these ranges in `TrustedProxies`** |
| `TrustedProxies` | Anything else, named exactly - a CDN's published ranges, or a balancer outside your network. Added to what the flags above allow |
| `ForwardLimit` | How many proxies stand in front. Entries beyond this are ignored, so padding the header cannot push the result further back than your real topology |
| `ForwardedForHeaderName` | `null` means `X-Forwarded-For`. Name your CDN's single-value header instead if it sends one |

Entries in `TrustedProxies` are read **exactly as written**: four plain decimal parts for IPv4, short lowercase
form for IPv6, and `/` means a CIDR prefix length. An entry that does not parse fails startup rather than being
quietly ignored, and enabling the feature with nothing trusted at all refuses to start.

The app also **warns once in the log** when a forwarding header arrives and does not take effect - either the
feature is off, or your proxy is not in the trust list. Check there first.

## Observability

Uncomment the `OpenTelemetry.Production.json` mount and edit the endpoint. The commented `aspire-dashboard`
service at the bottom of `docker-compose.yml` is the quickest thing to point it at while you are setting it up;
replace it with your real collector. Azure Monitor is the other exporter, in the same file.

Telemetry being off is not a security decision - it is a "no collector yet" one. A production deployment with
no observability is harder to run, not safer.

## It is not stateless

Packing logs are written to `/app/data`, which is why there is a volume. Keep it or the logs go when the
container does. `PackingLogs__RetentionDays` deletes files older than N days once a day; leaving it unset keeps
everything forever, which fills a disk eventually.

## Tested on every release

This configuration is smoke-tested against the image on every release, so it is a shape that is checked
rather than one nobody runs.
