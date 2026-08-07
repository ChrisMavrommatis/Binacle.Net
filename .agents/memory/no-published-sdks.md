---
name: no-published-sdks
description: We ship an OpenAPI document per version, not client SDKs — consumers generate their own; publishing a package needs real demand first.
type: decision
---

Binacle.Net publishes **no client libraries** — no npm package, no NuGet package, no generated client in any
language. What it publishes is one clean OpenAPI document per API version, and consumers generate their own
client from it with whatever tool they use (`hey-api`, `kiota`, `openapi-generator`, ...).

**Why:** a clean per-version spec covers the whole developer audience for close to zero upkeep. Every published
SDK is the opposite: a package to version, test, publish and patch, times the number of languages, and each one
becomes a compatibility promise the moment somebody depends on it. The spec generates a client in one command,
so shipping SDKs buys convenience we would then owe maintenance on forever. Decided 2026-07-19, and the spec
side of it is already built — `$api/openapi` describes the documents, `.spectral.yaml` and `just openapi lint`
hold them to a standard good enough to generate from.

**How to apply:**

- **Do not publish a package to close a "make integration easier" request.** The answer is the spec plus a
  short "generate a client" guide, not a shipped package.
- **Do not add a `PackageId`, drop `private` from a TS workspace package, or wire a publish step** without the
  maintainer deciding to reverse this. Doing so also makes a component earn a real version number, which is a
  second decision with its own consequences.
- Keep the documents generation-quality: descriptions on every schema and property, stable `operationId`s, no
  numeric-as-string unions. Those exist so a generated client is usable, not for tidiness.

**What reverses it:** real, expressed demand for one specific language, where the reach justifies the upkeep.
The clean spec makes that a config job rather than a project, so the decision stays cheap to change — publish
an SDK because somebody asked, never in anticipation.

Non-developer integration (store plugins and the like) is a separate question and is not covered by this — a
plugin user never runs a generator.
