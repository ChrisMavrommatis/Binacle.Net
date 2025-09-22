---
title: Configuration
permalink: /version/latest/configuration/
nav:
  order: 6
  icon: 🔧
---

Binacle.Net is designed for flexibility, allowing you to enable only the features you need. Most functionality is
provided through modules, each with its own requirements, configuration options, and dependencies.

This guide covers the configuration system for {{ page.version }} version.

Make sure to read [Configuration Basics]({% link _common_pages/configuration-basics.md %}) first then proceed with the
specifics for this version.

---
## 📖 Table of Contents

- [📂 Configuration Files](#-configuration-files)
    - [📑 Directory Structure](#-directory-structure)

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
    │   └── Serilog.json
    └── UiModule
        └── ConnectionStrings.json
```

---
## 🔧 Binacle.Net Modules Overview

Each module adds functionality to Binacle.Net. This section provides an overview and links to detailed configuration
pages.

### 🏗️ Binacle.Net Core

The foundation of Binacle.Net. Provides essential API functionality, Swagger UI, and presets.

- [🔍 Core Overview]({% vlink /configuration/core/index.md %})
- [📖 Presets]({% vlink /configuration/core/presets.md %})

### 📊 Diagnostics Module

Handles system health monitoring, logging, and telemetry. This module is always enabled, but not all of its features
come enabled by default.

- [🔍 Diagnostics Module Overview]({% vlink /configuration/diagnostics-module/index.md %})
- [📜 Logging]({% vlink /configuration/diagnostics-module/logging.md %})
- [❤️‍🩹 Health Checks]({% vlink /configuration/diagnostics-module/health-checks.md %})
- [📦 Packing Logs]({% vlink /configuration/diagnostics-module/packing-logs.md %})
- [📡 OpenTelemetry]({% vlink /configuration/diagnostics-module/open-telemetry.md %})

### 🛡️ Service Module

Allows Binacle.Net to run as a managed service with authentication and rate limiting.

This module is primarily made for the official Binacle.Net cloud service.
It is still possible to enable it for self-hosted instances, but no documentation is provided for that use case.

Please refer to the [Service Module]({% vlink /configuration/service-module/index.md %}) page for more details.

### 🖥️ UI Module

Provides a web-based UI for packing demos and protocol decoding.

- [🔍 UI Module Overview](% vlink /configuration/ui-module/index.md %})


