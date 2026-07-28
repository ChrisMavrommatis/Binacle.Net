# Smoke test the built docker image over HTTP

**Status:** Designed, nothing built. Promoted from an idea on 2026-07-29, once the build/image split landed and
the approach was settled. The blocker it named is gone: `just build image` builds and stops, and
`just image up` runs the result.

## The problem

Everything we test today runs **in process**. `Binacle.Net.IntegrationTests` boots the app with
`WebApplicationFactory<IApiMarker>` and replaces the presets with three test-only ones, so it never loads the
config files we ship. Nothing anywhere touches the image.

That leaves a whole class of failure invisible until someone pulls the tag: a config file that did not get
copied or landed at the wrong path; a module env var that no longer switches anything on, so routes are simply
absent; a connection string that works on the host and not inside the container network; the `VERSION` build
arg never reaching the process; the entry point, port or published runtime being wrong.

All of it is packaging and wiring. None of it is C# logic, which is why the current suites cannot see it.

**The mainstream .NET answer does not cover this.** `WebApplicationFactory`, Testcontainers and Aspire all host
the API in-process and containerize only its dependencies - Testcontainers and Aspire manage the database and
the broker, not the thing under test. There is no framework rung for "test the artifact"; the common shape is
build, run the image, poll a health endpoint, assert over HTTP, publish only if green.

## The line

> **Assert what the image contains and wires. Never assert what the algorithm computed.**

Re-running the integration assertions over HTTP buys the same coverage with worse diagnostics, ten times the
runtime, and a suite that goes red on every legitimate packing change. The integration tests own "is the answer
right", in-process, where a failure points at a line of C#.

The one place a known-good value **is** right: data that comes from the image's config files rather than from
the algorithm. The shipped presets are image content - they stay true when FFD changes and break when a file
does not get copied. Assert those hard. Assert a coordinate and you have built a second integration suite.

Corollary, and the test to apply to any new check:

> Every check must be able to fail for a reason that has nothing to do with the C# logic.

## Two containers, not a matrix

Off-states do not interact: a route that leaks when its flag is off leaks regardless of what else is on. So one
bare container catches every flag's off-state at once, and one all-on container catches every on-state. Two
runs instead of 2^7.

What that misses is "module A only works because module B registered something". That belongs to the
all-modules integration gate, not here.

| Surface | `bare` (no env) | `full` (all flags on) |
|---|---|---|
| `/api/v3/presets`, `/api/v4/presets` | 200 - core is not optional | 200 |
| `/_health` | 404 | 200 `Healthy`, carries `Version` |
| `/_debug` | 404 | 200, echoes the request |
| `/swagger/`, `/scalar/` | 404 | 200 |
| `/` (UI) | 404 | 200 HTML, its API connection string resolves |
| `/api/auth/token` | 404 | 200 for the seeded admin |
| `/api/admin/account/{id}` unauthenticated | 404 | **401** |

**The off column is the security test, and it is what justifies the second container.** `/_debug` echoes the
caller's headers and connection address and `/_health` exposes internals; if either ships on by accident, no
in-process test would notice, because the integration harness sets its own flags. The admin route is the same
shape in one request: 404 means never mounted, 200 means open to the world, only 401 means mounted *and*
protected.

The bare run proves one more thing on its own: **the image starts with no configuration at all.**

## Verified against `binacle-net:local` on 2026-07-29

Each of these was read out of the built image, and each is a check that pays for itself.

- **`Config_Files/DiagnosticsModule/HealthChecks.json` ships `"Enabled": false`.** `/_health` exists only
  because a stack sets `HealthChecks__Enabled=True`. So it is not a free readiness probe for a bare
  `docker run`, and "the flag still switches it on" is itself a check.
- **No `*.Development.json` in the image** - confirmed empty. The last line of `.dockerignore` is doing real
  work. If it ever stops, development overrides silently win in production and nothing in-process would see it.
- **`Config_Files/Presets.json` starts `EF BB BF`** - a UTF-8 BOM. A parser that chokes on it returns a preset
  list that looks fine with no bins in it, so read a preset *through to its bins*, do not just count them.
- **ServiceModule ships only `RateLimiter.json`** - no `JwtAuth.json`. The module cannot boot on the image
  alone; the compose files inject it via `configs:`, which is what users must also do. Worth proving that path
  works rather than assuming it.
- **`Config_Files/UiModule/ConnectionStrings.json` is in the image.** "The UI module works" means the page
  renders *and* that connection string resolves from inside the container network.
- **`RestrictedIPs: []` means `RestrictsNobody`**, so the health endpoint is readable from the host without
  special configuration. Behind a proxy it would not be - that is the forwarded-headers path, not this suite's.
- **The version is already on the health response.** `SystemHealthCheck` puts `{"Version", Metadata.Version}`
  into the check data, so the image stamp can be asserted without the UI module being on. No app change needed;
  an earlier note claiming one was wrong.

## What it must not do

Scope creep is how this becomes a slow second integration suite.

- **No project reference to the app.** Talk HTTP only, read responses as JSON documents rather than the app's
  contract types. A `ProjectReference` is how a smoke suite quietly turns into an integration suite.
- **No coverage collection** - the code under test runs in another process.
- **No shared fixtures or scenario data.** Payloads stay inline and tiny.
- **No numeric assertions.** Never a coordinate or a bin count from a packing run.
- **No validation or error-path cases.** Wrong-input handling is logic.
- **Keep it fast.** If it takes longer than the image takes to build, it is doing too much.

## Inventory

**The test project**

- [ ] `api/test/Binacle.Net.SmokeTests` - xunit.v3 on the Microsoft Testing Platform, like the other suites. No
      `ProjectReference`, no coverage package. Add it to `Binacle.Net.slnx`.
- [ ] Read `BINACLE_SMOKE_PROFILE` (`bare`/`full`), a base URL, the expected version, and the admin
      credentials from the environment, so the same suite can be pointed at a staging instance.
- [ ] Expected status per surface as a **keyed table** rather than branching code - one row per surface, a
      column per profile, matching how the theory data is keyed elsewhere in the repo.
- [ ] An assembly fixture holding one `HttpClient` with `AllowAutoRedirect = false`: the container serves plain
      HTTP, so a misconfigured HTTPS redirect must show up as a 307 rather than a connection error.
- [ ] A readiness wait in the fixture, so a slow start reads as slow rather than broken. `/_health` for the
      `full` profile; the `bare` profile has no health endpoint, so poll a core route instead. Give up after
      ~90s with a message that says how to start one - it is the message you would hit most often.
- [ ] The six groups above, one file each, named after the failure they catch.

**Stacks**

- [ ] `config/docker-compose.bare.yml` - one service, the image, a port, zero env. There is no bare stack
      today: `full`, `volume` and `bind` all switch the modules on. It doubles as the file a user reads to see
      the minimum.
- [ ] Add `bare` to the `image` module's name-to-file map.

**Recipes**

- [ ] `just test api-smoke <profile>` - runs the suite against something already up, the same shape as
      `api-service-integration <backend>`, with a `case` that rejects an unknown profile. **Not** in
      `just test all`: that is the set which needs nothing brought up.
- [ ] `just smoke` in the **root** justfile - build, then per profile: up, test, down. It sits at the root
      because it spans the `image` and `test` modules, the way `install` spans setup.
- [ ] No `config/smoke.sh`. The two scripts that did this kind of thing were deleted this month, and the three
      jobs are already recipes.

**Docs**

- [ ] Repo-layout row in `.agents/docs/README.md`; a Smoke section in `config/README.md`; the leaf in the
      commands doc.

## Things to watch out for

Found the hard way while sketching this; each would cost an hour otherwise.

- **v4's fit status is its own enum** - `BinFitResultStatus` is `Unknown/Fits/DoesNotFit/EarlyExit`, not v3's
  `AllItemsFit/NotAllItemsFit/EarlyFail_*`.
- **`/scalar` answers 302 to `/scalar/`.** With redirects off, request the trailing slash.
- **`Guid.Empty` is not a usable "missing" id.** The admin routes reject it as invalid before they look it up,
  so a 404 check comes back 422. Use an arbitrary real GUID.
- **xUnit1051 fires on every HTTP call in a test method.** Centralise the calls on the fixture, passing
  `TestContext.Current.CancellationToken` in one place, rather than annotating ~20 call sites.
- **An empty connection string falls through to the next provider**, because resolution checks
  `string.IsNullOrWhiteSpace`. Useful for running the app locally on SQLite while pointing the suite at it.

## CI

Run it in the **release** workflow, not the PR gate. The pattern is build with `load: true`, smoke the loaded
image, push only if green - the last gate before a tag goes out, which is the one place this pays for itself.
The PR gate stays a plain image build.

That depends on `release-docker-image.yml` calling `just build publish` first, which is the remaining item in
the CI shared-scripts plan. Until then the release path and the smoke path build the image two different ways,
and smoking one says little about the other.

## Open questions

- **How much ServiceModule?** It is the only group that needs a storage backend, and so the only reason the
  `full` stack needs more than one container. Current answer: keep it - the seeded admin getting a token covers
  the whole storage path in one request, and it is the only proof that storage is reachable from inside the
  container network.
- **A third profile for the documented quickstart?** The root README tells users to run with `SWAGGER_UI`,
  `UI_MODULE` and `SCALAR_UI` on and ServiceModule off. `bare` and `full` bound the space, but nothing proves
  the exact command we publish works. Cheap to add later if the two-profile suite proves useful.
- **Container `HEALTHCHECK` and `docker compose up --wait`?** It would move readiness out of the fixture, but
  `mcr.microsoft.com/dotnet/aspnet:10.0` ships no curl or wget, so it means adding a package to a production
  image for the tests' benefit. Polling from the fixture costs nothing and still works against staging.
