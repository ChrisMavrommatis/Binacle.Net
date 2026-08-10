---
title: Health Checks
nav:
  parent: Diagnostics Module
  order: 2
  icon: ❤️‍🩹
---

Health checks are vital for ensuring the reliability and availability of your application.

In Binacle.Net, health checks offer a structured way to monitor system health and integrate seamlessly
with external monitoring tools. Enabling health checks helps you to:

- ✅ Detect failures early to prevent service disruptions.
- 🚦 Integrate with load balancers to route traffic only to healthy instances.
- 📊 Monitor system health in real-time with external tools like Prometheus and Grafana.

> **`RestrictedIPs` changed in v3.0.0 and the changes are breaking.** If you have an allow-list, read
> [Restricting Access](#-restricting-access) before upgrading - existing CIDR entries now cover far fewer
> addresses than they did, and some entries now fail startup.
{: .block-warning}

## 🛠️ Configuration
Health checks are configured via the `HealthChecks.json` file.

**Default configuration:**
```json
{
  "HealthChecks": {
    "Enabled": false,
    "Path": "/_health",
    "RestrictedIPs": [],
    "RestrictedChecks": []
  }
}
```

You can modify the health check settings through **Production Overrides** by creating a
`HealthChecks.Production.json` file, or by using **Environment Variables**.

- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/HealthChecks.Production.json`

For more information on overriding configurations, refer to the
[Configuration Basics]({% link _common_pages/configuration-basics.md %}#%EF%B8%8F-overriding-configuration) page.


## 🔧 Configuration Options
- `Enabled` (_boolean_) - Enables (`true`) or disables (`false`) health checks.
- `Path` (_string_) - The endpoint path that health checks respond to (default: `/_health`).
- `RestrictedIPs` (_array_) - Which callers may query the endpoint. Empty means everyone.
- `RestrictedChecks` (_array_) - Which checks run. Empty means all of them.

## 🔒 Restricting Access
By default, health checks are publicly accessible.
You can restrict access by listing the addresses allowed to query them:

```json
{
  "RestrictedIPs": [
    "192.168.1.5",
    "192.168.1.0/24",
    "10.0.0.0/8",
    "2001:db8::1"
  ]
}
```

An entry is either a single address or a CIDR range. An entry the app cannot read **fails startup** rather
than being quietly ignored - an allow-list that silently drops an entry is worse than one that refuses to boot.

### What changed in v3.0.0

Four changes, and the first one narrows lists that already exist.

**1. `/` is now a prefix length.** It was read as an address mask, which made `192.168.1.0/24` cover nearly the
whole IPv4 range. It now means what it means everywhere else: 256 addresses. **Existing CIDR entries are much
narrower than they were** - check that the hosts you expect are still inside them, or you will lock yourself
out.

**2. IPv4 callers now match inside a container.** The server listens on a dual-mode socket, so an IPv4 caller
arrives as an IPv4-mapped IPv6 address. Those are unmapped before matching now. Previously no IPv4 entry could
match one, which made the list unusable in a container.

**3. The `start-end` range form is gone.** `192.168.1.0-192.168.1.255` now fails startup validation. Write it
as `192.168.1.0/24`. A range that does not line up with a CIDR boundary has to be split into several entries,
or widened to the enclosing subnet.

**4. Entries are read exactly as written.** See below.

### How an entry is read {#entry-format}

An entry must read as the host it admits. The forms below were accepted before and quietly meant something
else; all of them now fail startup validation.

| Written | Used to mean | Now |
|---|---|---|
| `010.10.10.10` | `8.10.10.10` - read as octal | Rejected. Write `10.10.10.10` |
| `10.1` | `10.0.0.1` | Rejected. Write `10.0.0.1` |
| `167772161` | `10.0.0.1` | Rejected. Write `10.0.0.1` |
| `2001:0DB8::1` | `2001:db8::1` | Rejected. Write `2001:db8::1` |

So: an IPv4 address is four plain decimal parts with no leading zeros, and an IPv6 address is in its short,
lowercase form.

> **Check an existing list before you upgrade.** The old failure was admitting a host nobody named -
> `010.10.10.10` let `8.10.10.10` in. Look at what your entries actually matched, not only at what they were
> meant to match.
{: .block-caution}

The same rule applies to `TrustedProxies` in
[Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}).

### Host bits are masked off

`192.168.1.1/24` means the whole `192.168.1.0/24` block - all 256 addresses. That is what CIDR notation means
everywhere, and it is accepted. The startup log says what each entry resolved to, so an entry that covers more
than you intended is visible rather than a surprise.

### Behind a proxy

The list is compared against the address the request came from, which is your **proxy's** address until
forwarded headers resolve the real caller. Behind a proxy, load balancer or CDN, enable
[Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}) as well, or the list can never match
your monitoring system.

## 🛠️ Built-in Checks
Binacle.Net comes with these health checks:

- ✅ **System** - always present.
- ✅ **Database** - present only when the [Service Module]({% vlink /configuration/service-module/index.md %})
  is enabled. Verifies the health of the database connection.

`RestrictedChecks` is an **allow-list, not a skip-list**. When it is empty every check runs. When it is not
empty, **only** the checks it names run:

```json
{
  "RestrictedChecks": [
    "System"
  ]
}
```

That configuration runs the `System` check and nothing else. Use the registered name, as listed above.

This is useful when you cannot restrict access with `RestrictedIPs` but still want to limit what the endpoint
reveals.
