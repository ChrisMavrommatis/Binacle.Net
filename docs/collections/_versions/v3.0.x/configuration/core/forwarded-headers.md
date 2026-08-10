---
title: Forwarded Headers
nav:
  parent: Core
  order: 2
  icon: 🌐
---

When something sits in front of Binacle.Net - a reverse proxy, a load balancer, a CDN, an ingress controller,
an SSH or `ngrok`-style tunnel - every request arrives from that thing's address, not the caller's. Forwarded
headers are how the real caller is put back.

Three parts of the app read the caller's address, and all three are wrong without this:

- **Health check IP restrictions** match the proxy instead of your monitoring system, so an allow-list can
  never match.
- **Rate limiting** puts every caller in one bucket, so one heavy client throttles everybody.
- **Logs** record your proxy's address on every line.

Nothing crashes when it is misconfigured, which is what makes it worth setting deliberately.

> **New in v3.0.0.** Earlier versions had no support for this.
{: .block-note}

## 🛠️ Configuration
Forwarded headers are configured via the `ForwardedHeaders.json` file.

**Default configuration:**
```json
{
  "ForwardedHeaders": {
    "Enabled": false,
    "TrustLoopback": true,
    "TrustPrivateNetworks": true,
    "TrustedProxies": [],
    "ForwardLimit": 1,
    "ForwardedForHeaderName": null
  }
}
```

You can modify the settings through **Production Overrides** by creating a `ForwardedHeaders.Production.json`
file, or by using **Environment Variables**.

- 📁 **Location**: `/app/Config_Files`
- 📌 **Full Path**: `/app/Config_Files/ForwardedHeaders.Production.json`

For more information on overriding configurations, refer to the
[Configuration Basics]({% link _common_pages/configuration-basics.md %}#%EF%B8%8F-overriding-configuration) page.

## 🔧 Configuration Options

- `Enabled` (_boolean_) - turns the feature on. **Off by default.**
- `TrustLoopback` (_boolean_, default `true`) - trust a proxy on the same machine (`127.0.0.0/8`, `::1`).
- `TrustPrivateNetworks` (_boolean_, default `true`) - trust a proxy on a private network:
  `10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`, `fc00::/7`.
- `TrustedProxies` (_array_, default empty) - any other address or range, named exactly. Added to what the two
  flags above already allow.
- `ForwardLimit` (_integer_, default `1`) - how many proxies stand in front of the app.
- `ForwardedForHeaderName` (_string?_, default `null`) - read a different header instead of `X-Forwarded-For`.

### Why it is off by default
With nothing in front of the app, the connection address **is** the caller's and cannot be forged. Reading the
headers would replace a fact with a value the caller chose. Turn it on only when something really is in front.

### Trust is explicit
A forwarded header is only believed when it comes from an address you trust. The three settings widen the trust
list in order: loopback, then private networks, then anything you name.

**Enabling the feature while trusting nothing fails startup.** Two empty trust lists make the underlying
middleware skip the check altogether, which would let every caller pick their own address. That state is
refused at boot rather than shipped.

The startup log prints the trust list it ended up with, so the flags are never opaque.

### How an entry is read {#entry-format}
Entries in `TrustedProxies` are read **exactly as written** - the same rule as health check `RestrictedIPs`. An
IPv4 address is four plain decimal parts, IPv6 is in its short lowercase form, and `/` is a CIDR prefix length.
Anything else fails startup validation instead of trusting a host you did not name.

The rule and the spellings it refuses are on the
[Health Checks]({% vlink /configuration/diagnostics-module/health-checks.md %}#entry-format) page.

## 📦 In a container

A container network is a private network, and `TrustPrivateNetworks` is on by default, so a proxy in the same
compose file or the same cluster is already trusted. Usually the only change you need is:

```json
{
  "ForwardedHeaders": {
    "Enabled": true
  }
}
```

Do not restate `10.0.0.0/8` and friends in `TrustedProxies` - the flag already covers them.

## ☁️ Behind a CDN or an external load balancer

Anything outside your own network has to be named:

```json
{
  "ForwardedHeaders": {
    "Enabled": true,
    "TrustedProxies": [ "203.0.113.0/24" ]
  }
}
```

Many CDNs also send their own single-value header instead of `X-Forwarded-For`. Point the app at it:

| Vendor header | Sent by |
|---|---|
| `CF-Connecting-IP` | Cloudflare |
| `X-Real-IP` | many nginx setups |
| `X-Azure-ClientIP` | Azure Front Door |

```json
{
  "ForwardedHeaders": {
    "Enabled": true,
    "ForwardedForHeaderName": "CF-Connecting-IP"
  }
}
```

Name a header nothing actually sends and the caller never resolves, so check what arrives before choosing.

## 🚇 Through a tunnel

A tunnel client (an SSH forward, `ngrok`, `cloudflared`) is a proxy like any other. It usually runs on
loopback or a private address, so it is already trusted; you still need `Enabled: true`.

## 🔍 Working out what your proxy sends

Two ways, cheapest first.

**Read the log.** The app **warns once** when a forwarding header arrived and did not take effect - either
because the feature is off, or because the trust list does not name the proxy. It is logged a single time per
start, not per request, so look near the first requests after a boot. It is the cheapest signal there is and it
names which of the two states you are in.

**Use `/_debug`.** Set `DEBUG_ENDPOINT=True`, call `/_debug` through your proxy, and read back the address the
app resolved and every header it received.

> `/_debug` echoes **every** header, including your `Authorization`. Turn it on, read what you need, turn it
> off. Do not leave it enabled where other people can reach it.
{: .block-warning}

## 🚫 `ASPNETCORE_FORWARDEDHEADERS_ENABLED` is ignored

The framework variable is deliberately not honoured. It switches the underlying middleware on **and empties
both trust lists**, which means any caller can send `X-Forwarded-For` and be believed. Setting it does nothing
here; use `Enabled` in the configuration file instead.
