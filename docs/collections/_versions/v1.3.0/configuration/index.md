---
title: Configuration
permalink: /version/v1.3.0/configuration/
nav:
  order: 6
  icon: 🔧
---

Binacle.Net is designed for flexibility, allowing you to enable only the features you need. Most functionality is provided through modules, each with its own requirements, configuration options, and dependencies.

This guide covers the configuration system, including file-based settings, environment variables, and overrides, so you can tailor Binacle.Net to your specific needs.

## 📖 Table of Contents
- [📂 Configuration Files](#-configuration-files)
    - [📑 Directory Structure](#-directory-structure)
- [⚙️ Overriding Configuration](#%EF%B8%8F-overriding-configuration)
    - [📄 Direct File Edits](#-direct-file-edits)
    - [📝 Using Production Overrides](#-production-overrides)
    - [🌍 Using Environment Variables](#-environment-variables)
    - [🔄 Connection String Fallbacks](#-connection-string-fallbacks)
- [⚖️ Configuration Precedence](#%EF%B8%8F-configuration-precidence)
- [🔧 Modules Overview](#-binaclenet-modules-overview)
    - [🏗️ Core](#%EF%B8%8F-binaclenet-core)
    - [📊 Diagnostics Module](#-diagnostics-module)
    - [🛡️ Service Module](#%EF%B8%8F-service-module)
    - [🖥️ UI Module](#%EF%B8%8F-ui-module)

---

## 📂 Configuration Files
All configuration files are located in `/app/Config_Files`.

### 📑 Directory Structure
```text
app
└── Config_Files
    ├── Presets.json
    ├── DiagnosticsModule
    │   ├── HealthChecks.json
    │   ├── OpenTelemetry.json
    │   ├── PackingLogs.json
    │   ├── Serilog.json
    ├── ServiceModule
    │   ├── ConnectionStrings.json
    │   ├── JwtAuth.json
    │   ├── RateLimiter.json
    │   └── Users.json
    └── UiModule
        └── ConnectionStrings.json
```

## ⚙️ Overriding Configuration
Binacle.Net allows multiple ways to override configuration settings. The recommended approach depends on your use case:
- 🔹 **Environment Variables** – Highest priority, ideal for secrets and dynamic configurations.
- 📝 **Production Overrides** (`<filename>.Production.json`) – Override settings without modifying the default configuration.
- 📄 **Direct File Edits** – Modify configuration files directly (use bind mounts in Docker or volumes in Kubernetes).
- 🔄 **Connection String Fallbacks** – A dedicated method for defining connection strings.

### 🔍 Example Configuration
For demonstration, we will use the following `Settings.json`:
```json
{
  "Settings": {
    "Enabled": false,
    "DataFolderPath": "/data",
    "Logs": {
      "FileFormat": "dd-MM-yyyy.txt",
      "Retention": 4
    }
  }
}
```

### 📄 Direct File Edits
You can manually modify configuration files or replace them entirely using:

- **Docker**: Bind mounts (`-v /host/path:/container/path`)
- **Kubernetes**: Volumes (e.g., `hostPath` or `ConfigMaps`)

> [!Warning]
> Not recommended for production.
>
> This approach overrides all default settings, which may lead to unexpected behavior.
> Only use this if you want a completely custom configuration.

### 📝 Production Overrides
AKA: `<filename>.Production.json`

Instead of modifying `Settings.json` directly, create a `Settings.Production.json` file with only the necessary changes:
```json
{
  "Settings": {
    "Enabled": true,
    "Logs": {
      "Retention": 5
    }
  }
}
```
Place this file in the same directory as `Settings.json`.

### 🌍 Environment Variables
Environment variables take precedence over configuration files and are recommended for secrets and dynamic settings.

To override a setting, replace nested properties with double underscores (`__`):

```bash
Settings__Enabled=True
Settings__Logs__Retention=5
```

### 🔄 Connection String Fallbacks
Connection strings can be configured using dedicated environment variables that act as a fallback mechanism. This is the recommended way for connection strings when they contain sensitive credentials.

- 🔹 **Format**: Uppercase connection name + `_CONNECTION_STRING`
- 🔹 **Example**: `DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413`


## ⚖️ Configuration Precidence
When multiple configuration methods define the same setting, the following priority order applies (higher wins):

| Order | Method                     | Example Setting <br> (`Logs.Retention`) | Example Connection String <br> (`ConnectionStrings.Database`) |
|-------|----------------------------|-----------------------------------------|---------------------------------------------------------------|
| 1     | Environment Variable       | `Settings__Logs__Retention=5`           | `ConnectionStrings__Database=endpoint=https://localhost:1413` |
| 2     | Production Override        | `Settings.Production.json`              | `ConnectionStrings.Production.json`                           |
| 3     | Direct File Edit           | `Settings.json`                         | `ConnectionStrings.json`                                      |
| 4     | Connection String Fallback | N/A                                     | `DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413`  |
---

## 🔧 Binacle.Net Modules Overview
Each module adds functionality to Binacle.Net. This section provides an overview and links to detailed configuration pages.

### 🏗️ Binacle.Net Core
The foundation of Binacle.Net. Provides essential API functionality, Swagger UI, and presets.
- [🔍 Core Overview]({% vlink /configuration/core/index.md %})
- [📖 Presets]({% vlink /configuration/core/presets.md %})


### 📊 Diagnostics Module
Handles system health monitoring, logging, and telemetry. This module is always enabled, but not all of its features come enabled by default.
- [🔍 Diagnostics Module Overview]({% vlink /configuration/diagnostics-module/index.md %})
- [📜 Logging]({% vlink /configuration/diagnostics-module/logging.md %})
- [❤️‍🩹 Health Checks]({% vlink /configuration/diagnostics-module/health-checks.md %})
- [📦 Packing Logs]({% vlink /configuration/diagnostics-module/packing-logs.md %})
- [📡 OpenTelemetry]({% vlink /configuration/diagnostics-module/open-telemetry.md %})

### 🛡️ Service Module
Allows Binacle.Net to run as a managed service with authentication and rate limiting.
- [🔍 Service Module Overview](./service-module/)
- [🗄️ Database](./service-module/database/)
- [🔐 Authentication](./service-module/authentication/)
- [👥 Users](./service-module/users/)
- [📉 Rate Limiter](./service-module/rate-limiter/)

### 🖥️ UI Module
Provides a web-based UI for packing demos and protocol decoding.
- [🔍 UI Module Overview](./ui-module/)