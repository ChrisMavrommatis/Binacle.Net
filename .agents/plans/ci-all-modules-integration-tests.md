# CI - run the integration tests with all modules enabled

**Status:** Not started. After v3.0.0.

## Why

The integration harnesses run **core modules only**. Every module combination the image actually ships is
untested end to end, so the gate is green without being meaningful. Three `// TODO` comments say so:

- `api/test/Binacle.Net.IntegrationTests/BinacleApi.cs:35`
- `api/test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
- `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`

## What

- Turn the modules on in the harnesses - Diagnostics, Service, UI.
- Decide whether that is one run with everything on, or a small matrix over the combinations that actually
  ship. Everything-on is cheaper and catches registration conflicts; a matrix catches "module A only works
  because module B registered something".
- Watch the runtime. The suite is already the long pole in the PR gate.

## Watch out

Test-host configuration goes through an env var the harness reads, never a `.runsettings` file - the MTP runner
ignores VSTest runsettings. `BINACLE_TEST_INFRA` already works this way.

## Done when

The integration suites run against the module set the image ships, and the three TODOs are gone.
