---
title: Diagnostics Module
permalink: /version/v1.3.0/configuration/diagnostics-module/
nav:
  parent: Configuration
  order: 2
  icon: 📊
---

The Diagnostics Module provides essential diagnostic features, including logging, health checks, and telemetry.
- This module is always active and cannot be disabled.
- Logging is enabled by default.
- Other features (health checks, OpenTelemetry, packing logs) require explicit opt-in.

## ⚙️ Configuration
All configuration files for the Diagnostics Module are located in the `/app/Config_Files/DiagnosticsModule` directory.

### 📑 Directory Structure
```text
app
└── Config_Files
    └── DiagnosticsModule
        ├── HealthChecks.json
        ├── OpenTelemetry.json
        ├── PackingLogs.json
        └── Serilog.json
```

## 🛠️ Logging
Binacle.Net utilizes Serilog for logging.

For setup and customization, see the [Logging]({% vlink /configuration/diagnostics-module/logging.md %}#%EF%B8%8F-overriding-configuration) page.

## 🩺 Health Checks
Health Checks monitor the status of Binacle.Net, ensuring reliability.

For more information, refer to the [Health Checks]({% vlink /configuration/diagnostics-module/health-checks.md %}) page.

## 📦 Packing Logs
Packing Logs track API usage by logging requests and their corresponding results for later analysis.

For detailed information on configuring and using packing logs, see the [Packing Logs]({% vlink /configuration/diagnostics-module/packing-logs.md %}) page.

## 📊 OpenTelemetry
OpenTelemetry enables distributed tracing, metrics collection, and logging to provide comprehensive insights into your application's performance.

To integrate and configure OpenTelemetry for these features, visit the [OpenTelemetry]({% vlink /configuration/diagnostics-module/open-telemetry.md %}) page.