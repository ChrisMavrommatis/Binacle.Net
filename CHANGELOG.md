# Changelog

## [Unreleased]

Binacle.Net v3.0.0 is a major update from v2.1.1.

> [!Warning]
> **v3.0.0 introduces breaking changes. Existing integrations must be reviewed and updated. V2 endpoints are removed, ViPaq strings from earlier versions no longer decode, and health check IP restrictions are matched differently.**

---

### 🔎 Overview
- **V2 endpoints** were removed.  
- **V4 endpoints** were introduced as experimental.  
- **V3 endpoints** remain stable and unchanged, and are the recommended version.  
- **ViPaq** was rebuilt with a smaller, simpler format. Strings from earlier versions no longer decode.  
- **ViPaq** left experimental status — the format is stable as of this release.  
- **Algorithms** were unified — fitting and packing now share one implementation.  
- **Packing Logs** configuration was flattened, with breaking changes for existing integrations.  
- **Forwarded headers** are now supported, so the real caller is resolved when running behind a proxy or CDN.  
- **Health check IP restrictions** are matched differently, with breaking changes for existing allow-lists.  
- **The image creates `/app/data`** and gives it to the app user, so a volume mounted there is writable.  
- **The image is signed**, and carries an SBOM and build provenance, so you can verify what you pull.  
- **The image is about a third smaller** — it uses the .NET runtime from its base image instead of bundling a second copy.  
- The project was **restructured**, separating the API, library, and ViPaq into their own roots.  
- **Versioned documentation** now covers every minor line, so older images keep their docs.  

### ⚙️ Core Changes
- Removal of all V2 endpoints.  
- Added **16 experimental V4 endpoints**, covering everything V3 does.  
- V4 splits a request into three shapes. **One bin, one answer** — `fit/bin`, `pack/bin`, and their `{preset}/{bin}` variants.  
- **Many bins, one answer** — `pack/smallest-bin`, `pack/smallest-bin/{preset}`, `fit/smallest-bin`, and `fit/smallest-bin/{preset}` return the smallest bin that works; `pack/best-bin` and `pack/best-bin/{preset}` return the bin the items fill the most.  
- **Many bins, every answer** — `fit/compare-bins`, `pack/compare-bins`, and their `{preset}` variants return one result per bin, in the order the bins were sent.  
- Presets can be **listed** with `presets` or **fetched one at a time** with `presets/{preset}`.  
- V4 is **experimental and can change at any time**. V3 remains stable and is the recommended version.  
- V3 endpoints are unchanged and remain stable, apart from the ViPaq payload.  
- **ViPaq is no longer experimental.** The format is settled as of this release, where it carried an experimental warning through v2.1.1. A future format change takes a new `Version` code rather than altering the current one, so an older decoder rejects a newer string outright instead of misreading it.  
- Added **forwarded headers** support, configured in `Config_Files/ForwardedHeaders.json`. **Disabled by default.**  
- When enabled, the caller's address and scheme are resolved from `X-Forwarded-For` and `X-Forwarded-Proto` before anything reads them, so rate limiting and health check IP restrictions see the real caller rather than the proxy.  
- Trust is explicit — a proxy on loopback or a private network is trusted by default, anything else must be named. The app **refuses to start** if nothing is trusted, because that would make every caller's header believable.  
- A different header can be read instead, for CDNs that send one — `CF-Connecting-IP`, `X-Real-IP`, `X-Azure-ClientIP`.  
- `ASPNETCORE_FORWARDEDHEADERS_ENABLED` is **ignored**. It switches the underlying middleware on with no proxy verification, which lets any caller choose their own address.  
- `TrustedProxies` entries are **read exactly as written**, the same rule as health check `RestrictedIPs`. `010.10.10.10` used to be read as octal and trust `8.10.10.10`, and `172.17.1` used to mean `172.17.0.1`; both now fail startup validation rather than trusting a host you did not name.  
- Added a **`/_debug` endpoint**, off by default, enabled with `DEBUG_ENDPOINT=True`. It echoes the caller's own request — connection address and headers — for working out what a proxy is sending.  
- A **startup warning** when a forwarding header arrives and does not take effect, either because the feature is off or because the trust list does not name your proxy. Logged once. Without it both states are silent and the app quietly reads the proxy as the caller.  
- **The image now creates `/app/data` and gives it to the app user.** A volume mounted there is writable with no extra setup. Previously docker created the mount point as root, the app does not run as root, and packing logs and the SQLite database could not be written to a fresh named volume.  
- The image ships `libgssapi-krb5-2`, so Npgsql stops printing `Cannot load library libgssapi_krb5.so.2` at every start. Nothing was broken — the app authenticates with a password, not Kerberos — but the message read like a fatal error.  
- The image carries **OCI labels** — title, description, source, url, documentation, vendor, licence and base image — plus version, revision and created per build.  
- **The image is signed, and ships an SBOM and build provenance.** Signing is keyless, so there is no public key to fetch — the signature is checked against the workflow that produced it — and it covers the digest, so it holds for every tag pointing at that image:

  ```bash
  cosign verify binacle/binacle-net:3.0.0 \
    --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
    --certificate-oidc-issuer https://token.actions.githubusercontent.com
  ```

  The SPDX SBOM and SLSA provenance travel inside the image index; `docker buildx imagetools inspect binacle/binacle-net:3.0.0` lists them.  
- **The image is smaller — around 103 MB, where the same image built the old way was 150 MB.** The app is published framework-dependent, so it runs on the .NET runtime already in the `aspnet:10.0` base image instead of carrying a second copy of it. Nothing about running the container changes.  
- Existing environment variables are unchanged.  

### 🧪 Diagnostics Module
- Packing Logs configuration was **flattened** — `Path`, `FileName`, `DateFormat`, and `ChannelLimit` now sit directly under `PackingLogs`.  
- Removed the **fitting** configuration block, now that fitting and packing share one log.  
- Implementations depending on the old nested shape must be updated, or startup validation will fail.  
- The default log path changed from `data/pack-logs/packing/` to `data/pack-logs/`.  
- Packing log entries now include a `Timestamp` field.  
- Added **`RetentionDays`** to `PackingLogs`. When set, packing log files older than that many days are deleted once a day, and each deletion is logged. **Off by default** (`null`) — files are kept until you remove them yourself. Only files matching the configured `FileName` pattern in the configured `Path` are touched, and only at the top level.  
- Health check **`RestrictedIPs` now uses CIDR notation correctly**. The value after `/` was previously read as an address mask, so `192.168.1.0/24` covered nearly the whole IPv4 range instead of 256 addresses. Existing CIDR entries are now **much narrower** than they were.  
- Health check `RestrictedIPs` now matches **IPv4 callers in containers**. Addresses arriving in IPv4-mapped IPv6 form are unmapped before comparison, which they previously were not — no IPv4 entry could match.  
- Removed the **`start-end` range form** from `RestrictedIPs`. Entries such as `192.168.1.0-192.168.1.255` now fail startup validation. Use CIDR instead.  
- `RestrictedIPs` entries are now **read exactly as written**. An IPv4 address must be four plain decimal parts with no leading zeros, and an IPv6 address must be in its short, lowercase form. `010.10.10.10` used to be read as octal and admit `8.10.10.10`; `10.1` used to mean `10.0.0.1`; `167772161` meant the same. All of these now fail startup validation instead of quietly admitting a host you did not name. `192.168.1.1/24` still means the whole `192.168.1.0/24` — that is what CIDR notation means — but the startup log now says so.  

### 🔌 Service Module
- The Service Module is **exempt from these notes** — since v2.0.0 it is developed for the hosted service, so a change to it is not documented here and does not force a major version. If you self-host with `SERVICE_MODULE` enabled, read the full changelog before upgrading. One fix is worth calling out on its own:  
- **The auth token rate limit no longer partitions on a caller-supplied header.** It partitions on the connection's remote address, which forwarded headers resolve to the real caller wherever a proxy is trusted. Before this, varying the header reset your own login throttle.  

### 🎨 UI Module
- The Protocol Decoder reads the **new ViPaq format only**. Strings from earlier versions are rejected.  

### 📈 Algorithms
- **Fitting and packing now share one algorithm.** Fitting stops early on the first item that does not fit.  
- Packing results are unchanged — the shared algorithm is the previous packing implementation.  
- The separate fitting algorithm family was retired.  

### 🏗️ Internal Work
- Restructured the repository — the API, library, ViPaq, and shared test data now live in their own roots.  
- Extracted **Binacle.Geometry** into its own library.  
- Reworked the packing log pipeline, moving the generic parts into the Kernel.  
- Added benchmark suites for algorithms, bin processing, result selection, and ViPaq.  
- Added cross-language ViPaq interop tests between C# and TypeScript.  
- Patched two **high-severity advisories** in transitive dependencies — `Microsoft.OpenApi` and the bundled **SQLite** native library.  
- **Rebuilt the release pipeline.** A tag now builds the image once, smoke tests it in a staging registry, then copies the tested digest to Docker Hub — so what is published is bit for bit what passed, and a failure anywhere leaves Docker Hub untouched. The release body is this changelog, extracted by the workflow.  
- Renamed two top-level folders — `config/` is now `tooling/`, and build output goes to `artifacts/` instead of `build/`.  
- Every GitHub Action is pinned to a commit SHA, kept current by Dependabot.  

### 📚 Versioned Docs
- Documentation is now versioned per minor line — `v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x` — so any image can be matched to its docs.  
- Backfilled the `v2.0.x` and `v2.1.x` documentation, which was previously missing.  
- The `latest` documentation now redirects to the current version, so existing links keep working.  

### 🛠️ Migration Guide
To upgrade to **v3.0.0**, follow these steps:

1. **Remove all V2 usage**  
   - Any calls to V2 endpoints must be removed or migrated.
   - Replace `/api/v2/presets`, `/api/v2/fit/by-custom`, `/api/v2/fit/by-preset/{preset}`, `/api/v2/pack/by-custom`, and `/api/v2/pack/by-preset/{preset}` with their V3 equivalents.

2. **Switch to V3 endpoints**  
   - V3 requires an algorithm to be selected, where V2 used a fixed one, and drops V2's other parameters.  
   - See the [v2.1.x documentation](https://docs.binacle.net/version/v2.1.x/) for the old contract.

3. **Regenerate all ViPaq strings**  
   - The format was rebuilt and is not backwards compatible.  
   - Strings from earlier versions no longer decode, and there is no fallback reader.  
   - Re-run the packing request to get a new one. Any stored string — a saved link or a bookmarked result — is stale.  
   - This applies to V3 responses as well, even though V3 is otherwise unchanged.

4. **Do not mix versions**  
   - Images before v3.0.0 produce the old ViPaq format; v3.0.0 onward produces and reads only the new one.  
   - An encoder and a decoder on different sides of this release will not interoperate.

5. **Update Packing Logs configuration**  
   - Move `Path`, `FileName`, `DateFormat`, and `ChannelLimit` out of the nested `Packing` block, directly under `PackingLogs`, and delete the `Fitting` block.  
   - Left in the old shape with `Enabled: true`, startup validation now fails.  
   - Repoint log collection from `data/pack-logs/packing/` to `data/pack-logs/`. The old `packing/` and `fitting/` directories are safe to remove.

6. **Review health check `RestrictedIPs`**  
   - Replace any `start-end` entries with CIDR — `192.168.1.0-192.168.1.255` becomes `192.168.1.0/24`. Left as they are, startup validation now fails.  
   - Re-check any CIDR entry. It now covers what it says, which is far less than before — confirm the addresses you expect are still inside it, or you will lock yourself out.  
   - A range that does not line up with a CIDR boundary must be split into several entries, or widened to the enclosing subnet.  
   - Drop any leading zeros — `010.10.10.10` becomes `10.10.10.10`, and note it used to admit `8.10.10.10`, so check that host was not the one you meant. Write IPv6 entries in the short lowercase form: `2001:0DB8::1` becomes `2001:db8::1`.  
   - If Binacle.Net runs behind a proxy, load balancer or CDN, enable **forwarded headers** as well. Without it the list is compared against the proxy's address and can never match your monitoring system.

---

**Full Changelog**: https://github.com/ChrisMavrommatis/Binacle.Net/compare/v2.1.1...v3.0.0

## [2.1.1] - 2026-01-12

### Overview
- Internal refactoring.
- Removed Postman metadata from API documentation.
- Updated packages.
​
### ⚙️ Core Changes
- Removed external dependencies on `ChrisMavrommatis.Features` and `ChrisMavrommatis.StartupTasks` by implementing internal versions.
- Consolidated logging (`ChrisMavrommatis.Logging`) and testing (`ChrisMavrommatis.Shouldly`) utilities into kernel modules and removed `/dep` folder.
- Migrated solution file to new Visual Studio `.slnx` format.
- Updated NuGet packages to latest stable versions.
- Removed Postman-related metadata from API documentation files.

## [2.1.0] - 2025-12-03

### Overview
- Upgraded to .NET 10.
- Added CORS support for the API.
- Various fixes and improvements.

### ⚙️ Core Changes
- Upgraded to .NET 10.
- Added CORS support for main API endpoints (requires setup).
- Fixed various spelling errors.

### 🎨 UI Module
- Improved performance for the visualizer.
- Updated license URL and target in the footer.
- Added cache for the new Docker version badge.
- Added badge for Scalar.
- Both Scalar and Swagger badges will show up when they are enabled only.

## [2.0.1] - 2025-10-02

### Overview
- Fixed an issue with Open api resolving the servers automatically

## [2.0.0] - 2025-10-02

Binacle.Net v2.0.0 is a major update from v1.3.0.

> [!Warning]
> **v2.0.0 introduces breaking changes and new features. Existing integrations must be reviewed and updated.**

---

### 🔎 Overview
- **Service Module** was completely rewritten with new core business logic, breaking all existing integrations.  
- **V1 endpoints** were removed.  
- **V3 endpoints** were promoted from experimental to stable.  
- **Algorithms** were improved, primarily impacting V3 endpoints.  
- **Packing Logs** were updated, with some breaking changes for existing integrations.  
- **API documentation** now follows the OpenAPI 3.0 specification.  
- The project introduces **versioned documentation** with an official site.  [docs.binacle.net](https://docs.binacle.net)

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

For additional details, see the [Service Module documentation](https://docs.binacle.net/version/latest/configuration/service-module/).  

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

---

**Full Changelog**: https://github.com/ChrisMavrommatis/Binacle.Net/compare/v1.3.0...v2.0.0

## [1.3.0] - 2025-03-27

This release introduces **OpenTelemetry** for improved observability, removes **Application Insights**, enhances logging, and restructures internal components for better maintainability.

### Breaking Changes

#### ⚠️ **Application Insights Removed** 
OpenTelemetry has replaced the previous Application Insights integration.
- **Action Required**: Migrate to **Azure Monitor** or another OpenTelemetry-compatible tool to maintain telemetry data.
- The previous integration using Serilog and the old SDK has been removed.


### Overview of New Features & Enhancements
- **OpenTelemetry** – Fully implemented, replacing the previous incomplete version, now supporting export via OTLP Exporter and Azure Monitor.
- **Packing Logs** – Added logs to track API usage for analytics and data gathering.
- **Service Module Logging** – Enhanced logging in the Infrastructure layer to display a clear message when no repository is configured.
- **UI Module Fix** – Resolved an issue where sample data did not reset correctly when new data was entered.
- **Docker Image Update** – Added `.dockerignore` to exclude development configuration files from the Docker image.


### Module-Specific Updates

#### Diagnostics Module
- OpenTelemetry has fully replaced Application Insights, supporting export via OTLP Exporter and Azure Monitor.
- Added Binacle Service Name and Version to logs for better traceability.
- Introduced Packing Logs to track API usage for analytics and data gathering.

#### Service Module
- Enhanced logging in the Infrastructure layer to ensure clearer messaging when no repository is configured.
- Improved error handling during initialization, ensuring clear error messages when the Service Module is not properly configured.

#### UI Module
- Fixed a bug where sample data persisted instead of being properly reset when new data was entered.

### Miscellaneous Changes
- Implemented Diagnostics for internal tracking using Activity Sources, improving telemetry and debugging capabilities.
- Added a `.dockerignore` file to exclude development configuration files from being included in the Docker image.

### Internal Improvements
- Moved configuration files to their respective modules, ensuring they are correctly copied during the build process.
- Addressed compiler warnings and formatted empty types for consistency and maintainability.
- Enhanced extension methods to simplify configuration setup.
- Restricted internal code visibility by moving public code to internal where appropriate.
- Integrated Aspire for local development, streamlining observability.
- Fixed issues with local tooling for Docker, improving the development experience.
-  Restructured the project to improve support for Docker-based samples, including renaming, relocating components, and adding new features.

### Upgrade Notes
- **Action Required**: If you were using Application Insights, you must migrate to Azure Monitor or another OpenTelemetry-compatible tool, as Application Insights is no longer supported.
- No performance or security changes are included in this release, but internal optimizations have been made to improve maintainability and stability.

## [1.2.2] - 2025-02-26

### Overview
- Various fixes.

### UI Module
- Fixed an issue in Packing Visualizer for the Protocol Decoder that was introduced in the camera adjustment in the previous version.

## [1.2.1] - 2025-02-25

### Overview
- Various fixes and improvements.

### Core
- Implemented validation logic to prevent volume overflow, which previously caused crashes.

### UI Module
- Resolved an issue in Packing Visualizer where entering large numbers caused the view to shift out of range. The correct distance is now calculated dynamically.
- Added validation logic to the Packing Demo form, ensuring smoother submission.
- The Packing Demo form now properly reads and displays API errors in the modal.
- Adjusted the close button to ensure proper visibility.

## [1.2.0] - 2025-02-12

### Overview
- Redesigned UI Module with new features.
- Introduced experimental v3 API endpoints; v1 endpoints are now deprecated.
- Introduced Binacle.ViPaq, an experimental Packing Visualization Protocol.
- Various fixes and improvements.

### Core
- Added an experimental v3 API that allows selection of packing algorithms.
- Deprecated v1 endpoints; they will be removed in version 2.0.0.
- Fixed a critical issue causing algorithms to exit prematurely.
- Documentation updates for new features and changes.
- Introduced Binacle.ViPaq, an experimental Packing Visualization Protocol that serializes packing results for efficient storage, transport, and easy copy-pasting.

### UI Module
- Complete UI redesign.
- Migrated from Materialize CSS to Beer CSS.
- Added Dark Mode support.
- Added Protocol Decoder applet to decode ViPaq-encoded data.
- Packing Demo now utilizes the experimental v3 API with algorithm selection support.

### Miscellaneous
- Removed Newtonsoft.Json dependency; migrated to System.Text.Json.
- Replaced FluentAssertions with Shouldly due to licensing changes.
- Replaced Bruno files with .http files for better compatibility with Rider and Visual Studio.
- Added a Docker sample for running the UI Module only.
- Various project tooling improvements.
- Added tests for the v3 API endpoints.
- Added initial tests for ViPaq.

## [1.1.4] - 2025-01-08

### Overview
Updated NuGet Packages

## [1.1.3] - 2024-11-26

### Overview
Migration and Improvements

### Core
Added Metadata class to hold information about the project

### UI Module
Added Improvements to the front end and reqorked badges

### Misc
Updated Project to plan to migrate to new repository

## [1.1.2] - 2024-11-25

### Overview
Update build

### Misc
Updated build proceedure planed migration to another dockerhub organization

## [1.1.1] - 2024-11-25

### Overview
Bug fixes

### UI Module
Critical fix in visualizer that caused it to not work due to internal improovements.
Fix in footer badge to display version 9

## [1.1.0] - 2024-11-25

### Overview
.Net 9
Various fixes
Tests

### Core
Updated projects to use the .Net 9 framework.
Fix in Packing service to order bins by volume.

### UI Module
Fixes in visualizer and camera adjustments.

### Misc
Docker Image now uses the unpriviled user provided by Microsfot instead of running as root.
Various algorithm changes that are in test.

### Tests
Refactored tests and benchmarks. 
Added packing efficiency tests for experimentation with new algorithms.

## [1.0.1] - 2024-10-11

### Overview
Bux Fixes
Text correction

### Core
Fixed logging of dimensions to be the correct sequence.
Corrected Swagger UI texts so BInacle.Net's slogan is uniform.

### Service Module
Corrected Swagger UI texts

### UI Module
Corrected Swagger UI texts so BInacle.Net's slogan is uniform.

## [1.0.0] - 2024-10-08

### Overview
Massive overhaul of the aglorithms. 
Added version 2 of the Binacle.Net API.
Internal restructures.
Service Module api changed.
Added UI for Packing visualization.

### Algorithms
Algorithms now are separated to Fitting and Packing as functions.

### Core
Added Version 2 to the API which supports Fitting and Packing
V1 remained unchanged. There will be plans to deprecate it in the future.

### Diagnostics Module
Health checks, Telemetry and logging now reside in their own module.

### Service Module
All endpoints are now prefixes with /api. this was done to separate the api from the front end

### UI Module
A New module exists that provides basic visualization of the Packing algorithms.

## [0.8.5] - 2024-08-30

### Overview
Nuget Package updates & internal restructure

### Details
- Updated all nuget packages
- Removed the following dependent projects as they were each made into a nuget package:
  - `ChrisMavrommatis.Results` 
  - `ChrisMavrommatis.Features` 
  - `ChrisMavrommatis.StartupTasks` 
  - `ChrisMavrommatis.Endpoints ` 
  - `ChrisMavrommatis.MinimalEndpointDefinitions` 

## [0.8.4] - 2024-07-22

### Overview
Minor and major improvements

### Details
- Code warning fixes 
- Updated swagger documentation for clarity and visibility in `ByPreset` and `ByQuery` endpoints
- Removed dependent project `ChrisMavrommatis.SwaggerExamples` as it was made into a nuget package

## [0.8.3] - 2024-07-18

### Overview
- Added reason to logging in find fitting bin

## [0.8.2] - 2024-07-18

### Overview
- Changed logged found bin
- Added open telemetry and export to azure in Service Module
- Disabled live metrics in application insights in Service Module

## [0.8.1] - 2024-07-12

### Overview
- Updated nuget packages

## [0.8.0] - 2024-05-21

### Overview
- Internal Restructure and maintenance.
- Added Experimental `v2` endpoint to Binacle.Net.
- Fixes
- Changes and added features in Service Module.


#### Binacle.Net Api
- Added experimental V2 endpoints.
- Changed Serilog file logs path to `/data/logs`.
- Added a Json converter that handles string to nullable enum gracefully `JsonStringNullableEnumConverter`.
- Fixed severe issue in Binacle.Net core that sent items without quantity.


#### Service Module
- Added Startup task to ensure admin user exists on startup.
- Added `IsActive` to Users.
- Fixed an issue on `EnsureAdminUserExistsStartupTask` that initialized the user with the email instead of the password.
- Added Update user endpoint.
- Added Swagger examples for token & user endoints.

##### Service Module  / Infrastructure Azure Tables
- Added normalization for the email as row key.

#### Internal
- Repository restructure to conform to OSS standards.
- Broke up `ChrisMavrommatis` project to smaller ones as these are dependencies and part of `Binacle.Net`.
- Merged the common test projects to a `TestsKernel`.
- Broke up lib into lib & abstractions to facilitate separation of concerns and be able to have `TestsKernel` depend on abstractions only.
- Brought requests in `res` folder instead on `res/Requests/`.
- Added Results in the style of discriminated unions as well as typed results.
- Added Shared kernel Project for the api.
- Restructures Service module to clean architecture style to allow multiple infrastructure providers.
- Endpoint restructure on Service Module to v0.
- Renamed Service Module Integration Tests to ServiceIntegration tests as adding the module slightly affects the core functionality by adding rate limiting.
- Improved Minimal endpoint definitions to include groups.
- Broken up the endpoints in service module to each endpoint in separate file.
- Added dependency StartupTasks.
- Internal restructure to Service Module.
- Restructured config files, added ConnectionStrings.
- Changes In swagger example to take application/json type only.

##### Tests
- Added Service integration test to ensure proper configuration of services.
- Added Service Integration Tests.
- Restructures in Tests

## [0.7.1] - 2024-04-29

### Overview
There are no functional changes in the core of Binacle.Net only in Service Module.

### Changes

#### Binacle.Net Api
- Updated swagger documentation to include link in Dockerhub.

#### Service Module
- Added a helper to resolve the connection string.
- Table Service client is now registered through AddAzureClients
- Added Serilog Default level for Azure.Core
- Updated swagger documentation to include link in Dockerhub.
- Used the secret in config instead of assembly attribute

#### Misc
- Corrected the list presets request in Binacle.Requests.http

#### API Tests
- Broke up Binacle.Net Api Integration tests into two project for separation of concerns
- Added Common tests project for the apis

## [0.7.0] - 2024-04-28

First public release
