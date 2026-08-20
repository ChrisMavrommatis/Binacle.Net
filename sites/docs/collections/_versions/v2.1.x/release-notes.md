---
title: Release Notes
nav:
  order: 2
  icon: 🛠️
---

Release notes for the **v2.1.x** line, newest release first. Every patch in this line is on this page.

> Upgrading from v2.0.x? Nothing in this line breaks an integration, but v2.0.0 itself did - see the
> [v2.0.x release notes]({{ '/version/v2.0.x/release-notes/' | relative_url }}) if you are coming from v1.
{: .block-note}

> 🛡️ **The Service Module is exempt from these notes.** From v2.0.0 it is developed for the hosted service, so a
> breaking change to it is not documented here and does not force a major version increment. If you self-host
> with the Service Module enabled, read every release before upgrading - a minor or patch release can break it.
> Everything else on this page follows the usual rules. See the
> [Service Module]({% vlink configuration/service-module/index.md %}) page.
{: .block-note}

---

## v2.1.1

*Released 12 January 2026 - [release on GitHub](https://github.com/ChrisMavrommatis/Binacle.Net/releases/tag/v2.1.1)*

### 🔎 Overview
- Internal refactoring.
- Removed Postman metadata from API documentation.
- Updated packages.

### ⚙️ Core Changes
- Removed external dependencies on `ChrisMavrommatis.Features` and `ChrisMavrommatis.StartupTasks` by implementing internal versions.
- Consolidated logging (`ChrisMavrommatis.Logging`) and testing (`ChrisMavrommatis.Shouldly`) utilities into kernel modules and removed the `/dep` folder.
- Migrated the solution file to the new Visual Studio `.slnx` format.
- Updated NuGet packages to the latest stable versions.
- Removed Postman related metadata from the API documentation files.

---

## v2.1.0

*Released 3 December 2025 - [release on GitHub](https://github.com/ChrisMavrommatis/Binacle.Net/releases/tag/v2.1.0)*

### 🔎 Overview
- Upgraded to .NET 10.
- Added CORS support for the API.
- Various fixes and improvements.

### ⚙️ Core Changes
- Upgraded to **.NET 10**.
- Added **CORS** support for main API endpoints. It requires setup and is off until configured.
- Fixed various spelling errors.

### 🎨 UI Module
- Improved performance for the visualizer.
- Updated the license URL and target in the footer.
- Added a cache for the new Docker version badge.
- Added a badge for **Scalar**.
- The Scalar and Swagger badges show only when those are enabled.
