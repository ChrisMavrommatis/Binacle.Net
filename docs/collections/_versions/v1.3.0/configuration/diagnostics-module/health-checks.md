---
title: Health Checks
nav:
  parent: Diagnostics Module
  order: 2
  icon: ❤️‍🩹
---


Health checks are vital for ensuring the reliability and availability of your application. In Binacle.Net, health checks offer a structured way to monitor system health and integrate seamlessly with external monitoring tools. Enabling health checks helps you to:

- ✅ Detect failures early to prevent service disruptions.
- 🚦 Integrate with load balancers to route traffic only to healthy instances.
- 📊 Monitor system health in real-time with external tools like Prometheus and Grafana.


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

You can modify the health check settings through **Production Overrides** by creating a `HealthChecks.Production.json` file, or by using **Environment Variables**.

- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/HealthChecks.Production.json`

For more information on overriding configurations, refer to the [Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.


## 🔧 Configuration Options
- `Enabled` (_boolean_) – Enables (`true`) or disables (`false`) health checks.
- `Path` (_string_) – The endpoint path that health checks respond to (default: `/_health`).
- `RestrictedIPs` (_array_) – Defines which IPs can perform health checks.
- `RestrictedChecks` (_array_) – Excludes specific health checks from execution.

## 🔒 Restricting Access
By default, health checks are publicly accessible. However, you can restrict access by specifying allowed IPs or subnets in the `RestrictedIPs` array:

```json
"RestrictedIPs": [
  "192.168.1.1",                // Single IP
  "192.168.1.0-192.168.1.255",  // IP Range
  "192.168.1.0/24",             // CIDR notation 
  "10.0.0.0/8"                  // Larger subnet range
]
```
This ensures that only authorized systems can query the health check endpoint.

## 🛠️ Built-in Checks
Binacle.Net comes with built-in health checks, including:

- ✅ **Database Check** – (Available with the **Service Module**) Verifies the health of the database connection.

You can disable specific health checks by listing them in the `RestrictedChecks` array. For example, to disable the **Database Check**, you can configure it like this:

```json
"RestrictedChecks": [
  "Database"
]
```

This feature is useful if you cannot restrict access using `RestrictedIPs` but still want to limit exposure to sensitive system details.
