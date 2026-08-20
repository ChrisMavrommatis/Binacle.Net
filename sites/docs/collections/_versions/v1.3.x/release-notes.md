---
title: Release Notes
nav:
  order: 4
  icon: 🛠️
---

Release notes for the **v1.3.x** line, newest release first. Every patch in this line is on this page.

---

## v1.3.0

*Released 27 March 2025 - [release on GitHub](https://github.com/ChrisMavrommatis/Binacle.Net/releases/tag/v1.3.0)*

This release introduces **OpenTelemetry** for improved observability, removes **Application Insights**, enhances
logging, and restructures internal components for better maintainability.

> v1.3.0 removes the Application Insights integration. If you relied on it, telemetry stops arriving until you
> migrate.
{: .block-warning}

### 🔎 Overview
- **OpenTelemetry** - fully implemented, replacing the previous incomplete version, now supporting export via OTLP Exporter and Azure Monitor.
- **Packing Logs** - added logs to track API usage for analytics and data gathering.
- **Service Module Logging** - enhanced logging in the Infrastructure layer to display a clear message when no repository is configured.
- **UI Module Fix** - resolved an issue where sample data did not reset correctly when new data was entered.
- **Docker Image Update** - added `.dockerignore` to exclude development configuration files from the Docker image.

### 🚨 Breaking Changes

**Application Insights removed.** OpenTelemetry has replaced the previous Application Insights integration.

- **Action required**: migrate to **Azure Monitor** or another OpenTelemetry-compatible tool to keep receiving telemetry.
- The previous integration using Serilog and the old SDK has been removed.

### 🧪 Diagnostics Module
- OpenTelemetry has fully replaced Application Insights, supporting export via OTLP Exporter and Azure Monitor.
  See [OpenTelemetry]({% vlink configuration/diagnostics-module/open-telemetry.md %}).
- Added Binacle Service Name and Version to logs for better traceability.
- Introduced Packing Logs to track API usage for analytics and data gathering.
  See [Packing Logs]({% vlink configuration/diagnostics-module/packing-logs.md %}).

### 🔌 Service Module
- Enhanced logging in the Infrastructure layer to ensure clearer messaging when no repository is configured.
- Improved error handling during initialization, ensuring clear error messages when the Service Module is not properly configured.

### 🎨 UI Module
- Fixed a bug where sample data persisted instead of being properly reset when new data was entered.

### 🧩 Miscellaneous
- Implemented Diagnostics for internal tracking using Activity Sources, improving telemetry and debugging capabilities.
- Added a `.dockerignore` file to exclude development configuration files from being included in the Docker image.

### 🏗️ Internal Work
- Moved configuration files to their respective modules, ensuring they are correctly copied during the build process.
- Addressed compiler warnings and formatted empty types for consistency and maintainability.
- Enhanced extension methods to simplify configuration setup.
- Restricted internal code visibility by moving public code to internal where appropriate.
- Integrated Aspire for local development, streamlining observability.
- Fixed issues with local tooling for Docker, improving the development experience.
- Restructured the project to improve support for Docker-based samples, including renaming, relocating components, and adding new features.

### 🛠️ Upgrade Notes
- **Action required**: if you were using Application Insights, you must migrate to Azure Monitor or another OpenTelemetry-compatible tool, as Application Insights is no longer supported.
- No performance or security changes are included in this release, but internal optimizations have been made to improve maintainability and stability.
