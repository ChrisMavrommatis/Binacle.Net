---
title: Release Notes
nav:
  order: 2
  icon: 🛠️
---

Release notes for the **v2.0.x** line, newest release first. Every patch in this line is on this page.

> 🛡️ **The Service Module is exempt from these notes.** From v2.0.0 it is developed for the hosted service, so a
> breaking change to it is not documented here and does not force a major version increment. If you self-host
> with the Service Module enabled, read every release before upgrading - a minor or patch release can break it.
> Everything else on this page follows the usual rules. See the
> [Service Module]({% vlink configuration/service-module/index.md %}) page.
{: .block-note}

---

## v2.0.1

*Released 2 October 2025 - [release on GitHub](https://github.com/ChrisMavrommatis/Binacle.Net/releases/tag/v2.0.1)*

### 🔎 Overview
- Fixed an issue with OpenAPI resolving the servers automatically.

---

## v2.0.0

*Released 2 October 2025 - [release on GitHub](https://github.com/ChrisMavrommatis/Binacle.Net/releases/tag/v2.0.0)*

Binacle.Net v2.0.0 is a major update from v1.3.0. 

> v2.0.0 introduces breaking changes and new features. Existing integrations must be reviewed and updated.
{: .block-warning}

### 🔎 Overview
- **Service Module** was completely rewritten with new core business logic, breaking all existing integrations.
- **V1 endpoints** were removed.
- **V3 endpoints** were promoted from experimental to stable.
- **Algorithms** were improved, primarily impacting V3 endpoints.
- **Packing Logs** were updated, with some breaking changes for existing integrations.
- **API documentation** now follows the OpenAPI 3.0 specification.
- The project introduces **versioned documentation** with an official site.

### ⚙️ Core Changes
- Removal of all V1 endpoints.
- V3 endpoints are now stable and fully supported.
- Documentation migrated to the OpenAPI 3.0 specification (may affect users relying on the old format).
- Swagger UI upgraded to OpenAPI 3.0.
- Added support for **Scalar UI**.

### 🧪 Diagnostics Module
- Packing Logs removed **legacy fitting** and **legacy packing** modes.
- These have been consolidated into **packing** and **fitting**.
- Implementations depending on the old paths must be updated.
- The default log path changed.

### 🔌 Service Module
The **Service Module** was entirely rewritten and its business logic has been fundamentally changed.

- Originally designed to power **Binacle.Net as a Service**, the module will now be developed exclusively for that use case.
- While it remains part of the open-source project (and can still be self-hosted), **no public documentation will be provided**.
- This change reduces maintenance overhead and ensures focus remains on the core hosted product.

For additional details, see the [Service Module documentation]({% vlink configuration/service-module/index.md %}).

### 🎨 UI Module
- Vendor libraries are now bundled directly into the image rather than loaded from a CDN.
- Includes various fixes and general UI improvements.

### 📈 Algorithms
- Enhanced performance for **BFD** and **WFD** algorithms.

### 🏗️ Internal Work
- Migrated testing framework to **XUnit v3**.
- Renamed project structure from *Binacle.Net.Api* to simply *Binacle.Net*.

### 📚 Versioned Docs
- Launched an official documentation site, replacing the GitHub Wiki.
- Documentation is now properly versioned, preserving older versions for reference.
- Older versions will be gradually removed as they become obsolete. 

### 🛠️ Migration Guide
To upgrade to **v2.0.0**, follow these steps:

1. **Remove all V1 usage**
    - Any calls to V1 endpoints must be removed or migrated.

2. **Review log configuration**
    - All logs under `/app/data/logs` now use the `yyyyMMdd.ndjson` format instead of `log-yyyyMMdd.txt`.

3. **Switch to V2 or V3 endpoints**
    - Update integrations to use stable V2 or V3 endpoints.
    - Verify that algorithms behave correctly with the updated logic.

4. **Update Packing Logs usage**
    - Replace any use of `legacy-fitting` and `legacy-packing` in `/app/data/pack-logs` with the new `packing` and `fitting` paths.

5. **Service Module users**
    - All integrations with the old Service Module will no longer work.
    - No public documentation is available — please contact directly if needed.
    - For self-hosted setups, you will need to rely on the source code, as documentation will not be provided.

6. **Adopt new documentation**
    - If you previously relied on the legacy JSON documentation, migrate to OpenAPI 3.0.
    - Use Swagger UI or Scalar UI for interactive references.  
